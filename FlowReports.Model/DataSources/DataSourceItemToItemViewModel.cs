using FlowReports.Model.DataSources.DataSourceItems;
using FlowReports.Model.ReportItems;

namespace FlowReports.Model.DataSources
{
  public static class DataSourceItemToReportItem
  {
    public static ReportItem GetReportItem(this IDataSourceItem item, double x, double y)
    {
      return item switch
      {
        TextField => GetFormattedItem(item as TextField, x, y),
        DateField => GetFormattedItem(item as DateField, x, y),
        NumberField => GetFormattedItem(item as NumberField, x, y),
        _ => throw new NotImplementedException("Unknown DataSourceItem type."),
      };
    }

    private static ReportItem GetFormattedItem(IFormatItem item, double x, double y)
    {
      var textItem = GetTextItem(item.Name, x, y);
      textItem.Format = item.DefaultFormat;
      return textItem;
    }

    private static TextItem GetTextItem(string dataSource, double x, double y)
    {
      string text = $"{Settings.DATASOURCE_OPENING_BRACKET}{dataSource}{Settings.DATASOURCE_CLOSING_BRACKET}";
      return new TextItem()
      {
        Text = text,
        Left = x,
        Top = y,
      };
    }
  }
}
