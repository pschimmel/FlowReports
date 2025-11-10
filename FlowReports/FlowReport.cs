using System.IO;
using ES.Tools.Core.MVVM;
using FlowReports.Model;
using FlowReports.Model.ImportExport;
using FlowReports.View;
using FlowReports.ViewModel;
using FlowReports.ViewModel.EditorItems;
using FlowReports.ViewModel.Printing;

namespace FlowReports
{
  public static class FlowReport
  {
    static FlowReport()
    {
      ViewFactory.Instance.Register<ReportEditorViewModel, ReportEditorWindow>();
      ViewFactory.Instance.Register<ReportBandViewModel, ReportBandDetails>();
      ViewFactory.Instance.Register<PrintPreviewViewModel, PrintPreviewWindow>();
      ViewFactory.Instance.Register<PageSettingsViewModel, PageSettingsWindow>();
      ViewFactory.Instance.Register<AboutViewModel, AboutWindow>();
    }

    /// <summary>
    /// Creates a new empty FlowReport instance and analyzes the given data.
    /// </summary>
    public static Report New<T>(IEnumerable<T> data, string dataSourceName) where T : class
    {
      var report = new Report();
      report.Analyze(data, dataSourceName);
      return report;
    }

    /// <summary>
    /// Loads a FlowReport from disk.
    /// </summary>
    /// <param name="filePath">Path to the file.</param>
    /// <exception cref="FileNotFoundException">The file does not exist.</exception>
    /// <exception cref="FileFormatException">The file is not a FlowReport file.</exception>
    public static Report Load(string filePath)
    {
      return !File.Exists(filePath)
        ? throw new FileNotFoundException("File not found.")
        : !Path.GetExtension(filePath).Equals(Globals.ReportExtension, StringComparison.CurrentCultureIgnoreCase)
        ? throw new FileFormatException("Wrong file type.")
        : ReportReader.Read(filePath);
    }

    /// <summary>
    /// Starts the Report Editor for the given report.
    /// </summary>
    public static void Edit(Report report)
    {
      using var viewModel = new ReportEditorViewModel(report);
      // Don't set the owner as it might be the only window.
      var view = ViewFactory.Instance.CreateView(viewModel, false);
      view.ShowDialog();
    }

    /// <summary>
    /// Starts the Report Editor for the given report and analyzes the data. 
    /// </summary>
    /// <param name="report">Report instance to edit.</param>
    /// <param name="data">Data used as data source.</param>
    /// <param name="dataSourceName">Name of the top level of the data source.</param>
    public static void Edit<T>(Report report, IEnumerable<T> data, string dataSourceName) where T : class
    {
      report.Analyze(data, dataSourceName);
      Edit(report);
    }

    /// <summary>
    /// Shows a print preview of the given report with the given data. 
    /// </summary>
    /// <param name="report">Report instance to show.</param>
    /// <param name="data">Data used as data source.</param>
    public static void Show<T>(Report report, IEnumerable<T> data) where T : class
    {
      report.Data = data;
      using var vm = new PrintPreviewViewModel(report);
      var view = ViewFactory.Instance.CreateView(vm);
      view.ShowDialog();
    }
  }
}
