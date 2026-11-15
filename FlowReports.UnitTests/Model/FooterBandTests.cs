using FlowReports.Model.ReportItems;

namespace FlowReports.UnitTests.Model
{
  public class FooterBandTests
  {
    private FooterBand _footerBand;

    [SetUp]
    public void Setup()
    {
      _footerBand = new FooterBand();
    }

    [Test]
    public void Constructor_CreatesValidFooterBand()
    {
      Assert.That(_footerBand, Is.Not.Null);
      Assert.That(_footerBand.ID, Is.Not.EqualTo(Guid.Empty));
    }

    [Test]
    public void Constructor_WithId_UsesProvidedId()
    {
      var id = Guid.NewGuid();
      var footerBand = new FooterBand(id);
      Assert.That(footerBand.ID, Is.EqualTo(id));
    }

    [Test]
    public void Height_DefaultValue_IsDefaultHeight()
    {
      Assert.That(_footerBand.Height, Is.EqualTo(ReportBandBase.DefaultHeight));
    }

    [Test]
    public void Height_CanBeSet()
    {
      _footerBand.Height = 75;
      Assert.That(_footerBand.Height, Is.EqualTo(75));
    }

    [Test]
    public void Items_CanBeAdded()
    {
      var item = new TextItem();
      _footerBand.AddReportItem(item);
      Assert.That(_footerBand.Items, Contains.Item(item));
    }

    [Test]
    public void AddMultipleItems_AllItemsAreAdded()
    {
      _footerBand.AddTextItem();
      _footerBand.AddBooleanItem();
      _footerBand.AddImageItem();

      Assert.That(_footerBand.Items, Has.Count.EqualTo(3));
    }

    [Test]
    public void RemoveItem_RemovesItemSuccessfully()
    {
      var item = new ImageItem();
      _footerBand.AddReportItem(item);
      _footerBand.RemoveItem(item);
      Assert.That(_footerBand.Items, Is.Empty);
    }

    [Test]
    public void ItemRemoved_Event_IsRaisedWhenItemRemoved()
    {
      var item = new TextItem();
      _footerBand.AddReportItem(item);

      var eventRaised = false;
      _footerBand.ItemRemoved += (s, e) => eventRaised = true;

      _footerBand.RemoveItem(item);

      Assert.That(eventRaised, Is.True);
    }

    [Test]
    public void ActualHeight_WithoutExplicitHeight_UsesMaxItemExtent()
    {
      _footerBand.Height = null;
      var item = new TextItem { Top = 10, Height = 30 };

      _footerBand.AddReportItem(item);

      Assert.That(_footerBand.ActualHeight, Is.EqualTo(40));
    }

    [Test]
    public void ActualHeight_WithExplicitHeight_IgnoresItemExtent()
    {
      _footerBand.Height = 100;
      var item = new TextItem { Top = 10, Height = 200 };

      _footerBand.AddReportItem(item);

      Assert.That(_footerBand.ActualHeight, Is.EqualTo(100));
    }

    [Test]
    public void Equals_SameFooterBands_ReturnsTrue()
    {
      var footer1 = new FooterBand { Height = 50 };
      var footer2 = new FooterBand { Height = 50 };
      Assert.That(footer1.Equals(footer2), Is.True);
    }

    [Test]
    public void Equals_DifferentHeight_ReturnsFalse()
    {
      var footer1 = new FooterBand { Height = 50 };
      var footer2 = new FooterBand { Height = 60 };
      Assert.That(footer1.Equals(footer2), Is.False);
    }

    [Test]
    public void GetHashCode_ConsistentWithEquals()
    {
      var footer1 = new FooterBand { Height = 50 };
      var footer2 = new FooterBand { Height = 50 };

      if (footer1.Equals(footer2))
      {
        Assert.That(footer1.GetHashCode(), Is.EqualTo(footer2.GetHashCode()));
      }
    }
  }
}
