namespace FlowReports.Model.DataSources.DataSourceItems
{
  public class DataSourceItemList : List<IDataSourceItem>, IDataSourceItemContainer
  {
    public string Name { get; set; }

    public Type Type { get; set; }
  }
}
