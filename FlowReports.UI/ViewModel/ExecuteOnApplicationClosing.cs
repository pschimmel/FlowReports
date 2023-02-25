namespace FlowReports.UI.ViewModel
{
  internal class ExecuteOnApplicationClosing
  {
    private readonly List<Action> _actions = new List<Action>();

    public void Add(Action action)
    {
      _actions.Add(action);
    }

    public void Execute()
    {
      foreach (var action in _actions)
      {
        action();
      }
    }
  }
}