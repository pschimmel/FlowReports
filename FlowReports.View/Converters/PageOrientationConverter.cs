using System.Globalization;
using System.Printing;
using System.Windows.Data;

namespace FlowReports.View.Converters
{
  [ValueConversion(typeof(PageOrientation), typeof(bool))]
  public class PageOrientationConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var orientation = (PageOrientation)value;
      return orientation == PageOrientation.Landscape || orientation == PageOrientation.ReverseLandscape;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      bool boolValue = (bool)value;
      return boolValue ? PageOrientation.Landscape : PageOrientation.Portrait;
    }
  }
}
