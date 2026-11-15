using FlowReports.Model.ReportItems;

namespace FlowReports.UnitTests.Model
{
  public class ReportBandTests
  {
    private ReportBand _band;

    [SetUp]
    public void Setup()
    {
      _band = new ReportBand();
    }

    [Test]
    public void Constructor_CreatesValidBand()
    {
      Assert.That(_band, Is.Not.Null);
      Assert.That(_band.ID, Is.Not.EqualTo(Guid.Empty));
    }

    [Test]
    public void Constructor_WithId_SetsProvidedId()
    {
      var id = Guid.NewGuid();
      var band = new ReportBand(id);
      Assert.That(band.ID, Is.EqualTo(id));
    }

    [Test]
    public void Height_DefaultValue_IsDefaultHeight()
    {
      Assert.That(_band.Height, Is.EqualTo(ReportBandBase.DefaultHeight));
    }

    [Test]
    public void Height_CanBeSet()
    {
      _band.Height = 50;
      Assert.That(_band.Height, Is.EqualTo(50));
    }

    [Test]
    public void ActualHeight_WithExplicitHeight_ReturnsExplicitHeight()
    {
      _band.Height = 100;
      Assert.That(_band.ActualHeight, Is.EqualTo(100));
    }

    [Test]
    public void ActualHeight_WithNoItems_ReturnsZero()
    {
      _band.Height = null;
      Assert.That(_band.ActualHeight, Is.EqualTo(0.0));
    }

    [Test]
    public void ActualHeight_WithItems_ReturnsMaximumExtent()
    {
      _band.Height = null;
      var item1 = new TextItem { Top = 0, Height = 20 };
      var item2 = new TextItem { Top = 30, Height = 25 };
      
      _band.AddReportItem(item1);
      _band.AddReportItem(item2);

      Assert.That(_band.ActualHeight, Is.EqualTo(55));
    }

    [Test]
    public void DataSource_CanBeSetAndRetrieved()
    {
      _band.DataSource = "Items";
      Assert.That(_band.DataSource, Is.EqualTo("Items"));
    }

    [Test]
    public void Items_DefaultValue_IsEmptyCollection()
    {
      Assert.That(_band.Items, Is.Empty);
    }

    [Test]
    public void AddTextItem_CreatesAndAddsTextItem()
    {
      _band.AddTextItem();
      Assert.That(_band.Items, Has.Count.EqualTo(1));
      Assert.That(_band.Items[0], Is.TypeOf<TextItem>());
    }

    [Test]
    public void AddBooleanItem_CreatesAndAddsBooleanItem()
    {
      _band.AddBooleanItem();
      Assert.That(_band.Items, Has.Count.EqualTo(1));
      Assert.That(_band.Items[0], Is.TypeOf<BooleanItem>());
    }

    [Test]
    public void AddImageItem_CreatesAndAddsImageItem()
    {
      _band.AddImageItem();
      Assert.That(_band.Items, Has.Count.EqualTo(1));
      Assert.That(_band.Items[0], Is.TypeOf<ImageItem>());
    }

    [Test]
    public void AddReportItem_AddsProvidedItem()
    {
      var item = new TextItem();
      _band.AddReportItem(item);
      Assert.That(_band.Items, Contains.Item(item));
    }

    [Test]
    public void RemoveItem_RemovesItem()
    {
      var item = new TextItem();
      _band.AddReportItem(item);
      _band.RemoveItem(item);
      Assert.That(_band.Items, Is.Empty);
    }

    [Test]
    public void RemoveItem_ItemNotInCollection_NoException()
    {
      var item = new TextItem();
      Assert.DoesNotThrow(() => _band.RemoveItem(item));
    }

    [Test]
    public void ItemAdded_Event_IsRaised()
    {
      var eventRaised = false;
      _band.ItemAdded += (s, e) => eventRaised = true;

      var item = new TextItem();
      _band.AddReportItem(item);

      Assert.That(eventRaised, Is.True);
    }

    [Test]
    public void ItemRemoved_Event_IsRaised()
    {
      var item = new TextItem();
      _band.AddReportItem(item);

      var eventRaised = false;
      _band.ItemRemoved += (s, e) => eventRaised = true;

      _band.RemoveItem(item);

      Assert.That(eventRaised, Is.True);
    }

    [Test]
    public void HeaderBand_CanBeSetAndRetrieved()
    {
      var headerBand = new HeaderBand();
      _band.HeaderBand = headerBand;
      Assert.That(_band.HeaderBand, Is.EqualTo(headerBand));
    }

    [Test]
    public void HeaderBand_Set_RaisesHeaderChangedEvent()
    {
      var eventRaised = false;
      _band.HeaderChanged += (s, e) => eventRaised = true;

      var headerBand = new HeaderBand();
      _band.HeaderBand = headerBand;

      Assert.That(eventRaised, Is.True);
    }

    [Test]
    public void HeaderBand_SetToNull_RaisesHeaderChangedEvent()
    {
      _band.HeaderBand = new HeaderBand();
      var eventRaised = false;
      _band.HeaderChanged += (s, e) => eventRaised = true;

      _band.HeaderBand = null;

      Assert.That(eventRaised, Is.True);
    }

    [Test]
    public void FooterBand_CanBeSetAndRetrieved()
    {
      var footerBand = new FooterBand();
      _band.FooterBand = footerBand;
      Assert.That(_band.FooterBand, Is.EqualTo(footerBand));
    }

    [Test]
    public void FooterBand_Set_RaisesFooterChangedEvent()
    {
      var eventRaised = false;
      _band.FooterChanged += (s, e) => eventRaised = true;

      var footerBand = new FooterBand();
      _band.FooterBand = footerBand;

      Assert.That(eventRaised, Is.True);
    }

    [Test]
    public void Bands_NestedBands_CanBeAdded()
    {
      var subBand = _band.Bands.AddBand();
      using (Assert.EnterMultipleScope())
      {
        Assert.That(_band.Bands, Contains.Item(subBand));
        Assert.That(new List<ReportBand>(_band.Bands), Has.Count.EqualTo(1));
      }
    }

    [Test]
    public void Equals_SameBands_ReturnsTrue()
    {
      var band1 = new ReportBand { Height = 50, DataSource = "Items" };
      var band2 = new ReportBand { Height = 50, DataSource = "Items" };

      Assert.That(band1.Equals(band2), Is.True);
    }

    [Test]
    public void Equals_DifferentHeight_ReturnsFalse()
    {
      var band1 = new ReportBand { Height = 50 };
      var band2 = new ReportBand { Height = 60 };

      Assert.That(band1.Equals(band2), Is.False);
    }

    [Test]
    public void Equals_DifferentDataSource_ReturnsFalse()
    {
      var band1 = new ReportBand { DataSource = "Items" };
      var band2 = new ReportBand { DataSource = "Products" };

      Assert.That(band1.Equals(band2), Is.False);
    }

    [Test]
    public void Equals_WithDifferentItems_ReturnsFalse()
    {
      var band1 = new ReportBand();
      band1.AddTextItem();

      var band2 = new ReportBand();
      band2.AddBooleanItem();

      Assert.That(band1.Equals(band2), Is.False);
    }

    [Test]
    public void GetHashCode_ConsistentWithEquals()
    {
      var band1 = new ReportBand { Height = 50, DataSource = "Items" };
      var band2 = new ReportBand { Height = 50, DataSource = "Items" };

      if (band1.Equals(band2))
      {
        Assert.That(band1.GetHashCode(), Is.EqualTo(band2.GetHashCode()));
      }
    }
  }
}
