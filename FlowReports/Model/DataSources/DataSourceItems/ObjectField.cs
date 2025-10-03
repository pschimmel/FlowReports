namespace FlowReports.Model.DataSources.DataSourceItems
{
  public class ObjectField : List<IDataSourceItem>, IDataSourceItemContainer
  {
    public string Name { get; set; }

    public Type Type { get; set; }
  }
}
