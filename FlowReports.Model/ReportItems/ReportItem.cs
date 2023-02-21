namespace FlowReports.Model.ReportItems
{
  public abstract class ReportItem : ReportElement
  {
    public const double DefaultX = 0;
    public const double DefaultY = 0;
    public const double DefaultWidth = 100;
    public const double DefaultHeight = ReportBand.DefaultHeight;

    protected ReportItem()
      : this(Guid.NewGuid())
    { }

    protected internal ReportItem(Guid id)
      : base(id)
    {
      Width = DefaultWidth;
      Height = DefaultHeight;
    }

    public double Left { get; set; }

    public double Top { get; set; }

    public double Width { get; set; }

    public double Height { get; set; }
  }
}
