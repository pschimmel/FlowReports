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

      foreach (var bandsNode in reportNode.GetElementsByTagName(Tags.Bands).OfType<XmlElement>())
      {
        foreach (var bandNode in bandsNode.GetElementsByTagName(Tags.Band).OfType<XmlElement>())
        {
          ReadBand(report, bandNode);
        }
      }
    }

    private static void ReadBand(Report report, XmlElement bandNode)
    {
      var band = report.Bands.AddBand();
      ReadReportElement(band, bandNode);
      band.DataSource = bandNode.ReadAttributeOrDefault(Tags.DataSource, default(string));

      foreach (var itemsNode in bandNode.GetElementsByTagName(Tags.Items).OfType<XmlElement>())
      {
        foreach (var itemNode in itemsNode.GetElementsByTagName(Tags.Item).OfType<XmlElement>())
        {
          ReadItem(band, itemNode);
        }
      }
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
      item.Left = node.ReadAttributeOrDefault(Tags.X, ReportItem.DefaultX);
      item.Top = node.ReadAttributeOrDefault(Tags.Y, ReportItem.DefaultY);
      item.Width = node.ReadAttributeOrDefault(Tags.Width, ReportItem.DefaultWidth);
      item.Height = node.ReadAttributeOrDefault(Tags.Height, ReportItem.DefaultHeight);
    }

    private static void ReadReportElement(ReportElement element, XmlElement node)
    {
      element.ID = node.ReadAttributeOrDefault(Tags.ID, Guid.NewGuid());
    }
  }
}
