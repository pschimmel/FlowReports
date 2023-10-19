using System.Windows;
using ES.Tools.Core.MVVM;

namespace FlowReports.View
{
  /// <summary>
  /// Interaction logic for PageSettingsWindow.xaml
  /// </summary>
  public partial class PageSettingsWindow : Window, IView
  {
    public PageSettingsWindow()
    {
      InitializeComponent();
    }

    public IViewModel ViewModel
    {
      get => DataContext as IViewModel;
      set => DataContext = value;
    }

    private void buttonCancel_Click(object sender, RoutedEventArgs e)
    {
      DialogResult = false;
    }

    private void buttonOK_Click(object sender, RoutedEventArgs e)
    {
      DialogResult = true;
    }
  }
}
