using FlowReports.Model.ReportItems;
using FlowReports.ViewModel.Editor;

namespace FlowReports.UnitTests.ViewModel
{
  public class FooterBandViewModelTests
  {
    private FooterBandViewModel _viewModel;
    private FooterBand _band;

    [SetUp]
    public void Setup()
    {
      _band = new FooterBand();
      _viewModel = new FooterBandViewModel(_band);
    }

    [Test]
    public void Constructor_CreatesViewModelWithFooterBand()
    {
      Assert.That(_viewModel, Is.Not.Null);
      Assert.That(_viewModel.Band, Is.EqualTo(_band));
    }

    [Test]
    public void IsSelected_CanBeSet()
    {
      _viewModel.IsSelected = true;
      Assert.That(_viewModel.IsSelected, Is.True);
    }

    [Test]
    public void Height_CanBeSet()
    {
      _viewModel.Height = 100;
      Assert.That(_viewModel.Height, Is.EqualTo(100));
    }

    [Test]
    public void HeightAuto_CanBeSet()
    {
      _viewModel.HeightAuto = true;
      Assert.That(_viewModel.HeightAuto, Is.True);
    }

    [Test]
    public void Items_IsNotNull()
    {
      Assert.That(_viewModel.Items, Is.Not.Null);
    }

    [Test]
    public void SelectCommand_IsNotNull()
    {
      Assert.That(_viewModel.SelectCommand, Is.Not.Null);
    }

    [Test]
    public void DeselectItemsCommand_IsNotNull()
    {
      Assert.That(_viewModel.DeselectItemsCommand, Is.Not.Null);
    }

    [TearDown]
    public void TearDown()
    {
      _viewModel?.Dispose();
    }
  }
}
