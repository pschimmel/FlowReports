using System.IO;
using System.Windows.Media.Imaging;

namespace FlowReports.TestApplication.Helpers
{
  internal static class Bitmaps
  {
    public static string ToBase64(this BitmapImage image, string format)
    {
      return Convert.ToBase64String(Encode(image, format));
    }

    public static BitmapImage FromBase64(this string data)
    {
      using var stream = new MemoryStream(Convert.FromBase64String(data));
      return stream.ToBitmapImage();
    }

    private static byte[] Encode(BitmapImage bitmapImage, string format)
    {
      byte[] data = null;
      BitmapEncoder encoder = null;
      switch (format.ToUpper())
      {
        case "PNG":
          encoder = new PngBitmapEncoder();
          break;
        case "GIF":
          encoder = new GifBitmapEncoder();
          break;
        case "BMP":
          encoder = new BmpBitmapEncoder();
          break;
        case "JPG":
          encoder = new JpegBitmapEncoder();
          break;
      }
      if (encoder != null)
      {
        encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
        using var ms = new MemoryStream();
        encoder.Save(ms);
        ms.Seek(0, SeekOrigin.Begin);
        data = ms.ToArray();
      }

      return data;
    }

    public static BitmapImage ToBitmapImage(this Stream stream)
    {
      var bitmap = new BitmapImage();
      bitmap.BeginInit();
      bitmap.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
      bitmap.CacheOption = BitmapCacheOption.OnLoad;
      bitmap.StreamSource = stream;
      bitmap.EndInit();
      return bitmap;
    }
  }
}
