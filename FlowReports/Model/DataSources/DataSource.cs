using System.Diagnostics;
using FlowReports.Model.DataSources.DataSourceItems;

namespace FlowReports.Model.DataSources
{
  [DebuggerDisplay("{Name} ({Count})")]
  public class DataSource : DataSourceItemList
  {
  }
}
