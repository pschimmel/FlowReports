using System.Collections.ObjectModel;
using FlowReports.Model;
using FlowReports.ViewModel.EditorItems;

namespace FlowReports.ViewModel
{
  public interface IBandParentViewModel
  {
    ObservableCollection<ReportBandViewModel> Bands { get; }
    void AddBand();
    void AddBand(ReportBandViewModel otherBand, InsertLocation location);
    void RemoveBand(ReportBandViewModel subBand);
    void MoveBandUp(ReportBandViewModel band);
    void MoveBandDown(ReportBandViewModel band);
    bool CanMoveBandUp(ReportBandViewModel band);
    bool CanMoveBandDown(ReportBandViewModel band);
  }
}