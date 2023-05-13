using System.Diagnostics;
using System.Windows.Documents;
using System.Windows.Navigation;
using Microsoft.Xaml.Behaviors;

namespace FlowReports.View.Behaviors
{
  /// <summary>
  /// Behavior that can be attached to a hyperlink object to start the hyperlink in an external browser.
  /// </summary>
  internal class HyperlinkNavigateBehavior : Behavior<Hyperlink>
  {
    protected override void OnAttached()
    {
      base.OnAttached();
      AssociatedObject.RequestNavigate += Hyperlink_RequestNavigate;
    }

    protected override void OnDetaching()
    {
      AssociatedObject.RequestNavigate += Hyperlink_RequestNavigate;
      base.OnDetaching();
    }

    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
      Process.Start(e.Uri.ToString());
    }
  }
}