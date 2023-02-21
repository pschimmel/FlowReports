using FlowReports.Model.DataSources;
using FlowReports.Model.DataSources.DataSourceItems;

namespace FlowReports.UI.ViewModel
{
  public class DataSourceViewModel : ViewModelBase
  {
    private readonly DataSource _dataSource;
    private readonly Lazy<List<IDataSourceItemViewModel>> _lazyChildren;

    public DataSourceViewModel(DataSource dataSource)
    {
      _dataSource = dataSource;
      _lazyChildren = new Lazy<List<IDataSourceItemViewModel>>(() => DataSourceViewModel.GetChildren(dataSource));
    }

    internal static List<IDataSourceItemViewModel> GetChildren(IDataSourceItemContainer dataSource)
    {
      var list = new List<IDataSourceItemViewModel>();

      foreach (var item in dataSource)
      {
        if (item is IDataSourceItemContainer container)
        {
          list.Add(new DataSourceListViewModel(container));
        }
        else
        {
          list.Add(new DataSourceItemViewModel(item));
        }
      }

      return list;
    }

    public string Name => _dataSource.Name;

    public List<IDataSourceItemViewModel> Children => _lazyChildren.Value;

    #region IDisposable

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);

      if (disposing)
      {
        if (_lazyChildren.IsValueCreated)
        {
          foreach (var child in Children)
          {
            child.Dispose();
          }
        }
      }
    }

    #endregion
  }
}

