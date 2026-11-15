using FlowReports.Model.ReportItems;

namespace FlowReports.Model.Events
{
  public class ReportItemsEventArgs : GenericEventArgs<ReportItem>
  {
    public ReportItemsEventArgs(ReportItem item)
      : base(item)
    { }
  }
}
