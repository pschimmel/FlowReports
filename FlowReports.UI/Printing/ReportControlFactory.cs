using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using FlowReports.Model;
using FlowReports.Model.ReportItems;

namespace FlowReports.UI.Printing
{
  internal class ReportControlFactory
  {
    static readonly Lazy<ReportControlFactory> _lazyInstance = new(()=> new ReportControlFactory());

    private ReportControlFactory()
    { }

    public static ReportControlFactory Instance => _lazyInstance.Value;

    public UIElement CreateControl(ReportItem item, object data)
    {
      if (item is TextItem textItem)
      {
        var textBlock = new TextBlock
        {
          Text = ReplaceText(textItem.Text, data),
          Width = textItem.Width,
          Height = textItem.Height,
          TextWrapping = TextWrapping.Wrap
        };

        return textBlock;
      }

      return null;
    }

    private static string ReplaceText(string text, object itemData)
    {
      string result = text;
      //  \(             # Escaped parenthesis, means "starts with a '(' character"
      //      (          # Parentheses in a regex mean "put (capture) the stuff in between into the Groups array"
      //         [^)]    # Any character that is not a ')' character
      //         *       # Zero or more occurrences of the aforementioned "non ')' char"
      //      )          # Close the capturing group
      //  \)             # "Ends with a ')' character"
      string regex= $"\\{Settings.DATASOURCE_OPENING_BRACKET}([^{Settings.DATASOURCE_CLOSING_BRACKET}]*)\\{Settings.DATASOURCE_CLOSING_BRACKET}";
      var matches = Regex.Matches(text, regex);

      foreach (var match in matches.OfType<Match>())
      {
        string token = match.Groups[1].Value;
        string itemValue = itemData.GetType().GetProperty(token).GetValue(itemData).ToString();
        result = result.Replace($"{Settings.DATASOURCE_OPENING_BRACKET}{token}{Settings.DATASOURCE_CLOSING_BRACKET}", itemValue);
      }

      return result;
    }
  }
}
