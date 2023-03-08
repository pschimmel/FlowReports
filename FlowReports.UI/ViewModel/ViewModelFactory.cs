using FlowReports.Model.ReportItems;

namespace FlowReports.UI.ViewModel
{
  internal static class ViewModelFactory
  {
    internal static IItemViewModel CreateItemViewModel(ReportElement item, ReportBandViewModel bandVM)
    {
      return item switch
      {
        TextItem textItem => new TextItemViewModel(textItem, bandVM),
        BooleanItem booleanItem => new BooleanItemViewModel(booleanItem, bandVM),
        ImageItem imageItem => new ImageItemViewModel(imageItem, bandVM),
        _ => throw new NotImplementedException($"There is no ViewModel defined for item type {item.GetType().Name}.")
      };
    }
  }
}