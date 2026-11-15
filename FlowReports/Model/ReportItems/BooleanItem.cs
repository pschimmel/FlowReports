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

    public override double DefaultWidth => ReportBandBase.DefaultHeight;

    public override double DefaultHeight => ReportBandBase.DefaultHeight;

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
