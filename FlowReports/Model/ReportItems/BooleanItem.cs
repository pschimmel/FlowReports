namespace FlowReports.Model.ReportItems
{
  public class BooleanItem : ReportItem
  {
    public BooleanItem()
    : base()
    { }

    internal BooleanItem(Guid id)
      : base(id)
    { }

    public override double DefaultWidth => ReportBand.DefaultHeight;

    public override double DefaultHeight => ReportBand.DefaultHeight;

    public override bool Equals(object obj)
    {
      return obj is BooleanItem other &&
        Equals(DataSource, other.DataSource);
    }

    public override int GetHashCode()
    {
      return nameof(BooleanItem).GetHashCode() ^ new { DataSource }.GetHashCode();
    }
  }
}
