using System.Windows;

namespace FlowReports.UI.Infrastructure
{
  internal static class Extensions
  {
    public static MessageBoxButton Get(this MessageBoxButtons buttons)
    {
      return buttons switch
      {
        MessageBoxButtons.OK => MessageBoxButton.OK,
        MessageBoxButtons.OKCancel => MessageBoxButton.OKCancel,
        MessageBoxButtons.YesNo => MessageBoxButton.YesNo,
        MessageBoxButtons.YesNoCancel => MessageBoxButton.YesNoCancel,
        _ => throw new ArgumentException("Unknown enumeration value."),
      };
    }

    public static MessageBoxImage Get(this MessageBoxIcons icon)
    {
      return icon switch
      {
        MessageBoxIcons.Information => MessageBoxImage.Information,
        MessageBoxIcons.Warning => MessageBoxImage.Warning,
        MessageBoxIcons.Error => MessageBoxImage.Error,
        MessageBoxIcons.Question => MessageBoxImage.Question,
        _ => throw new ArgumentException("Unknown enumeration value.")
      };
    }

    public static MessageBoxResult Get(this System.Windows.MessageBoxResult result)
    {
      return result switch
      {
        System.Windows.MessageBoxResult.Yes or System.Windows.MessageBoxResult.OK => MessageBoxResult.Yes,
        System.Windows.MessageBoxResult.No => MessageBoxResult.No,
        _ => MessageBoxResult.Cancel
      };
    }
  }
}
