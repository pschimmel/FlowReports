using FlowReports.Model.DataSources.DataSourceItems;

namespace FlowReports.UI.ViewModel
{
  public class DataSourceItemViewModel : ViewModelBase, IDataSourceItemViewModel
  {
    private readonly IDataSourceItem _item;

    public DataSourceItemViewModel(IDataSourceItem item)
    {
      if (item is IDataSourceItemContainer)
      {
        throw new ArgumentException($"Parameter {nameof(item)} cannot be of type {nameof(IDataSourceItemContainer)}", nameof(item));
      }

      _item = item;

      Icon = _item switch
      {
        IDataSourceItemContainer => "Database_16x.png",
        DateField => "Calendar_16x.png",
        NumberField => "Number_16x.png",
        TextField => "Text_16x.png",
        _ => throw new NotImplementedException("Unknown DataSourceItem type."),
      };
    }

    public string Name => _item.Name;

    public bool CanHaveChildren => false;

    public string Icon { get; }
  }
}
