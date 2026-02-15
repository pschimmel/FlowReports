using FlowReports.Model.DataSources;

namespace FlowReports.Model
{
  /// <summary>
  /// Provides static methods for analyzing data and managing report settings.
  /// </summary>
  public static class ReportEngine
  {
    /// <summary>
    /// Analyzes data structure and updates the report with the data and data source information.
    /// </summary>
    /// <typeparam name="T">The type of items in the data collection.</typeparam>
    /// <param name="report">The report to update.</param>
    /// <param name="items">The data items to analyze.</param>
    /// <param name="dataSourceName">The name of the top-level data source.</param>
    public static void Analyze<T>(Report report, IEnumerable<T> items, string dataSourceName) where T : class
    {
      report.Data = items;
      report.TypeOfData = typeof(T);
      report.DataSource = DataSourceAnalyzer.Analyze(items, dataSourceName);
    }

    /// <summary>
    /// Gets or sets the default report engine settings.
    /// </summary>
    public static Settings Settings { get; set; } = Settings.Default;
  }
}
