using FlowReports.Model;
using FlowReports.ViewModel;

namespace FlowReports.UnitTests.ViewModel
{
  public class AboutViewModelTests
  {
    private AboutViewModel _viewModel;

    [SetUp]
    public void Setup()
    {
      _viewModel = new AboutViewModel();
    }

    [Test]
    public void Version_ReturnsVersionString()
    {
      var version = _viewModel.Version;
      Assert.That(version, Is.Not.Null);
      Assert.That(version, Is.Not.Empty);
    }

    [Test]
    public void Copy_ReturnsCopyright()
    {
      var copy = _viewModel.Copy;
      Assert.That(copy, Is.Not.Null);
      Assert.That(copy, Is.Not.Empty);
    }

    [Test]
    public void ApplicationName_ReturnsApplicationName()
    {
      var appName = _viewModel.ApplicationName;
      Assert.That(appName, Is.Not.Null);
      Assert.That(appName, Is.Not.Empty);
    }

    [Test]
    public void ApplicationLongName_ReturnsApplicationLongName()
    {
      var longName = _viewModel.ApplicationLongName;
      Assert.That(longName, Is.Not.Null);
      Assert.That(longName, Contains.Substring("("));
      Assert.That(longName, Contains.Substring(")"));
    }

    [Test]
    public void Website_ReturnsWebsiteUrl()
    {
      var website = _viewModel.Website;
      Assert.That(website, Is.Not.Null);
      Assert.That(website, Is.Not.Empty);
    }

    [TearDown]
    public void TearDown()
    {
      _viewModel?.Dispose();
    }
  }
}