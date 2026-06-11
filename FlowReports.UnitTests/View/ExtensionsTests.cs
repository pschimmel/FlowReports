using System.Windows;
using FlowReports.View.Infrastructure;
using FlowReports.ViewModel.Infrastructure;

namespace FlowReports.UnitTests.View
{
  public class ExtensionsTests
  {
    [Test]
    public void Get_MessageBoxButton_Mapping_Works()
    {
      using (Assert.EnterMultipleScope())
      {
        Assert.That(MessageBoxButtons.OK.Get(), Is.EqualTo(MessageBoxButton.OK));
        Assert.That(MessageBoxButtons.OKCancel.Get(), Is.EqualTo(MessageBoxButton.OKCancel));
        Assert.That(MessageBoxButtons.YesNo.Get(), Is.EqualTo(MessageBoxButton.YesNo));
        Assert.That(MessageBoxButtons.YesNoCancel.Get(), Is.EqualTo(MessageBoxButton.YesNoCancel));
      }
    }

    [Test]
    public void Get_MessageBoxImage_Mapping_Works()
    {
      using (Assert.EnterMultipleScope())
      {
        Assert.That(MessageBoxIcons.Information.Get(), Is.EqualTo(MessageBoxImage.Information));
        Assert.That(MessageBoxIcons.Warning.Get(), Is.EqualTo(MessageBoxImage.Warning));
        Assert.That(MessageBoxIcons.Error.Get(), Is.EqualTo(MessageBoxImage.Error));
        Assert.That(MessageBoxIcons.Question.Get(), Is.EqualTo(MessageBoxImage.Question));
      }
    }

    [Test]
    public void Get_MessageBoxResult_Mapping_Works()
    {
      using (Assert.EnterMultipleScope())
      {
        Assert.That(System.Windows.MessageBoxResult.OK.Get(), Is.EqualTo(FlowReports.ViewModel.Infrastructure.MessageBoxResult.Yes));
        Assert.That(System.Windows.MessageBoxResult.Yes.Get(), Is.EqualTo(FlowReports.ViewModel.Infrastructure.MessageBoxResult.Yes));
        Assert.That(System.Windows.MessageBoxResult.No.Get(), Is.EqualTo(FlowReports.ViewModel.Infrastructure.MessageBoxResult.No));
        Assert.That(System.Windows.MessageBoxResult.Cancel.Get(), Is.EqualTo(FlowReports.ViewModel.Infrastructure.MessageBoxResult.Cancel));
        Assert.That(System.Windows.MessageBoxResult.None.Get(), Is.EqualTo(FlowReports.ViewModel.Infrastructure.MessageBoxResult.Cancel));
      }
    }
  }
}
