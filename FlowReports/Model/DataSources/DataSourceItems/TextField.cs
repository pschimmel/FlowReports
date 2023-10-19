namespace FlowReports.Model.DataSources.DataSourceItems
{
  public class TextField : DataSourceItem, IFormatItem
  {
    public string DefaultFormat => "";
    public IEnumerable<string> Formats => Enumerable.Empty<string>();
  }
}
