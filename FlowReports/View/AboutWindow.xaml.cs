using System.Windows;
using ES.Tools.Core.MVVM;

namespace FlowReports.View
{
  /// <summary>
  /// Interaction logic for AboutWindow.xaml
  /// </summary>
  public partial class AboutWindow : Window, IView
  {
    public AboutWindow()
    {
      InitializeComponent();
    }

    public IViewModel ViewModel
    {
      get => ContentControl.Content as IViewModel;
      set => ContentControl.Content = value;
    }

    private void buttonOK_Click(object sender, RoutedEventArgs e)
    {
      DialogResult = true;
    }
  }
}
