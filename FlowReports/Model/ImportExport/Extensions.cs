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

    public static T ReadAttributeOrDefault<T>(this XmlElement parent, string name, T defaultValue)
    {
      switch (Type.GetTypeCode(typeof(T)))
      {
        case TypeCode.String:
          return TryGetStringAttribute(parent, name, out string stringResult)
            ? (T)(object)stringResult
            : defaultValue;
        case TypeCode.DateTime:
          return TryGetDateTimeAttribute(parent, name, out DateTime dateTimeResult)
            ? (T)(object)dateTimeResult
            : defaultValue;
        case TypeCode.Double:
          return TryGetDoubleAttribute(parent, name, out double doubleResult)
            ? (T)(object)doubleResult
            : defaultValue;
        case TypeCode.Object:
          if (typeof(T) == typeof(Guid))
          {
            return TryGetGuidAttribute(parent, name, out Guid guidResult)
             ? (T)(object)guidResult
             : defaultValue;
          }
          break;
      }

      throw new NotImplementedException("Unknown type.");
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
