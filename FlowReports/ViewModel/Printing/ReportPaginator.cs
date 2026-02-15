using System.Collections;
using System.Diagnostics;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using FlowReports.Model;
using FlowReports.Model.ReportItems;

namespace FlowReports.ViewModel.Printing
{
  /// <summary>
  /// Provides pagination services for rendering reports as document pages.
  /// </summary>
  internal class ReportPaginator : DocumentPaginator
  {

    #region Fields

    private readonly Report _report;
    private readonly IEnumerable _data;
    private Size _pageSize;
    private readonly PageImageableArea _printableArea;
    private List<DocumentPage> _pages = new();
    private Canvas _currentCanvas;
    private double _currentY;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="ReportPaginator"/> class.
    /// </summary>
    /// <param name="report">The report to paginate.</param>
    /// <param name="pageInfo">The page information for rendering. Defaults to the default page information if not provided.</param>
    public ReportPaginator(Report report, PageInformation pageInfo = null)
    {
      _report = report ?? throw new ArgumentNullException(nameof(report));
      _data = report.Data;

      pageInfo ??= PageInformation.Default;

      if (pageInfo is null) { return; }

      _printableArea = pageInfo.PrintableArea;
      Orientation = pageInfo.Orientation;
      var pageMediaSize = pageInfo.PageSize;
      Debug.Assert(pageMediaSize != null);
      _pageSize = Orientation == PageOrientation.Landscape || Orientation == PageOrientation.ReverseLandscape
        ? new Size(pageMediaSize.Height ?? 0, pageMediaSize.Width ?? 0)
        : new Size(pageMediaSize.Width ?? 0, pageMediaSize.Height ?? 0);

      CreatePages();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the page orientation.
    /// </summary>
    public PageOrientation Orientation { get; set; }

    #endregion

    #region Overwritten Members

    /// <summary>
    /// Gets the count of the number of pages currently formatted.
    /// </summary>
    public override int PageCount => _pages.Count;

    /// <summary>
    /// Gets or sets the suggested width and height of each page.
    /// </summary>
    public override Size PageSize
    {
      get => _pageSize;
      set
      {
        var newSize = Orientation == PageOrientation.Landscape || Orientation == PageOrientation.ReversePortrait
          ? _pageSize.Height > _pageSize.Width ? new Size(value.Height, value.Width) : value
          : _pageSize.Height > _pageSize.Width ? value : new Size(value.Height, value.Width);

        if (_pageSize != newSize)
        {
          _pageSize = newSize;
          // New Page Size -> Reset buffer to recreate all pages
          _pages = new();
        }
      }
    }

    /// <summary>
    /// Gets a value indicating whether the page count is the total number of pages.
    /// </summary>
    public override bool IsPageCountValid => true;

    /// <summary>
    /// Gets the element being paginated.
    /// </summary>
    public override IDocumentPaginatorSource Source => null;

    /// <summary>
    /// Gets the document page for the specified page number.
    /// </summary>
    /// <param name="pageNumber">The zero-based page number of the document page to retrieve.</param>
    /// <returns>The document page for the specified page number, or throws an exception if the page number is out of range.</returns>
    public override DocumentPage GetPage(int pageNumber)
    {
      return pageNumber < 0 || pageNumber > _pages.Count - 1
             ? throw new ArgumentOutOfRangeException(nameof(pageNumber))
             : _pages[pageNumber];
    }

    #endregion

    #region Private Methods

    private List<DocumentPage> CreatePages()
    {
      _pages = new List<DocumentPage>();

      // There is at least one page.
      CreateNewCanvas();

      foreach (var band in _report.Bands)
      {
        DrawBand(band, _data);
      }

      // Create the page from the current canvas
      CreatePageFromCurrentCanvas();

      return _pages;
    }

    private void DrawBand(ReportBand band, IEnumerable data)
    {
      if (data == null)
      {
        // Prevents crashes if there is no data
        return;
      }

      if (_currentY + band.Height >= ActualHeight)
      {
        // Current band does not fit onto page -> create next page
        CreatePageFromCurrentCanvas();

        // Create canvas for next page and reset y valze
        CreateNewCanvas();
      }

      int bandCount = 0;

      foreach (var itemData in data)
      {
        // Draw all report items once for each item in the data source
        foreach (var item in band.Items)
        {
          var vm = ViewModelFactory.CreatePreviewItemViewModel(item, itemData, _currentY);
          var control = new ContentControl();
          control.Content = vm;

          _currentCanvas.Children.Add(control);
          control.SetValue(Canvas.TopProperty, vm.Top);
          control.SetValue(Canvas.LeftProperty, vm.Left);
        }

        // Increase current y position
        _currentY += band.Height;

        // Draw sub bands
        foreach (var subBand in band.SubBands)
        {
          var subData = GetSubData(itemData, subBand.DataSource);
          DrawBand(subBand, subData);
        }

        bandCount++;

        // If there is no data source defined, we only want to draw the band once
        if (string.IsNullOrWhiteSpace(band.DataSource) && bandCount == 1)
        {
          return;
        }
      }
    }

    private void CreatePageFromCurrentCanvas()
    {
      _currentCanvas.Measure(new Size(_currentCanvas.Width, _currentCanvas.Height));
      _currentCanvas.Arrange(new Rect(new Point(_printableArea.OriginWidth, _printableArea.OriginHeight), new Size(_currentCanvas.Width, _currentCanvas.Height)));
      var dp = new DocumentPage(_currentCanvas, _pageSize, new Rect(), new Rect(new Point(_printableArea.OriginWidth, _printableArea.OriginHeight), new Size(ActualWidth, ActualHeight)));
      _pages.Add(dp);
    }

    private static IEnumerable GetSubData(object data, string dataSource)
    {
      var type = data.GetType();
      var property = type.GetProperty(dataSource);

      if (property != null && typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
      {
        return (IEnumerable)property.GetValue(data);
      }
      else
      {
        // TODO: Error handling
      }

      return null;
    }

    private void CreateNewCanvas()
    {
      _currentCanvas = new Canvas
      {
        Height = ActualHeight,
        Width = ActualWidth
      };

      _currentCanvas.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/FlowReports;component/View/DataTemplates/ReportDataTemplates.xaml", UriKind.RelativeOrAbsolute) });
      _currentY = 0;
    }

    private double ActualWidth
    {
      get
      {
        if (_printableArea == null)
        {
          return _pageSize.Width;
        }

        if (Orientation == PageOrientation.Landscape || Orientation == PageOrientation.ReversePortrait)
        {
          if (_printableArea.ExtentHeight > _printableArea.ExtentWidth)
          {
            return _printableArea.ExtentHeight;
          }
        }
        return _printableArea.ExtentWidth;
      }
    }

    private double ActualHeight
    {
      get
      {
        if (_printableArea == null)
        {
          return _pageSize.Height;
        }

        if (Orientation == PageOrientation.Landscape || Orientation == PageOrientation.ReversePortrait)
        {
          if (_printableArea.ExtentHeight > _printableArea.ExtentWidth)
          {
            return _printableArea.ExtentWidth;
          }
        }
        return _printableArea.ExtentHeight;
      }
    }

    #endregion

  }
}
