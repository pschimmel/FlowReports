using System.Collections.ObjectModel;

namespace FlowReports.TestApplication.Model
{
  public class Company
  {
    public string Name { get; set; }
    public ObservableCollection<Employee> Employees { get; } = new ObservableCollection<Employee>();
  }
}
