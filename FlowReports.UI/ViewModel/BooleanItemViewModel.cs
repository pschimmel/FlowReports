using FlowReports.Model.ReportItems;

namespace FlowReports.UI.ViewModel
{
  internal class BooleanItemViewModel : EditorItemViewModel<BooleanItem>
  {
    public BooleanItemViewModel(BooleanItem item, ReportBandViewModel bandVM)
      : base(item, bandVM)
    { }
  }
}
