using System.IO;
using System.Xml;
using FlowReports.Model.ReportItems;

namespace FlowReports.Model.ImportExport
{
  public static class ReportReader
  {
    public static Report Read(string filePath)
    {
      var xml = PrepareDocument();
      xml.Load(filePath);
      Report report = new();
      report.ReadReport(xml);
      report.FilePath = filePath;
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
      return new XmlDocument();
    }

    private static void ReadReport(this Report report, XmlDocument doc)
    {
      var reportNode = doc.DocumentElement; // DocumentElement will be the root element, i.e. the report
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
          ReadItem(band.Items, itemNode);
        }
      }

      ReadBands(band.SubBands, bandNode);
    }

    public static IEnumerable<ReportItem> GetItems(string xml)
    {
      var items = new List<ReportItem>();

      if (string.IsNullOrWhiteSpace(xml))
      {
        return Enumerable.Empty<ReportItem>();
      }

      var doc = PrepareDocument();
      doc.LoadXml(xml);
      var itemsNode = doc.DocumentElement;

      foreach (var itemNode in itemsNode.SelectNodes(Tags.Item).OfType<XmlElement>())
      {
        ReadItem(items, itemNode);
      }

      return items;
    }

    private static void ReadItem(List<ReportItem> items, XmlElement itemNode)
    {
      if (TryReadTextItem(itemNode, out TextItem textItem))
      {
        items.Add(textItem);
      }
      else if (TryReadBooleanItem(itemNode, out BooleanItem booleanItem))
      {
        items.Add(booleanItem);
      }
      else if (TryReadImageItem(itemNode, out ImageItem imageItem))
      {
        items.Add(imageItem);
      }
      else
      {
        throw new Exception("Cannot read element.");
      }
    }

    private static bool TryReadTextItem(XmlElement node, out TextItem item)
    {
      item = null;

      if (node.HasAttribute(Tags.Type) && node.GetAttribute(Tags.Type) == Tags.TextItem)
      {
        item = new TextItem();
        ReadReportItem(item, node);
        item.Format = node.ReadAttributeOrDefault(Tags.Format, string.Empty);
        return true;
      }

      return false;
    }

    private static bool TryReadBooleanItem(XmlElement node, out BooleanItem item)
    {
      item = null;

      if (node.HasAttribute(Tags.Type) && node.GetAttribute(Tags.Type) == Tags.BooleanItem)
      {
        item = new BooleanItem();
        ReadReportItem(item, node);
        return true;
      }

      return false;
    }

    private static bool TryReadImageItem(XmlElement node, out ImageItem item)
    {
      item = null;

      if (node.HasAttribute(Tags.Type) && node.GetAttribute(Tags.Type) == Tags.ImageItem)
      {
        item = new ImageItem();
        ReadReportItem(item, node);
        return true;
      }

      return false;
    }

    private static void ReadReportItem(ReportItem item, XmlElement node)
    {
      ReadReportElement(item, node);
      item.Left = node.ReadAttributeOrDefault(Tags.X, item.DefaultX);
      item.Top = node.ReadAttributeOrDefault(Tags.Y, item.DefaultY);
      item.Width = node.ReadAttributeOrDefault(Tags.Width, item.DefaultWidth);
      item.Height = node.ReadAttributeOrDefault(Tags.Height, item.DefaultHeight);
      item.DataSource = node.ReadAttributeOrDefault(Tags.DataSource, string.Empty);
    }

    private static void ReadReportElement(ReportElement element, XmlElement node)
    {
      element.ID = node.ReadAttributeOrDefault(Tags.ID, Guid.NewGuid());
    }
  }
}
