using FlowReports.Model.DataSources;
using FlowReports.ViewModel;

namespace FlowReports.UnitTests.ViewModel
{
  public class DataSourceViewModelTests
  {
    private DataSourceViewModel _viewModel;
    private DataSource _dataSource;

    [SetUp]
    public void Setup()
    {
      _dataSource = new DataSource();
      _viewModel = new DataSourceViewModel(_dataSource);
    }

    [Test]
    public void Constructor_CreatesViewModelWithDataSource()
    {
      Assert.That(_viewModel, Is.Not.Null);
    }

    [Test]
    public void Name_ReturnsDataSourceName()
    {
      Assert.That(_viewModel.Name, Is.Not.Null);
      Assert.That(_viewModel.Name, Is.EqualTo(_dataSource.Name));
    }

    [Test]
    public void CanHaveChildren_ReturnsTrue()
    {
      Assert.That(_viewModel.CanHaveChildren, Is.True);
    }

    [Test]
    public void Icon_ReturnsIconName()
    {
      Assert.That(_viewModel.Icon, Is.Not.Null);
      Assert.That(_viewModel.Icon, Is.EqualTo("Database_16x.png"));
    }

    [Test]
    public void Children_InitiallyEmpty()
    {
      Assert.That(_viewModel.Children, Is.Not.Null);
      Assert.That(_viewModel.Children.Count, Is.EqualTo(0));
    }

    [TearDown]
    public void TearDown()
    {
      _viewModel?.Dispose();
    }
  }
}
