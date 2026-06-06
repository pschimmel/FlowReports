namespace FlowReports.Model.ReportItems
{
  public class SortDescriptor
  {
    public string Property { get; set; }

    public SortDirection Direction { get; set; }

    public override bool Equals(object obj)
    {
      return obj is SortDescriptor other && string.Equals(Property, other.Property, StringComparison.OrdinalIgnoreCase) && Direction == other.Direction;
    }

    public override int GetHashCode()
    {
      return HashCode.Combine(Property?.ToLowerInvariant(), Direction);
    }
  }
}
