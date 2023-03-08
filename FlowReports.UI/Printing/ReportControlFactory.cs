using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using FlowReports.Model;
using FlowReports.Model.ReportItems;

namespace FlowReports.UI.Printing
{
  internal static class ReportControlFactory
  {
    public static UIElement CreateControl(ReportItem item, object data)
    {
      if (item is TextItem textItem)
      {
        var textBlock = new TextBlock
        {
          Text = ReplaceText(textItem, data),
          Width = textItem.Width,
          Height = textItem.Height,
          TextWrapping = TextWrapping.Wrap
        };

        return textBlock;
      }

      return null;
    }

    private static string ReplaceText(TextItem textItem, object itemData)
    {
      string result = textItem.DataSource;
      //  \(             # Escaped parenthesis, means "starts with a '(' character"
      //      (          # Parentheses in a regex mean "put (capture) the stuff in between into the Groups array"
      //         [^)]    # Any character that is not a ')' character
      //         *       # Zero or more occurrences of the aforementioned "non ')' char"
      //      )          # Close the capturing group
      //  \)             # "Ends with a ')' character"
      string regex= $"\\{Settings.DATASOURCE_OPENING_BRACKET}([^{Settings.DATASOURCE_CLOSING_BRACKET}]*)\\{Settings.DATASOURCE_CLOSING_BRACKET}";
      var matches = Regex.Matches(textItem.DataSource, regex);

      foreach (var match in matches.OfType<Match>())
      {
        string token = match.Groups[1].Value;
        object value = itemData.GetType().GetProperty(token).GetValue(itemData);
        string itemValue = GetValueAsString(value, textItem.Format);
        result = result.Replace($"{Settings.DATASOURCE_OPENING_BRACKET}{token}{Settings.DATASOURCE_CLOSING_BRACKET}", itemValue);
      }

      return result;
    }

    private static string GetValueAsString(object value, string format)
    {
      return value switch
      {
        null => null,
        double valueAsDouble => valueAsDouble.ToString(format),
        int valueAsInt => valueAsInt.ToString(format),
        DateTime valueAsDateTime => valueAsDateTime.ToString(format),
        _ => value.ToString(),
      };
    }
  }
}
