using System.IO;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Xps.Packaging;
using ES.Tools.Core.Infrastructure;
using ES.Tools.Core.MVVM;
using FlowReports.Model;
using FlowReports.UI.Printing;

namespace FlowReports.UI.ViewModel
{
  public class PrintPreviewViewModel : ViewModelBase
  {
    private readonly List<string> _tempFileNames = new();
    private readonly ActionCommand _closeCommand;

    public PrintPreviewViewModel(Report report)
    {
      string tempFileName = Path.ChangeExtension(Path.GetTempFileName(), "xps");
      _tempFileNames.Add(tempFileName);

      using var xpsDocument = new XpsDocument(tempFileName, FileAccess.ReadWrite);

      var writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);
      writer.Write(new ReportPaginator(report));
      Document = xpsDocument.GetFixedDocumentSequence();
      _closeCommand = new ActionCommand(Close, CanClose);
    }

    public IDocumentPaginatorSource Document { get; }

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);

      foreach (var path in _tempFileNames)
      {
        try
        {
          File.Delete(path);
        }
        catch
        {
          var service = Services.Instance.GetService<ExecuteOnApplicationClosing>();
          service.Add(() => { try { File.Delete(path); } catch { } });
        }
      }
    }


    #region Close

    public ICommand CloseCommand => _closeCommand;

    private void Close()
    {
      EventService.Instance.Publish("ClosePrintPreview", true);
    }

    private bool CanClose()
    {
      return true;
    }

    #endregion
  }
}
