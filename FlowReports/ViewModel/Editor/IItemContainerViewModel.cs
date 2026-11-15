using System.Collections.ObjectModel;

namespace FlowReports.ViewModel.Editor
{
  public interface IItemContainerViewModel
  {
    Guid ID { get; }

    ObservableCollection<IEditorItemViewModel> Items { get; }

    IEditorItemViewModel AddBooleanItem();

    IEditorItemViewModel AddImageItem();

    IEditorItemViewModel AddTextItem();

    void RemoveItem(IEditorItemViewModel itemVM);

    void Select();
  }
}