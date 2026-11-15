using FlowReports.Model.ReportItems;

namespace FlowReports.UnitTests.Model
{
  public class HeaderBandTests
  {
    private HeaderBand _headerBand;

    [SetUp]
    public void Setup()
    {
      _headerBand = new HeaderBand();
    }

    [Test]
    public void Constructor_CreatesValidHeaderBand()
    {
      Assert.That(_headerBand, Is.Not.Null);
      Assert.That(_headerBand.ID, Is.Not.EqualTo(Guid.Empty));
    }

    [Test]
    public void Constructor_WithId_UsesProvidedId()
    {
      var id = Guid.NewGuid();
      var headerBand = new HeaderBand(id);
      Assert.That(headerBand.ID, Is.EqualTo(id));
    }

    [Test]
    public void Height_DefaultValue_IsDefaultHeight()
    {
      Assert.That(_headerBand.Height, Is.EqualTo(ReportBandBase.DefaultHeight));
    }

    [Test]
    public void Items_CanBeAdded()
    {
      var item = new TextItem();
      _headerBand.AddReportItem(item);
      Assert.That(_headerBand.Items, Contains.Item(item));
    }

    [Test]
    public void RepeatOnEachPage_DefaultValue_IsFalse()
    {
      Assert.That(_headerBand.RepeatOnEachPage, Is.False);
    }

    [Test]
    public void RepeatOnEachPage_CanBeSetToTrue()
    {
      _headerBand.RepeatOnEachPage = true;
      Assert.That(_headerBand.RepeatOnEachPage, Is.True);
    }

    [Test]
    public void AddTextItem_CreatesTextItem()
    {
      _headerBand.AddTextItem();
      Assert.That(_headerBand.Items, Has.Count.EqualTo(1));
      Assert.That(_headerBand.Items[0], Is.TypeOf<TextItem>());
    }

    [Test]
    public void RemoveItem_RemovesItemSuccessfully()
    {
      var item = new BooleanItem();
      _headerBand.AddReportItem(item);
      _headerBand.RemoveItem(item);
      Assert.That(_headerBand.Items, Is.Empty);
    }

    [Test]
    public void ItemAdded_Event_IsRaisedWhenItemAdded()
    {
      var eventRaised = false;
      _headerBand.ItemAdded += (s, e) => eventRaised = true;

      _headerBand.AddTextItem();

      Assert.That(eventRaised, Is.True);
    }

    [Test]
    public void ActualHeight_WithItems_CalculatesCorrectHeight()
    {
      _headerBand.Height = null;
      var item1 = new TextItem { Top = 0, Height = 15 };
      var item2 = new TextItem { Top = 20, Height = 20 };

      _headerBand.AddReportItem(item1);
      _headerBand.AddReportItem(item2);

      Assert.That(_headerBand.ActualHeight, Is.EqualTo(40));
    }

    [Test]
    public void Equals_SameHeaderBands_ReturnsTrue()
    {
      var header1 = new HeaderBand { Height = 50, RepeatOnEachPage = true };
      var header2 = new HeaderBand { Height = 50, RepeatOnEachPage = true };
      Assert.That(header1.Equals(header2), Is.True);
    }

    [Test]
    public void Equals_DifferentRepeatOnEachPage_ReturnsFalse()
    {
      var header1 = new HeaderBand { RepeatOnEachPage = true };
      var header2 = new HeaderBand { RepeatOnEachPage = false };
      Assert.That(header1.Equals(header2), Is.False);
    }

    [Test]
    public void GetHashCode_ConsistentWithEquals()
    {
      var header1 = new HeaderBand { Height = 50, RepeatOnEachPage = true };
      var header2 = new HeaderBand { Height = 50, RepeatOnEachPage = true };

      if (header1.Equals(header2))
      {
        Assert.That(header1.GetHashCode(), Is.EqualTo(header2.GetHashCode()));
      }
    }
  }
}
