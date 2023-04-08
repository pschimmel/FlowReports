using FlowReports.Model.ReportItems;

namespace FlowReports.UI.ViewModel
{
  internal class BooleanItemViewModel : ItemViewModel<BooleanItem>
  {
    public BooleanItemViewModel(BooleanItem item, object data, double deltaY)
      : base(item, data, deltaY)
    { }
  }
}
