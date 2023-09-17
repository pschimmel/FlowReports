using System.Collections.Generic;
using System.IO;
using ES.Tools.Core.MVVM;
using FlowReports.Model;
using FlowReports.Model.ImportExport;
using FlowReports.View;
using FlowReports.ViewModel;
using FlowReports.ViewModel.EditorItems;
using FlowReports.ViewModel.Printing;

namespace FlowReports.App
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

    public static Report New<T>(IEnumerable<T> data) where T : class
    {
      var report = new Report();
      report.Analyze(data);
      return report;
    }

    /// <summary>
    /// Loads a FlowReport from disk.
    /// </summary>
    /// <param name="filePath">Path to the file.</param>
    /// <exception cref="Exce"
    public static Report Load(string filePath)
    {
      if (!File.Exists(filePath))
      {
        throw new FileNotFoundException("File not found.");
      }

      if (Path.GetExtension(filePath).ToLower() != Globals.ReportExtension)
      {
        throw new FileFormatException("Wrong file type.");
      }

      return ReportReader.Read(filePath);
    }

    public static void Edit(Report report)
    {
      using var viewModel = new ReportEditorViewModel(report);
      // Don't set the owner as it might be the only window.
      var view = ViewFactory.Instance.CreateView(viewModel, false);
      view.ShowDialog();
    }

    public static void Edit<T>(Report report, IEnumerable<T> data) where T : class
    {
      report.Analyze(data);
      Edit(report);
    }

    public static void Show<T>(Report report, IEnumerable<T> data) where T : class
    {
      using var vm = new PrintPreviewViewModel(report);
      var view = ViewFactory.Instance.CreateView(vm);
      view.ShowDialog();
    }
  }
}
