namespace FlowReports.ViewModel.Infrastructure
{
  public class ExecuteOnApplicationClosing
  {
    private readonly List<Action> _actions = new();

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