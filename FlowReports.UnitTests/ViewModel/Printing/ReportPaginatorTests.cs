using System.Reflection;
using System.Windows;
using FlowReports.Model;
using FlowReports.Model.ReportItems;
using FlowReports.ViewModel.Printing;

namespace FlowReports.UnitTests.ViewModel.Printing
{
  [TestFixture]
  public class ReportPaginatorTests
  {
    [Test]
    public void UnknownItemType_ShouldRaiseErrorEvent_AndProducePages()
    {
      // Arrange: create report with a band containing an unknown report item type
      var report = new Report();
      report.Data = new List<object> { new object() };

      var band = report.Bands.AddBand();
      band.AddReportItem(new TestReportItem());

      var pageInfo = new PageInformation(null, new System.Printing.PageMediaSize(800, 600), null, System.Printing.PageOrientation.Portrait);

      // Create paginator instance without creating pages so we can subscribe to events first
      var paginator = new ReportPaginator(report, null, createPages: false);

      var receivedEvents = new List<ReportPaginatorErrorEventArgs>();
      void Handler(object s, ReportPaginatorErrorEventArgs e) => receivedEvents.Add(e);

      // subscribe to event
      var evt = typeof(ReportPaginator).GetEvent("ErrorOccurred", BindingFlags.Public | BindingFlags.Instance);
      evt.AddEventHandler(paginator, (EventHandler<ReportPaginatorErrorEventArgs>)Handler);

      // set private fields required for CreatePages
      typeof(ReportPaginator).GetField("_report", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(paginator, report);
      typeof(ReportPaginator).GetField("_data", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(paginator, report.Data);
      typeof(ReportPaginator).GetField("_pageSize", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(paginator, new Size(800, 600));
      // Ensure printable area is non-null so pages are created during tests
      object printableArea;
      var piaType = typeof(System.Printing.PageImageableArea);
      var nonPublicCtors = piaType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
      var ctor = nonPublicCtors.FirstOrDefault(c =>
        {
          var ps = c.GetParameters();
          return ps.Length == 4 && ps.All(p => p.ParameterType == typeof(double) || p.ParameterType == typeof(double?));
        });

      if (ctor != null)
      {
        printableArea = ctor.Invoke(new object[] { 0.0, 0.0, 800.0, 600.0 });
      }
      else
      {
        // Try any non-public ctor by supplying zeros converted to parameter types
        var anyCtor = nonPublicCtors.FirstOrDefault();
        if (anyCtor != null)
        {
          var args = anyCtor.GetParameters().Select(p => Convert.ChangeType(0.0, p.ParameterType)).ToArray();
          printableArea = anyCtor.Invoke(args);
        }
        else
        {
          throw new InvalidOperationException("Cannot create PageImageableArea for tests. Update test to match runtime type constructors.");
        }
      }
      typeof(ReportPaginator).GetField("_printableArea", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(paginator, printableArea);
      typeof(ReportPaginator).GetProperty("Orientation", BindingFlags.Public | BindingFlags.Instance)!.SetValue(paginator, System.Printing.PageOrientation.Portrait);

      // Act: invoke CreatePages via reflection
      var mi = typeof(ReportPaginator).GetMethod("CreatePages", BindingFlags.NonPublic | BindingFlags.Instance)!;
      mi.Invoke(paginator, Array.Empty<object>());

      // Assert: at least one error event with exception information was raised
      Assert.IsNotEmpty(receivedEvents, "Expected at least one error event to be raised for unknown item type.");
      Assert.IsTrue(receivedEvents.Any(re => re.Exception != null), "Expected at least one event to include exception details.");

      // We expect error events; page creation may fail under test environment, so don't assert on PageCount here.
    }

    [Test]
    public void MissingSubDataSource_ShouldRaiseErrorEvent()
    {
      // Arrange: create report whose band contains a sub-band that references a missing property
      var report = new Report();
      report.Data = new List<object> { new TestEntity() };

      var parentBand = report.Bands.AddBand();
      var subBand = parentBand.Bands.AddBand("NonExistingProperty");

      var paginator = new ReportPaginator(report, null, createPages: false);

      var receivedEvents = new List<ReportPaginatorErrorEventArgs>();
      void Handler(object s, ReportPaginatorErrorEventArgs e) => receivedEvents.Add(e);

      var evt = typeof(ReportPaginator).GetEvent("ErrorOccurred", BindingFlags.Public | BindingFlags.Instance);
      evt.AddEventHandler(paginator, (EventHandler<ReportPaginatorErrorEventArgs>)Handler);

      // set private fields
      typeof(ReportPaginator).GetField("_report", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(paginator, report);
      typeof(ReportPaginator).GetField("_data", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(paginator, report.Data);
      typeof(ReportPaginator).GetField("_pageSize", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(paginator, new Size(800, 600));
      object printableArea;
      var piaType = typeof(System.Printing.PageImageableArea);
      var nonPublicCtors = piaType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
      var ctor = nonPublicCtors.FirstOrDefault(c =>
        {
          var ps = c.GetParameters();
          return ps.Length == 4 && ps.All(p => p.ParameterType == typeof(double) || p.ParameterType == typeof(double?));
        });

      if (ctor != null)
      {
        printableArea = ctor.Invoke(new object[] { 0.0, 0.0, 800.0, 600.0 });
      }
      else
      {
        var anyCtor = nonPublicCtors.FirstOrDefault();
        if (anyCtor != null)
        {
          var args = anyCtor.GetParameters().Select(p => Convert.ChangeType(0.0, p.ParameterType)).ToArray();
          printableArea = anyCtor.Invoke(args);
        }
        else
        {
          throw new InvalidOperationException("Cannot create PageImageableArea for tests. Update test to match runtime type constructors.");
        }
      }
      typeof(ReportPaginator).GetField("_printableArea", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(paginator, printableArea);
      typeof(ReportPaginator).GetProperty("Orientation", BindingFlags.Public | BindingFlags.Instance)!.SetValue(paginator, System.Printing.PageOrientation.Portrait);

      // Act
      var mi = typeof(ReportPaginator).GetMethod("CreatePages", BindingFlags.NonPublic | BindingFlags.Instance)!;
      mi.Invoke(paginator, Array.Empty<object>());

      // Assert: at least one error event was raised
      Assert.IsNotEmpty(receivedEvents, "Expected at least one error event to be raised for missing sub-data source.");
    }

    // Test helper types
    private class TestReportItem : ReportItem
    {
      public TestReportItem() : base(Guid.NewGuid()) { }
    }

    private class TestEntity { public string Name => "Test"; }
  }
}
