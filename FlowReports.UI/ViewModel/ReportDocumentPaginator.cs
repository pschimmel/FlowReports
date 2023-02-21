using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using FlowReports.Model.DataSources.DataSourceItems;
using FlowReports.Model.ReportItems;
using FlowReports.UI.Infrastructure;

namespace FlowReports.UI.ViewModel
{
  internal class ReportDocumentPaginator : DocumentPaginator
  {
    private Size _pageSize;
    private List<DocumentPage> _pages = new();
    private readonly ReportDocumentPaginatorSource _source;

    /// <summary>
    /// Initializes a new instance of the <see cref="NASDocumentPaginator"/> class.
    /// </summary>
    public ReportDocumentPaginator(ReportDocumentPaginatorSource source)
    {
      _source = source ?? throw new ArgumentNullException(nameof(source));
      PageMediaSize pageMediaSize = source.Size;
      PrintableArea = source.PrintableArea;
      Orientation = source.Orientation;

      _pageSize = Orientation == PageOrientation.Landscape || Orientation == PageOrientation.ReverseLandscape
        ? new Size(pageMediaSize.Height ?? 0, pageMediaSize.Width ?? 0)
        : new Size(pageMediaSize.Width ?? 0, pageMediaSize.Height ?? 0);
    }

    /// <summary>
    /// Gets a count of the number of pages currently formatted
    /// </summary>
    /// <returns>A count of the number of pages that have been formatted.</returns>
    public override int PageCount => PagesX * PagesY;

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
    /// Gets or sets the printable area.
    /// </summary>
    /// <value>
    /// The printable area.
    /// </value>
    public PageImageableArea PrintableArea { get; set; }

    /// <summary>
    /// Gets or sets the orientation.
    /// </summary>
    /// <value>
    /// The orientation.
    /// </value>
    public PageOrientation Orientation { get; set; }

    /// <summary>
    /// Gets a value indicating whether <see cref="P:System.Windows.Documents.DocumentPaginator.PageCount"/> is the total number of pages.
    /// </summary>
    /// <returns>true if pagination is complete and <see cref="P:System.Windows.Documents.DocumentPaginator.PageCount"/> is the total number of pages; otherwise, false, if pagination is in process and <see cref="P:System.Windows.Documents.DocumentPaginator.PageCount"/> is the number of pages currently formatted (not the total).This value may revert to false, after being true, if <see cref="P:System.Windows.Documents.DocumentPaginator.PageSize"/> or content changes; because those events would force a repagination.</returns>
    public override bool IsPageCountValid => true;

    /// <summary>
    /// Returns the element being paginated.
    /// </summary>
    /// <returns>An <see cref="T:System.Windows.Documents.IDocumentPaginatorSource"/> representing the element being paginated.</returns>
    public override IDocumentPaginatorSource Source => _source;

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
      if (pageNumber < 0)
      {
        throw new ArgumentOutOfRangeException(nameof(pageNumber));
      }

      if (_pages.Count == 0)
      {
        CreatePages();
      }

      return pageNumber >= _pages.Count ? throw new ArgumentOutOfRangeException(nameof(pageNumber)) : _pages[pageNumber - 1];
    }

    #region Private members

    private List<DocumentPage> CreatePages()
    {
      var pages = new List<DocumentPage>();
      double currentY = 0;

      // There s at least one page.
      var currentCanvas = new Canvas();

      foreach (var band in _source.Report.Bands)
      {
        if (currentY + band.Height < ActualHeight)
        {
          if (TryFindSource(_source.DataSource, band.DataSource, out IDataSourceItemContainer source))
          {
          }

          // Current band will fit into page
          foreach (var item in band.Items)
          {

            if (item is TextItem textItem)
            {
              var textBlock = new TextBlock
              {
                Text = textItem.Text,
                Width = textItem.Width,
                Height = textItem.Height,
                TextWrapping = TextWrapping.Wrap
              };
              currentCanvas.Children.Add(textBlock);
              Canvas.SetTop(textBlock, currentY + textItem.Top);
              Canvas.SetLeft(textBlock, textItem.Left);
            }
          }

          // Increase current y position
          currentY += band.Height;
        }
        else
        {
          var page = new DocumentPage(currentCanvas);
          pages.Add(page);

          // Create canvas for next page and reset y valze
          currentCanvas = new Canvas();
          currentY = 0;
        }
      }

      return pages;
    }

    private bool TryFindSource(IDataSourceItemContainer root, string fullName, out IDataSourceItemContainer source)
    {
      source = null;
      string[] nameArray = fullName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

      if (nameArray.Length > 0)
      {
        string name = nameArray.First();

        if (root.Name == name)
        {
          if (nameArray.Length == 1)
          {
            source = root;
            return true;
          }

          foreach (var childRoot in root)
          {
            if (childRoot is IDataSourceItemContainer childRootContainer &&
                TryFindSource(childRootContainer, string.Join(".", nameArray.Skip(1).ToArray()), out IDataSourceItemContainer childSource) &&
                childSource != null)
            {
              source = childSource;
              return true;
            }
          }
        }
      }

      return false;
    }

    private void OnProgress(double value)
    {
      EventService.Instance.Publish("Progress", value);
    }

    private int PagesX => 1;

    private int PagesY => _pages.Count;

    private double ActualWidth
    {
      get
      {
        if (PrintableArea == null)
        {
          return _pageSize.Width;
        }

        if (Orientation == PageOrientation.Landscape || Orientation == PageOrientation.ReversePortrait)
        {
          if (PrintableArea.ExtentHeight > PrintableArea.ExtentWidth)
          {
            return PrintableArea.ExtentHeight;
          }
        }
        return PrintableArea.ExtentWidth;
      }
    }

    private double ActualHeight
    {
      get
      {
        if (PrintableArea == null)
        {
          return _pageSize.Height;
        }

        if (Orientation == PageOrientation.Landscape || Orientation == PageOrientation.ReversePortrait)
        {
          if (PrintableArea.ExtentHeight > PrintableArea.ExtentWidth)
          {
            return PrintableArea.ExtentWidth;
          }
        }
        return PrintableArea.ExtentHeight;
      }
    }

    private double OriginActualWidth
    {
      get
      {
        if (Orientation == PageOrientation.Landscape || Orientation == PageOrientation.ReversePortrait)
        {
          if (PrintableArea.ExtentHeight > PrintableArea.ExtentWidth)
          {
            return PrintableArea.OriginHeight;
          }
        }
        return PrintableArea.OriginWidth;
      }
    }

    private double OriginActualHeight
    {
      get
      {
        if (Orientation == PageOrientation.Landscape || Orientation == PageOrientation.ReversePortrait)
        {
          if (PrintableArea.ExtentHeight > PrintableArea.ExtentWidth)
          {
            return PrintableArea.OriginWidth;
          }
        }
        return PrintableArea.OriginHeight;
      }
    }

    #endregion
  }
}
