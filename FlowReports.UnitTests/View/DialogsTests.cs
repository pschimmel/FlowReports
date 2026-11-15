using System.Threading;
using System.Printing;
using FlowReports.View;
using FlowReports.ViewModel;
using FlowReports.ViewModel.Printing;

namespace FlowReports.UnitTests.View
{
  [Apartment(ApartmentState.STA)]
  public class DialogsTests
  {
    [Test]
    public void AboutWindow_ViewModel_GetSet_Works()
    {
      var window = new AboutWindow();
      var vm = new AboutViewModel();
      window.ViewModel = vm;
      Assert.That(window.ViewModel, Is.SameAs(vm));
    }

    [Test]
    public void PageSettingsWindow_ViewModel_GetSet_Works()
    {
      var window = new PageSettingsWindow();
      var vm = new PageSettingsViewModel(new PageInformation(null, null, null, PageOrientation.Portrait));
      window.ViewModel = vm;
      Assert.That(window.ViewModel, Is.SameAs(vm));
    }

    [Test]
    public void PrintPreviewWindow_ViewModel_GetSet_Works()
    {
      var window = new PrintPreviewWindow();
      var vm = new ReportEditorViewModel();
      window.ViewModel = vm;
      Assert.That(window.ViewModel, Is.SameAs(vm));
    }

    [Test]
    public void ReportEditorWindow_ViewModel_GetSet_Works()
    {
      var window = new ReportEditorWindow();
      var vm = new ReportEditorViewModel();
      window.ViewModel = vm;
      Assert.That(window.ViewModel, Is.SameAs(vm));
    }
  }
}
