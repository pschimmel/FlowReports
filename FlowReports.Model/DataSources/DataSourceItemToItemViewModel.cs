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
        BooleanField => GetBooleanItem(item as BooleanField, x, y),
        _ => throw new NotImplementedException("Unknown DataSourceItem type."),
      };
    }

    private static ReportItem GetBooleanItem(BooleanField booleanField, double x, double y)
    {
      return new BooleanItem
      {
        DataSource = FormatDataSource(booleanField.Name),
        Left = x,
        Top = y
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
      return new TextItem
      {
        Text = FormatDataSource(dataSource),
        Left = x,
        Top = y
      };
    }

    private static string FormatDataSource(string dataSource)
    {
      return $"{Settings.DATASOURCE_OPENING_BRACKET}{dataSource}{Settings.DATASOURCE_CLOSING_BRACKET}";
    }
  }
}
