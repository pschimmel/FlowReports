using System.ComponentModel;
using System.Windows;
using FlowReports.Model.ReportItems;

namespace FlowReports.UI.ViewModel
{
  public interface IItemViewModel : INotifyPropertyChanged, IDisposable
  {
    ReportItem Item { get; }
    double Left { get; set; }
    double Top { get; set; }
    double Width { get; set; }
    double Height { get; set; }
    Point Location { get; }
    Size Size { get; }
    bool IsSelected { get; set; }
  }
}