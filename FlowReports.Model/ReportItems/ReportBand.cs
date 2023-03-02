using FlowReports.Model.Events;
using FlowReports.Model.Tools;

namespace FlowReports.Model.ReportItems
{
  public class ReportBand : ReportElement
  {
    #region Fields

    private double _insertPosition = 0;

    #endregion

    #region Events

    public event EventHandler<ItemsEventArgs> ItemAdded;
    public event EventHandler<ItemsEventArgs> ItemRemoved;

    #endregion

    #region Constants

    public const double DefaultHeight = 20.0;

    #endregion

    #region Constructor

    internal ReportBand(Guid id)
      : base(id)
    {
      Height = DefaultHeight;
      SubBands = new ReportBandCollection();
    }

    public ReportBand()
      : this(Guid.NewGuid())
    { }

    #endregion

    #region Properties

    public List<ReportItem> Items { get; } = new List<ReportItem>();

    public double Height { get; set; }

    public string DataSource { get; set; }

    public ReportBandCollection SubBands { get; }

    #endregion

    #region Public Methods

    public void AddTextItem()
    {
      var textItem = new TextItem
      {
        Left = _insertPosition
      };

      AddItem(textItem);
    }

    public void AddBooleanItem()
    {
      var booleanItem = new BooleanItem
      {
        Left = _insertPosition
      };

      AddItem(booleanItem);
    }

    public void AddReportItem(ReportItem item)
    {
      AddItem(item);
    }

    public void RemoveItem(ReportItem item)
    {
      if (Items.Remove(item))
      {
        OnItemRemoved(item);
      }
    }

    #endregion

    #region Overwritten Members

    public override bool Equals(object obj)
    {
      return obj is ReportBand other &&
        Equals(Height, other.Height) &&
        Equals(DataSource, other.DataSource) &&
        Equals(SubBands, other.SubBands) &&
        List.Equals(Items, other.Items);
    }

    public override int GetHashCode()
    {
      return new
      {
        Height,
        DataSource,
        SubBands,
        Items
      }.GetHashCode();
    }

    #endregion

    #region Private Methods

    private void AddItem(ReportItem item)
    {
      Items.Add(item);
      OnItemAdded(item);
      _insertPosition = item.Left + item.Width;
    }

    private void OnItemAdded(ReportItem item)
    {
      ItemAdded?.Invoke(this, new ItemsEventArgs(item));
    }

    private void OnItemRemoved(ReportItem item)
    {
      ItemRemoved?.Invoke(this, new ItemsEventArgs(item));
    }

    #endregion
  }
}
