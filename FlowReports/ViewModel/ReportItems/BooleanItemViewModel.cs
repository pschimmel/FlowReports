using FlowReports.Model.ReportItems;

namespace FlowReports.ViewModel.ReportItems
{
  public class BooleanItemViewModel : ItemViewModel<BooleanItem>
  {
    public BooleanItemViewModel(BooleanItem item, object data, double deltaY)
      : base(item, data, deltaY)
    { }
  }
}
