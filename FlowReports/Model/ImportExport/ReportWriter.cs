﻿using System.IO;
using System.Text;
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
      var settings = GetXMLSettings();

      using var writer = XmlWriter.Create(fileName, settings);
      xml.Save(writer);
    }

    public static void Write(Report report, Stream stream)
    {
      var xml = PrepareDocument();
      WriteReport(report, xml);
      var settings = GetXMLSettings();
      using var writer = XmlWriter.Create(stream, settings);
      xml.Save(writer);
    }

    private static XmlWriterSettings GetXMLSettings()
    {
      return new XmlWriterSettings
      {
        Indent = true,
        IndentChars = "  ",
        NewLineChars = "\r\n",
        NewLineHandling = NewLineHandling.Replace
      };
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

    public static string GetXMLRepresentation(IEnumerable<ReportItem> items)
    {
      if (items == null)
      {
        return string.Empty;
      }

      var doc = PrepareDocument();
      var itemsNode = doc.AppendChild(Tags.Items);

      foreach (var item in items)
      {
        WriteItem(item, itemsNode);
      }

      var sb = new StringBuilder();
      var settings = GetXMLSettings();

      using (var writer = XmlWriter.Create(sb, settings))
      {
        doc.Save(writer);
      }

      return sb.ToString();
    }

    private static void WriteItem(ReportItem item, XmlNode itemsNode)
    {
      var itemNode = itemsNode.AppendChild(Tags.Item);

      switch (item)
      {
        case TextItem textBoxItem:
          WriteTextItem(textBoxItem, itemNode);
          break;
        case BooleanItem booleanItem:
          WriteBooleanItem(booleanItem, itemNode);
          break;
        case ImageItem imageItem:
          WriteImageItem(imageItem, itemNode);
          break;
        default:
          throw new NotImplementedException("Unknown ReportItem type.");
      }
    }

    private static void WriteTextItem(TextItem item, XmlElement node)
    {
      node.WriteAttribute(Tags.Type, Tags.TextItem);
      WriteReportItem(item, node);
      node.WriteAttribute(Tags.Format, item.Format);
    }

    private static void WriteBooleanItem(BooleanItem item, XmlElement node)
    {
      node.WriteAttribute(Tags.Type, Tags.BooleanItem);
      WriteReportItem(item, node);
    }

    private static void WriteImageItem(ImageItem item, XmlElement node)
    {
      node.WriteAttribute(Tags.Type, Tags.ImageItem);
      WriteReportItem(item, node);
    }

    private static void WriteReportItem(ReportItem item, XmlElement node)
    {
      WriteReportElement(item, node);
      node.WriteAttribute(Tags.X, item.Left);
      node.WriteAttribute(Tags.Y, item.Top);
      node.WriteAttribute(Tags.Width, item.Width);
      node.WriteAttribute(Tags.Height, item.Height);
      node.WriteAttribute(Tags.DataSource, item.DataSource);
    }

    private static void WriteReportElement(ReportElement element, XmlElement node)
    {
      node.WriteAttribute(Tags.ID, element.ID);
    }
  }
}
