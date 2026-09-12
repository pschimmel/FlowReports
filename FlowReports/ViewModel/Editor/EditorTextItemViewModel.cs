using FlowReports.Model.ReportItems;

namespace FlowReports.ViewModel.Editor
{
  public class EditorTextItemViewModel : EditorItemViewModel<TextItem>
  {
    public EditorTextItemViewModel(TextItem item, IItemContainerViewModel bandVM)
      : base(item, bandVM)
    { }

    public bool Bold
    {
      get => _item.Bold;
      set
      {
        if (_item.Bold != value)
        {
          _item.Bold = value;
          OnPropertyChanged();
        }
      }
    }

    public bool Italic
    {
      get => _item.Italic;
      set
      {
        if (_item.Italic != value)
        {
          _item.Italic = value;
          OnPropertyChanged();
        }
      }
    }

    public bool Underline
    {
      get => _item.Underline;
      set
      {
        if (_item.Underline != value)
        {
          _item.Underline = value;
          OnPropertyChanged();
        }
      }
    }

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
