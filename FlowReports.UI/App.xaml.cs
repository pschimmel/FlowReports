using System.Diagnostics;
using System.IO;
using System.Windows;
using ES.Tools.Core.MVVM;
using FlowReports.UI;
using FlowReports.UI.View;
using FlowReports.UI.ViewModel;

namespace Reports.NET
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    protected override void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);

      ViewFactory.Instance.Register<ReportEditorViewModel, ReportEditorWindow>();
      ViewFactory.Instance.Register<ReportBandViewModel, ReportBandDetails>();
      ViewFactory.Instance.Register<PrintPreviewViewModel, PrintPreviewWindow>();

      var args = e.Args;
      if (args.Length > 0 &&
          string.Equals(Path.GetExtension(args[0]), ".nas", StringComparison.InvariantCultureIgnoreCase) &&
          File.Exists(Path.GetFullPath(args[0])))
      {
        // Load from file
        try
        {
#if DEBUG
          var employee1 = new Employee { FirstName = "Michael", LastName = "Meyers", DOB = new DateTime(75, 11, 1) };
          var employee2 = new Employee { FirstName = "Jason", LastName = "Vorhees", DOB = new DateTime(65, 12, 11) };
          var employee3 = new Employee { FirstName = "Freddy", LastName = "Krueger" };
          var employee4 = new Employee { FirstName = "Wayne", LastName = "Bruce" };
          var company1 = new Company { Name = "Nightmare Inc." };
          var company2 = new Company { Name = "DC" };
          company1.Employees.Add(employee1);
          company1.Employees.Add(employee2);
          company1.Employees.Add(employee3);
          company2.Employees.Add(employee4);
          var companies = new List<Company> { company1 };

          FlowReport.Edit(args[0], companies);
#else
          var report = FlowReport.Load(args[0]);
          FlowReport.Edit(report);
#endif
        }
        catch (Exception ex)
        {
          Debug.Fail("Unable to load report file. " + ex.Message);
        }
      }
      else
      {
        var viewModel = new ReportEditorViewModel();
        var view = ViewFactory.Instance.CreateView(viewModel);
        view.ShowDialog();
      }
    }

    internal class Company
    {
      public string Name { get; set; }
      public List<Employee> Employees { get; } = new List<Employee>();
    }

    internal class Employee
    {
      public string FirstName { get; set; }
      public string LastName { get; set; }
      public DateTime? DOB { get; set; }
    }
  }
}
