using FlowReports.Model.Tools;

namespace FlowReports.Model.ReportItems
{
  /// <summary>
  /// Band that is printed before a band as a headeer.
  /// </summary>
  public class HeaderBand : ReportBandBase
  {
    #region Constructor

    public HeaderBand(Guid id)
      : base(id)
    { }

    public HeaderBand()
      : base(Guid.NewGuid())
    { }

    #endregion

    #region Properties

    public bool RepeatOnEachPage { get; set; }

    #endregion

    #region Overwritten Members

    public override bool Equals(object obj)
    {
      return obj is HeaderBand other &&
        Equals(Height, other.Height) &&
        Equals(RepeatOnEachPage, other.RepeatOnEachPage) &&
        List.Equals(Items, other.Items);
    }

    public override int GetHashCode()
    {
      return HashCode.Combine(Height, RepeatOnEachPage, Items);
    }

    #endregion
  }
}
