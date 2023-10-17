using FlowReports.Model.ReportItems;

namespace FlowReports.ViewModel.EditorItems
{
  public class EditorTextItemViewModel : EditorItemViewModel<TextItem>
  {
    public EditorTextItemViewModel(TextItem item, ReportBandViewModel bandVM)
      : base(item, bandVM)
    { }

    public string Format
    {
      get => _item.Format;
      set
      {
        if (_item.Format != value)
        {
          _item.Format = value;
          OnPropertyChanged();
        }
      }
    }
  }
}
