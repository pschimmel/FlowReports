using FlowReports.Model.DataSources.DataSourceItems;

namespace FlowReports.UI.ViewModel
{
  public class DataSourceListViewModel : ViewModelBase, IDataSourceItemViewModel
  {
    #region Fields

    private readonly IDataSourceItemContainer _item;
    private readonly Lazy<List<IDataSourceItemViewModel>> _lazyChildren;

    #endregion

    #region Constructor

    public DataSourceListViewModel(IDataSourceItemContainer item)
    {
      _item = item;
      _lazyChildren = new Lazy<List<IDataSourceItemViewModel>>(() => GetChildren(item));
    }

    #endregion

    #region Properties

    public string Name => _item.Name;

    public bool CanHaveChildren => true;

    public string Icon => "Database_16x.png";

    public List<IDataSourceItemViewModel> Children => _lazyChildren.Value;

    #endregion

    #region Private methods

    private static List<IDataSourceItemViewModel> GetChildren(IDataSourceItemContainer dataSource)
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

    #endregion

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