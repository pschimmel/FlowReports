using System.IO;
using System.Xml;
using FlowReports.Model.ReportItems;

namespace FlowReports.Model.ImportExport
{
  public static class ReportReader
  {
    public static Report Read(string fileName)
    {
      var xml = PrepareDocument();
      xml.Load(fileName);
      Report report = new();
      report.ReadReport(xml);
      return report;
    }

    public static Report Read(Stream stream)
    {
      var xml = PrepareDocument();
      xml.Load(stream);
      Report report = new();
      report.ReadReport(xml);
      return report;
    }

    private static XmlDocument PrepareDocument()
    {
      var xml = new XmlDocument();
      return xml;
    }

    private static void ReadReport(this Report report, XmlDocument xml)
    {
      var reportNode = xml.DocumentElement; // DocumentElement will be the root element, i.e. the report
      report.LastChanged = reportNode.ReadAttributeOrDefault(Tags.LastChanged, DateTime.Now);
      ReadBands(report.Bands, reportNode);
    }

    private static void ReadBands(ReportBandCollection bandCollection, XmlElement parentNode)
    {
      foreach (var bandsNode in parentNode.SelectNodes(Tags.Bands).OfType<XmlElement>())
      {
        foreach (var bandNode in bandsNode.SelectNodes(Tags.Band).OfType<XmlElement>())
        {
          ReadBand(bandCollection, bandNode);
        }
      }
    }

    private static void ReadBand(ReportBandCollection bandCollection, XmlElement bandNode)
    {
      var band = bandCollection.AddBand();
      ReadReportElement(band, bandNode);
      band.DataSource = bandNode.ReadAttributeOrDefault(Tags.DataSource, default(string));

      foreach (var itemsNode in bandNode.SelectNodes(Tags.Items).OfType<XmlElement>())
      {
        foreach (var itemNode in itemsNode.SelectNodes(Tags.Item).OfType<XmlElement>())
        {
          ReadItem(band, itemNode);
        }
      }

      ReadBands(band.SubBands, bandNode);
    }

    private static void ReadItem(ReportBand band, XmlElement itemNode)
    {
      ReportItem item = ReadTextItem(itemNode);

      if (item != null)
      {
        band.Items.Add(item);
      }
      else
      {
        throw new Exception("Cannot read element.");
      }
    }

    private static TextItem ReadTextItem(XmlElement node)
    {
      if (node.HasAttribute(Tags.Type) && node.GetAttribute(Tags.Type) == Tags.TextItem)
      {
        var item = new TextItem();
        ReadReportItem(item, node);
        item.Text = node.ReadAttributeOrDefault(Tags.Text, string.Empty);
        item.Format = node.ReadAttributeOrDefault(Tags.Format, string.Empty);
        return item;
      }

      return null;
    }

    private static void ReadReportItem(ReportItem item, XmlElement node)
    {
      ReadReportElement(item, node);
      item.Left = node.ReadAttributeOrDefault(Tags.X, item.DefaultX);
      item.Top = node.ReadAttributeOrDefault(Tags.Y, item.DefaultY);
      item.Width = node.ReadAttributeOrDefault(Tags.Width, item.DefaultWidth);
      item.Height = node.ReadAttributeOrDefault(Tags.Height, item.DefaultHeight);
    }

    private static void ReadReportElement(ReportElement element, XmlElement node)
    {
      element.ID = node.ReadAttributeOrDefault(Tags.ID, Guid.NewGuid());
    }
  }
}
