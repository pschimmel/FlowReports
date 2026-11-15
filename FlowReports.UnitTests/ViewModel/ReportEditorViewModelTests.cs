using FlowReports.Model;
using FlowReports.ViewModel;

namespace FlowReports.UnitTests.ViewModel
{
  public class ReportEditorViewModelTests
  {
    private ReportEditorViewModel _viewModel;

    [SetUp]
    public void Setup()
    {
      _viewModel = new ReportEditorViewModel();
    }

    [Test]
    public void Constructor_CreatesViewModelWithNewReport()
    {
      Assert.That(_viewModel, Is.Not.Null);
      Assert.That(_viewModel.ReportVM, Is.Not.Null);
    }

    [Test]
    public void Constructor_WithReport_CreatesViewModelWithGivenReport()
    {
      var report = new Report();
      var vm = new ReportEditorViewModel(report);
      Assert.That(vm, Is.Not.Null);
      Assert.That(vm.ReportVM, Is.Not.Null);
      vm.Dispose();
    }

    [Test]
    public void ReportVM_IsNotNull()
    {
      Assert.That(_viewModel.ReportVM, Is.Not.Null);
    }

    [Test]
    public void Title_ContainsFlowReports()
    {
      Assert.That(_viewModel.Title, Contains.Substring("FlowReports"));
    }

    [Test]
    public void Title_ContainsAsteriskWhenReportIsDirty()
    {
      _viewModel.ReportVM.IsDirty = true;
      Assert.That(_viewModel.Title, Contains.Substring("*"));
    }

    [Test]
    public void Title_DoesNotContainAsteriskWhenReportIsNotDirty()
    {
      _viewModel.ReportVM.IsDirty = false;
      Assert.That(_viewModel.Title, Does.Not.Contains("*"));
    }

    [Test]
    public void NewCommand_IsNotNull()
    {
      Assert.That(_viewModel.NewCommand, Is.Not.Null);
    }

    [Test]
    public void NewCommand_CanExecute()
    {
      Assert.That(_viewModel.NewCommand.CanExecute(null), Is.True);
    }

    [Test]
    public void LoadCommand_IsNotNull()
    {
      Assert.That(_viewModel.LoadCommand, Is.Not.Null);
    }

    [Test]
    public void LoadCommand_CanExecute()
    {
      Assert.That(_viewModel.LoadCommand.CanExecute(null), Is.True);
    }

    [Test]
    public void SaveCommand_IsNotNull()
    {
      Assert.That(_viewModel.SaveCommand, Is.Not.Null);
    }

    [Test]
    public void SaveCommand_CannotExecuteWhenReportIsNotDirty()
    {
      _viewModel.ReportVM.IsDirty = false;
      Assert.That(_viewModel.SaveCommand.CanExecute(null), Is.False);
    }

    [Test]
    public void SaveAsCommand_IsNotNull()
    {
      Assert.That(_viewModel.SaveAsCommand, Is.Not.Null);
    }

    [Test]
    public void SaveAsCommand_CannotExecuteWhenReportIsNotDirty()
    {
      _viewModel.ReportVM.IsDirty = false;
      Assert.That(_viewModel.SaveAsCommand.CanExecute(null), Is.False);
    }

    [Test]
    public void CloseCommand_IsNotNull()
    {
      Assert.That(_viewModel.CloseCommand, Is.Not.Null);
    }

    [Test]
    public void CloseCommand_CanExecute()
    {
      Assert.That(_viewModel.CloseCommand.CanExecute(null), Is.True);
    }

    [Test]
    public void ShowPrintPreviewCommand_IsNotNull()
    {
      Assert.That(_viewModel.ShowPrintPreviewCommand, Is.Not.Null);
    }

    [Test]
    public void ShowPrintPreviewCommand_CanExecute()
    {
      Assert.That(_viewModel.ShowPrintPreviewCommand.CanExecute(null), Is.True);
    }

    [Test]
    public void AboutCommand_IsNotNull()
    {
      Assert.That(_viewModel.AboutCommand, Is.Not.Null);
    }

    [Test]
    public void OpenWebsiteCommand_IsNotNull()
    {
      Assert.That(_viewModel.OpenWebsiteCommand, Is.Not.Null);
    }

    [TearDown]
    public void TearDown()
    {
      _viewModel?.Dispose();
    }
  }
}
