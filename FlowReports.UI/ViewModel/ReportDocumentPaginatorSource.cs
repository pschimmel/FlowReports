//using System.Collections;
//using System.Printing;
//using System.Windows.Documents;
//using FlowReports.Model;
//using FlowReports.Model.DataSources;

//namespace FlowReports.UI.ViewModel
//{
//  public class ReportDocumentPaginatorSource : IDocumentPaginatorSource
//  {
//    public static readonly PageMediaSize DefaultPageSize = new(PageMediaSizeName.ISOA4);
//    public const PageOrientation DefaultOrientation = PageOrientation.Portrait;

//    public ReportDocumentPaginatorSource(Report report, PageMediaSize size = null, PageImageableArea printableArea = null, PageOrientation? orientation = null)
//    {
//      Report = report ?? throw new ArgumentNullException(nameof(report));
//      DataSource = report.DataSource;
//      Data = report.Data;
//      Size = size ?? DefaultPageSize;
//      if (printableArea != null)
//      {
//        try
//        {
//          var printer = LocalPrintServer.GetDefaultPrintQueue();
//          PrintableArea = printer.GetPrintCapabilities().PageImageableArea;
//        }
//        catch
//        {
//        }
//      }
//      else
//      {
//        PrintableArea = printableArea;
//      }

//      Orientation = orientation ?? DefaultOrientation;
//    }

//    public Report Report { get; }

//    public PageMediaSize Size { get; }

//    public PageImageableArea PrintableArea { get; }

//    public PageOrientation Orientation { get; }

//    public DocumentPaginator DocumentPaginator => new ReportDocumentPaginator(this);

//    public DataSource DataSource { get; }

//    public IEnumerable Data { get; }
//  }
//}