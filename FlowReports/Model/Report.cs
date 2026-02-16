using System.Collections;
using FlowReports.Model.DataSources;
using FlowReports.Model.ReportItems;

namespace FlowReports.Model
{
  /// <summary>
  /// Represents a report that contains bands and display items.
  /// </summary>
  public class Report : IHasBands
  {
    /// <summary>
    /// Gets the collection of bands in the report.
    /// </summary>
    public ReportBandCollection Bands { get; } = new ReportBandCollection();

    /// <summary>
    /// Gets the data source associated with the report.
    /// </summary>
    public DataSource DataSource { get; internal set; }

    /// <summary>
    /// Gets the date and time when the report was last modified.
    /// </summary>
    public DateTime LastChanged { get; internal set; }

    /// <summary>
    /// Gets the data items used by the report.
    /// </summary>
    public IEnumerable Data { get; internal set; }

    /// <summary>
    /// Gets the type of data items in the report.
    /// </summary>
    public Type TypeOfData { get; internal set; }

    /// <summary>
    /// Gets or sets the file path of the report.
    /// </summary>
    public string FilePath { get; set; }

    /// <summary>
    /// Analyzes the data structure and updates the report's data source.
    /// </summary>
    /// <typeparam name="T">The type of items in the data collection.</typeparam>
    /// <param name="items">The data items to analyze.</param>
    /// <param name="dataSourceName">The name of the top-level data source.</param>
    public void Analyze<T>(IEnumerable<T> items, string dataSourceName) where T : class
    {
      ReportEngine.Analyze(this, items, dataSourceName);
    }
  }
}
