using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using FlowReports.Model;
using FlowReports.TestApplication.Model;

namespace FlowReports.TestApplication
{
  internal static class DataHelper
  {
    private static readonly string ApplicationDataDirectory = Globals.GetStorageDir();

    private static readonly string ApplicationExecutablePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

    public static string SettingsFilePath = Path.Combine(ApplicationDataDirectory, "TestApp.Settings.xml");

    public static string ExampleDataFilePath = Path.Combine(ApplicationDataDirectory, "ExampleData.xml");

    public static string ApplicationExampleSourceDirectory = Path.Combine(ApplicationExecutablePath, "Examples");

    public static string ApplicationExampleWorkingDirectory = Path.Combine(ApplicationDataDirectory, "Examples");

    private const string ReportExtension = Globals.ReportExtension;

    public static IEnumerable<Company> ReadExampleData()
    {
      CopyExampleFilesIfNeeded();

      if (File.Exists(ExampleDataFilePath))
      {
        try
        {
          var serializer = new XmlSerializer(typeof(Company[]));
          using var reader = XmlReader.Create(ExampleDataFilePath);
          return (Company[])serializer.Deserialize(reader);
        }
        catch (Exception ex)
        {
          Debug.Fail(ex.Message);
        }
      }
      else
      {
        return CreateNewExampleData();
      }

      return new List<Company>();
    }

    private static void CopyExampleFilesIfNeeded()
    {
      var files = Directory.GetFiles(ApplicationExampleSourceDirectory, "*" + ReportExtension);
      foreach (var sourceFileName in files)
      {
        string fileName = Path.GetFileName(sourceFileName);
        string targetFileName = Path.Combine(ApplicationExampleWorkingDirectory, fileName);

        // Only copy non-existing files
        if (!File.Exists(targetFileName))
        {
          try
          {
            Directory.CreateDirectory(Path.GetDirectoryName(targetFileName));
            File.Copy(sourceFileName, targetFileName);
          }
          catch (Exception ex)
          {
            Debug.Fail(ex.Message);
          }
        }
      }
    }

    private static List<Company> CreateNewExampleData()
    {
      var hamster = new Uri("pack://application:,,,/FlowReports.TestApplication;component/Images/Hamster.png", UriKind.RelativeOrAbsolute);
      var employee1 = new Employee { FirstName = "Michael", LastName = "Meyers", DOB = new DateTime(1975, 11, 1)/*, Image = LoadImage("Fox.jpg")*/ };
      var employee2 = new Employee { FirstName = "Jason", LastName = "Vorhees", DOB = new DateTime(1965, 12, 11), IsExternal = true };
      var employee3 = new Employee { FirstName = "Freddy", LastName = "Krueger", IsExternal = true/*, Image = LoadImage("Bunny.png")*/ };
      var employee4 = new Employee { FirstName = "Wayne", LastName = "Bruce", Image = hamster } ;
      var company1 = new Company { Name = "Nightmare Inc." };
      var company2 = new Company { Name = "DC" };
      company1.Employees.Add(employee1);
      company1.Employees.Add(employee2);
      company1.Employees.Add(employee3);
      company2.Employees.Add(employee4);
      return new List<Company> { company1, company2 };
    }

    public static void WriteExampleData(IEnumerable<Company> companies)
    {
      try
      {
        if (File.Exists(ExampleDataFilePath))
        {
          File.Delete(ExampleDataFilePath);
        }
      }
      catch (Exception ex)
      {
        Debug.Fail(ex.Message);
      }

      try
      {
        // Prevent useless namespaces in XML file
        var ns = new XmlSerializerNamespaces();
        ns.Add("", "");
        var serializer = new XmlSerializer(typeof(Company[]));
        using var writer = XmlWriter.Create(ExampleDataFilePath, new XmlWriterSettings { Indent = true });
        serializer.Serialize(writer, companies.ToArray(), ns);
      }
      catch (Exception ex)
      {
        Debug.Fail(ex.Message);
      }
    }

    public static TestAppSettings ReadSettings()
    {
      if (File.Exists(SettingsFilePath))
      {
        try
        {
          var serializer = new XmlSerializer(typeof(TestAppSettings));
          using var reader = XmlReader.Create(SettingsFilePath);
          return (TestAppSettings)serializer.Deserialize(reader);
        }
        catch (Exception ex)
        {
          Debug.Fail(ex.Message);
        }
      }

      return new TestAppSettings();
    }

    public static void WriteSettings(TestAppSettings settings)
    {
      try
      {
        if (File.Exists(SettingsFilePath))
        {
          File.Delete(SettingsFilePath);
        }
      }
      catch (Exception ex)
      {
        Debug.Fail(ex.Message);
      }

      try
      {
        // Prevent useless namespaces in XML file
        var ns = new XmlSerializerNamespaces();
        ns.Add("", "");
        var serializer = new XmlSerializer(typeof(TestAppSettings));
        using var writer = XmlWriter.Create(SettingsFilePath, new XmlWriterSettings { Indent = true });
        serializer.Serialize(writer, settings, ns);
      }
      catch (Exception ex)
      {
        Debug.Fail(ex.Message);
      }
    }
  }
}