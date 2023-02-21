using System.Windows;
using ES.Tools.Core.MVVM;

namespace FlowReports.UI.View
{
  /// <summary>
  /// Interaction logic for ReportBandDetails.xaml
  /// </summary>
  public partial class ReportBandDetails : Window, IView
  {
    public ReportBandDetails()
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
