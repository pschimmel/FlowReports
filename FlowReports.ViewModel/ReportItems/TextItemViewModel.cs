using System.Text.RegularExpressions;
using FlowReports.Model;
using FlowReports.Model.ReportItems;

namespace FlowReports.ViewModel.ReportItems
{
  public class TextItemViewModel : ItemViewModel<TextItem>
  {
    public TextItemViewModel(TextItem item, object data, double deltaY)
      : base(item, data, deltaY)
    { }

    public string Format => _item.Format;

    public string Text => (string)GetValue();

    protected override object GetValue()
    {
      string result = _item.DataSource;
      //  \(             # Escaped parenthesis, means "starts with a '(' character"
      //      (          # Parentheses in a regex mean "put (capture) the stuff in between into the Groups array"
      //         [^)]    # Any character that is not a ')' character
      //         *       # Zero or more occurrences of the aforementioned "non ')' char"
      //      )          # Close the capturing group
      //  \)             # "Ends with a ')' character"
      string regex= $"\\{Settings.DATASOURCE_OPENING_BRACKET}([^{Settings.DATASOURCE_CLOSING_BRACKET}]*)\\{Settings.DATASOURCE_CLOSING_BRACKET}";
      var matches = Regex.Matches(result, regex);

      foreach (var match in matches.OfType<Match>())
      {
        string token = match.Groups[1].Value;
        object value = _data.GetType().GetProperty(token).GetValue(_data);
        string itemValue = GetValueAsString(value, _item.Format);
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