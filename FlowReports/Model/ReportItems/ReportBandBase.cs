using FlowReports.Model.Events;

namespace FlowReports.Model.ReportItems
{
  public abstract class ReportBandBase : ReportElement
  {
    #region Fields

    private double _insertPosition = 0;

    #endregion

    #region Events

    public event EventHandler<ReportItemsEventArgs> ItemAdded;
    public event EventHandler<ReportItemsEventArgs> ItemRemoved;

    #endregion

    #region Constants

    public const double DefaultHeight = 20.0;

    #endregion

    #region Constructor

    protected ReportBandBase(Guid id)
      : base(id)
    {
      Height = DefaultHeight;
    }

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

    #region Private Methods

    private void AddItem(ReportItem item)
    {
      Items.Add(item);
      OnItemAdded(item);
      _insertPosition = item.Left + item.Width;
    }

    private void OnItemAdded(ReportItem item)
    {
      ItemAdded?.Invoke(this, new ReportItemsEventArgs(item));
    }

    private void OnItemRemoved(ReportItem item)
    {
      ItemRemoved?.Invoke(this, new ReportItemsEventArgs(item));
    }

    #endregion
  }
}
