using FlowReports.Model.ReportItems;

namespace FlowReports.ViewModel.ReportItems
{
  public class ImageItemViewModel : ItemViewModel<ImageItem>
  {
    public ImageItemViewModel(ImageItem item, object data, double deltaY)
      : base(item, data, deltaY)
    { }

    public object ImageSource => GetValue();
  }
}
