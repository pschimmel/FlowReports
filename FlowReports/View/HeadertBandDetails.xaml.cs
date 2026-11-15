using System.Windows;
using ES.Tools.Core.MVVM;

namespace FlowReports.View
{
  /// <summary>
  /// Interaction logic for HeaderBandDetails.xaml
  /// </summary>
  public partial class HeaderBandDetails : Window, IView
  {
    public HeaderBandDetails()
    {
      InitializeComponent();
    }

    public IViewModel ViewModel
    {
      get => (IViewModel)DataContext;
      set => DataContext = value;
    }

    private void OKButton_Click(object sender, RoutedEventArgs e)
    {
      DialogResult = true;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
      DialogResult = false;
    }
  }
}
