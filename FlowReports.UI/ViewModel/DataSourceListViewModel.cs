using FlowReports.Model.DataSources.DataSourceItems;

namespace FlowReports.UI.ViewModel
{
  public class DataSourceListViewModel : ViewModelBase, IDataSourceItemViewModel
  {
    private readonly IDataSourceItemContainer _item;
    private readonly Lazy<List<IDataSourceItemViewModel>> _lazyChildren;

    public DataSourceListViewModel(IDataSourceItemContainer item)
    {
      _item = item;
      _lazyChildren = new Lazy<List<IDataSourceItemViewModel>>(() => DataSourceViewModel.GetChildren(item));
    }

    public string Name => _item.Name;

    public bool CanHaveChildren => true;

    public string Icon => "Database_16x.png";

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