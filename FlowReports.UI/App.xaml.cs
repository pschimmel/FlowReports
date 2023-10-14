using System.Windows;
using FlowReports.TestApplication.View;
using FlowReports.TestApplication.ViewModel;

namespace FlowReports.UI
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    private MainWindow mainWindow;

    protected override void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);
      mainWindow = new MainWindow();
      mainWindow.ViewModel = new MainViewModel();
      mainWindow.ShowDialog();
    }

    protected override void OnExit(ExitEventArgs e)
    {
      base.OnExit(e);
      mainWindow.ViewModel.Dispose();
    }
  }
}