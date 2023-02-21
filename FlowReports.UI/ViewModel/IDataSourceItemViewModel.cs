namespace FlowReports.UI.ViewModel
{
  public interface IDataSourceItemViewModel : IDisposable
  {
    bool CanHaveChildren { get; }
    string Icon { get; }
    string Name { get; }
  }
}