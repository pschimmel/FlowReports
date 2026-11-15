using FlowReports.Model.ReportItems;

namespace FlowReports.UnitTests.Model
{
  public class BooleanItemTests
  {
    private BooleanItem _item;

    [SetUp]
    public void Setup()
    {
      _item = new BooleanItem();
    }

    [Test]
    public void Constructor_CreatesValidBooleanItem()
    {
      Assert.That(_item, Is.Not.Null);
      Assert.That(_item.ID, Is.Not.EqualTo(Guid.Empty));
    }

    [Test]
    public void Constructor_WithId_UsesProvidedId()
    {
      var id = Guid.NewGuid();
      var item = new BooleanItem(id);
      Assert.That(item.ID, Is.EqualTo(id));
    }

    [Test]
    public void DefaultWidth_EqualsDefaultHeight()
    {
      Assert.That(_item.DefaultWidth, Is.EqualTo(ReportBandBase.DefaultHeight));
    }

    [Test]
    public void DefaultHeight_EqualsDefaultHeight()
    {
      Assert.That(_item.DefaultHeight, Is.EqualTo(ReportBandBase.DefaultHeight));
    }

    [Test]
    public void Width_InitializedToDefaultWidth()
    {
      Assert.That(_item.Width, Is.EqualTo(_item.DefaultWidth));
    }

    [Test]
    public void Height_InitializedToDefaultHeight()
    {
      Assert.That(_item.Height, Is.EqualTo(_item.DefaultHeight));
    }

    [Test]
    public void DataSource_CanBeSet()
    {
      _item.DataSource = "IsActive";
      Assert.That(_item.DataSource, Is.EqualTo("IsActive"));
    }

    [Test]
    public void Position_CanBeSet()
    {
      _item.Left = 25;
      _item.Top = 50;
      using (Assert.EnterMultipleScope())
      {
        Assert.That(_item.Left, Is.EqualTo(25));
        Assert.That(_item.Top, Is.EqualTo(50));
      }
    }

    [Test]
    public void Dimensions_CanBeCustomized()
    {
      _item.Width = 50;
      _item.Height = 30;
      using (Assert.EnterMultipleScope())
      {
        Assert.That(_item.Width, Is.EqualTo(50));
        Assert.That(_item.Height, Is.EqualTo(30));
      }
    }

    [Test]
    public void Equals_SameBooleanItems_ReturnsTrue()
    {
      var item1 = new BooleanItem { DataSource = "IsActive" };
      var item2 = new BooleanItem { DataSource = "IsActive" };

      Assert.That(item1.Equals(item2), Is.True);
    }

    [Test]
    public void Equals_DifferentDataSource_ReturnsFalse()
    {
      var item1 = new BooleanItem { DataSource = "IsActive" };
      var item2 = new BooleanItem { DataSource = "IsDeleted" };

      Assert.That(item1.Equals(item2), Is.False);
    }

    [Test]
    public void Equals_WithNull_ReturnsFalse()
    {
      Assert.That(_item.Equals(null), Is.False);
    }

    [Test]
    public void Equals_WithDifferentType_ReturnsFalse()
    {
      var textItem = new TextItem { DataSource = "IsActive" };
      _item.DataSource = "IsActive";

      Assert.That(_item.Equals(textItem), Is.False);
    }

    [Test]
    public void GetHashCode_ConsistentWithEquals()
    {
      var item1 = new BooleanItem { DataSource = "IsActive" };
      var item2 = new BooleanItem { DataSource = "IsActive" };

      if (item1.Equals(item2))
      {
        Assert.That(item1.GetHashCode(), Is.EqualTo(item2.GetHashCode()));
      }
    }

    [Test]
    public void GetHashCode_DifferentDataSources_MayDiffer()
    {
      var item1 = new BooleanItem { DataSource = "IsActive" };
      var item2 = new BooleanItem { DataSource = "IsDeleted" };

      Assert.That(item1.GetHashCode(), Is.Not.EqualTo(item2.GetHashCode()));
    }

    [Test]
    public void BooleanItem_InheritesFromReportItem()
    {
      Assert.That(_item, Is.InstanceOf<ReportItem>());
    }

    [Test]
    public void MultipleInstances_HaveDifferentIds()
    {
      var item1 = new BooleanItem();
      var item2 = new BooleanItem();

      Assert.That(item1.ID, Is.Not.EqualTo(item2.ID));
    }

    [Test]
    public void BooleanItem_IsSquare()
    {
      Assert.That(_item.DefaultWidth, Is.EqualTo(_item.DefaultHeight));
    }
  }

  public class ImageItemTests
  {
    private ImageItem _item;

    [SetUp]
    public void Setup()
    {
      _item = new ImageItem();
    }

    [Test]
    public void Constructor_CreatesValidImageItem()
    {
      Assert.That(_item, Is.Not.Null);
      Assert.That(_item.ID, Is.Not.EqualTo(Guid.Empty));
    }

    [Test]
    public void Constructor_WithId_UsesProvidedId()
    {
      var id = Guid.NewGuid();
      var item = new ImageItem(id);
      Assert.That(item.ID, Is.EqualTo(id));
    }

    [Test]
    public void DefaultWidth_Is100()
    {
      Assert.That(_item.DefaultWidth, Is.EqualTo(100));
    }

    [Test]
    public void DefaultHeight_Is100()
    {
      Assert.That(_item.DefaultHeight, Is.EqualTo(100));
    }

    [Test]
    public void Width_InitializedToDefaultWidth()
    {
      Assert.That(_item.Width, Is.EqualTo(_item.DefaultWidth));
    }

    [Test]
    public void Height_InitializedToDefaultHeight()
    {
      Assert.That(_item.Height, Is.EqualTo(_item.DefaultHeight));
    }

    [Test]
    public void DataSource_CanBeSet()
    {
      _item.DataSource = "ProductImage";
      Assert.That(_item.DataSource, Is.EqualTo("ProductImage"));
    }

    [Test]
    public void Position_CanBeSet()
    {
      _item.Left = 100;
      _item.Top = 150;
      using (Assert.EnterMultipleScope())
      {
        Assert.That(_item.Left, Is.EqualTo(100));
        Assert.That(_item.Top, Is.EqualTo(150));
      }
    }

    [Test]
    public void Dimensions_CanBeCustomized()
    {
      _item.Width = 200;
      _item.Height = 150;
      using (Assert.EnterMultipleScope())
      {
        Assert.That(_item.Width, Is.EqualTo(200));
        Assert.That(_item.Height, Is.EqualTo(150));
      }
    }

    [Test]
    public void Equals_SameImageItems_ReturnsTrue()
    {
      var item1 = new ImageItem { DataSource = "Photo" };
      var item2 = new ImageItem { DataSource = "Photo" };

      Assert.That(item1.Equals(item2), Is.True);
    }

    [Test]
    public void Equals_DifferentDataSource_ReturnsFalse()
    {
      var item1 = new ImageItem { DataSource = "Photo" };
      var item2 = new ImageItem { DataSource = "Thumbnail" };

      Assert.That(item1.Equals(item2), Is.False);
    }

    [Test]
    public void Equals_WithNull_ReturnsFalse()
    {
      Assert.That(_item.Equals(null), Is.False);
    }

    [Test]
    public void Equals_WithDifferentType_ReturnsFalse()
    {
      var textItem = new TextItem { DataSource = "Photo" };
      _item.DataSource = "Photo";

      Assert.That(_item.Equals(textItem), Is.False);
    }

    [Test]
    public void GetHashCode_ConsistentWithEquals()
    {
      var item1 = new ImageItem { DataSource = "Photo" };
      var item2 = new ImageItem { DataSource = "Photo" };

      if (item1.Equals(item2))
      {
        Assert.That(item1.GetHashCode(), Is.EqualTo(item2.GetHashCode()));
      }
    }

    [Test]
    public void ImageItem_InheritesFromReportItem()
    {
      Assert.That(_item, Is.InstanceOf<ReportItem>());
    }

    [Test]
    public void MultipleInstances_HaveDifferentIds()
    {
      var item1 = new ImageItem();
      var item2 = new ImageItem();

      Assert.That(item1.ID, Is.Not.EqualTo(item2.ID));
    }

    [Test]
    public void ImageItem_IsSquare()
    {
      Assert.That(_item.DefaultWidth, Is.EqualTo(_item.DefaultHeight));
    }
  }
}
