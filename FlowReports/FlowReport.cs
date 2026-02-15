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
  /// <summary>
  /// Provides static methods for creating, loading, editing, and displaying reports.
  /// </summary>
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
    /// Creates a new empty report and analyzes the given data.
    /// </summary>
    /// <typeparam name="T">The type of items in the data collection.</typeparam>
    /// <param name="data">The data to attach to the report.</param>
    /// <param name="dataSourceName">The name of the top-level data source.</param>
    /// <returns>A new Report instance with the data analyzed.</returns>
    public static Report New<T>(IEnumerable<T> data, string dataSourceName) where T : class
    {
      var report = new Report();
      report.Analyze(data, dataSourceName);
      return report;
    }

    /// <summary>
    /// Loads a report from the specified file path.
    /// </summary>
    /// <param name="filePath">The path to the report file to load.</param>
    /// <returns>The loaded Report instance.</returns>
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
    /// Opens the Report Editor for the given report.
    /// </summary>
    /// <param name="report">The report to edit.</param>
    public static void Edit(Report report)
    {
      using var viewModel = new ReportEditorViewModel(report);
      var view = ViewFactory.Instance.CreateView(viewModel, false);
      view.ShowDialog();
    }

    /// <summary>
    /// Opens the Report Editor for the given report and analyzes the data.
    /// </summary>
    /// <typeparam name="T">The type of items in the data collection.</typeparam>
    /// <param name="report">The report to edit.</param>
    /// <param name="data">The data to attach to the report.</param>
    /// <param name="dataSourceName">The name of the top-level data source.</param>
    public static void Edit<T>(Report report, IEnumerable<T> data, string dataSourceName) where T : class
    {
      report.Analyze(data, dataSourceName);
      Edit(report);
    }

    /// <summary>
    /// Shows a print preview of the report with the specified data.
    /// </summary>
    /// <typeparam name="T">The type of items in the data collection.</typeparam>
    /// <param name="report">The report to preview.</param>
    /// <param name="data">The data to use for the preview.</param>
    public static void Show<T>(Report report, IEnumerable<T> data) where T : class
    {
      report.Data = data;
      using var vm = new PrintPreviewViewModel(report);
      var view = ViewFactory.Instance.CreateView(vm);
      view.ShowDialog();
    }
  }
}
