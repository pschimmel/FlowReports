using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ES.Tools.Core.MVVM;
using FlowReports.App;
using FlowReports.TestApplication.Model;
using Microsoft.Win32;

namespace FlowReports.TestApplication.ViewModel
{
  public class MainViewModel : ES.Tools.Core.MVVM.ViewModel
  {
    #region Fields

    private string _reportFilePath;
    private Company _selectedCompany;
    private Employee _selectedEmployee;
    private ActionCommand _selectReportFileCommand;
    private ActionCommand _showReportCommand;
    private ActionCommand _editReportCommand;
    private ActionCommand _addCompanyCommand;
    private ActionCommand _removeCompanyCommand;
    private ActionCommand _addEmployeeCommand;
    private ActionCommand _removeEmployeeCommand;
    private ActionCommand _setImageCommand;
    private ActionCommand _importImageCommand;

    #endregion

    #region Constructor

    public MainViewModel()
    {
      ReadSettings();
      ReadExampleData();
    }

    #endregion

    #region Public Properties

    public string ReportFilePath
    {
      get => _reportFilePath;
      set
      {
        if (_reportFilePath != value)
        {
          _reportFilePath = value;
          _showReportCommand?.RaiseCanExecuteChanged();
          _editReportCommand?.RaiseCanExecuteChanged();
          OnPropertyChanged();
        }
      }
    }

    public ObservableCollection<Company> Companies { get; private set; }

    public Company SelectedCompany
    {
      get => _selectedCompany;
      set
      {
        if (_selectedCompany != value)
        {
          _selectedCompany = value;
          OnPropertyChanged();
          _removeCompanyCommand?.RaiseCanExecuteChanged();
          _addEmployeeCommand?.RaiseCanExecuteChanged();
          OnPropertyChanged(nameof(CompanySelected));
        }
      }
    }

    public bool CompanySelected => _selectedCompany != null;

    public Employee SelectedEmployee
    {
      get => _selectedEmployee;
      set
      {
        if (_selectedEmployee != value)
        {
          _selectedEmployee = value;
          OnPropertyChanged();
          _removeEmployeeCommand?.RaiseCanExecuteChanged();
          _setImageCommand?.RaiseCanExecuteChanged();
          _importImageCommand?.RaiseCanExecuteChanged();
          OnPropertyChanged(nameof(EmployeeSelected));
        }
      }
    }

    public bool EmployeeSelected => _selectedEmployee != null;

    #endregion

    #region Commands

    #region Select Report File

    public ICommand SelectReportFileCommand => _selectReportFileCommand ??= new ActionCommand(SelectReportFile);

    private void SelectReportFile()
    {
      var dialog = new OpenFileDialog();
      dialog.CheckFileExists = true;
      dialog.Filter = "FlowReport Files (*.flow)|*.flow";
      if (dialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(dialog.FileName) && File.Exists(dialog.FileName))
      {
        ReportFilePath = dialog.FileName;
      }
    }

    #endregion

    #region Show Report

    public ICommand ShowReportCommand => _showReportCommand ??= new ActionCommand(ShowReport, CanShowReport);

    public void ShowReport()
    {
      if (File.Exists(ReportFilePath))
      {
        var report =  FlowReport.Load(ReportFilePath);
        FlowReport.Show(report, Companies);
      }
      else
      {
        MessageBox.Show($"Cannot open file '{ReportFilePath}'.");
      }
    }

    private bool CanShowReport()
    {
      return !string.IsNullOrWhiteSpace(ReportFilePath) && Companies.Any();
    }

    #endregion

    #region Edit Report

    public ICommand EditReportCommand => _editReportCommand ??= new ActionCommand(EditReport, CanEditReport);

    public void EditReport()
    {
      if (File.Exists(ReportFilePath))
      {
        var report =  FlowReport.Load(ReportFilePath);
        FlowReport.Edit(report);
      }
      else
      {
        MessageBox.Show($"Cannot open file '{ReportFilePath}'.");
      }
    }

    private bool CanEditReport()
    {
      return !string.IsNullOrWhiteSpace(ReportFilePath) && Companies.Any();
    }

    #endregion

    #region Add Company

    public ICommand AddCompanyCommand => _addCompanyCommand ??= new ActionCommand(AddCompany);

    private void AddCompany()
    {
      Companies.Add(new Company());
      _showReportCommand.RaiseCanExecuteChanged();
      _editReportCommand.RaiseCanExecuteChanged();
    }

    #endregion

    #region Remove Company

    public ICommand RemoveCompanyCommand => _removeCompanyCommand ??= new ActionCommand(RemoveCompany, CanRemoveCompany);

    private void RemoveCompany()
    {
      Companies.Remove(SelectedCompany);
      SelectedCompany = null;
      _showReportCommand.RaiseCanExecuteChanged();
      _editReportCommand.RaiseCanExecuteChanged();
    }

    private bool CanRemoveCompany()
    {
      return SelectedCompany != null;
    }

    #endregion

    #region Add Employee

    public ICommand AddEmployeeCommand => _addEmployeeCommand ??= new ActionCommand(AddEmployee, CanAddEmployee);

    private void AddEmployee()
    {
      var newEmployee = new Employee();
      SelectedCompany.Employees.Add(newEmployee);
      SelectedEmployee = newEmployee;
    }

    private bool CanAddEmployee()
    {
      return SelectedCompany != null;
    }

    #endregion

    #region Remove Employee

    public ICommand RemoveEmployeeCommand => _removeEmployeeCommand ??= new ActionCommand(RemoveEmployee, CanRemoveEmployee);

    private void RemoveEmployee()
    {
      SelectedCompany.Employees.Remove(SelectedEmployee);
      SelectedEmployee = null;
    }

    private bool CanRemoveEmployee()
    {
      return SelectedCompany != null && SelectedEmployee != null;
    }

    #endregion

    #region Set Image

    public ICommand SetImageCommand => _setImageCommand ??= new ActionCommand(SetImage, CanSetImage);

    private void SetImage(object commandParameter)
    {
      SelectedEmployee.Image = commandParameter == null ? null : new Uri(commandParameter.ToString(), UriKind.RelativeOrAbsolute);
    }

    private bool CanSetImage(object commandParameter)
    {
      return SelectedEmployee != null;
    }

    #endregion

    #region Import Image

    public ICommand ImportImageCommand => _importImageCommand ??= new ActionCommand(ImportImage, () => CanSetImage(null));

    private void ImportImage()
    {
      var openFileDialog = new OpenFileDialog();
      openFileDialog.Title = "Select a picture";
      openFileDialog.Filter = "Images|*.jpg;*.jpeg;*.png|"
        + "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|"
        + "Portable Network Graphic (*.png)|*.png";
      openFileDialog.CheckFileExists = true;
      if (openFileDialog.ShowDialog() == true)
      {
        SelectedEmployee.Image = new BitmapImage(new Uri(openFileDialog.FileName));
      }
    }

    #endregion

    #endregion

    #region Private Methods

    private void ReadSettings()
    {
      var settings = DataHelper.ReadSettings();
      ReportFilePath = settings.ReportFilePath;
    }

    private void WriteSettings()
    {
      var settings = new TestAppSettings
      {
        ReportFilePath = ReportFilePath
      };

      DataHelper.WriteSettings(settings);
    }

    private void ReadExampleData()
    {
      IEnumerable<Company> data = DataHelper.ReadExampleData();
      Companies = new ObservableCollection<Company>(data);
    }

    private void WriteExampleData()
    {
      DataHelper.WriteExampleData(Companies);
    }

    #endregion

    #region IDisposable Implementation

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        WriteSettings();
        WriteExampleData();
      }

      base.Dispose(disposing);
    }

    #endregion
  }
}
