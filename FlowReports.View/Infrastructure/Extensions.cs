using FlowReports.ViewModel.Infrastructure;

namespace FlowReports.View.Infrastructure
{
  internal static class Extensions
  {
    public static System.Windows.MessageBoxButton Get(this MessageBoxButtons buttons)
    {
      return buttons switch
      {
        MessageBoxButtons.OK => System.Windows.MessageBoxButton.OK,
        MessageBoxButtons.OKCancel => System.Windows.MessageBoxButton.OKCancel,
        MessageBoxButtons.YesNo => System.Windows.MessageBoxButton.YesNo,
        MessageBoxButtons.YesNoCancel => System.Windows.MessageBoxButton.YesNoCancel,
        _ => throw new ArgumentException("Unknown enumeration value."),
      };
    }

    public static System.Windows.MessageBoxImage Get(this MessageBoxIcons icon)
    {
      return icon switch
      {
        MessageBoxIcons.Information => System.Windows.MessageBoxImage.Information,
        MessageBoxIcons.Warning => System.Windows.MessageBoxImage.Warning,
        MessageBoxIcons.Error => System.Windows.MessageBoxImage.Error,
        MessageBoxIcons.Question => System.Windows.MessageBoxImage.Question,
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
