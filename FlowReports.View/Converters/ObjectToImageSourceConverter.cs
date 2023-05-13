using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FlowReports.View.Converters
{
  [ValueConversion(typeof(object), typeof(ImageSource))]
  internal class ObjectToImageSourceConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      // Nothing to convert
      if (value == null)
      {
        return null;
      }

      // If item is an ImageSource, return it
      if (value is ImageSource imageSource)
      {
        return imageSource;
      }

      // Check if it's an image resource string or path to an image
      if (value is string imagePath)
      {
        return GetImageSource(imagePath);
      }

      // Check if it is an image Uri
      if (value is Uri imageUri)
      {
        return GetImageSource(imageUri);
      }

      // Check if it's a byte array containing an image
      if (value is byte[] imageAsByteArray)
      {
        return GetImageSource(imageAsByteArray);
      }

      if (value is Stream imageAsStream)
      {
        return GetImageSource(imageAsStream);
      }

      // Don't know what it is
      return null;
    }

    private static ImageSource GetImageSource(string imagePath)
    {
      var imageUri = new Uri(imagePath, UriKind.RelativeOrAbsolute);

      // Allow things like "Images\Green.png"
      if (imageUri.IsAbsoluteUri == false)
      {
        // If that file does not exist, try to find it using resource notation
        if (File.Exists(imagePath) == false)
        {
          var slash = string.Empty;

          if (imagePath.StartsWith("/", StringComparison.OrdinalIgnoreCase) == false)
          {
            slash = "/";
          }

          imageUri = new Uri("pack://application:,,," + slash + imagePath, UriKind.RelativeOrAbsolute);
        }
      }

      return GetImageSource(imageUri);
    }

    private static ImageSource GetImageSource(Uri imageUri)
    {
      return new BitmapImage(imageUri);
    }

    private static object GetImageSource(byte[] imageAsByteArray)
    {
      var ms = new MemoryStream(imageAsByteArray);
      return GetImageSource(ms);
    }

    private static object GetImageSource(Stream stream)
    {
      if (!stream.CanRead)
      {
        return null;
      }

      var image = new BitmapImage();
      image.BeginInit();
      image.StreamSource = stream;
      image.EndInit();
      image.Freeze();
      return image;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}