using System.IO;
using System.Xml;
using FlowReports.Model.ReportItems;

namespace FlowReports.Model.ImportExport
{
  public static class ReportWriter
  {
    public static void Write(Report report, string fileName)
    {
      var xml = PrepareDocument();
      WriteReport(report, xml);
      xml.Save(fileName);
    }

    public static void Write(Report report, Stream stream)
    {
      var xml = PrepareDocument();
      WriteReport(report, xml);
      xml.Save(stream);
    }

    private static XmlDocument PrepareDocument()
    {
      var xml = new XmlDocument();
      var declarationNode = xml.CreateXmlDeclaration("1.0", null, null);
      xml.AppendChild(declarationNode);

      return xml;
    }

    private static void WriteReport(Report report, XmlDocument xml)
    {
      report.LastChanged = DateTime.Now;

      var reportNode = xml.AppendChild(Tags.Report);
      reportNode.WriteAttribute(Tags.LastChanged, report.LastChanged);

      WriteBands(report.Bands, reportNode);
    }

    private static void WriteBands(ReportBandCollection bands, XmlElement reportNode)
    {
      if (bands.Any())
      {
        var bandsNode = reportNode.AppendChild(Tags.Bands);

        foreach (var band in bands)
        {
          WriteBand(band, bandsNode);
        }
      }
    }

    private static void WriteBand(ReportBand band, XmlElement bandsNode)
    {
      var bandNode = bandsNode.AppendChild(Tags.Band);
      WriteReportElement(band, bandNode);
      bandNode.WriteAttribute(Tags.DataSource, band.DataSource);

      if (band.Items.Any())
      {
        var itemsNode = bandNode.AppendChild(Tags.Items);
        foreach (var item in band.Items)
        {
          WriteItem(item, itemsNode);
        }
      }

      if (band.SubBands.Any())
      {
        WriteBands(band.SubBands, bandNode);
      }
    }

    private static void WriteItem(ReportItem item, XmlElement itemsNode)
    {
      var itemNode = itemsNode.AppendChild(Tags.Item);

      if (item is TextItem textBoxItem)
      {
        WriteTextItem(textBoxItem, itemNode);
      }
    }

    private static void WriteTextItem(TextItem item, XmlElement node)
    {
      WriteReportItem(item, node);
      node.WriteAttribute(Tags.Type, Tags.TextItem);
      node.WriteAttribute(Tags.Text, item.Text);
      node.WriteAttribute(Tags.Format, item.Format);
    }

    private static void WriteReportItem(ReportItem item, XmlElement node)
    {
      WriteReportElement(item, node);
      node.WriteAttribute(Tags.X, item.Left);
      node.WriteAttribute(Tags.Y, item.Top);
      node.WriteAttribute(Tags.Width, item.Width);
      node.WriteAttribute(Tags.Height, item.Height);
    }

    private static void WriteReportElement(ReportElement element, XmlElement node)
    {
      node.WriteAttribute(Tags.ID, element.ID);
    }
  }
}
