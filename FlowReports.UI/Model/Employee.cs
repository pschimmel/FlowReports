using System.Diagnostics;
using System.IO;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using FlowReports.TestApplication.Helpers;

namespace FlowReports.TestApplication.Model
{
  public class Employee : ES.Tools.Core.MVVM.NotifyObject
  {
    private object _image;

    public string FirstName { get; set; }

    public string LastName { get; set; }

    [XmlIgnore]
    public DateTime? DOB { get; set; }

    [XmlElement("DOB")]
    public string SerializableDOB
    {
      get => DOB.HasValue ? DOB.Value.ToString("o") : null;
      set => DOB = value != null ? DateTime.Parse(value) : null;
    }

    public string Email { get; set; }

    public bool IsExternal { get; set; }

    [XmlIgnore]
    public object Image
    {
      get => _image;
      set
      {
        if (_image != value)
        {
          _image = value;
          OnPropertyChanged();
        }
      }
    }

    [XmlElement("Image")]
    public string SerializableImage
    {
      get
      {
        if (Image is Uri imageAsUri)
        {
          return imageAsUri.OriginalString;
        }

        if (Image is BitmapImage bitmapImage)
        {
          string extension = Path.GetExtension(bitmapImage.UriSource?.AbsoluteUri)?.ToLower();
          string format = "PNG";

          BitmapMetadata metadata = null;
          try
          {
            metadata = bitmapImage.Metadata as BitmapMetadata;
          }
          catch (NotSupportedException)
          {
          }

          if (extension == ".jpg" || extension == ".jpeg" || metadata?.Format == "jpg")
          {
            format = "JPG";
          }
          else if (extension == ".png" || metadata?.Format == "png")
          {
            format = "PNG";
          }

          if (!string.IsNullOrWhiteSpace(format))
          {
            var imageAsString = bitmapImage.ToBase64(format);
            return $"{format}: {imageAsString}";
          }
        }

        return null;
      }
      set
      {
        if (value == null)
        {
          Image = null;
          return;
        }

        // Try to convert string to Bitmap source.
        if (value.StartsWith("PNG: ") || value.StartsWith("JPG: "))
        {
          Image = value.Substring(5).FromBase64();
          return;
        }

        // try to use string as URI
        try
        {
          var uri = new Uri(value, UriKind.RelativeOrAbsolute);
          Image = uri;
        }
        catch (Exception ex)
        {
          Debug.Fail(ex.Message);
        }
      }
    }

    public string FullName => $"{LastName}, {FirstName}";
  }
}
