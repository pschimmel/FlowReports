using System.IO;
using System.Xml;
using FlowReports.Model.ReportItems;

namespace FlowReports.Model.ImportExport
{
  /// <summary>
  /// Provides functionality to read and deserialize report data from XML files and streams.
  /// </summary>
  public static class ReportReader
  {
    /// <summary>
    /// Reads a report from an XML file at the specified file path.
    /// </summary>
    public static Report Read(string filePath)
    {
      var xml = PrepareDocument();
      xml.Load(filePath);
      Report report = new();
      report.ReadReport(xml);
      report.FilePath = filePath;
      return report;
    }

    /// <summary>
    /// Reads a report from an XML stream.
    /// </summary>
    public static Report Read(Stream stream)
    {
      var xml = PrepareDocument();
      xml.Load(stream);
      Report report = new();
      report.ReadReport(xml);
      return report;
    }

    /// <summary>
    /// Creates and returns a new, empty <see cref="XmlDocument"/> instance.
    /// </summary>
    private static XmlDocument PrepareDocument()
    {
      return new XmlDocument();
    }

    /// <summary>
    /// Reads report data from an XML document and populates the report object.
    /// </summary>
    private static void ReadReport(this Report report, XmlDocument doc)
    {
      var reportNode = doc.DocumentElement; // DocumentElement will be the root element, i.e. the report
      report.LastChanged = reportNode.ReadAttributeOrDefault(Tags.LastChanged, DateTime.Now);
      ReadBands(report, reportNode);
    }

    /// <summary>
    /// Reads all band elements from the parent XML node and adds them to the band owner.
    /// </summary>
    private static void ReadBands(IHasBands bandOwner, XmlElement parentNode)
    {
      foreach (var bandsNode in parentNode.SelectNodes(Tags.Bands).OfType<XmlElement>())
      {
        foreach (var bandNode in bandsNode.SelectNodes(Tags.Band).OfType<XmlElement>())
        {
          ReadBand(bandOwner, bandNode);
        }
      }
    }

    /// <summary>
    /// Reads a single band element from XML and adds it to the band owner, including its items and child bands.
    /// </summary>
    private static void ReadBand(IHasBands bandOwner, XmlElement bandNode)
    {
      var band = bandOwner.Bands.AddBand();
      ReadReportElement(band, bandNode);
      band.DataSource = bandNode.ReadAttributeOrDefault(Tags.DataSource, default(string));
      band.Height = bandNode.ReadAttributeOrDefault<double?>(Tags.Height, null);

      // Read filtering if present
      if (bandNode.SelectSingleNode(Tags.Filtering) is XmlElement filteringNode)
      {
        foreach (var filterNode in filteringNode.SelectNodes(Tags.Filter).OfType<XmlElement>())
        {
          var expression = filterNode.ReadAttributeOrDefault(Tags.Expression, string.Empty);
          if (!string.IsNullOrWhiteSpace(expression))
          {
            band.FilterExpression = new Filtering.FilterExpression(expression);
          }
        }
      }

      // Read ordering if present
      if (bandNode.SelectSingleNode(Tags.Ordering) is XmlElement orderingNode)
      {
        foreach (var orderNode in orderingNode.SelectNodes(Tags.Order).OfType<XmlElement>())
        {
          var prop = orderNode.ReadAttributeOrDefault(Tags.Property, string.Empty);
          var dirStr = orderNode.ReadAttributeOrDefault(Tags.Direction, string.Empty);
          if (!string.IsNullOrWhiteSpace(prop))
          {
            if (Enum.TryParse<SortDirection>(dirStr, out var dir))
            {
              band.Ordering.Add(new SortDescriptor { Property = prop, Direction = dir });
            }
            else
            {
              band.Ordering.Add(new SortDescriptor { Property = prop, Direction = SortDirection.Ascending });
            }
          }
        }
      }

      // Read header and footer nodes
      ReadHeaderNode(bandNode, band);
      ReadFooterNode(bandNode, band);

      // Read all items on the band
      ReadItems(bandNode, band);

      // Recursively read child bands
      ReadBands(band, bandNode);
    }

    /// <summary>
    /// Reads the header band from the given XML node and adds it to the specified band, including its items.
    /// </summary>
    private static void ReadHeaderNode(XmlElement bandNode, ReportBand band)
    {
      if (bandNode.SelectSingleNode(Tags.Header) is XmlElement headerNode)
      {
        band.HeaderBand = new HeaderBand();
        band.HeaderBand.Height = headerNode.ReadAttributeOrDefault<double?>(Tags.Height, null);
        ReadItems(headerNode, band.HeaderBand);
      }
    }

    /// <summary>
    /// Reads the footer band from the given XML node and adds it to the specified band, including its items.
    /// </summary>
    private static void ReadFooterNode(XmlElement bandNode, ReportBand band)
    {
      if (bandNode.SelectSingleNode(Tags.Footer) is XmlElement footerNode)
      {
        band.FooterBand = new FooterBand();
        band.FooterBand.Height = footerNode.ReadAttributeOrDefault<double?>(Tags.Height, null);
        ReadItems(footerNode, band.FooterBand);
      }
    }

    /// <summary>
    /// Reads all item elements from the given band XML node and adds them to the specified band. 
    /// </summary>
    private static void ReadItems(XmlElement bandNode, ReportBandBase band)
    {
      foreach (var itemsNode in bandNode.SelectNodes(Tags.Items).OfType<XmlElement>())
      {
        foreach (var itemNode in itemsNode.SelectNodes(Tags.Item).OfType<XmlElement>())
        {
          ReadItem(band.Items, itemNode);
        }
      }
    }

    /// <summary>
    /// Parses a string of XML and extracts all report items from it.
    /// </summary>
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

    /// <summary>
    /// Reads a single report item from an XML element and adds it to the items collection.
    /// </summary>
    /// <exception cref="Exception">Thrown when the item type is not recognized.</exception>
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

    /// <summary>
    /// Attempts to read and parse a text item from the given XML element.
    /// </summary>
    /// <returns>True if the element was successfully parsed as a text item; otherwise, false.</returns>
    private static bool TryReadTextItem(XmlElement node, out TextItem item)
    {
      item = null;

      if (node.HasAttribute(Tags.Type) && node.GetAttribute(Tags.Type) == Tags.TextItem)
      {
        item = new TextItem();
        ReadReportItem(item, node);
        item.Format = node.ReadAttributeOrDefault(Tags.Format, string.Empty);
        item.Bold = node.ReadAttributeOrDefault(Tags.Bold, false);
        item.Italic = node.ReadAttributeOrDefault(Tags.Italic, false);
        item.Underline = node.ReadAttributeOrDefault(Tags.Underline, false);
        return true;
      }

      return false;
    }

    /// <summary>
    /// Attempts to read and parse a boolean item from the given XML element.
    /// </summary>
    /// <returns>True if the element was successfully parsed as a boolean item; otherwise, false.</returns>
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

    /// <summary>
    /// Attempts to read and parse an image item from the given XML element.
    /// </summary>
    /// <returns>True if the element was successfully parsed as an image item; otherwise, false.</returns>
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

    /// <summary>
    /// Reads common report item properties from an XML element and populates the report item object.
    /// </summary>
    private static void ReadReportItem(ReportItem item, XmlElement node)
    {
      ReadReportElement(item, node);
      item.Left = node.ReadAttributeOrDefault(Tags.X, item.DefaultX);
      item.Top = node.ReadAttributeOrDefault(Tags.Y, item.DefaultY);
      item.Width = node.ReadAttributeOrDefault(Tags.Width, item.DefaultWidth);
      item.Height = node.ReadAttributeOrDefault(Tags.Height, item.DefaultHeight);
      item.DataSource = node.ReadAttributeOrDefault(Tags.DataSource, string.Empty);
    }

    /// <summary>
    /// Reads the basic report element properties from an XML element.
    /// </summary>
    private static void ReadReportElement(ReportElement element, XmlElement node)
    {
      element.ID = node.ReadAttributeOrDefault(Tags.ID, Guid.NewGuid());
    }
  }
}
