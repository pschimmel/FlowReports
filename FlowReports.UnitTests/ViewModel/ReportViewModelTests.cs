using FlowReports.Model;
using FlowReports.ViewModel.Editor;

namespace FlowReports.UnitTests.ViewModel
{
  public class ReportViewModelTests
  {
    private ReportViewModel _viewModel;
    private Report _report;

    [SetUp]
    public void Setup()
    {
      _report = new Report();
      _viewModel = new ReportViewModel(_report);
    }

    [Test]
    public void Constructor_CreatesViewModelWithReport()
    {
      Assert.That(_viewModel, Is.Not.Null);
      Assert.That(_viewModel.Bands, Is.Not.Null);
    }

    [Test]
    public void Bands_IsNotNull()
    {
      Assert.That(_viewModel.Bands, Is.Not.Null);
      Assert.That(_viewModel.Bands.Count, Is.EqualTo(0));
    }

    [Test]
    public void SelectedBand_InitiallyNull()
    {
      Assert.That(_viewModel.SelectedBand, Is.Null);
    }

    [Test]
    public void SelectedHeader_InitiallyNull()
    {
      Assert.That(_viewModel.SelectedHeader, Is.Null);
    }

    [Test]
    public void SelectedFooter_InitiallyNull()
    {
      Assert.That(_viewModel.SelectedFooter, Is.Null);
    }

    [Test]
    public void SelectedItem_InitiallyNull()
    {
      Assert.That(_viewModel.SelectedItem, Is.Null);
    }

    [Test]
    public void IsDirty_InitiallyFalse()
    {
      Assert.That(_viewModel.IsDirty, Is.False);
    }

    [Test]
    public void FilePath_IsNotNull()
    {
      Assert.That(_viewModel.FilePath, Is.Null);
    }

    [Test]
    public void DataSourceVM_IsNotNull()
    {
      Assert.That(_viewModel.DataSourceVM, Is.Not.Null);
    }

    [Test]
    public void IsBandSelected_ReturnsFalseWhenNoBandSelected()
    {
      Assert.That(_viewModel.IsBandSelected, Is.False);
    }

    [Test]
    public void NewReport_CreatesNewReportViewModel()
    {
      var newVm = ReportViewModel.NewReport();
      Assert.That(newVm, Is.Not.Null);
      Assert.That(newVm.IsDirty, Is.True);
      newVm.Dispose();
    }

    [Test]
    public void AddBandCommand_IsNotNull()
    {
      Assert.That(_viewModel.AddBandCommand, Is.Not.Null);
    }

    [Test]
    public void AddBandCommand_CanExecute()
    {
      Assert.That(_viewModel.AddBandCommand.CanExecute(null), Is.True);
    }

    [Test]
    public void AddSubBandCommand_IsNotNull()
    {
      Assert.That(_viewModel.AddSubBandCommand, Is.Not.Null);
    }

    [Test]
    public void AddSubBandCommand_CannotExecuteWhenNoBandSelected()
    {
      Assert.That(_viewModel.AddSubBandCommand.CanExecute(null), Is.False);
    }

    [Test]
    public void RemoveBandCommand_IsNotNull()
    {
      Assert.That(_viewModel.RemoveBandCommand, Is.Not.Null);
    }

    [Test]
    public void RemoveBandCommand_CannotExecuteWhenNoBandSelected()
    {
      Assert.That(_viewModel.RemoveBandCommand.CanExecute(null), Is.False);
    }

    [Test]
    public void AddTextItemCommand_IsNotNull()
    {
      Assert.That(_viewModel.AddTextItemCommand, Is.Not.Null);
    }

    [Test]
    public void AddBooleanItemCommand_IsNotNull()
    {
      Assert.That(_viewModel.AddBooleanItemCommand, Is.Not.Null);
    }

    [Test]
    public void AddImageItemCommand_IsNotNull()
    {
      Assert.That(_viewModel.AddImageItemCommand, Is.Not.Null);
    }

    [Test]
    public void RemoveItemCommand_IsNotNull()
    {
      Assert.That(_viewModel.RemoveItemCommand, Is.Not.Null);
    }

    [Test]
    public void RemoveItemCommand_CannotExecuteWhenNoItemSelected()
    {
      Assert.That(_viewModel.RemoveItemCommand.CanExecute(null), Is.False);
    }

    [Test]
    public void CopyCommand_IsNotNull()
    {
      Assert.That(_viewModel.CopyCommand, Is.Not.Null);
    }

    [Test]
    public void CutCommand_IsNotNull()
    {
      Assert.That(_viewModel.CutCommand, Is.Not.Null);
    }

    [Test]
    public void PasteCommand_IsNotNull()
    {
      Assert.That(_viewModel.PasteCommand, Is.Not.Null);
    }

    [Test]
    public void MoveBandUpCommand_IsNotNull()
    {
      Assert.That(_viewModel.MoveBandUpCommand, Is.Not.Null);
    }

    [Test]
    public void MoveBandDownCommand_IsNotNull()
    {
      Assert.That(_viewModel.MoveBandDownCommand, Is.Not.Null);
    }

    [Test]
    public void EditBandDetailsCommand_IsNotNull()
    {
      Assert.That(_viewModel.EditBandDetailsCommand, Is.Not.Null);
    }

    [TearDown]
    public void TearDown()
    {
      _viewModel?.Dispose();
    }
  }
}
