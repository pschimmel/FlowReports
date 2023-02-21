namespace FlowReports.Model.ReportItems
{
  public abstract class ReportElement
  {
    protected ReportElement()
    {
      ID = Guid.NewGuid();
    }

    protected internal ReportElement(Guid id)
    {
      ID = id;
    }

    public Guid ID { get; internal set; }
  }
}
