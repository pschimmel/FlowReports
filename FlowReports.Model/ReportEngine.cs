using System.Collections.Generic;
using FlowReports.Model.DataSources;

namespace FlowReports.Model
{
  public static class ReportEngine
  {
    public static void Analyze<T>(Report report, IEnumerable<T> items) where T : class
    {
      report.DataSource = DataSourceAnalyzer.Analyze(items);
    }

    public static Settings Settings { get; } = Settings.Default;
  }
}
