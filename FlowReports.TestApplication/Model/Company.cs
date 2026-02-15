using System.Collections.ObjectModel;

namespace FlowReports.TestApplication.Model
{
  /// <summary>
  /// Represents a company with a collection of employees.
  /// </summary>
  public class Company
  {
    public string Name { get; set; }
    public ObservableCollection<Employee> Employees { get; } = new ObservableCollection<Employee>();
  }
}
