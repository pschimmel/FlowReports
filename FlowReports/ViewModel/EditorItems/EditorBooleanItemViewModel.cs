using FlowReports.Model.ReportItems;

namespace FlowReports.ViewModel.EditorItems
{
  public class EditorBooleanItemViewModel : EditorItemViewModel<BooleanItem>
  {
    public EditorBooleanItemViewModel(BooleanItem item, ReportBandViewModel bandVM)
      : base(item, bandVM)
    { }
  }
}
