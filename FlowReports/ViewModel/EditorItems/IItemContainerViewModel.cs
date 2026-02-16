using System.Collections.ObjectModel;

namespace FlowReports.ViewModel.EditorItems
{
  public interface IItemContainerViewModel
  {
    Guid ID { get; }

    ObservableCollection<IEditorItemViewModel> Items { get; }

    void RemoveItem(IEditorItemViewModel itemVM);

    void Select();
  }
}