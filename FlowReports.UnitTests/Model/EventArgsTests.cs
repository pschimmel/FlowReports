using FlowReports.Model.Events;
using FlowReports.Model.ReportItems;

namespace FlowReports.UnitTests.Model
{
  public class EventArgsTests
  {
    [Test]
    public void BandsEventArgs_Constructor_StoresIndexAndBand()
    {
      var band = new ReportBand();
      var index = 5;

      var eventArgs = new BandsEventArgs(index, band);

      Assert.That(eventArgs.Item.Index, Is.EqualTo(index));
      Assert.That(eventArgs.Item.Band, Is.EqualTo(band));
    }

    [Test]
    public void BandsEventArgs_Item_IsReadOnly()
    {
      var band = new ReportBand();
      var eventArgs = new BandsEventArgs(0, band);

      // Item is a value type tuple, not null
      Assert.That(eventArgs.Item.Band, Is.EqualTo(band));
    }

    [Test]
    public void BandsEventArgs_InheritesFromGenericEventArgs()
    {
      var band = new ReportBand();
      var eventArgs = new BandsEventArgs(0, band);

      Assert.That(eventArgs, Is.InstanceOf<GenericEventArgs<(int, ReportBand)>>());
    }

    [Test]
    public void BandsEventArgs_DifferentIndices_AreStored()
    {
      var band = new ReportBand();
      var eventArgs1 = new BandsEventArgs(0, band);
      var eventArgs2 = new BandsEventArgs(10, band);

      Assert.That(eventArgs1.Item.Index, Is.Not.EqualTo(eventArgs2.Item.Index));
    }

    [Test]
    public void BandsEventArgs_WithNegativeIndex_IsStored()
    {
      var band = new ReportBand();
      var eventArgs = new BandsEventArgs(-1, band);

      Assert.That(eventArgs.Item.Index, Is.EqualTo(-1));
    }

    [Test]
    public void ReportItemsEventArgs_Constructor_StoresItem()
    {
      var item = new TextItem();

      var eventArgs = new ReportItemsEventArgs(item);

      Assert.That(eventArgs.Item, Is.EqualTo(item));
    }

    [Test]
    public void ReportItemsEventArgs_WithDifferentItems_StoresCorrectly()
    {
      var textItem = new TextItem();
      var boolItem = new BooleanItem();

      var textEventArgs = new ReportItemsEventArgs(textItem);
      var boolEventArgs = new ReportItemsEventArgs(boolItem);

      Assert.That(textEventArgs.Item, Is.EqualTo(textItem));
      Assert.That(boolEventArgs.Item, Is.EqualTo(boolItem));
      Assert.That(textEventArgs.Item, Is.Not.EqualTo(boolEventArgs.Item));
    }

    [Test]
    public void ReportItemsEventArgs_InheritesFromGenericEventArgs()
    {
      var item = new TextItem();
      var eventArgs = new ReportItemsEventArgs(item);

      Assert.That(eventArgs, Is.InstanceOf<GenericEventArgs<ReportItem>>());
    }

    [Test]
    public void ReportItemsEventArgs_WithNullItem_StoresNull()
    {
      var eventArgs = new ReportItemsEventArgs(null);

      Assert.That(eventArgs.Item, Is.Null);
    }

    [Test]
    public void HeaderEventArgs_Constructor_StoresHeader()
    {
      var header = new HeaderBand();

      var eventArgs = new HeaderEventArgs(header);

      Assert.That(eventArgs.Item, Is.EqualTo(header));
    }

    [Test]
    public void HeaderEventArgs_WithDifferentHeaders_StoresCorrectly()
    {
      var header1 = new HeaderBand { RepeatOnEachPage = true };
      var header2 = new HeaderBand { RepeatOnEachPage = false };

      var eventArgs1 = new HeaderEventArgs(header1);
      var eventArgs2 = new HeaderEventArgs(header2);

      Assert.That(eventArgs1.Item, Is.EqualTo(header1));
      Assert.That(eventArgs2.Item, Is.EqualTo(header2));
      Assert.That(eventArgs1.Item.RepeatOnEachPage, Is.True);
      Assert.That(eventArgs2.Item.RepeatOnEachPage, Is.False);
    }

    [Test]
    public void HeaderEventArgs_InheritesFromGenericEventArgs()
    {
      var header = new HeaderBand();
      var eventArgs = new HeaderEventArgs(header);

      Assert.That(eventArgs, Is.InstanceOf<GenericEventArgs<HeaderBand>>());
    }

    [Test]
    public void FooterEventArgs_Constructor_StoresFooter()
    {
      var footer = new FooterBand();

      var eventArgs = new FooterEventArgs(footer);

      Assert.That(eventArgs.Item, Is.EqualTo(footer));
    }

    [Test]
    public void FooterEventArgs_WithDifferentFooters_StoresCorrectly()
    {
      var footer1 = new FooterBand { Height = 50 };
      var footer2 = new FooterBand { Height = 100 };

      var eventArgs1 = new FooterEventArgs(footer1);
      var eventArgs2 = new FooterEventArgs(footer2);

      Assert.That(eventArgs1.Item, Is.EqualTo(footer1));
      Assert.That(eventArgs2.Item, Is.EqualTo(footer2));
      Assert.That(eventArgs1.Item.Height, Is.EqualTo(50));
      Assert.That(eventArgs2.Item.Height, Is.EqualTo(100));
    }

    [Test]
    public void FooterEventArgs_InheritesFromGenericEventArgs()
    {
      var footer = new FooterBand();
      var eventArgs = new FooterEventArgs(footer);

      Assert.That(eventArgs, Is.InstanceOf<GenericEventArgs<FooterBand>>());
    }

    [Test]
    public void GenericEventArgs_IsAbstract()
    {
      var type = typeof(GenericEventArgs<>);
      Assert.That(type.IsAbstract, Is.True);
    }

    [Test]
    public void GenericEventArgs_InheritesFromEventArgs()
    {
      var item = new TextItem();
      var eventArgs = new ReportItemsEventArgs(item);

      Assert.That(eventArgs, Is.InstanceOf<EventArgs>());
    }

    [Test]
    public void EventArgs_CanBePassedToEventHandlers()
    {
      ReportItemsEventArgs capturedArgs = null;
      EventHandler<ReportItemsEventArgs> handler = (s, e) => capturedArgs = e;

      var item = new TextItem();
      var eventArgs = new ReportItemsEventArgs(item);

      handler(null, eventArgs);

      Assert.That(capturedArgs, Is.EqualTo(eventArgs));
      Assert.That(capturedArgs.Item, Is.EqualTo(item));
    }

    [Test]
    public void BandsEventArgs_WithZeroIndex_IsStored()
    {
      var band = new ReportBand();
      var eventArgs = new BandsEventArgs(0, band);

      Assert.That(eventArgs.Item.Index, Is.EqualTo(0));
    }
  }
}
