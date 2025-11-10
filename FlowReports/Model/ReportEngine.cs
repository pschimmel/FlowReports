using FlowReports.Model.DataSources;

namespace FlowReports.Model
{
  public static class ReportEngine
  {
    public static void Analyze<T>(Report report, IEnumerable<T> items, string dataSourceName) where T : class
    {
      report.Data = items;
      report.TypeOfData = typeof(T);
      report.DataSource = DataSourceAnalyzer.Analyze(items, dataSourceName);
    }

    public static Settings Settings { get; set; } = Settings.Default;
  }
}
