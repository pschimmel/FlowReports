using FlowReports.Model;
using FlowReports.Model.ReportItems;

namespace FlowReports.UnitTests.Model
{
  public class ReportTests
  {
    private Report _report;

    [SetUp]
    public void Setup()
    {
      _report = new Report();
    }

    [Test]
    public void Constructor_CreatesEmptyBandCollection()
    {
      Assert.That(_report.Bands, Is.Not.Null);
      Assert.That(_report.Bands, Is.Empty);
    }

    [Test]
    public void DataSource_DefaultValue_IsNull()
    {
      Assert.That(_report.DataSource, Is.Null);
    }

    [Test]
    public void Data_DefaultValue_IsNull()
    {
      Assert.That(_report.Data, Is.Null);
    }

    [Test]
    public void FilePath_CanBeSetAndRetrieved()
    {
      var filePath = "C:\\Reports\\MyReport.xml";
      _report.FilePath = filePath;
      Assert.That(_report.FilePath, Is.EqualTo(filePath));
    }

    [Test]
    public void Bands_AddBand_BandIsAdded()
    {
      var band = _report.Bands.AddBand();
      Assert.That(_report.Bands, Contains.Item(band));
      Assert.That(_report.Bands.Count(), Is.EqualTo(1));
    }

    [Test]
    public void Bands_AddMultipleBands_AllBandsAreAdded()
    {
      var band1 = _report.Bands.AddBand();
      var band2 = _report.Bands.AddBand();
      var band3 = _report.Bands.AddBand();

      using (Assert.EnterMultipleScope())
      {
        Assert.That(_report.Bands.Count(), Is.EqualTo(3));
        Assert.That(_report.Bands, Contains.Item(band1).And.Contains(band2).And.Contains(band3));
      }
    }

    [Test]
    public void Bands_AddBandWithDataSource_DataSourceIsSet()
    {
      var band = _report.Bands.AddBand("Items");
      Assert.That(band.DataSource, Is.EqualTo("Items"));
    }

    [Test]
    public void Bands_RemoveBand_BandIsRemoved()
    {
      var band = _report.Bands.AddBand();
      _report.Bands.RemoveBand(band);
      Assert.That(_report.Bands, Is.Empty);
    }

    [Test]
    public void Bands_RemoveBand_ReturnsIndex()
    {
      var band1 = _report.Bands.AddBand();
      var band2 = _report.Bands.AddBand();
      var band3 = _report.Bands.AddBand();

      var removedIndex = _report.Bands.RemoveBand(band2);
      Assert.That(removedIndex, Is.EqualTo(1));
    }

    [Test]
    public void Bands_MoveBandUp_BandPositionChanges()
    {
      var band1 = _report.Bands.AddBand();
      var band2 = _report.Bands.AddBand();
      var band3 = _report.Bands.AddBand();

      _report.Bands.MoveBandUp(band3);

      var bandsArray = new List<ReportBand>(_report.Bands).ToArray();
      using (Assert.EnterMultipleScope())
      {
        Assert.That(bandsArray[0], Is.EqualTo(band1));
        Assert.That(bandsArray[1], Is.EqualTo(band3));
        Assert.That(bandsArray[2], Is.EqualTo(band2));
      }
    }

    [Test]
    public void Bands_MoveBandDown_BandPositionChanges()
    {
      var band1 = _report.Bands.AddBand();
      var band2 = _report.Bands.AddBand();
      var band3 = _report.Bands.AddBand();

      _report.Bands.MoveBandDown(band1);

      var bandsArray = new List<ReportBand>(_report.Bands).ToArray();
      using (Assert.EnterMultipleScope())
      {
        Assert.That(bandsArray[0], Is.EqualTo(band2));
        Assert.That(bandsArray[1], Is.EqualTo(band1));
        Assert.That(bandsArray[2], Is.EqualTo(band3));
      }
    }

    [Test]
    public void Bands_CanMoveBandUp_FirstBand_ReturnsFalse()
    {
      var band1 = _report.Bands.AddBand();
      _report.Bands.AddBand();

      Assert.That(_report.Bands.CanMoveBandUp(band1), Is.False);
    }

    [Test]
    public void Bands_CanMoveBandUp_MiddleBand_ReturnsTrue()
    {
      _report.Bands.AddBand();
      var band2 = _report.Bands.AddBand();
      _report.Bands.AddBand();

      Assert.That(_report.Bands.CanMoveBandUp(band2), Is.True);
    }

    [Test]
    public void Bands_CanMoveBandDown_LastBand_ReturnsFalse()
    {
      _report.Bands.AddBand();
      var band2 = _report.Bands.AddBand();

      Assert.That(_report.Bands.CanMoveBandDown(band2), Is.False);
    }

    [Test]
    public void Bands_CanMoveBandDown_FirstBand_ReturnsTrue()
    {
      var band1 = _report.Bands.AddBand();
      _report.Bands.AddBand();

      Assert.That(_report.Bands.CanMoveBandDown(band1), Is.True);
    }

    [Test]
    public void Bands_Clear_AllBandsAreRemoved()
    {
      _report.Bands.AddBand();
      _report.Bands.AddBand();
      _report.Bands.AddBand();

      _report.Bands.Clear();

      Assert.That(_report.Bands, Is.Empty);
    }

    [Test]
    public void Bands_SubBandAdded_EventIsRaised()
    {
      var eventRaised = false;
      _report.Bands.SubBandAdded += (s, e) => eventRaised = true;

      _report.Bands.AddBand();

      Assert.That(eventRaised, Is.True);
    }

    [Test]
    public void Bands_SubBandRemoved_EventIsRaised()
    {
      var band = _report.Bands.AddBand();
      var eventRaised = false;
      _report.Bands.SubBandRemoved += (s, e) => eventRaised = true;

      _report.Bands.RemoveBand(band);

      Assert.That(eventRaised, Is.True);
    }

    [Test]
    public void Bands_Enumerate_ReturnsAllBands()
    {
      var band1 = _report.Bands.AddBand();
      var band2 = _report.Bands.AddBand();
      var band3 = _report.Bands.AddBand();

      var allBands = new List<ReportBand>(_report.Bands);

      Assert.That(allBands, Has.Count.EqualTo(3));
      Assert.That(allBands, Is.EqualTo(new[] { band1, band2, band3 }));
    }

    [Test]
    public void Bands_AddBandAfterAnotherBand_BandIsInsertedAtCorrectPosition()
    {
      var band1 = _report.Bands.AddBand();
      var band2 = _report.Bands.AddBand();
      var newBand = _report.Bands.AddBand(band1, InsertLocation.After);

      var bandsArray = new List<ReportBand>(_report.Bands).ToArray();
      using (Assert.EnterMultipleScope())
      {
        Assert.That(bandsArray[0], Is.EqualTo(band1));
        Assert.That(bandsArray[1], Is.EqualTo(newBand));
        Assert.That(bandsArray[2], Is.EqualTo(band2));
      }
    }

    [Test]
    public void Bands_AddBandBeforeAnotherBand_BandIsInsertedAtCorrectPosition()
    {
      var band1 = _report.Bands.AddBand();
      var band2 = _report.Bands.AddBand();
      var newBand = _report.Bands.AddBand(band2, InsertLocation.Before);

      var bandsArray = new List<ReportBand>(_report.Bands).ToArray();
      using (Assert.EnterMultipleScope())
      {
        Assert.That(bandsArray[0], Is.EqualTo(band1));
        Assert.That(bandsArray[1], Is.EqualTo(newBand));
        Assert.That(bandsArray[2], Is.EqualTo(band2));
      }
    }

    [Test]
    public void TypeOfData_CanBeSetAndRetrieved()
    {
      var dataType = typeof(string);
      _report.TypeOfData = dataType;
      Assert.That(_report.TypeOfData, Is.EqualTo(dataType));
    }

    [Test]
    public void LastChanged_CanBeSetAndRetrieved()
    {
      var now = DateTime.Now;
      _report.LastChanged = now;
      Assert.That(_report.LastChanged, Is.EqualTo(now));
    }
  }
}
