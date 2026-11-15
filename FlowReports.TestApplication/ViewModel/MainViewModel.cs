using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ES.Tools.Core.MVVM;
using FlowReports.Model;
using FlowReports.TestApplication.Model;
using Microsoft.Win32;

namespace FlowReports.TestApplication.ViewModel
{
  /// <summary>
  /// Provides view model functionality for the test application main window.
  /// </summary>
  public class MainViewModel : ES.Tools.Core.MVVM.ViewModel
  {

    #region Fields

    private string _reportFilePath;
    private Company _selectedCompany;
    private Employee _selectedEmployee;
    private ActionCommand _selectReportFileCommand;
    private ActionCommand _showReportCommand;
    private ActionCommand _editReportCommand;
    private ActionCommand _newReportCommand;
    private ActionCommand _addCompanyCommand;
    private ActionCommand _removeCompanyCommand;
    private ActionCommand _addEmployeeCommand;
    private ActionCommand _removeEmployeeCommand;
    private ActionCommand _setImageCommand;
    private ActionCommand _importImageCommand;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="MainViewModel"/> class.
    /// </summary>
    public MainViewModel()
    {
      ReadSettings();
      ReadExampleData();
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets or sets the file path of the selected report.
    /// </summary>
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

    /// <summary>
    /// Gets the collection of companies in the test application.
    /// </summary>
    public ObservableCollection<Company> Companies { get; private set; }

    /// <summary>
    /// Gets or sets the currently selected company.
    /// </summary>
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

    /// <summary>
    /// Gets a value indicating whether a company is currently selected.
    /// </summary>
    public bool CompanySelected => _selectedCompany != null;

    /// <summary>
    /// Gets or sets the currently selected employee.
    /// </summary>
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

    /// <summary>
    /// Gets a value indicating whether an employee is currently selected.
    /// </summary>
    public bool EmployeeSelected => _selectedEmployee != null;

    #endregion

    #region Commands

    #region Select Report File

    /// <summary>
    /// Gets the command to select a report file.
    /// </summary>
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

    /// <summary>
    /// Gets the command to show the selected report with the current data.
    /// </summary>
    public ICommand ShowReportCommand => _showReportCommand ??= new ActionCommand(ShowReport, CanShowReport);

    /// <summary>
    /// Shows the print preview of the selected report with the current companies.
    /// </summary>
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

    /// <summary>
    /// Gets the command to edit the selected report.
    /// </summary>
    public ICommand EditReportCommand => _editReportCommand ??= new ActionCommand(EditReport, CanEditReport);

    /// <summary>
    /// Opens the report editor for the selected report with the current companies.
    /// </summary>
    public void EditReport()
    {
      if (File.Exists(ReportFilePath))
      {
        var report = FlowReport.Load(ReportFilePath);
        FlowReport.Edit(report, Companies, "Companies");
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

    #region New Report

    /// <summary>
    /// Gets the command to create a new report.
    /// </summary>
    public ICommand NewReportCommand => _newReportCommand ??= new ActionCommand(NewReport);

    private void NewReport()
    {
      FlowReport.Edit(new Report(), Companies, "Companies");
    }

    #endregion

    #region Add Company

    /// <summary>
    /// Gets the command to add a new company.
    /// </summary>
    public ICommand AddCompanyCommand => _addCompanyCommand ??= new ActionCommand(AddCompany);

    private void AddCompany()
    {
      Companies.Add(new Company());
      _showReportCommand.RaiseCanExecuteChanged();
      _editReportCommand.RaiseCanExecuteChanged();
    }

    #endregion

    #region Remove Company

    /// <summary>
    /// Gets the command to remove the selected company.
    /// </summary>
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

    /// <summary>
    /// Gets the command to add a new employee to the selected company.
    /// </summary>
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

    /// <summary>
    /// Gets the command to remove the selected employee.
    /// </summary>
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

    /// <summary>
    /// Gets the command to set the image for the selected employee.
    /// </summary>
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

    /// <summary>
    /// Gets the command to import and set an image for the selected employee.
    /// </summary>
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
