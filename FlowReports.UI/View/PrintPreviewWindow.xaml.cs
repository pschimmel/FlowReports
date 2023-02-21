using System.Windows;
using ES.Tools.Core.MVVM;
using FlowReports.UI.ViewModel;

namespace FlowReports.UI.View
{
  /// <summary>
  /// Interaction logic for PrintPreviewWindow.xaml
  /// </summary>
  public partial class PrintPreviewWindow : Window, IView
  {
    public PrintPreviewWindow()
    {
      InitializeComponent();
    }

    public IViewModel ViewModel
    {
      get => (ReportEditorViewModel)DataContext;
      set => DataContext = value;
    }
  }
}
