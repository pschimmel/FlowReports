using FlowReports.Model.ReportItems;

namespace FlowReports.Model.Events
{
  public class HeaderEventArgs : GenericEventArgs<HeaderBand>
  {
    public HeaderEventArgs(HeaderBand header)
      : base(header)
    { }
  }
}
