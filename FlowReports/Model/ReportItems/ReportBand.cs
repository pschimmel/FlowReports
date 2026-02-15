using FlowReports.Model.Events;
using FlowReports.Model.Tools;

namespace FlowReports.Model.ReportItems
{
  /// <summary>
  /// Represents a band in a report, which can contain multiple report items and sub-bands.
  /// </summary>
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

    /// <summary>
    /// Gets the collection of report items contained in this instance.
    /// </summary>
    public List<ReportItem> Items { get; } = new List<ReportItem>();

    /// <summary>
    /// Gets or sets the height value. If set to null, the height will be determined by the maximum extent of contained items.
    /// </summary>
    public double? Height { get; set; }

    /// <summary>
    /// Gets the actual height of the layout, using the explicit height if set, or the maximum extent of contained items
    /// otherwise.
    /// </summary>
    public double ActualHeight => Height ?? Items.Max(i => i.Top + i.Height);

    /// <summary>
    /// Gets or sets the name or network address of the data source.
    /// </summary>
    public string DataSource { get; set; }

    /// <summary>
    /// Gets the collection of sub-bands contained within this band. 
    /// </summary>
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

    public void AddImageItem()
    {
      var imageItem = new ImageItem
      {
        Left = _insertPosition
      };

      AddItem(imageItem);
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
