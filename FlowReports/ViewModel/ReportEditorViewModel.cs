using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using ES.Tools.Core.Infrastructure;
using ES.Tools.Core.MVVM;
using FlowReports.Model;
using FlowReports.ViewModel.Editor;
using FlowReports.ViewModel.Infrastructure;
using Microsoft.Win32;

namespace FlowReports.ViewModel
{
  /// <summary>
  /// Provides view model functionality for the report editor application.
  /// </summary>
  public class ReportEditorViewModel : ViewModelBase
  {

    #region Fields

    private ActionCommand _newCommand;
    private ActionCommand _loadCommand;
    private ActionCommand _saveCommand;
    private ActionCommand _saveAsCommand;
    private ActionCommand _closeCommand;
    private ActionCommand _showPrintPreviewCommand;
    private ActionCommand _aboutCommand;
    private ActionCommand _openWebsiteCommand;
    private readonly Lazy<OpenFileDialog> _openFileDialog = new(CreateOpenFileDialog);
    private readonly Lazy<SaveFileDialog> _saveFileDialog = new(CreateSaveFileDialog);
    private ReportViewModel _reportVM = ReportViewModel.NewReport();

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="ReportEditorViewModel"/> class with a specific report.
    /// </summary>
    /// <param name="report">The report to edit.</param>
    public ReportEditorViewModel(Report report)
    {
      IsInitializing = true;
      ReportVM = new ReportViewModel(report);
      IsInitializing = false;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReportEditorViewModel"/> class with a new empty report.
    /// </summary>
    public ReportEditorViewModel()
      : this(new Report())
    { }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether the view model is currently initializing.
    /// </summary>
    public static bool IsInitializing { get; private set; }

    /// <summary>
    /// Gets the view model for the report being edited.
    /// </summary>
    public ReportViewModel ReportVM
    {
      get => _reportVM;
      private set
      {
        if (_reportVM != value)
        {
          if (_reportVM != null)
          {
            _reportVM.PropertyChanged -= Report_PropertyChanged;
          }

          _reportVM = value;
          if (_reportVM != null)
          {
            _reportVM.PropertyChanged += Report_PropertyChanged;
          }

          OnPropertyChanged(nameof(ReportVM));
          OnPropertyChanged(nameof(Title));
        }
      }
    }

    /// <summary>
    /// Gets the title to display for the window, including file name and dirty status.
    /// </summary>
    public string Title
    {
      get
      {
        string title = "FlowReports";
        if (ReportVM?.FilePath != null)
        {
          title += " - " + Path.GetFileName(ReportVM.FilePath);
        }
        if (ReportVM?.IsDirty == true)
        {
          title += "*";
        }
        return title;
      }
    }

    #endregion

    #region New

    /// <summary>
    /// Gets the command to create a new empty report.
    /// </summary>
    public ICommand NewCommand => _newCommand ??= new ActionCommand(New, CanNew);

    private void New()
    {
      if (AskToSave() == false)
      {
        return;
      }

      IsInitializing = true;
      ReportVM = ReportViewModel.NewReport();
      IsInitializing = false;
    }

    private bool CanNew()
    {
      return true;
    }

    #endregion

    #region Load

    /// <summary>
    /// Gets the command to load a report from a file.
    /// </summary>
    public ICommand LoadCommand => _loadCommand ??= new ActionCommand(Load, CanLoad);

    private void Load()
    {
      if (AskToSave() == false)
      {
        return;
      }

      var dialog = _openFileDialog.Value;
      if (dialog.ShowDialog() == true)
      {
        IsInitializing = true;
        ReportVM = ReportViewModel.LoadReport(dialog.FileName);
        IsInitializing = false;
      }
    }

    private bool CanLoad()
    {
      return true;
    }

    #endregion

    #region Save

    /// <summary>
    /// Gets the command to save the current report.
    /// </summary>
    public ICommand SaveCommand => _saveCommand ??= new ActionCommand(Save, CanSave);

    private void Save()
    {
      if (ReportVM.FilePath == null && CanSaveAs())
      {
        SaveAs();
        return;
      }

      ReportVM.SaveReport();
    }

    private bool CanSave()
    {
      return ReportVM.IsDirty;
    }

    #endregion

    #region Save As

    /// <summary>
    /// Gets the command to save the current report with a new file path.
    /// </summary>
    public ICommand SaveAsCommand => _saveAsCommand ??= new ActionCommand(SaveAs, CanSaveAs);

    private void SaveAs()
    {
      var dialog = _saveFileDialog.Value;
      if (dialog.ShowDialog() == true)
      {
        ReportVM.SaveReport(dialog.FileName);
      }
    }

    private bool CanSaveAs()
    {
      return ReportVM.IsDirty;
    }

    #endregion

    #region Close

    /// <summary>
    /// Gets the command to close the editor window.
    /// </summary>
    public ICommand CloseCommand => _closeCommand ??= new ActionCommand(Close, CanClose);

    private void Close()
    {
      EventService.Instance.Publish("CloseEditor", true);
    }

    private bool CanClose()
    {
      return true;
    }

    #endregion

    #region Show Print Preview

    /// <summary>
    /// Gets the command to show the print preview window.
    /// </summary>
    public ICommand ShowPrintPreviewCommand => _showPrintPreviewCommand ??= new ActionCommand(ShowPrintPreview, CanShowPrintPreview);

    private void ShowPrintPreview()
    {
      using var vm = new PrintPreviewViewModel(ReportVM.Report);
      var view = ViewFactory.Instance.CreateView(vm);
      view.ShowDialog();
    }

    private bool CanShowPrintPreview()
    {
      return true;
    }

    #endregion

    #region About

    /// <summary>
    /// Gets the command to show the about dialog.
    /// </summary>
    public ICommand AboutCommand => _aboutCommand ??= new ActionCommand(About);

    private void About(object commandParameter)
    {
      var view = ViewFactory.Instance.CreateView<AboutViewModel>();
      view.ShowDialog();
    }

    #endregion

    #region Open Website

    /// <summary>
    /// Gets the command to open the FlowReports website in the default browser.
    /// </summary>
    public ICommand OpenWebsiteCommand => _openWebsiteCommand ??= new ActionCommand(OpenWebsite);

    private void OpenWebsite(object commandParameter)
    {
      Process.Start(Globals.Website);
    }

    #endregion

    #region Event Handlers

    private void Report_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(ReportVM.IsDirty))
      {
        OnPropertyChanged(nameof(Title));
        _saveCommand?.RaiseCanExecuteChanged();
        _saveAsCommand?.RaiseCanExecuteChanged();
      }
    }

    #endregion

    #region Private Members

    private static OpenFileDialog CreateOpenFileDialog()
    {
      return new OpenFileDialog()
      {
        CheckPathExists = true,
        CheckFileExists = true,
        Filter = Properties.Resources.FlowReportFiles + "|*.flow"
      };
    }

    private static SaveFileDialog CreateSaveFileDialog()
    {
      return new SaveFileDialog()
      {
        CheckPathExists = true,
        CheckFileExists = true,
        Filter = Properties.Resources.FlowReportFiles + "|*.flow"
      };
    }

    public bool AskToSave()
    {
      if (CanSave())
      {
        var message = MessageInfo.Question(Properties.Resources.SaveFileQuestion, canCancel: true);
        EventService.Instance.Publish("Message", message);

        if (message.DialogResult == MessageBoxResult.Yes)
        {
          Save();
        }
        else if (message.DialogResult == MessageBoxResult.Cancel)
        {
          return false;
        }
      }

      return true;
    }

    #endregion

    #region IDisposable

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);

      if (disposing)
      {
        ReportVM.Dispose();
      }
    }

    #endregion

  }
}
