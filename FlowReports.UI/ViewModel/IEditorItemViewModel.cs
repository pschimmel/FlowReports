using System.ComponentModel;
using System.Windows;
using FlowReports.Model.ReportItems;

namespace FlowReports.UI.ViewModel
{
  /// <summary>
  /// Interface for ViewModels representing the items in the report.
  /// </summary>
  public interface IEditorItemViewModel
    : INotifyPropertyChanged, IDisposable
  {
    ReportItem Item { get; }
    double Left { get; set; }
    double Top { get; set; }
    double Width { get; set; }
    double Height { get; set; }
    bool IsSelected { get; set; }
    Point Location { get; }
    Size Size { get; }
  }
}