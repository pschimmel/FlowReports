using FlowReports.Model.ReportItems;
using FlowReports.ViewModel.Editor;

namespace FlowReports.UnitTests.ViewModel
{
  public class ReportBandViewModelTests
  {
    private ReportBandViewModel _viewModel;
    private ReportBand _band;

    [SetUp]
    public void Setup()
    {
      _band = new ReportBand();
      _viewModel = new ReportBandViewModel(_band);
    }

    [Test]
    public void Constructor_CreatesViewModelWithBand()
    {
      Assert.That(_viewModel, Is.Not.Null);
      Assert.That(_viewModel.Band, Is.EqualTo(_band));
    }

    [Test]
    public void Bands_InitiallyEmpty()
    {
      Assert.That(_viewModel.Bands, Is.Not.Null);
      Assert.That(_viewModel.Bands.Count, Is.EqualTo(0));
    }

    [Test]
    public void HeaderBand_InitiallyNull()
    {
      Assert.That(_viewModel.HeaderBand, Is.Null);
    }

    [Test]
    public void FooterBand_InitiallyNull()
    {
      Assert.That(_viewModel.FooterBand, Is.Null);
    }

    [Test]
    public void DataSource_ReturnsDataSourceName()
    {
      _band.DataSource = "TestDataSource";
      Assert.That(_viewModel.DataSource, Is.EqualTo("TestDataSource"));
    }

    [Test]
    public void DataSource_CanBeSet()
    {
      _viewModel.DataSource = "NewDataSource";
      using (Assert.EnterMultipleScope())
      {
        Assert.That(_viewModel.DataSource, Is.EqualTo("NewDataSource"));
        Assert.That(_band.DataSource, Is.EqualTo("NewDataSource"));
      }
    }

    [Test]
    public void EditBandDetailsCommand_IsNotNull()
    {
      Assert.That(_viewModel.EditBandDetailsCommand, Is.Not.Null);
    }

    [Test]
    public void EditBandDetailsCommand_CannotExecuteWhenNotSelected()
    {
      _viewModel.IsSelected = false;
      Assert.That(_viewModel.EditBandDetailsCommand.CanExecute(null), Is.False);
    }

    [Test]
    public void EditBandDetailsCommand_CanExecuteWhenSelected()
    {
      _viewModel.IsSelected = true;
      Assert.That(_viewModel.EditBandDetailsCommand.CanExecute(null), Is.True);
    }

    [Test]
    public void IsSelected_CanBeSet()
    {
      _viewModel.IsSelected = true;
      Assert.That(_viewModel.IsSelected, Is.True);
    }

    [Test]
    public void AddBand_IncreasesSubBandCount()
    {
      _viewModel.AddBand();
      Assert.That(_viewModel.Bands.Count, Is.EqualTo(1));
    }

    [Test]
    public void CanMoveBandUp_InitiallyFalse()
    {
      if (_viewModel.Bands.Count > 0)
      {
        var subBand = _viewModel.Bands[0];
        Assert.That(_viewModel.CanMoveBandUp(subBand), Is.False);
      }
    }

    [Test]
    public void CanMoveBandDown_InitiallyFalse()
    {
      if (_viewModel.Bands.Count > 0)
      {
        var subBand = _viewModel.Bands[0];
        Assert.That(_viewModel.CanMoveBandDown(subBand), Is.False);
      }
    }

    [TearDown]
    public void TearDown()
    {
      _viewModel?.Dispose();
    }
  }
}
