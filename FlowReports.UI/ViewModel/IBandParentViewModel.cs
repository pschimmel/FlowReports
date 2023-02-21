using System.Collections.ObjectModel;
using FlowReports.Model;

namespace FlowReports.UI.ViewModel
{
  public interface IBandParentViewModel
  {
    ObservableCollection<ReportBandViewModel> Bands { get; }
    void AddBand();
    void AddBand(ReportBandViewModel otherBand, InsertLocation location);
    void RemoveBand(ReportBandViewModel subBand);
  }
}