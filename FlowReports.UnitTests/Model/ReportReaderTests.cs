using System.IO;
using FlowReports.Model;
using FlowReports.Model.ImportExport;
using FlowReports.Model.ReportItems;

namespace FlowReports.UnitTests.Model
{
  public class ReportReaderTests
  {
    [Test]
    public void GetItems_ReturnsEmpty_For_NullOrWhitespace()
    {
      var items = ReportReader.GetItems(null);
      Assert.That(items, Is.Empty);

      items = ReportReader.GetItems(string.Empty);
      Assert.That(items, Is.Empty);
    }

    [Test]
    public void GetItems_Parses_Items_Written_By_GetXMLRepresentation()
    {
      var text = new TextItem
      {
        Format = "N2",
        Left = 10,
        Top = 5,
        Width = 50,
        Height = 20,
        DataSource = "FieldA"
      };

      var boolean = new BooleanItem
      {
        Left = 15,
        Top = 6,
        Width = 10,
        Height = 10,
        DataSource = "Flag"
      };

      var image = new ImageItem
      {
        Left = 5,
        Top = 3,
        Width = 30,
        Height = 30,
        DataSource = "ImageField"
      };

      var xml = ReportWriter.GetXMLRepresentation(new ReportItem[] { text, boolean, image });

      var parsed = ReportReader.GetItems(xml).ToList();
      Assert.That(parsed, Has.Count.EqualTo(3));

      Assert.That(parsed[0], Is.TypeOf<TextItem>());
      var parsedText = (TextItem)parsed[0];
      using (Assert.EnterMultipleScope())
      {
        Assert.That(parsedText.Format, Is.EqualTo("N2"));
        Assert.That(parsedText.DataSource, Is.EqualTo("FieldA"));
        Assert.That(parsedText.Left, Is.EqualTo(10));

        Assert.That(parsed[1], Is.TypeOf<BooleanItem>());
      }
      var parsedBool = (BooleanItem)parsed[1];
      using (Assert.EnterMultipleScope())
      {
        Assert.That(parsedBool.DataSource, Is.EqualTo("Flag"));

        Assert.That(parsed[2], Is.TypeOf<ImageItem>());
      }
      var parsedImage = (ImageItem)parsed[2];
      Assert.That(parsedImage.DataSource, Is.EqualTo("ImageField"));
    }

    [Test]
    public void Write_Report_To_Stream_And_Read_Back_Preserves_Bands_And_Items()
    {
      var report = new Report();
      var band = report.Bands.AddBand("TopDataSource");

      var text = new TextItem
      {
        Format = "C",
        Left = 1,
        Top = 2,
        Width = 20,
        Height = 10,
        DataSource = "Amount"
      };

      var boolean = new BooleanItem { Left = 2, DataSource = "Visible" };
      band.AddReportItem(text);
      band.AddReportItem(boolean);

      using var stream = new MemoryStream();
      ReportWriter.Write(report, stream);
      stream.Position = 0;

      var read = ReportReader.Read(stream);
      Assert.That(read, Is.Not.Null);
      Assert.That(read.Bands, Is.Not.Null);
      Assert.That(read.Bands.Count(), Is.EqualTo(1));

      var readBand = read.Bands.First();
      Assert.That(readBand.Items, Has.Count.EqualTo(2));
      Assert.That(readBand.Items[0], Is.TypeOf<TextItem>());
      var readText = (TextItem)readBand.Items[0];
      using (Assert.EnterMultipleScope())
      {
        Assert.That(readText.Format, Is.EqualTo("C"));
        Assert.That(readText.DataSource, Is.EqualTo("Amount"));
      }
    }
  }
}
