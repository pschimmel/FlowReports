using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FlowReports.UI.View.Converters
{
  [ValueConversion(typeof(string), typeof(ImageSource))]
  internal class IconToResourceConverter : IValueConverter
  {
    private static readonly Dictionary<string, ImageSource> _iconCache = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      string valueAsString = (string)value;
      try
      {

        if (!_iconCache.TryGetValue(valueAsString, out ImageSource imageSource))
        {
          imageSource = new BitmapImage(new Uri("pack://application:,,,/FlowReports.UI;component/Images/" + valueAsString, UriKind.Absolute));
          _iconCache[valueAsString] = imageSource;
        }

        return imageSource;
      }
      catch
      {
        return DependencyProperty.UnsetValue;
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
