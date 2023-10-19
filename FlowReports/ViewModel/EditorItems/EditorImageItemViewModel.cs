using FlowReports.Model.ReportItems;

namespace FlowReports.ViewModel.EditorItems
{
  public class EditorImageItemViewModel

    : EditorItemViewModel<ImageItem>
  {
    public EditorImageItemViewModel(ImageItem item, ReportBandViewModel bandVM)
      : base(item, bandVM)
    { }
  }
}
