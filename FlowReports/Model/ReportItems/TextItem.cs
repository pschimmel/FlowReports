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

    public bool Bold { get; set; }

    public bool Italic { get; set; }

    public bool Underline { get; set; }

    public override bool Equals(object obj)
    {
      return obj is TextItem other &&
        Equals(DataSource, other.DataSource) &&
        Equals(Format, other.Format) &&
        Equals(Bold, other.Bold) &&
        Equals(Italic, other.Italic) &&
        Equals(Underline, other.Underline);
    }

    public override int GetHashCode()
    {
      return HashCode.Combine( DataSource, Format, Bold, Italic, Underline );
    }
  }
}
