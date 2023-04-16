using System.Collections;
using System.Diagnostics;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using ES.Tools.Core.Infrastructure;
using FlowReports.Model;
using FlowReports.Model.ReportItems;

namespace FlowReports.ViewModel.Printing
{
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
    /// Initializes a new instance of the <see cref="NASDocumentPaginator"/> class.
    /// </summary>
    public ReportPaginator(Report report, (PageMediaSize Size, PageImageableArea PrintableArea, PageOrientation Orientation)? pageInformation = null)
    {
      _report = report ?? throw new ArgumentNullException(nameof(report));
      _data = report.Data;

      PageMediaSize pageMediaSize = null;

      if (pageInformation == null)
      {
        try
        {
          var printQueue = LocalPrintServer.GetDefaultPrintQueue();
          var ticket = printQueue.DefaultPrintTicket;
          pageMediaSize = ticket.PageMediaSize;
          _printableArea = printQueue.GetPrintCapabilities(ticket).PageImageableArea;
          Orientation = ticket.PageOrientation ?? PageOrientation.Portrait;
        }
        catch
        {
        }
      }
      else
      {
        _printableArea = pageInformation.Value.PrintableArea;
        Orientation = pageInformation.Value.Orientation;
        pageMediaSize = pageInformation.Value.Size;
      }

      Debug.Assert(pageMediaSize != null);
      _pageSize = Orientation == PageOrientation.Landscape || Orientation == PageOrientation.ReverseLandscape
        ? new Size(pageMediaSize.Height ?? 0, pageMediaSize.Width ?? 0)
        : new Size(pageMediaSize.Width ?? 0, pageMediaSize.Height ?? 0);

      CreatePages();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the orientation.
    /// </summary>
    public PageOrientation Orientation { get; set; }

    #endregion

    #region Overwritten Members

    /// <summary>
    /// Gets a count of the number of pages currently formatted
    /// </summary>
    /// <returns>A count of the number of pages that have been formatted.</returns>
    public override int PageCount => _pages.Count;

    /// <summary>
    /// Gets or sets the suggested width and height of each page.
    /// </summary>
    /// <returns>A <see cref="T:System.Windows.Size"/> representing the width and height of each page.</returns>
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
    /// Gets a value indicating whether <see cref="P:System.Windows.Documents.DocumentPaginator.PageCount"/> is the total number of pages.
    /// </summary>
    /// <returns>true if pagination is complete and <see cref="P:System.Windows.Documents.DocumentPaginator.PageCount"/> is the total number of pages; otherwise, false, if pagination is in process and <see cref="P:System.Windows.Documents.DocumentPaginator.PageCount"/> is the number of pages currently formatted (not the total).This value may revert to false, after being true, if <see cref="P:System.Windows.Documents.DocumentPaginator.PageSize"/> or content changes; because those events would force a repagination.</returns>
    public override bool IsPageCountValid => true;

    /// <summary>
    /// Returns the element being paginated.
    /// </summary>
    /// <returns>null</returns>
    public override IDocumentPaginatorSource Source => null;

    /// <summary>
    /// Gets the <see cref="T:System.Windows.Documents.DocumentPage"/> for the specified page number.
    /// </summary>
    /// <param number="pageNumber">The zero-based page number of the document page that is needed.</param>
    /// <returns>
    /// The <see cref="T:System.Windows.Documents.DocumentPage"/> for the specified <paramref number="pageNumber"/>, or <see cref="F:System.Windows.Documents.DocumentPage.Missing"/> if the page does not exist.
    /// </returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    ///   <paramref number="pageNumber"/> is negative.</exception>
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
      if (_currentY + band.Height >= ActualHeight)
      {
        // Current band does not fit onto page -> create next page
        CreatePageFromCurrentCanvas();

        // Create canvas for next page and reset y valze
        CreateNewCanvas();
      }

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

      _currentCanvas.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/FlowReports.UI;Component/View/DataTemplates/ReportDataTemplates.xaml", UriKind.RelativeOrAbsolute) });
      _currentY = 0;
    }

    private void OnProgress(double value)
    {
      EventService.Instance.Publish("Progress", value);
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
