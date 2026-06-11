using FlowReports.Model.DataSources;
using FlowReports.Model.DataSources.DataSourceItems;

namespace FlowReports.UnitTests.Model
{
  public class DataSourceTests
  {
    private DataSource _dataSource;

    [SetUp]
    public void Setup()
    {
      _dataSource = new DataSource();
    }

    [Test]
    public void Constructor_CreatesEmptyDataSource()
    {
      Assert.That(_dataSource, Is.Not.Null);
      Assert.That(_dataSource, Is.Empty);
    }

    [Test]
    public void Name_CanBeSetAndRetrieved()
    {
      _dataSource.Name = "Customers";
      Assert.That(_dataSource.Name, Is.EqualTo("Customers"));
    }

    [Test]
    public void Type_CanBeSetAndRetrieved()
    {
      var type = typeof(string);
      _dataSource.Type = type;
      Assert.That(_dataSource.Type, Is.EqualTo(type));
    }

    [Test]
    public void DataSourceItem_CanBeAdded()
    {
      var field = new TextField { Name = "FirstName", Type = typeof(string) };
      _dataSource.Add(field);
      Assert.That(_dataSource, Contains.Item(field));
      Assert.That(_dataSource, Has.Count.EqualTo(1));
    }

    [Test]
    public void DataSourceItem_CanBeRemoved()
    {
      var field = new TextField { Name = "FirstName", Type = typeof(string) };
      _dataSource.Add(field);
      _dataSource.Remove(field);
      Assert.That(_dataSource, Is.Empty);
    }

    [Test]
    public void MultipleItems_CanBeAdded()
    {
      var field1 = new TextField { Name = "FirstName", Type = typeof(string) };
      var field2 = new NumberField { Name = "Age", Type = typeof(int) };
      var field3 = new BooleanField { Name = "IsActive", Type = typeof(bool) };

      _dataSource.Add(field1);
      _dataSource.Add(field2);
      _dataSource.Add(field3);

      Assert.That(_dataSource, Has.Count.EqualTo(3));
      Assert.That(_dataSource, Contains.Item(field1).And.Contains(field2).And.Contains(field3));
    }

    [Test]
    public void DataSourceItemList_IsEnumerable()
    {
      var field1 = new TextField { Name = "FirstName" };
      var field2 = new NumberField { Name = "Salary" };

      _dataSource.Add(field1);
      _dataSource.Add(field2);

      var items = new List<IDataSourceItem>(_dataSource);
      Assert.That(items, Has.Count.EqualTo(2));
      using (Assert.EnterMultipleScope())
      {
        Assert.That(items[0], Is.EqualTo(field1));
        Assert.That(items[1], Is.EqualTo(field2));
      }
    }

    [Test]
    public void DataSourceItemList_Clear_RemovesAllItems()
    {
      _dataSource.Add(new TextField { Name = "Name" });
      _dataSource.Add(new NumberField { Name = "Age" });
      _dataSource.Add(new BooleanField { Name = "Active" });

      _dataSource.Clear();

      Assert.That(_dataSource, Is.Empty);
    }

    [Test]
    public void TextField_CanBeCreatedAndConfigured()
    {
      var field = new TextField { Name = "Description", Type = typeof(string) };
      using (Assert.EnterMultipleScope())
      {
        Assert.That(field.Name, Is.EqualTo("Description"));
        Assert.That(field.Type, Is.EqualTo(typeof(string)));
        Assert.That(field.DefaultFormat, Is.Empty);
        Assert.That(field.Formats, Is.Empty);
      }
    }

    [Test]
    public void NumberField_HasDefaultFormat()
    {
      var field = new NumberField();
      Assert.That(field.DefaultFormat, Is.EqualTo("f2"));
    }

    [Test]
    public void BooleanField_CanBeCreatedAndConfigured()
    {
      var field = new BooleanField { Name = "IsActive", Type = typeof(bool) };
      using (Assert.EnterMultipleScope())
      {
        Assert.That(field.Name, Is.EqualTo("IsActive"));
        Assert.That(field.Type, Is.EqualTo(typeof(bool)));
      }
    }

    [Test]
    public void DateField_CanBeCreatedAndConfigured()
    {
      var field = new DateField { Name = "CreatedDate", Type = typeof(DateTime) };
      using (Assert.EnterMultipleScope())
      {
        Assert.That(field.Name, Is.EqualTo("CreatedDate"));
        Assert.That(field.Type, Is.EqualTo(typeof(DateTime)));
      }
    }

    [Test]
    public void ImageField_CanBeCreatedAndConfigured()
    {
      var field = new ImageField { Name = "ProductImage", Type = typeof(byte[]) };
      using (Assert.EnterMultipleScope())
      {
        Assert.That(field.Name, Is.EqualTo("ProductImage"));
        Assert.That(field.Type, Is.EqualTo(typeof(byte[])));
      }
    }

    [Test]
    public void ObjectField_ContainsNestedItems()
    {
      var field = new ObjectField { Name = "AddressInfo", Type = typeof(object) };
      var nestedField = new TextField { Name = "Street", Type = typeof(string) };
      
      field.Add(nestedField);

      Assert.That(field, Contains.Item(nestedField));
      Assert.That(field.Name, Is.EqualTo("AddressInfo"));
    }

    [Test]
    public void DataSourceItemList_SupportsIndexing()
    {
      var field1 = new TextField { Name = "First" };
      var field2 = new TextField { Name = "Second" };
      var field3 = new TextField { Name = "Third" };

      _dataSource.Add(field1);
      _dataSource.Add(field2);
      _dataSource.Add(field3);

      using (Assert.EnterMultipleScope())
      {
        Assert.That(_dataSource[0], Is.EqualTo(field1));
        Assert.That(_dataSource[1], Is.EqualTo(field2));
        Assert.That(_dataSource[2], Is.EqualTo(field3));
      }
    }

    [Test]
    public void DataSourceItemList_Contains_ReturnsTrueForExistingItem()
    {
      var field = new TextField { Name = "Name" };
      _dataSource.Add(field);

      Assert.That(_dataSource.Contains(field), Is.True);
    }

    [Test]
    public void DataSourceItemList_Contains_ReturnsFalseForNonExistingItem()
    {
      var field = new TextField { Name = "Name" };
      Assert.That(_dataSource.Contains(field), Is.False);
    }

    [Test]
    public void DataSourceItemList_RemoveAt_RemovesItemAtIndex()
    {
      var field1 = new TextField { Name = "First" };
      var field2 = new TextField { Name = "Second" };

      _dataSource.Add(field1);
      _dataSource.Add(field2);

      _dataSource.RemoveAt(0);

      Assert.That(_dataSource, Has.Count.EqualTo(1));
      Assert.That(_dataSource[0], Is.EqualTo(field2));
    }
  }
}
