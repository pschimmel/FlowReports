using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using ES.Tools.Core.MVVM;
using FlowReports.View;
using FlowReports.ViewModel;
using FlowReports.ViewModel.EditorItems;
using NAS.ViewModel.Printing;

namespace FlowReports.UI
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
      ViewFactory.Instance.Register<PageSettingsViewModel, PageSettingsWindow>();

      var args = e.Args;
      if (args.Length > 0 &&
          string.Equals(Path.GetExtension(args[0]), ".nas", StringComparison.InvariantCultureIgnoreCase) &&
          File.Exists(Path.GetFullPath(args[0])))
      {
        // Load from file
        try
        {
#if DEBUG
          var hamster = /*new BitmapImage(*/new Uri("pack://application:,,,/FlowReports.UI;component/Images/Hamster.png", UriKind.Absolute/*)*/);
          //var employee1 = new Employee { FirstName = "Michael", LastName = "Meyers", DOB = new DateTime(1975, 11, 1), Image = LoadImage("Fox.jpg") };
          var employee2 = new Employee { FirstName = "Jason", LastName = "Vorhees", DOB = new DateTime(1965, 12, 11), IsExternal = true };
          //var employee3 = new Employee { FirstName = "Freddy", LastName = "Krueger", IsExternal = true, Image = LoadImage("Bunny.png") };
          var employee4 = new Employee { FirstName = "Wayne", LastName = "Bruce", Image = hamster } ;
          var company1 = new Company { Name = "Nightmare Inc." };
          var company2 = new Company { Name = "DC" };
          //company1.Employees.Add(employee1);
          company1.Employees.Add(employee2);
          //company1.Employees.Add(employee3);
          company2.Employees.Add(employee4);
          var companies = new List<Company> { company1, company2 };

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

    private static byte[] LoadImage(string fileName)
    {
      string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Path.Combine("Images", fileName));
      return File.ReadAllBytes(path);
    }

    internal class Company
    {
      public string Name { get; set; }
      public List<Employee> Employees { get; } = new List<Employee>();
      // public ImageSource Image { get; set; }
    }

    internal class Employee
    {
      public string FirstName { get; set; }
      public string LastName { get; set; }
      public DateTime? DOB { get; set; }
      public string Email { get; set; }
      public bool IsExternal { get; set; }
      public object Image { get; set; }
    }
  }
}
