using System.IO;
using System.Reflection;

namespace FlowReports.Model
{
  public static class Globals
  {
    public const string ApplicationName = "FlowReports";

    public const string ApplicationShortName = "FlowReports";

    public static Version Version => Assembly.GetEntryAssembly().GetName().Version;

    public static string CopyRight => "Engineering Solutions 2023-" + DateTime.Now.Year;

    public static string SettingsFileName => Path.Combine(GetStoragePath(), "FlowReports.Settings.xml");

    public static string Website => @"http://www.engineeringsolutions.de";

    public static string GetStoragePath()
    {
      return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FlowReports");
    }
  }
}
