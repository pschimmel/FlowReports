namespace FlowReports.Model.Events
{
  public abstract class GenericEventArgs<T> : EventArgs
  {
    protected GenericEventArgs(T item)
    {
      Item = item;
    }

    public T Item { get; }
  }
}