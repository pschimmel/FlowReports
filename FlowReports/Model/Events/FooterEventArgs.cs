using FlowReports.Model.ReportItems;

namespace FlowReports.Model.Events
{
  public class FooterEventArgs : GenericEventArgs<FooterBand>
  {
    public FooterEventArgs(FooterBand footer)
      : base(footer)
    { }
  }
}
