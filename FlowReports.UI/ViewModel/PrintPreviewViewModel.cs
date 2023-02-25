using System.IO;
using System.Windows.Documents;
using System.Windows.Xps.Packaging;
using ES.Tools.Core.Infrastructure;
using FlowReports.Model;

namespace FlowReports.UI.ViewModel
{
  public class PrintPreviewViewModel : ViewModelBase
  {
    private readonly List<string> tempFileNames = new List<string>();

    public PrintPreviewViewModel(Report report)
    {
      //using var stream = new MemoryStream();
      //using var package = Package.Open(stream, FileMode.Create, FileAccess.ReadWrite);
      //var uri = new Uri(@"memorystream://myXps.xps");
      //PackageStore.AddPackage(uri, package);
      //using var xpsDoc = new XpsDocument(package, CompressionOption.SuperFast, uri.AbsoluteUri);

      string tempFileName = Path.ChangeExtension(Path.GetTempFileName(), "xps");
      tempFileNames.Add(tempFileName);

      using var xpsDocument = new XpsDocument(tempFileName, FileAccess.ReadWrite);

      var writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);
      writer.Write(new ReportPaginator(report));
      Document = xpsDocument.GetFixedDocumentSequence();
      // PackageStore.RemovePackage(uri);
    }

    public IDocumentPaginatorSource Document { get; }

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);

      foreach (var path in tempFileNames)
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
  }
}
