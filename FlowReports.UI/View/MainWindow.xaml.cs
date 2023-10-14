using System.Windows;
using FlowReports.TestApplication.ViewModel;

namespace FlowReports.TestApplication.View
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
    }

    public MainViewModel ViewModel
    {
      get => DataContext as MainViewModel;
      internal set => DataContext = value;
    }
  }
}
