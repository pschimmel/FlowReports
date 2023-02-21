namespace FlowReports.UI.Infrastructure
{
  internal class MessageInfo
  {
    private MessageInfo(string message, string title, MessageBoxButtons buttons, MessageBoxIcons icon)
    {
      Message = message;
      Title = title;
      Buttons = buttons;
      Icon = icon;
    }

    public string Message { get; }

    public string Title { get; }

    public MessageBoxButtons Buttons { get; }

    public MessageBoxIcons Icon { get; }

    public MessageBoxResult DialogResult { get; set; }

    public static MessageInfo Information(string message) => new(message, Properties.Resources.Information, MessageBoxButtons.OK, MessageBoxIcons.Information);

    public static MessageInfo Warning(string message) => new(message, Properties.Resources.Warning, MessageBoxButtons.OKCancel, MessageBoxIcons.Warning);

    public static MessageInfo Error(string message) => new(message, Properties.Resources.Error, MessageBoxButtons.OK, MessageBoxIcons.Error);

    public static MessageInfo Question(string message, bool canCancel = false) => new(message, Properties.Resources.Question, canCancel ? MessageBoxButtons.YesNoCancel : MessageBoxButtons.YesNo, MessageBoxIcons.Question);
  }
}
