namespace FlowReports.Model.DataSources.DataSourceItems
{
  public class NumberField : DataSourceItem, IFormatItem
  {
    public string DefaultFormat => "f2";
    public IEnumerable<string> Formats => new List<string> { };
  }
}
