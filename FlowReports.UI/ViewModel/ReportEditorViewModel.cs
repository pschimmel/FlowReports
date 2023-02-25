using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using ES.Tools.Core.Infrastructure;
using ES.Tools.Core.MVVM;
using FlowReports.Model;
using FlowReports.UI.Infrastructure;
using Microsoft.Win32;

namespace FlowReports.UI.ViewModel
{
  public class ReportEditorViewModel : ViewModelBase
  {
    #region Fields

    private readonly ActionCommand _newCommand;
    private readonly ActionCommand _loadCommand;
    private readonly ActionCommand _saveCommand;
    private readonly ActionCommand _saveAsCommand;
    private readonly ActionCommand _closeCommand;
    private readonly ActionCommand _printPreviewCommand;
    private readonly Lazy<OpenFileDialog> _openFileDialog = new(() => CreateOpenFileDialog());
    private readonly Lazy<SaveFileDialog> _saveFileDialog = new(() => CreateSaveFileDialog());
    private ReportViewModel _reportVM = ReportViewModel.NewReport();

    #endregion

    #region Constructor

    public ReportEditorViewModel(Report report, string filePath)
    {
      IsInitializing = true;
      ReportVM = new ReportViewModel(report) { FilePath = filePath };
      _newCommand = new ActionCommand(New, CanNew);
      _closeCommand = new ActionCommand(Close, CanClose);
      _loadCommand = new ActionCommand(Load, CanLoad);
      _saveCommand = new ActionCommand(Save, CanSave);
      _saveAsCommand = new ActionCommand(SaveAs, CanSaveAs);
      _printPreviewCommand = new ActionCommand(PrintPreview, CanPrintPreview);
      IsInitializing = false;
    }

    public ReportEditorViewModel()
      : this(new Report(), null)
    { }

    #endregion

    #region Properties

    public static bool IsInitializing { get; private set; }

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

    public string Title
    {
      get
      {
        string title = "FlowReports";
        if (ReportVM?.FilePath != null)
        {
          title += " - " + Path.GetFileName(ReportVM.FilePath);
          if (ReportVM.IsDirty)
          {
            title += "*";
          }
        }
        return title;
      }
    }

    #endregion

    #region New

    public ICommand NewCommand => _newCommand;

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

    public ICommand LoadCommand => _loadCommand;

    private void Load()
    {
      if (AskToSave() == false)
      {
        return;
      }

      OpenFileDialog dialog = _openFileDialog.Value;
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

    public ICommand SaveCommand => _saveCommand;

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

    public ICommand SaveAsCommand => _saveAsCommand;

    private void SaveAs()
    {
      SaveFileDialog dialog = _saveFileDialog.Value;
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

    public ICommand CloseCommand => _closeCommand;

    private void Close()
    {
      EventService.Instance.Publish("CloseEditor", true);
    }

    private bool CanClose()
    {
      return true;
    }

    #endregion

    #region Print Preview

    public ICommand PrintPreviewCommand => _printPreviewCommand;

    private void PrintPreview()
    {
      using var vm = new PrintPreviewViewModel(ReportVM.Report);
      var view = ViewFactory.Instance.CreateView(vm);
      view.ShowDialog();
    }

    private bool CanPrintPreview()
    {
      return true;
    }

    #endregion

    #region Event Handlers

    private void Report_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(ReportVM.IsDirty))
      {
        OnPropertyChanged(nameof(Title));
        _saveCommand.RaiseCanExecuteChanged();
        _saveAsCommand.RaiseCanExecuteChanged();
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
        Filter = Properties.Resources.NASFiles + "|*.nas"
      };
    }

    private static SaveFileDialog CreateSaveFileDialog()
    {
      return new SaveFileDialog()
      {
        CheckPathExists = true,
        CheckFileExists = true,
        Filter = Properties.Resources.NASFiles + "|*.nas"
      };
    }

    public bool AskToSave()
    {
      if (CanSave())
      {
        var message = MessageInfo.Question(Properties.Resources.SaveFileQuestion);
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
