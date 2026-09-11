using System.Collections;
using System.Diagnostics;
using System.Printing;
using System.Reflection;
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

      int bandCount = 0;

      // Draw header band if it exists
      if (band.HeaderBand != null)
      {
        DrawBandContent(band.HeaderBand, GetFirstItem(data));
      }

      // Prepare ordered data
      var orderedData = ApplyOrdering(data, band);

      // Draw content for each item in the data source
      foreach (var itemData in orderedData)
      {
        // Apply filter expression if it exists
        // Include item if: no filter exists OR the item matches the filter
        if (band.FilterExpression == null || band.FilterExpression.Evaluate(itemData))
        {
          DrawBandContent(band, itemData);

          // Draw sub bands
          foreach (var subBand in band.Bands)
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

      // Draw footer band if it exists
      if (band.FooterBand != null)
      {
        DrawBandContent(band.FooterBand, GetFirstItem(data));
      }
    }

    private static IEnumerable<object> ApplyOrdering(IEnumerable data, ReportBand band)
    {
      if (data == null)
      {
        return Enumerable.Empty<object>();
      }

      var items = data.Cast<object>().ToList();
      if (band?.Ordering == null || band.Ordering.Count == 0)
      {
        return items;
      }

      IOrderedEnumerable<object> ordered = null;
      var comparer = new ObjectComparer();

      foreach (var ord in band.Ordering)
      {
        object keySelector(object obj) => GetPropertyValue(obj, ord.Property);
        ordered = ordered == null
          ? ord.Direction == SortDirection.Ascending
            ? items.OrderBy(keySelector, comparer)
            : items.OrderByDescending(keySelector, comparer)
          : ord.Direction == SortDirection.Ascending
            ? ordered.ThenBy(keySelector, comparer)
            : ordered.ThenByDescending(keySelector, comparer);
      }

      return ordered != null ? ordered : items;
    }

    private static object GetPropertyValue(object obj, string propertyPath)
    {
      if (obj == null || string.IsNullOrWhiteSpace(propertyPath))
      {
        return null;
      }

      var parts = propertyPath.Split('.');
      object current = obj;
      foreach (var part in parts)
      {
        if (current == null)
        {
          return null;
        }

        var prop = current.GetType().GetProperty(part, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (prop == null)
        {
          return null;
        }

        current = prop.GetValue(current);
      }
      return current;
    }

    private class ObjectComparer : IComparer<object>, IComparer
    {
      int IComparer<object>.Compare(object x, object y)
      {
        return CompareObjects(x, y);
      }

      int IComparer.Compare(object x, object y)
      {
        return CompareObjects(x, y);
      }

      private static int CompareObjects(object x, object y)
      {
        if (ReferenceEquals(x, y))
        {
          return 0;
        }

        if (x == null)
        {
          return -1;
        }

        if (y == null)
        {
          return 1;
        }

        if (x is IComparable xc && y is IComparable yc && x.GetType() == y.GetType())
        {
          return xc.CompareTo(yc);
        }

        // Try to compare as numbers
        if (TryConvertToDouble(x, out var dx) && TryConvertToDouble(y, out var dy))
        {
          return dx.CompareTo(dy);
        }

        // Fallback to string comparison
        var xs = x.ToString();
        var ys = y.ToString();
        return string.Compare(xs, ys, StringComparison.CurrentCulture);
      }

      private static bool TryConvertToDouble(object o, out double d)
      {
        if (o is double dd) { d = dd; return true; }
        if (o is float f) { d = f; return true; }
        if (o is int i) { d = i; return true; }
        if (o is long l) { d = l; return true; }
        if (o is decimal dec) { d = (double)dec; return true; }
        if (double.TryParse(Convert.ToString(o), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var parsed)) { d = parsed; return true; }
        d = 0; return false;
      }
    }

    private void DrawBandContent(ReportBandBase band, object itemData)
    {
      // Check if current band fits onto page, if not create new page
      if (CreateNewPage(band))
      {
        // Current band does not fit onto page -> create next page
        CreatePageFromCurrentCanvas();

        // Create canvas for next page and reset y value
        CreateNewCanvas();

        // Draw header band on new page if it exists
        DrawHeader(band);
      }

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
      _currentY += band.ActualHeight;
    }

    /// <summary>
    /// Draws the header band of the given band if it exists.
    /// This is used to draw the header band on new pages when a band does not fit onto one page. 
    /// </summary>
    private void DrawHeader(ReportBandBase band)
    {
      if (band is not ReportBand reportBand)
      {
        return;
      }

      // Draw header band if it exists
      if (reportBand.HeaderBand != null && reportBand.HeaderBand.RepeatOnEachPage)
      {
        DrawBandContent(reportBand.HeaderBand, GetFirstItem(_data));
      }
    }

    /// <summary>
    /// Creates a new page if the current band does not fit onto the current page.
    /// A new page is only created if the current band does not fit onto the current page and if the band itself is smaller than a page (to prevent infinite loops).
    /// </summary>
    private bool CreateNewPage(ReportBandBase band)
    {
      return band != null && _currentY + band.ActualHeight >= ActualHeight && band.ActualHeight < ActualHeight;
    }

    private void CreatePageFromCurrentCanvas()
    {
      if (_currentCanvas == null || _printableArea == null)
      {
        return;
      }

      _currentCanvas.Measure(new Size(_currentCanvas.Width, _currentCanvas.Height));
      _currentCanvas.Arrange(new Rect(new Point(_printableArea.OriginWidth, _printableArea.OriginHeight), new Size(_currentCanvas.Width, _currentCanvas.Height)));
      var dp = new DocumentPage(_currentCanvas, _pageSize, new Rect(), new Rect(new Point(_printableArea.OriginWidth, _printableArea.OriginHeight), new Size(ActualWidth, ActualHeight)));
      _pages.Add(dp);
    }

    private static IEnumerable GetSubData(object data, string dataSource)
    {
      if (data == null || string.IsNullOrWhiteSpace(dataSource))
      {
        return null;
      }

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

    private static object GetFirstItem(IEnumerable data)
    {
      if (data == null)
      {
        return null;
      }

      var enumerator = data.GetEnumerator();
      return enumerator.MoveNext() ? enumerator.Current : null;
    }

    #endregion
  }
}
