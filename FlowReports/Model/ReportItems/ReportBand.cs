using FlowReports.Model.Events;
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
          HeaderChanged.Invoke(this, EventArgs.Empty);
        }
        else if (value != null)
        {
          _headerBand = value;
          HeaderChanged.Invoke(this, EventArgs.Empty);
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
          FooterChanged.Invoke(this, EventArgs.Empty);
        }
        else if (value != null)
        {
          _footerBand = value;
          FooterChanged.Invoke(this, EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Gets or sets the name or network address of the data source.
    /// </summary>
    public string DataSource { get; set; }

    /// <summary>
    /// Gets the collection of sub-bands contained within this band. 
    /// </summary>
    public ReportBandCollection Bands { get; } = new ReportBandCollection();


    #endregion

    #region Overwritten Members

    public override bool Equals(object obj)
    {
      return obj is ReportBand other &&
        Equals(Height, other.Height) &&
        Equals(DataSource, other.DataSource) &&
        Equals(Bands, other.Bands) &&
        List.Equals(Items, other.Items);
    }

    public override int GetHashCode()
    {
      return HashCode.Combine(Height, DataSource, Bands, Items);  
    }

    #endregion
  }
}
