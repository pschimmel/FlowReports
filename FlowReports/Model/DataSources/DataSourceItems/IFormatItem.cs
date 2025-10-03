namespace FlowReports.Model.DataSources.DataSourceItems
{
  public interface IFormatItem : IDataSourceItem
  {
    string DefaultFormat { get; }
    IEnumerable<string> Formats { get; }
  }
}