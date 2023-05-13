using System.Diagnostics;
using System.Printing;

namespace FlowReports.ViewModel.Printing
{
  public class PageInformation
  {
    public PageInformation(PrintQueue printer,
                           PageMediaSize size,
                           PageImageableArea printableArea,
                           PageOrientation orientation)
    {
      Printer = printer;
      PageSize = size;
      PrintableArea = printableArea;
      Orientation = orientation;
    }

    public PrintQueue Printer { get; }

    public PageMediaSize PageSize { get; }

    public PageImageableArea PrintableArea { get; }

    public PageOrientation Orientation { get; }

    public static PageInformation Default
    {
      get
      {
        try
        {
          var printQueue = LocalPrintServer.GetDefaultPrintQueue();
          var ticket = printQueue.DefaultPrintTicket;
          var pageMediaSize = ticket.PageMediaSize;
          var printableArea = printQueue.GetPrintCapabilities(ticket).PageImageableArea;
          var orientation = ticket.PageOrientation ?? PageOrientation.Portrait;

          var information = new PageInformation(printQueue, pageMediaSize, printableArea, orientation);
          return information;
        }
        catch (Exception ex)
        {
          Debug.Fail(ex.Message);
          return null;
        }
      }
    }
  }
}
