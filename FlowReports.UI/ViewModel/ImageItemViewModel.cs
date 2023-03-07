using FlowReports.Model.ReportItems;

namespace FlowReports.UI.ViewModel
{
  internal class ImageItemViewModel : EditorItemViewModel<ImageItem>
  {
    public ImageItemViewModel(ImageItem item, ReportBandViewModel bandVM)
      : base(item, bandVM)
    { }
  }
}
