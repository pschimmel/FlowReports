using System.Collections;
using FlowReports.Model.Events;
using FlowReports.Model.Tools;

namespace FlowReports.Model.ReportItems
{
  public class ReportBandCollection : IEnumerable<ReportBand>
  {
    public event EventHandler<BandsEventArgs> SubBandAdded;
    public event EventHandler<BandsEventArgs> SubBandRemoved;

    private readonly List<ReportBand> _bands = new();

    public ReportBandCollection()
    { }

    public ReportBand AddBand(string dataSource = null)
    {
      var band = new ReportBand { DataSource = dataSource };
      _bands.Add(band);
      int index = _bands.IndexOf(band);
      OnSubBandAdded(index, band);
      return band;
    }

    public ReportBand AddBand(ReportBand otherBand, InsertLocation location, string dataSource = null)
    {
      if (otherBand is null)
      {
        throw new ArgumentNullException(nameof(otherBand));
      }

      int otherIndex = _bands.IndexOf(otherBand);
      var band = new ReportBand { DataSource = dataSource };

      switch (location)
      {
        case InsertLocation.Before:
          _bands.Insert(otherIndex, band);
          break;
        case InsertLocation.After:
          if (otherIndex < _bands.Count - 1)
          {
            _bands.Insert(otherIndex + 1, band);
          }
          else
          {
            _bands.Add(band);
          }
          break;
        default:
          throw new ArgumentException("Unknown Enumeration value.");
      }

      int index = _bands.IndexOf(band);
      OnSubBandAdded(index, band);
      return band;
    }

    public int RemoveBand(ReportBand band)
    {
      int index = _bands.IndexOf(band);
      _bands.Remove(band);
      OnSubBandRemoved(index, band);
      return index;
    }

    public void Clear()
    {
      foreach (var band in _bands)
      {
        RemoveBand(band);
      }
    }

    private void OnSubBandAdded(int index, ReportBand band)
    {
      SubBandAdded?.Invoke(this, new BandsEventArgs(index, band));
    }

    private void OnSubBandRemoved(int index, ReportBand band)
    {
      SubBandRemoved?.Invoke(this, new BandsEventArgs(index, band));
    }

    public IEnumerator<ReportBand> GetEnumerator()
    {
      return _bands.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return _bands.GetEnumerator();
    }

    public override bool Equals(object obj)
    {
      return obj is ReportBandCollection other &&
        List.Equals(other._bands, _bands);
    }

    public override int GetHashCode()
    {
      return 7 ^ _bands.GetHashCode();
    }
  }
}
