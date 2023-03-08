namespace FlowReports.Model.ReportItems
{
  public class TextItem : ReportItem
  {
    public TextItem()
    : base()
    { }

    internal TextItem(Guid id)
      : base(id)
    { }

    public string Format { get; set; }

    public override bool Equals(object obj)
    {
      return obj is TextItem other &&
        Equals(DataSource, other.DataSource) &&
        Equals(Format, other.Format);
    }

    public override int GetHashCode()
    {
      return nameof(ReportItem).GetHashCode() ^ new { DataSource, Format }.GetHashCode();
    }
  }
}
