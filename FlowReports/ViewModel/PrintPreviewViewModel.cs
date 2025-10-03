using System.IO;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Xps.Packaging;
using ES.Tools.Core.Infrastructure;
using ES.Tools.Core.MVVM;
using FlowReports.Model;
using FlowReports.ViewModel.Infrastructure;
using FlowReports.ViewModel.Printing;

namespace FlowReports.ViewModel
{
  public class PrintPreviewViewModel : ViewModelBase
  {
    #region Fields

    private readonly List<string> _tempFileNames = new();
    private static PageInformation _pageInformation;
    private readonly ActionCommand _printSetupCommand;
    private readonly ActionCommand _closeCommand;

    #endregion

    #region Constructor

    public PrintPreviewViewModel(Report report)
    {
      if (_pageInformation == null)
      {
        _pageInformation = PageInformation.Default;

        if (_pageInformation == null)
        {
          return;
        }
      }

      string tempFileName = Path.ChangeExtension(Path.GetTempFileName(), "xps");
      _tempFileNames.Add(tempFileName);

      using var xpsDocument = new XpsDocument(tempFileName, FileAccess.ReadWrite);
      var writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);
      writer.Write(new ReportPaginator(report));
      Document = xpsDocument.GetFixedDocumentSequence();
      _closeCommand = new ActionCommand(Close, CanClose);
      _printSetupCommand = new ActionCommand(PrintSetup, CanPrintSetup);
    }

    #endregion

    #region Properties

    public IDocumentPaginatorSource Document { get; }

    #endregion

    #region Print setup

    public ICommand PrintSetupCommand => _printSetupCommand;

    private void PrintSetup()
    {
      var vm = new PageSettingsViewModel(_pageInformation);
      var view = ViewFactory.Instance.CreateView(vm);
      view.ShowDialog();
    }

    private bool CanPrintSetup()
    {
      return true;
    }

    #endregion

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

    #region IDisposable

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

    #endregion
  }
}
