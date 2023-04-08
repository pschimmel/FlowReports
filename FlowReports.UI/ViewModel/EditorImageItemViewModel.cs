using FlowReports.Model.ReportItems;

namespace FlowReports.UI.ViewModel
{
  internal class EditorImageItemViewModel

    : EditorItemViewModel<ImageItem>
  {
    public EditorImageItemViewModel(ImageItem item, ReportBandViewModel bandVM)
      : base(item, bandVM)
    { }
  }
}
