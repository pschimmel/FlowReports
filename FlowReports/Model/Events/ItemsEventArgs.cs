using FlowReports.Model.ReportItems;

namespace FlowReports.Model.Events
{
  public class ItemsEventArgs : GenericEventArgs<ReportItem>
  {
    public ItemsEventArgs(ReportItem item)
      : base(item)
    { }
  }
}
