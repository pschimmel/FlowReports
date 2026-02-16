namespace FlowReports.Model.Tools
{
  public static class List
  {
    public static bool Equals<T>(IEnumerable<T> first, IEnumerable<T> second)
    {
      ArgumentNullException.ThrowIfNull(first);

      ArgumentNullException.ThrowIfNull(second);

      int firstCount = first.Count();
      int secondCount = second.Count();

      if (firstCount != secondCount)
      {
        return false;
      }

      using IEnumerator<T> e1 = first.GetEnumerator();
      using IEnumerator<T> e2 = second.GetEnumerator();
      while (e1.MoveNext() && e2.MoveNext())
      {
        if (!Equals(e1.Current, e2.Current))
        {
          return false;
        }
      }

      return true;
    }
  }
}
