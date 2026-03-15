using FlowReports.Model;
using FlowReports.Model.ReportItems;

namespace FlowReports.UnitTests.Model
{
  public class ReportIntegrationTests
  {
    private Report _report;

    [SetUp]
    public void Setup()
    {
      _report = new Report();
    }

    [Test]
    public void Report_WithHeaderAndFooterBands_CanBeConstructed()
    {
      var band = _report.Bands.AddBand("Items");
      var headerBand = new HeaderBand { RepeatOnEachPage = true };
      var footerBand = new FooterBand();

      band.HeaderBand = headerBand;
      band.FooterBand = footerBand;

      Assert.That(band.HeaderBand, Is.EqualTo(headerBand));
      Assert.That(band.FooterBand, Is.EqualTo(footerBand));
      Assert.That(headerBand.RepeatOnEachPage, Is.True);
    }

    [Test]
    public void Report_WithNestedBands_CanBeConstructed()
    {
      var mainBand = _report.Bands.AddBand("Items");
      var subBand1 = mainBand.Bands.AddBand("SubItems1");
      var subBand2 = mainBand.Bands.AddBand("SubItems2");

      Assert.That(mainBand.Bands.Count(), Is.EqualTo(2));
      Assert.That(mainBand.Bands, Contains.Item(subBand1).And.Contains(subBand2));
    }

    [Test]
    public void Report_WithComplexStructure_AllPropertiesAreAccessible()
    {
      var band = _report.Bands.AddBand("Items");
      band.Height = 100;
      
      var textItem = new TextItem { DataSource = "Name", Format = "g" };
      var boolItem = new BooleanItem { DataSource = "IsActive" };
      
      band.AddReportItem(textItem);
      band.AddReportItem(boolItem);

      Assert.That(band.Items, Has.Count.EqualTo(2));
      Assert.That(band.Height, Is.EqualTo(100));
      Assert.That(band.DataSource, Is.EqualTo("Items"));
    }

    [Test]
    public void Report_BandRemoval_EventsAreRaised()
    {
      var band = _report.Bands.AddBand();
      var removedEventRaised = false;
      
      _report.Bands.SubBandRemoved += (s, e) => removedEventRaised = true;
      _report.Bands.RemoveBand(band);

      Assert.That(removedEventRaised, Is.True);
    }

    [Test]
    public void Report_BandMovement_OrderingIsPreserved()
    {
      var band1 = _report.Bands.AddBand("Band1");
      var band2 = _report.Bands.AddBand("Band2");
      var band3 = _report.Bands.AddBand("Band3");
      var band4 = _report.Bands.AddBand("Band4");

      _report.Bands.MoveBandDown(band1);
      _report.Bands.MoveBandUp(band4);

      var bandsArray = new List<ReportBand>(_report.Bands).ToArray();
      Assert.That(bandsArray[0], Is.EqualTo(band2));
      Assert.That(bandsArray[1], Is.EqualTo(band1));
      Assert.That(bandsArray[2], Is.EqualTo(band4));
      Assert.That(bandsArray[3], Is.EqualTo(band3));
    }

    [Test]
    public void BandWithoutDataSource_IsDrawnOnce()
    {
      var band = _report.Bands.AddBand(dataSource: null);
      Assert.That(band.DataSource, Is.Null);
      Assert.That(string.IsNullOrWhiteSpace(band.DataSource), Is.True);
    }

    [Test]
    public void BandWithDataSource_CanBeSet()
    {
      var band = _report.Bands.AddBand();
      band.DataSource = "Customers";
      Assert.That(band.DataSource, Is.EqualTo("Customers"));
    }

    [Test]
    public void MultipleItemsInBand_CanBeManipulated()
    {
      var band = _report.Bands.AddBand();
      
      var item1 = new TextItem { Left = 0, Top = 0, Width = 100, Height = 20 };
      var item2 = new TextItem { Left = 100, Top = 0, Width = 100, Height = 20 };
      var item3 = new TextItem { Left = 0, Top = 20, Width = 200, Height = 20 };

      band.AddReportItem(item1);
      band.AddReportItem(item2);
      band.AddReportItem(item3);

      Assert.That(band.Items, Has.Count.EqualTo(3));
      
      band.RemoveItem(item2);
      Assert.That(band.Items, Has.Count.EqualTo(2));
      Assert.That(band.Items, Contains.Item(item1).And.Contains(item3));
    }

    [Test]
    public void BandWithNullHeight_ActualHeightCalculatesFromItems()
    {
      var band = _report.Bands.AddBand();
      band.Height = null;

      var item1 = new TextItem { Top = 0, Height = 25 };
      var item2 = new TextItem { Top = 30, Height = 35 };

      band.AddReportItem(item1);
      band.AddReportItem(item2);

      Assert.That(band.ActualHeight, Is.EqualTo(65));
    }

    [Test]
    public void BandWithExplicitHeight_ActualHeightUsesExplicitValue()
    {
      var band = _report.Bands.AddBand();
      band.Height = 200;

      var item = new TextItem { Top = 0, Height = 500 };
      band.AddReportItem(item);

      Assert.That(band.ActualHeight, Is.EqualTo(200));
    }

    [Test]
    public void BandWithItems_EventsAreFiredCorrectly()
    {
      var band = _report.Bands.AddBand();
      var addedCount = 0;
      var removedCount = 0;

      band.ItemAdded += (s, e) => addedCount++;
      band.ItemRemoved += (s, e) => removedCount++;

      var item1 = new TextItem();
      var item2 = new BooleanItem();

      band.AddReportItem(item1);
      band.AddReportItem(item2);
      band.RemoveItem(item1);

      Assert.That(addedCount, Is.EqualTo(2));
      Assert.That(removedCount, Is.EqualTo(1));
    }

    [Test]
    public void HeaderBand_RepeatOnEachPageProperty_ControlsRepeating()
    {
      var band = _report.Bands.AddBand();
      var headerBand = new HeaderBand();

      headerBand.RepeatOnEachPage = false;
      band.HeaderBand = headerBand;
      Assert.That(band.HeaderBand.RepeatOnEachPage, Is.False);

      headerBand.RepeatOnEachPage = true;
      Assert.That(band.HeaderBand.RepeatOnEachPage, Is.True);
    }

    [Test]
    public void Report_CanContainMultipleBandSequences()
    {
      var band1 = _report.Bands.AddBand("Customers");
      var band2 = _report.Bands.AddBand("Orders");
      var band3 = _report.Bands.AddBand("Items");

      band1.AddTextItem();
      band2.AddTextItem();
      band3.AddTextItem();

      Assert.That(_report.Bands.Count(), Is.EqualTo(3));
      var allBands = new List<ReportBand>(_report.Bands);
      Assert.That(allBands[0].DataSource, Is.EqualTo("Customers"));
      Assert.That(allBands[1].DataSource, Is.EqualTo("Orders"));
      Assert.That(allBands[2].DataSource, Is.EqualTo("Items"));
    }

    [Test]
    public void Report_BandInsertion_MaintainsOrder()
    {
      var band1 = _report.Bands.AddBand();
      var band3 = _report.Bands.AddBand();
      var band2 = _report.Bands.AddBand(band1, InsertLocation.After);

      var bandsArray = new List<ReportBand>(_report.Bands).ToArray();
      Assert.That(bandsArray[0], Is.EqualTo(band1));
      Assert.That(bandsArray[1], Is.EqualTo(band2));
      Assert.That(bandsArray[2], Is.EqualTo(band3));
    }
  }
}
