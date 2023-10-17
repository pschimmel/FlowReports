using FlowReports.Model.ReportItems;

namespace FlowReports.Model.Events
{
  public class BandsEventArgs : GenericEventArgs<(int Index, ReportBand Band)>
  {
    public BandsEventArgs(int index, ReportBand band)
      : base((index, band))
    { }
  }
}