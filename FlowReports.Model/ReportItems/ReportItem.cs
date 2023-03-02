namespace FlowReports.Model.ReportItems
{
  public abstract class ReportItem : ReportElement
  {

    protected ReportItem()
      : this(Guid.NewGuid())
    { }

    protected internal ReportItem(Guid id)
      : base(id)
    {
      Width = DefaultWidth;
      Height = DefaultHeight;
    }

    public virtual double DefaultX => 0;

    public virtual double DefaultY => 0;

    public virtual double DefaultWidth => 100;

    public virtual double DefaultHeight => ReportBand.DefaultHeight;

    public double Left { get; set; }

    public double Top { get; set; }

    public virtual double Width { get; set; }

    public virtual double Height { get; set; }
  }
}
