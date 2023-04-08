using FlowReports.Model.ReportItems;

namespace FlowReports.UI.ViewModel
{
  internal class ImageItemViewModel : ItemViewModel<ImageItem>
  {
    public ImageItemViewModel(ImageItem item, object data, double deltaY)
      : base(item, data, deltaY)
    { }
  }
}
