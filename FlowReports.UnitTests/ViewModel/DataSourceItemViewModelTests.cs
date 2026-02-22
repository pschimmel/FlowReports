using FlowReports.Model.DataSources;
using FlowReports.Model.DataSources.DataSourceItems;
using FlowReports.ViewModel;

namespace FlowReports.UnitTests.ViewModel
{
  public class DataSourceItemViewModelTests
  {
    private DataSourceItemViewModel _viewModel;
    private TextField _textField;

    [SetUp]
    public void Setup()
    {
      _textField = new TextField { Name = "TestField" };
      _viewModel = new DataSourceItemViewModel(_textField);
    }

    [Test]
    public void Constructor_CreatesViewModelWithDataSourceItem()
    {
      Assert.That(_viewModel, Is.Not.Null);
    }

    [Test]
    public void Constructor_ThrowsExceptionForContainerItem()
    {
      var dataSource = new DataSource();
      Assert.Throws<ArgumentException>(() => new DataSourceItemViewModel(dataSource));
    }

    [Test]
    public void Name_ReturnsItemName()
    {
      Assert.That(_viewModel.Name, Is.EqualTo("TestField"));
    }

    [Test]
    public void CanHaveChildren_ReturnsFalse()
    {
      Assert.That(_viewModel.CanHaveChildren, Is.False);
    }

    [Test]
    public void Icon_ReturnsTextIconForTextField()
    {
      Assert.That(_viewModel.Icon, Is.EqualTo("Text_16x.png"));
    }

    [Test]
    public void Icon_ReturnsDateIconForDateField()
    {
      var dateField = new DateField { Name = "DateField" };
      var vm = new DataSourceItemViewModel(dateField);
      Assert.That(vm.Icon, Is.EqualTo("Calendar_16x.png"));
      vm.Dispose();
    }

    [Test]
    public void Icon_ReturnsNumberIconForNumberField()
    {
      var numberField = new NumberField { Name = "NumberField" };
      var vm = new DataSourceItemViewModel(numberField);
      Assert.That(vm.Icon, Is.EqualTo("Number_16x.png"));
      vm.Dispose();
    }

    [Test]
    public void Icon_ReturnsBooleanIconForBooleanField()
    {
      var booleanField = new BooleanField { Name = "BooleanField" };
      var vm = new DataSourceItemViewModel(booleanField);
      Assert.That(vm.Icon, Is.EqualTo("Checkbox_16x.png"));
      vm.Dispose();
    }

    [Test]
    public void Icon_ReturnsImageIconForImageField()
    {
      var imageField = new ImageField { Name = "ImageField" };
      var vm = new DataSourceItemViewModel(imageField);
      Assert.That(vm.Icon, Is.EqualTo("Image_16x.png"));
      vm.Dispose();
    }

    [Test]
    public void Item_ReturnsDataSourceItem()
    {
      Assert.That(_viewModel.Item, Is.EqualTo(_textField));
    }

    [TearDown]
    public void TearDown()
    {
      _viewModel?.Dispose();
    }
  }
}
