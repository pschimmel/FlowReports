using FlowReports.Model.ReportItems;

namespace FlowReports.UI.ViewModel
{
  internal class EditorBooleanItemViewModel : EditorItemViewModel<BooleanItem>
  {
    public EditorBooleanItemViewModel(BooleanItem item, ReportBandViewModel bandVM)
      : base(item, bandVM)
    { }
  }
}
