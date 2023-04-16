using System.ComponentModel;
using ES.Tools.Core.Infrastructure;
using ES.Tools.Core.MVVM;
using FlowReports.ViewModel;
using Fluent;

namespace FlowReports.UI.View
{
  /// <summary>
  /// Interaction logic for PrintPreviewWindow.xaml
  /// </summary>
  public partial class PrintPreviewWindow : RibbonWindow, IView
  {
    public PrintPreviewWindow()
    {
      InitializeComponent();
      EventService.Instance.Subscribe<bool>("CloseEditor", CloseWindow);
    }

    public IViewModel ViewModel
    {
      get => (ReportEditorViewModel)DataContext;
      set => DataContext = value;
    }

    protected override void OnClosing(CancelEventArgs e)
    {
      base.OnClosing(e);

      if (!e.Cancel)
      {
        EventService.Instance.Unsubscribe("CloseEditor");
      }
    }

    private void CloseWindow(bool close)
    {
      if (close)
      {
        Close();
      }
    }
  }
}
