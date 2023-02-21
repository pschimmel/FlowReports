using FlowReports.Model.DataSources;
using FlowReports.Model.ReportItems;

namespace FlowReports.Model
{
  public class Report
  {
    public ReportBandCollection Bands { get; } = new ReportBandCollection();

    public DataSource DataSource { get; internal set; }

    public DateTime LastChanged { get; internal set; }

    public void Analyze<T>(IEnumerable<T> items) where T : class
    {
      ReportEngine.Analyze(this, items);
    }
  }
}
