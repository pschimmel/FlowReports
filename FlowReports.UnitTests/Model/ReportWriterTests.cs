using System.IO;
using FlowReports.Model;
using FlowReports.Model.ImportExport;

namespace FlowReports.UnitTests.Model
{
  public class ReportWriterTests
  {
    private Report _report;

    [SetUp]
    public void Setup()
    {
      _report = new Report();
    }

    [Test]
    public void Test1()
    {
      using var stream = new MemoryStream();
      ReportWriter.Write(_report, stream);
      Assert.That(stream, Is.Not.Null);
      stream.Position = 0;

      using var reader = new StreamReader(stream);
      var reportAsString = reader.ReadToEnd();
      Assert.That(reportAsString, Is.Not.Empty);
    }
  }
}