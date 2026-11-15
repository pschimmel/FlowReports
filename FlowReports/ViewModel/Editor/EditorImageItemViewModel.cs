using FlowReports.Model.ReportItems;

namespace FlowReports.ViewModel.Editor
{
  public class EditorImageItemViewModel

    : EditorItemViewModel<ImageItem>
  {
    public EditorImageItemViewModel(ImageItem item, IItemContainerViewModel bandVM)
      : base(item, bandVM)
    { }
  }
}
