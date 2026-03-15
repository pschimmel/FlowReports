using FlowReports.Model.DataSources;
using FlowReports.Model.DataSources.Analyzers;
using FlowReports.Model.DataSources.DataSourceItems;
using FlowReports.UnitTests.Model.TestEntities;

namespace FlowReports.UnitTests.Model
{
  // =====================================================
  // Original DataSourceAnalyzerTests with Test Entities
  // =====================================================
  public class DataSourceAnalyzerBasicTests
  {
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void DataSourceAnalyzerTest_ItemIsObject_KnownListType()
    {
      var item = new TestItemWithKnownListType();
      DataSourceAnalyzer.Analyze(new List<TestItemWithKnownListType> { item }, "List");
    }

    [Test]
    public void DataSourceAnalyzerTest_ItemIsObject_UnknownListType()
    {
      var item = new TestItemWithUnknownListType();
      DataSourceAnalyzer.Analyze(new List<TestItemWithUnknownListType> { item }, "List");
    }

    [Test]
    public void DataSourceAnalyzerTest_ItemIsIEnumerable()
    {
      var list = new TestItemWithKnownListType[] { new TestItemWithKnownListType(), new TestItemWithKnownListType() };
      DataSourceAnalyzer.Analyze(list, "List");
    }

    [Test/*, Ignore("Infinite loop")*/]
    public void DataSourceAnalyzerTest_Recursion()
    {
      var hans = new Person("Hans", "Müller");
      var gerda = new Person("Gerda", "Müller");
      var josef = new Person("Josef", "Müller");
      var helmut = new Person("Helmut", "Schmidt");

      hans.Father = josef;
      hans.Mother = gerda;
      josef.Children.Add(hans);
      gerda.Children.Add(hans);

      var list = new Person[] { hans, helmut };
      DataSource dataSource = DataSourceAnalyzer.Analyze(list, "Persons");
      Assert.That(dataSource, Is.Not.Null);
    }
  }

  // =====================================================
  // PropertyAnalyzerTests
  // =====================================================
  public class PropertyAnalyzerTests
  {
    [Test]
    public void TextFieldAnalyzer_IsSupported_String_ReturnsTrue()
    {
      var analyzer = new TextFieldAnalyzer();
      Assert.That(analyzer.IsSupported(typeof(string)), Is.True);
    }

    [Test]
    public void TextFieldAnalyzer_IsSupported_Int_ReturnsFalse()
    {
      var analyzer = new TextFieldAnalyzer();
      Assert.That(analyzer.IsSupported(typeof(int)), Is.False);
    }

    [Test]
    public void TextFieldAnalyzer_GetItem_CreatesTextField()
    {
      var analyzer = new TextFieldAnalyzer();
      var item = analyzer.GetItem(typeof(string), "Name");

      Assert.That(item, Is.InstanceOf<TextField>());
      Assert.That(item.Name, Is.EqualTo("Name"));
      Assert.That(item.Type, Is.EqualTo(typeof(string)));
    }

    [Test]
    public void NumberFieldAnalyzer_IsSupported_Int_ReturnsTrue()
    {
      var analyzer = new NumberFieldAnalyzer();
      Assert.That(analyzer.IsSupported(typeof(int)), Is.True);
    }

    [Test]
    public void NumberFieldAnalyzer_IsSupported_Double_ReturnsTrue()
    {
      var analyzer = new NumberFieldAnalyzer();
      Assert.That(analyzer.IsSupported(typeof(double)), Is.True);
    }

    [Test]
    public void NumberFieldAnalyzer_IsSupported_Decimal_ReturnsTrue()
    {
      var analyzer = new NumberFieldAnalyzer();
      Assert.That(analyzer.IsSupported(typeof(decimal)), Is.True);
    }

    [Test]
    public void NumberFieldAnalyzer_IsSupported_String_ReturnsFalse()
    {
      var analyzer = new NumberFieldAnalyzer();
      Assert.That(analyzer.IsSupported(typeof(string)), Is.False);
    }

    [Test]
    public void NumberFieldAnalyzer_GetItem_CreatesNumberField()
    {
      var analyzer = new NumberFieldAnalyzer();
      var item = analyzer.GetItem(typeof(decimal), "Price");

      Assert.That(item, Is.InstanceOf<NumberField>());
      Assert.That(item.Name, Is.EqualTo("Price"));
      Assert.That(item.Type, Is.EqualTo(typeof(decimal)));
    }

    [Test]
    public void BooleanFieldAnalyzer_IsSupported_Bool_ReturnsTrue()
    {
      var analyzer = new BooleanFieldAnalyzer();
      Assert.That(analyzer.IsSupported(typeof(bool)), Is.True);
    }

    [Test]
    public void BooleanFieldAnalyzer_IsSupported_String_ReturnsFalse()
    {
      var analyzer = new BooleanFieldAnalyzer();
      Assert.That(analyzer.IsSupported(typeof(string)), Is.False);
    }

    [Test]
    public void BooleanFieldAnalyzer_GetItem_CreatesBooleanField()
    {
      var analyzer = new BooleanFieldAnalyzer();
      var item = analyzer.GetItem(typeof(bool), "IsActive");

      Assert.That(item, Is.InstanceOf<BooleanField>());
      Assert.That(item.Name, Is.EqualTo("IsActive"));
      Assert.That(item.Type, Is.EqualTo(typeof(bool)));
    }

    [Test]
    public void DateFieldAnalyzer_IsSupported_DateTime_ReturnsTrue()
    {
      var analyzer = new DateFieldAnalyzer();
      Assert.That(analyzer.IsSupported(typeof(DateTime)), Is.True);
    }

    [Test]
    public void DateFieldAnalyzer_IsSupported_String_ReturnsFalse()
    {
      var analyzer = new DateFieldAnalyzer();
      Assert.That(analyzer.IsSupported(typeof(string)), Is.False);
    }

    [Test]
    public void DateFieldAnalyzer_GetItem_CreatesDateField()
    {
      var analyzer = new DateFieldAnalyzer();
      var item = analyzer.GetItem(typeof(DateTime), "CreatedDate");

      Assert.That(item, Is.InstanceOf<DateField>());
      Assert.That(item.Name, Is.EqualTo("CreatedDate"));
      Assert.That(item.Type, Is.EqualTo(typeof(DateTime)));
    }

    [Test]
    public void ImageAnalyzer_IsSupported_ByteArray_ReturnsTrue()
    {
      var analyzer = new ImageAnalyzer();
      Assert.That(analyzer.IsSupported(typeof(byte[])), Is.True);
    }

    [Test]
    public void ImageAnalyzer_IsSupported_String_ReturnsFalse()
    {
      var analyzer = new ImageAnalyzer();
      Assert.That(analyzer.IsSupported(typeof(string)), Is.False);
    }

    [Test]
    public void ImageAnalyzer_GetItem_CreatesImageField()
    {
      var analyzer = new ImageAnalyzer();
      var item = analyzer.GetItem(typeof(byte[]), "Photo");

      Assert.That(item, Is.InstanceOf<ImageField>());
      Assert.That(item.Name, Is.EqualTo("Photo"));
      Assert.That(item.Type, Is.EqualTo(typeof(byte[])));
    }

    [Test]
    public void PropertyAnalyzer_GetItem_WithoutName_GeneratesName()
    {
      var analyzer = new TextFieldAnalyzer();
      var item = analyzer.GetItem(typeof(string));

      Assert.That(item.Name, Is.Not.Null);
      Assert.That(item.Name, Is.Not.Empty);
    }

    [Test]
    public void PropertyAnalyzer_DifferentAnalyzers_SupportDifferentTypes()
    {
      var stringAnalyzer = new TextFieldAnalyzer();
      var numberAnalyzer = new NumberFieldAnalyzer();
      var boolAnalyzer = new BooleanFieldAnalyzer();

      Assert.That(stringAnalyzer.IsSupported(typeof(string)), Is.True);
      Assert.That(numberAnalyzer.IsSupported(typeof(int)), Is.True);
      Assert.That(boolAnalyzer.IsSupported(typeof(bool)), Is.True);

      Assert.That(stringAnalyzer.IsSupported(typeof(int)), Is.False);
      Assert.That(numberAnalyzer.IsSupported(typeof(bool)), Is.False);
      Assert.That(boolAnalyzer.IsSupported(typeof(string)), Is.False);
    }

    [Test]
    public void NumberFieldAnalyzer_IsSupported_Long_ReturnsTrue()
    {
      var analyzer = new NumberFieldAnalyzer();
      Assert.That(analyzer.IsSupported(typeof(long)), Is.True);
    }

    [Test]
    public void NumberFieldAnalyzer_IsSupported_Float_ReturnsTrue()
    {
      var analyzer = new NumberFieldAnalyzer();
      Assert.That(analyzer.IsSupported(typeof(float)), Is.True);
    }

    [Test]
    public void NumberFieldAnalyzer_IsSupported_Short_ReturnsTrue()
    {
      var analyzer = new NumberFieldAnalyzer();
      Assert.That(analyzer.IsSupported(typeof(short)), Is.True);
    }

    [Test]
    public void TextFieldAnalyzer_GetItem_WithCustomName_SetsName()
    {
      var analyzer = new TextFieldAnalyzer();
      var customName = "CustomPropertyName";
      var item = analyzer.GetItem(typeof(string), customName);

      Assert.That(item.Name, Is.EqualTo(customName));
    }

    [Test]
    public void DateFieldAnalyzer_IsSupported_TimeOnly_ReturnsFalse()
    {
      var analyzer = new DateFieldAnalyzer();
      Assert.That(analyzer.IsSupported(typeof(TimeOnly)), Is.False);
    }
  }

  // =====================================================
  // DataSourceAnalyzerTests
  // =====================================================
  public class DataSourceAnalyzerComprehensiveTests
  {
    [Test]
    public void Analyze_SimpleStringList_CreatesDataSource()
    {
      var items = new List<string> { "item1", "item2", "item3" };
      var dataSource = DataSourceAnalyzer.Analyze(items, "Items");

      Assert.That(dataSource, Is.Not.Null);
      Assert.That(dataSource.Name, Is.EqualTo("Items"));
    }

    [Test]
    public void Analyze_NullSource_ThrowsArgumentNullException()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        DataSourceAnalyzer.Analyze<string>(null, "Items");
      });
    }

    [Test]
    public void Analyze_SimpleObjectList_CreatesDataSource()
    {
      var items = new List<SimpleObject>
      {
        new SimpleObject { Name = "Item1", Count = 10 },
        new SimpleObject { Name = "Item2", Count = 20 }
      };

      var dataSource = DataSourceAnalyzer.Analyze(items, "SimpleObjects");

      Assert.That(dataSource, Is.Not.Null);
      Assert.That(dataSource.Name, Is.EqualTo("SimpleObjects"));
      Assert.That(dataSource.Count, Is.GreaterThan(0));
    }

    [Test]
    public void Analyze_ObjectWithStringProperty_CreatesTextField()
    {
      var items = new List<ObjectWithString>
      {
        new ObjectWithString { Value = "test" }
      };

      var dataSource = DataSourceAnalyzer.Analyze(items, "Objects");

      var textField = dataSource.FirstOrDefault(f => f.Name == "Value");
      Assert.That(textField, Is.Not.Null);
      Assert.That(textField, Is.InstanceOf<TextField>());
    }

    [Test]
    public void Analyze_ObjectWithNumberProperty_CreatesNumberField()
    {
      var items = new List<ObjectWithNumber>
      {
        new ObjectWithNumber { Price = 99.99m }
      };

      var dataSource = DataSourceAnalyzer.Analyze(items, "Objects");

      var numberField = dataSource.FirstOrDefault(f => f.Name == "Price");
      Assert.That(numberField, Is.Not.Null);
      Assert.That(numberField, Is.InstanceOf<NumberField>());
    }

    [Test]
    public void Analyze_ObjectWithBoolProperty_CreatesBooleanField()
    {
      var items = new List<ObjectWithBool>
      {
        new ObjectWithBool { IsActive = true }
      };

      var dataSource = DataSourceAnalyzer.Analyze(items, "Objects");

      var boolField = dataSource.FirstOrDefault(f => f.Name == "IsActive");
      Assert.That(boolField, Is.Not.Null);
      Assert.That(boolField, Is.InstanceOf<BooleanField>());
    }

    [Test]
    public void Analyze_ObjectWithDateProperty_CreatesDateField()
    {
      var items = new List<ObjectWithDate>
      {
        new ObjectWithDate { CreatedDate = DateTime.Now }
      };

      var dataSource = DataSourceAnalyzer.Analyze(items, "Objects");

      var dateField = dataSource.FirstOrDefault(f => f.Name == "CreatedDate");
      Assert.That(dateField, Is.Not.Null);
      Assert.That(dateField, Is.InstanceOf<DateField>());
    }

    [Test]
    public void Analyze_ObjectWithImageProperty_CreatesImageField()
    {
      var items = new List<ObjectWithImage>
      {
        new ObjectWithImage { Photo = new byte[] { 1, 2, 3 } }
      };

      var dataSource = DataSourceAnalyzer.Analyze(items, "Objects");

      var imageField = dataSource.FirstOrDefault(f => f.Name == "Photo");
      Assert.That(imageField, Is.Not.Null);
      Assert.That(imageField, Is.InstanceOf<ImageField>());
    }

    [Test]
    public void Analyze_ObjectWithComplexProperty_CreatesObjectField()
    {
      var items = new List<ObjectWithComplex>
      {
        new ObjectWithComplex { Address = new Address { City = "New York" } }
      };

      var dataSource = DataSourceAnalyzer.Analyze(items, "Objects");

      var objectField = dataSource.FirstOrDefault(f => f.Name == "Address");
      Assert.That(objectField, Is.Not.Null);
      Assert.That(objectField, Is.InstanceOf<ObjectField>());
    }

    [Test]
    public void Analyze_MultipleProperties_CreatesMultipleFields()
    {
      var items = new List<MultiPropertyObject>
      {
        new MultiPropertyObject
        {
          Name = "Test",
          Age = 30,
          IsActive = true,
          CreatedDate = DateTime.Now
        }
      };

      var dataSource = DataSourceAnalyzer.Analyze(items, "Objects");

      Assert.That(dataSource.Count, Is.GreaterThanOrEqualTo(4));
      Assert.That(dataSource.Any(f => f.Name == "Name"), Is.True);
      Assert.That(dataSource.Any(f => f.Name == "Age"), Is.True);
      Assert.That(dataSource.Any(f => f.Name == "IsActive"), Is.True);
      Assert.That(dataSource.Any(f => f.Name == "CreatedDate"), Is.True);
    }

    [Test]
    public void Analyze_ObjectWithReadOnlyProperty_IncludesProperty()
    {
      var items = new List<ObjectWithReadOnly>
      {
        new ObjectWithReadOnly { Name = "Test" }
      };

      var dataSource = DataSourceAnalyzer.Analyze(items, "Objects");

      var nameField = dataSource.FirstOrDefault(f => f.Name == "Name");
      Assert.That(nameField, Is.Not.Null);
    }

    [Test]
    public void Analyze_EmptyList_CreatesEmptyDataSource()
    {
      var items = new List<SimpleObject>();
      var dataSource = DataSourceAnalyzer.Analyze(items, "EmptyObjects");

      Assert.That(dataSource.Name, Is.EqualTo("EmptyObjects"));
    }

    [Test]
    public void Analyze_ObjectWithNullableProperties_CreatesFields()
    {
      var items = new List<ObjectWithNullable>
      {
        new ObjectWithNullable { OptionalValue = 10 }
      };

      var dataSource = DataSourceAnalyzer.Analyze(items, "Objects");

      Assert.That(dataSource.Count, Is.GreaterThan(0));
    }

    [Test]
    public void Analyze_DataSourcePreservesName()
    {
      var items = new List<SimpleObject>();
      var dataSourceName = "MyCustomDataSource";

      var dataSource = DataSourceAnalyzer.Analyze(items, dataSourceName);

      Assert.That(dataSource.Name, Is.EqualTo(dataSourceName));
    }

    // Test objects
    public class SimpleObject
    {
      public string Name { get; set; }
      public int Count { get; set; }
    }

    public class ObjectWithString
    {
      public string Value { get; set; }
    }

    public class ObjectWithNumber
    {
      public decimal Price { get; set; }
    }

    public class ObjectWithBool
    {
      public bool IsActive { get; set; }
    }

    public class ObjectWithDate
    {
      public DateTime CreatedDate { get; set; }
    }

    public class ObjectWithImage
    {
      public byte[] Photo { get; set; }
    }

    public class ObjectWithComplex
    {
      public Address Address { get; set; }
    }

    public class Address
    {
      public string City { get; set; }
      public string Street { get; set; }
    }

    public class MultiPropertyObject
    {
      public string Name { get; set; }
      public int Age { get; set; }
      public bool IsActive { get; set; }
      public DateTime CreatedDate { get; set; }
    }

    public class ObjectWithReadOnly
    {
      public string Name { get; set; }
      public DateTime CreatedAt => DateTime.Now;
    }

    public class ObjectWithNullable
    {
      public int? OptionalValue { get; set; }
      public string Name { get; set; }
    }
  }

  // =====================================================
  // DataSourceFieldTypesTests
  // =====================================================
  public class DataSourceFieldTypesTests
  {
    [Test]
    public void TextField_ImplementsIFormatItem()
    {
      var field = new TextField();
      Assert.That(field, Is.InstanceOf<IFormatItem>());
    }

    [Test]
    public void TextField_DefaultFormat_IsEmpty()
    {
      var field = new TextField();
      Assert.That(field.DefaultFormat, Is.Empty);
    }

    [Test]
    public void TextField_Formats_IsEmpty()
    {
      var field = new TextField();
      Assert.That(field.Formats, Is.Empty);
    }

    [Test]
    public void TextField_NameCanBeSet()
    {
      var field = new TextField();
      field.Name = "FirstName";
      Assert.That(field.Name, Is.EqualTo("FirstName"));
    }

    [Test]
    public void TextField_TypeCanBeSet()
    {
      var field = new TextField();
      field.Type = typeof(string);
      Assert.That(field.Type, Is.EqualTo(typeof(string)));
    }

    [Test]
    public void NumberField_ImplementsIFormatItem()
    {
      var field = new NumberField();
      Assert.That(field, Is.InstanceOf<IFormatItem>());
    }

    [Test]
    public void NumberField_DefaultFormat_IsF2()
    {
      var field = new NumberField();
      Assert.That(field.DefaultFormat, Is.EqualTo("f2"));
    }

    [Test]
    public void NumberField_Formats_IsNotNull()
    {
      var field = new NumberField();
      Assert.That(field.Formats, Is.Not.Null);
    }

    [Test]
    public void NumberField_NameCanBeSet()
    {
      var field = new NumberField();
      field.Name = "Price";
      Assert.That(field.Name, Is.EqualTo("Price"));
    }

    [Test]
    public void NumberField_TypeCanBeSet()
    {
      var field = new NumberField();
      field.Type = typeof(decimal);
      Assert.That(field.Type, Is.EqualTo(typeof(decimal)));
    }

    [Test]
    public void BooleanField_ImplementsIDataSourceItem()
    {
      var field = new BooleanField();
      Assert.That(field, Is.InstanceOf<IDataSourceItem>());
    }

    [Test]
    public void BooleanField_NameCanBeSet()
    {
      var field = new BooleanField();
      field.Name = "IsActive";
      Assert.That(field.Name, Is.EqualTo("IsActive"));
    }

    [Test]
    public void BooleanField_TypeCanBeSet()
    {
      var field = new BooleanField();
      field.Type = typeof(bool);
      Assert.That(field.Type, Is.EqualTo(typeof(bool)));
    }

    [Test]
    public void DateField_ImplementsIFormatItem()
    {
      var field = new DateField();
      Assert.That(field, Is.InstanceOf<IFormatItem>());
    }

    [Test]
    public void DateField_NameCanBeSet()
    {
      var field = new DateField();
      field.Name = "CreatedDate";
      Assert.That(field.Name, Is.EqualTo("CreatedDate"));
    }

    [Test]
    public void DateField_TypeCanBeSet()
    {
      var field = new DateField();
      field.Type = typeof(DateTime);
      Assert.That(field.Type, Is.EqualTo(typeof(DateTime)));
    }

    [Test]
    public void DateField_DefaultFormat_IsNotEmpty()
    {
      var field = new DateField();
      Assert.That(field.DefaultFormat, Is.Not.Empty);
    }

    [Test]
    public void ImageField_ImplementsIDataSourceItem()
    {
      var field = new ImageField();
      Assert.That(field, Is.InstanceOf<IDataSourceItem>());
    }

    [Test]
    public void ImageField_NameCanBeSet()
    {
      var field = new ImageField();
      field.Name = "Photo";
      Assert.That(field.Name, Is.EqualTo("Photo"));
    }

    [Test]
    public void ImageField_TypeCanBeSet()
    {
      var field = new ImageField();
      field.Type = typeof(byte[]);
      Assert.That(field.Type, Is.EqualTo(typeof(byte[])));
    }

    [Test]
    public void ObjectField_ImplementsIDataSourceItemContainer()
    {
      var field = new ObjectField();
      Assert.That(field, Is.InstanceOf<IDataSourceItemContainer>());
    }

    [Test]
    public void ObjectField_ImplementsList()
    {
      var field = new ObjectField();
      Assert.That(field, Is.InstanceOf<List<IDataSourceItem>>());
    }

    [Test]
    public void ObjectField_NameCanBeSet()
    {
      var field = new ObjectField();
      field.Name = "Address";
      Assert.That(field.Name, Is.EqualTo("Address"));
    }

    [Test]
    public void ObjectField_TypeCanBeSet()
    {
      var field = new ObjectField();
      field.Type = typeof(object);
      Assert.That(field.Type, Is.EqualTo(typeof(object)));
    }

    [Test]
    public void ObjectField_CanAddChildItems()
    {
      var field = new ObjectField { Name = "Address" };
      var childField = new TextField { Name = "Street" };

      field.Add(childField);

      Assert.That(field.Contains(childField), Is.True);
      Assert.That(field.Count, Is.EqualTo(1));
    }

    [Test]
    public void ObjectField_CanRemoveChildItems()
    {
      var field = new ObjectField();
      var childField = new TextField { Name = "Street" };

      field.Add(childField);
      field.Remove(childField);

      Assert.That(field.Contains(childField), Is.False);
      Assert.That(field.Count, Is.EqualTo(0));
    }

    [Test]
    public void AllFieldTypes_ImplementIDataSourceItem()
    {
      var fields = new IDataSourceItem[]
      {
        new TextField(),
        new NumberField(),
        new BooleanField(),
        new DateField(),
        new ImageField(),
        new ObjectField()
      };

      foreach (var field in fields)
      {
        Assert.That(field, Is.InstanceOf<IDataSourceItem>());
      }
    }

    [Test]
    public void FormatFields_HaveDefaultFormat()
    {
      var formatFields = new IFormatItem[]
      {
        new TextField(),
        new NumberField(),
        new DateField()
      };

      foreach (var field in formatFields)
      {
        Assert.That(field.DefaultFormat, Is.Not.Null);
      }
    }

    [Test]
    public void FormatFields_HaveFormatsCollection()
    {
      var formatFields = new IFormatItem[]
      {
        new TextField(),
        new NumberField(),
        new DateField()
      };

      foreach (var field in formatFields)
      {
        Assert.That(field.Formats, Is.Not.Null);
      }
    }

    [Test]
    public void DataSourceItem_PropertyInitialization()
    {
      var field = new TextField { Name = "Test", Type = typeof(string) };

      Assert.That(field.Name, Is.EqualTo("Test"));
      Assert.That(field.Type, Is.EqualTo(typeof(string)));
    }

    [Test]
    public void ObjectField_AsContainer_IsEnumerable()
    {
      var field = new ObjectField { Name = "Root" };
      field.Add(new TextField { Name = "Child1" });
      field.Add(new NumberField { Name = "Child2" });

      var items = field.ToList();

      Assert.That(items, Has.Count.EqualTo(2));
    }
  }
}
