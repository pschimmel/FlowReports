namespace FlowReports.Model.DataSources.DataSourceItems
{
  public interface IDataSourceItem
  {
    string Name { get; set; }

    Type Type { get; set; }
  }
}