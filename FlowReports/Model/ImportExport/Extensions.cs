using System.Diagnostics;
using System.Globalization;
using System.Xml;

namespace FlowReports.Model.ImportExport
{
  internal static class Extensions
  {
    public static XmlElement AppendChild(this XmlNode parent, string name)
    {
      XmlDocument doc = parent is XmlDocument document ? document : parent.OwnerDocument;
      Debug.Assert(doc != null);
      var child = doc.CreateElement(name);
      parent.AppendChild(child);
      return child;
    }

    public static void WriteAttribute<T>(this XmlElement parent, string name, T value)
    {
      // Don't write empty attributes
      if (value == null)
      {
        return;
      }

      switch (value)
      {
        case string stringValue:
          if (string.IsNullOrEmpty(stringValue))
          {
            break;
          }

          parent.SetAttribute(name, stringValue);
          break;
        case DateTime dateTimeValue:
          parent.SetAttribute(name, dateTimeValue.ToString("O", CultureInfo.InvariantCulture));
          break;
        case Guid guidValue:
          parent.SetAttribute(name, guidValue.ToString("N", CultureInfo.InvariantCulture));
          break;
        case double doubleValue:
          parent.SetAttribute(name, doubleValue.ToString("G17", CultureInfo.InvariantCulture));
          break;
        default:
          throw new NotImplementedException("Unknown type.");
      }
    }

    /// <summary>
    /// Reads an XML attribute and returns its value as the specified type, or the default value if the attribute doesn't exist or cannot be parsed.
    /// </summary>
    public static T ReadAttributeOrDefault<T>(this XmlElement parent, string name, T defaultValue)
    {
      return typeof(T) switch
      {
        var t when t == typeof(string) => TryGetStringAttribute(parent, name, out string stringResult)
          ? (T)(object)stringResult
          : defaultValue,
        var t when t == typeof(DateTime) || t == typeof(DateTime?) => TryGetDateTimeAttribute(parent, name, out DateTime dateTimeResult)
          ? (T)(object)dateTimeResult
          : defaultValue,
        var t when t == typeof(double) || t == typeof(double?) => TryGetDoubleAttribute(parent, name, out double doubleResult)
          ? (T)(object)doubleResult
          : defaultValue,
        var t when t == typeof(int) || t == typeof(int?) => TryGetIntAttribute(parent, name, out int intResult)
          ? (T)(object)intResult
          : defaultValue,
        var t when t == typeof(Guid) => TryGetGuidAttribute(parent, name, out Guid guidResult)
          ? (T)(object)guidResult
          : defaultValue,
        _ => throw new NotImplementedException($"Unsupported type: {typeof(T).Name}")
      };
    }

    public static bool TryGetStringAttribute(this XmlElement parent, string name, out string result)
    {
      result = null;

      var attribute = parent.Attributes[name];
      if (attribute != null)
      {
        result = attribute.InnerText;
        return true;
      }

      return false;
    }

    public static bool TryGetDoubleAttribute(this XmlElement parent, string name, out double result)
    {
      result = default;
      var attribute = parent.Attributes[name];
      if (attribute != null)
      {
        if (double.TryParse(attribute.InnerText, NumberStyles.Any, CultureInfo.InvariantCulture, out double r))
        {
          result = r;
          return true;
        }
      }

      return false;
    }

    public static bool TryGetIntAttribute(this XmlElement parent, string name, out int result)
    {
      result = default;
      var attribute = parent.Attributes[name];
      if (attribute != null)
      {
        if (int.TryParse(attribute.InnerText, NumberStyles.Any, CultureInfo.InvariantCulture, out int r))
        {
          result = r;
          return true;
        }
      }
      return false;
    }

    public static bool TryGetDateTimeAttribute(this XmlElement parent, string name, out DateTime result)
    {
      result = DateTime.Now;
      var attribute = parent.Attributes[name];
      if (attribute != null)
      {
        if (DateTime.TryParse(attribute.InnerText, null, DateTimeStyles.RoundtripKind, out DateTime r))
        {
          result = r;
          return true;
        }
      }

      return false;
    }

    public static bool TryGetGuidAttribute(this XmlElement parent, string name, out Guid result)
    {
      result = Guid.Empty;
      var attribute = parent.Attributes[name];
      if (attribute != null)
      {
        if (Guid.TryParse(attribute.InnerText, out Guid r))
        {
          result = r;
          return true;
        }
      }

      return false;
    }
  }
}
