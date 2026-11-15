using FlowReports.Model.Filtering;
using FlowReports.Model.Tools;

namespace FlowReports.Model.ReportItems
{
  /// <summary>
  /// Represents a band in a report, which can contain multiple report items and sub-bands.
  /// </summary>
  public class ReportBand : ReportBandBase, IHasBands
  {
    #region Events

    public event EventHandler HeaderChanged;
    public event EventHandler FooterChanged;

    #endregion

    #region Fields

    private HeaderBand _headerBand;
    private FooterBand _footerBand;

    #endregion

    #region Constructor

    public ReportBand(Guid id)
      : base(id)
    { }

    public ReportBand()
      : base(Guid.NewGuid())
    { }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets a header band that will be printed before this band when it is rendered in a report.
    /// </summary>
    public HeaderBand HeaderBand
    {
      get => _headerBand;
      set
      {
        if (_headerBand != null && value == null)
        {
          _headerBand = null;
          HeaderChanged?.Invoke(this, EventArgs.Empty);
        }
        else if (value != null)
        {
          _headerBand = value;
          HeaderChanged?.Invoke(this, EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Gets or sets a footer band that will be printed after this band when it is rendered in a report.
    /// </summary>
    public FooterBand FooterBand
    {
      get => _footerBand;
      set
      {
        if (_footerBand != null && value == null)
        {
          _footerBand = null;
          FooterChanged?.Invoke(this, EventArgs.Empty);
        }
        else if (value != null)
        {
          _footerBand = value;
          FooterChanged?.Invoke(this, EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Gets or sets the name or network address of the data source.
    /// </summary>
    public string DataSource { get; set; }

    /// <summary>
    /// Gets or sets a filter expression that determines which items from the data source
    /// will be rendered in this band. The expression should be a string like "PropertyName == 'value'"
    /// or "Age > 18 && Status == 'Active'".
    /// </summary>
    public FilterExpression FilterExpression { get; set; }

    /// <summary>
    /// Gets the collection of sub-bands contained within this band. 
    /// </summary>
    public ReportBandCollection Bands { get; } = new ReportBandCollection();

    /// <summary>
    /// Gets or sets the ordering descriptors used to sort items in this band.
    /// </summary>
    public List<SortDescriptor> Ordering { get; } = new List<SortDescriptor>();


    #endregion

    #region Overwritten Members

    public override bool Equals(object obj)
    {
      return obj is ReportBand other &&
        Equals(Height, other.Height) &&
        Equals(DataSource, other.DataSource) &&
        Equals(FilterExpression, other.FilterExpression) &&
        Equals(Bands, other.Bands) &&
        List.Equals(Items, other.Items) &&
        List.Equals(Ordering, other.Ordering);
    }

    public override int GetHashCode()
    {
      var hash = new HashCode();
      hash.Add(Height);
      hash.Add(DataSource);
      hash.Add(FilterExpression);
      hash.Add(Bands);
      foreach (var item in Items)
      {
        hash.Add(item);
      }
      foreach (var ord in Ordering)
      {
        hash.Add(ord);
      }
      return hash.ToHashCode();
    }

    #endregion
  }
}
