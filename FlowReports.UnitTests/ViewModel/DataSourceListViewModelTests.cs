using FlowReports.Model.DataSources;
using FlowReports.ViewModel;

namespace FlowReports.UnitTests.ViewModel
{
  public class DataSourceListViewModelTests
  {
    private DataSourceListViewModel _viewModel;
    private DataSource _dataSource;

    [SetUp]
    public void Setup()
    {
      _dataSource = new DataSource();
      _viewModel = new DataSourceListViewModel(_dataSource);
    }

    [Test]
    public void Constructor_CreatesViewModelWithDataSourceContainer()
    {
      Assert.That(_viewModel, Is.Not.Null);
    }

    [Test]
    public void Name_ReturnsContainerName()
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
    public void Children_ReturnsEmptyListWhenNoChildren()
    {
      Assert.That(_viewModel.Children, Is.Not.Null);
      Assert.That(_viewModel.Children.Count, Is.EqualTo(0));
    }

    [Test]
    public void Children_LazyLoading()
    {
      var vm1 = new DataSourceListViewModel(_dataSource);
      var vm2 = new DataSourceListViewModel(_dataSource);
      using (Assert.EnterMultipleScope())
      {
        Assert.That(vm1.Children, Is.Not.Null);
        Assert.That(vm2.Children, Is.Not.Null);
      }

      vm1.Dispose();
      vm2.Dispose();
    }

    [TearDown]
    public void TearDown()
    {
      _viewModel?.Dispose();
    }
  }
}
