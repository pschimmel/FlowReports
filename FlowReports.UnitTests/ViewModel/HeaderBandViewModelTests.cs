using FlowReports.Model.ReportItems;
using FlowReports.ViewModel.Editor;

namespace FlowReports.UnitTests.ViewModel
{
  public class HeaderBandViewModelTests
  {
    private HeaderBandViewModel _viewModel;
    private HeaderBand _band;

    [SetUp]
    public void Setup()
    {
      _band = new HeaderBand();
      _viewModel = new HeaderBandViewModel(_band);
    }

    [Test]
    public void Constructor_CreatesViewModelWithHeaderBand()
    {
      Assert.That(_viewModel, Is.Not.Null);
      Assert.That(_viewModel.Band, Is.EqualTo(_band));
    }

    [Test]
    public void RepeatOnEachPage_ReturnsBandProperty()
    {
      _band.RepeatOnEachPage = true;
      Assert.That(_viewModel.RepeatOnEachPage, Is.True);
    }

    [Test]
    public void RepeatOnEachPage_CanBeSet()
    {
      _viewModel.RepeatOnEachPage = true;
      using (Assert.EnterMultipleScope())
      {
        Assert.That(_viewModel.RepeatOnEachPage, Is.True);
        Assert.That(_band.RepeatOnEachPage, Is.True);
      }
    }

    [Test]
    public void RepeatOnEachPage_RaisesPropertyChangedEvent()
    {
      var propertyChanged = false;
      _viewModel.PropertyChanged += (s, e) =>
      {
        if (e.PropertyName == nameof(HeaderBandViewModel.RepeatOnEachPage))
        {
          propertyChanged = true;
        }
      };

      _viewModel.RepeatOnEachPage = true;
      Assert.That(propertyChanged, Is.True);
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

    [TearDown]
    public void TearDown()
    {
      _viewModel?.Dispose();
    }
  }
}
