namespace FlowReports.Model.DataSources.DataSourceItems
{
  public class DateField : DataSourceItem, IFormatItem
  {
    public string DefaultFormat => "d";
    public IEnumerable<string> Formats => new List<string>();
  }
}
