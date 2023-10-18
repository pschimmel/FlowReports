namespace FlowReports.Model.ReportItems
{
  public class ImageItem : ReportItem
  {
    public ImageItem()
    : base()
    { }

    internal ImageItem(Guid id)
      : base(id)
    { }

    public override double DefaultWidth => 100;

    public override double DefaultHeight => 100;

    public override bool Equals(object obj)
    {
      return obj is ImageItem other &&
        Equals(DataSource, other.DataSource);
    }

    public override int GetHashCode()
    {
      return nameof(ImageItem).GetHashCode() ^ new { DataSource }.GetHashCode();
    }
  }
}
