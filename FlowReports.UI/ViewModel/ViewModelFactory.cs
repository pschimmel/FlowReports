using FlowReports.Model.ReportItems;

namespace FlowReports.UI.ViewModel
{
  internal static class ViewModelFactory
  {
    internal static IEditorItemViewModel CreateItemViewModel(ReportElement item, ReportBandViewModel bandVM)
    {
      return item switch
      {
        TextItem textItem => new EditorTextItemViewModel(textItem, bandVM),
        BooleanItem booleanItem => new EditorBooleanItemViewModel(booleanItem, bandVM),
        ImageItem imageItem => new EditorImageItemViewModel(imageItem, bandVM),
        _ => throw new NotImplementedException($"There is no ViewModel defined for item type {item.GetType().Name}.")
      };
    }

    internal static IItemViewModel CreatePreviewItemViewModel(ReportElement item, object data, double deltaY)
    {
      return item switch
      {
        TextItem textItem => new TextItemViewModel(textItem, data, deltaY),
        BooleanItem booleanItem => new BooleanItemViewModel(booleanItem, data, deltaY),
        ImageItem imageItem => new ImageItemViewModel(imageItem, data, deltaY),
        _ => throw new NotImplementedException($"There is no ViewModel defined for item type {item.GetType().Name}.")
      };
    }
  }
}