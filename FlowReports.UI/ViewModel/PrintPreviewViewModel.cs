using System.Windows.Documents;

namespace FlowReports.UI.ViewModel
{
  public class PrintPreviewViewModel : ViewModelBase
  {
    public PrintPreviewViewModel()
    {
      Document = new FixedDocument();
    }

    public IDocumentPaginatorSource Document { get; }
  }
}
