using FlowReports.Model.Tools;

namespace FlowReports.Model.ReportItems
{
  /// <summary>
  /// Band that is printed after a band as a footer.
  /// </summary>
  public class FooterBand : ReportBandBase
  {
    public FooterBand(Guid id)
      : base(id)
    { }

    public FooterBand()
      : base(Guid.NewGuid())
    { }

    #region Overwritten Members

    public override bool Equals(object obj)
    {
      return obj is FooterBand other &&
        Equals(Height, other.Height) &&
        List.Equals(Items, other.Items);
    }

    public override int GetHashCode()
    {
      var hash = new HashCode();
      hash.Add(Height);
      foreach (var item in Items)
      {
        hash.Add(item);
      }
      return hash.ToHashCode();
    }

    #endregion
  }
}
