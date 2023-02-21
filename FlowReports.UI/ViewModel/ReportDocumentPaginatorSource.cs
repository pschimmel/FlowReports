using System.Collections;
using System.Printing;
using System.Windows.Documents;
using FlowReports.Model;
using FlowReports.Model.DataSources;

namespace FlowReports.UI.ViewModel
{
  public class ReportDocumentPaginatorSource : IDocumentPaginatorSource
  {
    private static readonly PageMediaSize _defaultPageSize = new(PageMediaSizeName.ISOA4);

    public ReportDocumentPaginatorSource(Report report, DataSource dataSource, IEnumerable data, PageMediaSize size, PageImageableArea printableArea, PageOrientation orientation)
    {
      //      else
      //      {
      //        try
      //        {
      //          var printer = LocalPrintServer.GetDefaultPrintQueue();
      //    PrintableArea = printer.GetPrintCapabilities().PageImageableArea;
      //        }
      //        catch { }
      //Orientation = PageOrientation.Portrait;
      //      }

      Report = report ?? throw new ArgumentNullException(nameof(report));
      Size = size ?? throw new ArgumentNullException(nameof(size));
      PrintableArea = printableArea ?? throw new ArgumentNullException(nameof(printableArea));
      Orientation = orientation;
      DataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
      Data = data ?? throw new ArgumentNullException(nameof(data));
    }

    public Report Report { get; }

    public PageMediaSize Size { get; }

    public PageImageableArea PrintableArea { get; }

    public PageOrientation Orientation { get; }

    public DocumentPaginator DocumentPaginator => new ReportDocumentPaginator(this);

    public DataSource DataSource { get; }

    public IEnumerable Data { get; }
  }
}