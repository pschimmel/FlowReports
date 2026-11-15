using FlowReports.Model.ReportItems;

namespace FlowReports.UnitTests.Model
{
  public class ReportItemTests
  {
    [Test]
    public void TextItem_Constructor_CreatesValidItem()
    {
      var item = new TextItem();
      using (Assert.EnterMultipleScope())
      {
        Assert.That(item.ID, Is.Not.EqualTo(Guid.Empty));
        Assert.That(item.Width, Is.EqualTo(item.DefaultWidth));
        Assert.That(item.Height, Is.EqualTo(item.DefaultHeight));
      }
    }

    [Test]
    public void TextItem_Format_CanBeSetAndRetrieved()
    {
      var item = new TextItem();
      item.Format = "mm/dd/yyyy";
      Assert.That(item.Format, Is.EqualTo("mm/dd/yyyy"));
    }

    [Test]
    public void TextItem_DataSource_CanBeSetAndRetrieved()
    {
      var item = new TextItem();
      item.DataSource = "Name";
      Assert.That(item.DataSource, Is.EqualTo("Name"));
    }

    [Test]
    public void TextItem_Position_CanBeSetAndRetrieved()
    {
      var item = new TextItem();
      item.Left = 10;
      item.Top = 20;
      using (Assert.EnterMultipleScope())
      {
        Assert.That(item.Left, Is.EqualTo(10));
        Assert.That(item.Top, Is.EqualTo(20));
      }
    }

    [Test]
    public void TextItem_Dimensions_CanBeSetAndRetrieved()
    {
      var item = new TextItem();
      item.Width = 150;
      item.Height = 30;
      using (Assert.EnterMultipleScope())
      {
        Assert.That(item.Width, Is.EqualTo(150));
        Assert.That(item.Height, Is.EqualTo(30));
      }
    }

    [Test]
    public void TextItem_DefaultDimensions_AreSet()
    {
      var item = new TextItem();
      using (Assert.EnterMultipleScope())
      {
        Assert.That(item.DefaultWidth, Is.EqualTo(100));
        Assert.That(item.DefaultHeight, Is.EqualTo(ReportBandBase.DefaultHeight));
      }
    }

    [Test]
    public void TextItem_Equals_SameItems_ReturnsTrue()
    {
      var item1 = new TextItem { DataSource = "Name", Format = "g" };
      var item2 = new TextItem { DataSource = "Name", Format = "g" };
      Assert.That(item1.Equals(item2), Is.True);
    }

    [Test]
    public void TextItem_Equals_DifferentFormat_ReturnsFalse()
    {
      var item1 = new TextItem { Format = "mm/dd/yyyy" };
      var item2 = new TextItem { Format = "dd/mm/yyyy" };
      Assert.That(item1.Equals(item2), Is.False);
    }

    [Test]
    public void TextItem_Equals_DifferentDataSource_ReturnsFalse()
    {
      var item1 = new TextItem { DataSource = "Name" };
      var item2 = new TextItem { DataSource = "Address" };
      Assert.That(item1.Equals(item2), Is.False);
    }

    [Test]
    public void BooleanItem_Constructor_CreatesValidItem()
    {
      var item = new BooleanItem();
      using (Assert.EnterMultipleScope())
      {
        Assert.That(item.ID, Is.Not.EqualTo(Guid.Empty));
        Assert.That(item.Width, Is.GreaterThan(0));
        Assert.That(item.Height, Is.GreaterThan(0));
      }
    }

    [Test]
    public void BooleanItem_DataSource_CanBeSetAndRetrieved()
    {
      var item = new BooleanItem();
      item.DataSource = "IsActive";
      Assert.That(item.DataSource, Is.EqualTo("IsActive"));
    }

    [Test]
    public void BooleanItem_Position_CanBeSetAndRetrieved()
    {
      var item = new BooleanItem();
      item.Left = 50;
      item.Top = 100;
      using (Assert.EnterMultipleScope())
      {
        Assert.That(item.Left, Is.EqualTo(50));
        Assert.That(item.Top, Is.EqualTo(100));
      }
    }

    [Test]
    public void ImageItem_Constructor_CreatesValidItem()
    {
      var item = new ImageItem();
      using (Assert.EnterMultipleScope())
      {
        Assert.That(item.ID, Is.Not.EqualTo(Guid.Empty));
        Assert.That(item.Width, Is.GreaterThan(0));
        Assert.That(item.Height, Is.GreaterThan(0));
      }
    }

    [Test]
    public void ImageItem_DataSource_CanBeSetAndRetrieved()
    {
      var item = new ImageItem();
      item.DataSource = "Photo";
      Assert.That(item.DataSource, Is.EqualTo("Photo"));
    }

    [Test]
    public void ReportItem_DifferentIds_HaveDifferentIdentities()
    {
      var item1 = new TextItem();
      var item2 = new TextItem();
      Assert.That(item1.ID, Is.Not.EqualTo(item2.ID));
    }

    [Test]
    public void ReportItem_ConstructorWithId_UsesProvidedId()
    {
      var id = Guid.NewGuid();
      var item = new TextItem(id);
      Assert.That(item.ID, Is.EqualTo(id));
    }

    [Test]
    public void ReportItem_DefaultPosition_IsOrigin()
    {
      var item = new TextItem();
      using (Assert.EnterMultipleScope())
      {
        Assert.That(item.DefaultX, Is.EqualTo(0));
        Assert.That(item.DefaultY, Is.EqualTo(0));
      }
    }
  }
}
