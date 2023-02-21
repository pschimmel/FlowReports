namespace FlowReports.Model.DataSources.DataSourceItems
{
  public abstract class DataSourceItem : IDataSourceItem
  {
    public string Name { get; set; }

    public Type Type { get; set; }
  }
}
