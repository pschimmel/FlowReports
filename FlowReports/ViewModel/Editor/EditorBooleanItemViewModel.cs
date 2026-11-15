using FlowReports.Model.ReportItems;

namespace FlowReports.ViewModel.Editor
{
  public class EditorBooleanItemViewModel : EditorItemViewModel<BooleanItem>
  {
    public EditorBooleanItemViewModel(BooleanItem item, IItemContainerViewModel bandVM)
      : base(item, bandVM)
    { }
  }
}
