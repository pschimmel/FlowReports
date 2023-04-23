using FlowReports.Model;

namespace FlowReports.ViewModel
{
  public class AboutViewModel : ViewModelBase
  {
#pragma warning disable CA1822 // Mark members as static

    public string Version => Globals.Version.ToString(3);

    public string Copy => Globals.CopyRight;

    public string ApplicationName => Globals.ApplicationName;

    public string ApplicationLongName => $"{Globals.ApplicationName} ({Globals.ApplicationShortName})";

#pragma warning restore CA1822 // Mark members as static
  }
}
