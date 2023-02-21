using ES.Tools.Core.MVVM;
using FlowReports.Model;
using FlowReports.Model.ImportExport;
using FlowReports.UI.ViewModel;

namespace FlowReports.UI
{
  public static class FlowReport
  {
    public static Report Create<T>(IEnumerable<T> data) where T : class
    {
      var report = new Report();
      report.Analyze(data);
      return report;
    }

    public static Report Load(string filePath)
    {
      return ReportReader.Read(filePath);
    }

    public static void Edit(Report report, string filePath)
    {
      var viewModel = new ReportEditorViewModel(report, filePath);
      // Don't set the owner as it might be the only window.
      var view = ViewFactory.Instance.CreateView(viewModel, false);
      view.ShowDialog();
    }

    public static void Edit<T>(string filePath, IEnumerable<T> data) where T : class
    {
      var report = Load(filePath);
      report.Analyze(data);
      Edit(report, filePath);
    }
  }
}
