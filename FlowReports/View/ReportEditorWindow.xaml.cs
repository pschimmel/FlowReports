using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;
using ES.Tools.Core.Infrastructure;
using ES.Tools.Core.MVVM;
using FlowReports.View.Infrastructure;
using FlowReports.ViewModel;
using FlowReports.ViewModel.Infrastructure;
using Fluent;

namespace FlowReports.View
{
  /// <summary>
  /// Interaction logic for ReportEditorWindow.xaml
  /// </summary>
  public partial class ReportEditorWindow : RibbonWindow, IView
  {
    public ReportEditorWindow()
    {
      InitializeComponent();
      EventService.Instance.Subscribe<bool>("CloseEditor", CloseWindow);
      EventService.Instance.Subscribe<MessageInfo>("Message", ShowMessage);
    }

    protected override void OnClosing(CancelEventArgs e)
    {
      if (((ReportEditorViewModel)ViewModel).AskToSave() == false)
      {
        e.Cancel = true;
      }
      else
      {
        EventService.Instance.Unsubscribe("CloseEditor");
        EventService.Instance.Unsubscribe("Message");

        if (Services.Instance.HasService<ExecuteOnApplicationClosing>())
        {
          var service = Services.Instance.GetService<ExecuteOnApplicationClosing>();
          service.Execute();
        }
      }
    }

    private void CloseWindow(bool close)
    {
      if (close)
      {
        Close();
      }
    }

    public IViewModel ViewModel
    {
      get => (ReportEditorViewModel)DataContext;
      set => DataContext = value;
    }

    protected override void OnClosed(EventArgs e)
    {
      base.OnClosed(e);
    }

    private void ShowMessage(MessageInfo messageInfo)
    {
      var result = MessageBox.Show(messageInfo.Message, messageInfo.Title, messageInfo.Buttons.Get(), messageInfo.Icon.Get());
      messageInfo.DialogResult = result.Get();
    }

    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
      Process.Start(e.Uri.ToString());
      Backstage.IsOpen = false;
    }
  }
}
