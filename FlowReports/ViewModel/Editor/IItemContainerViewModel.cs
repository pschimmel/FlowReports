using System.Collections.ObjectModel;

namespace FlowReports.ViewModel.Editor
{
  public interface IItemContainerViewModel
  {
    Guid ID { get; }

    ObservableCollection<IEditorItemViewModel> Items { get; }

    void RemoveItem(IEditorItemViewModel itemVM);

    void Select();
  }
}