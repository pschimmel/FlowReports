using System.Collections.ObjectModel;
using FlowReports.Model;
using FlowReports.Model.Events;
using FlowReports.Model.ReportItems;

namespace FlowReports.ViewModel.EditorItems
{
  public abstract class BandContainerViewModel : ViewModelBase, IBandParentViewModel
  {
    #region Events

    internal event EventHandler SelectionChanged;

    #endregion

    #region Fields

    private readonly ReportBandCollection _subBands;

    #endregion

    #region Constructor

    protected BandContainerViewModel(ReportBandCollection subBands)
    {
      _subBands = subBands;
      subBands.SubBandAdded += SubBands_SubBandAdded;
      subBands.SubBandRemoved += SubBands_SubBandRemoved;

      foreach (var subBand in subBands)
      {
        var newReportBandVM = new ReportBandViewModel(subBand) { Parent = this };
        newReportBandVM.SelectionChanged += ReportBandVM_SelectionChanged;
        Bands.Add(newReportBandVM);
      }
    }

    #endregion

    #region Properties

    public ObservableCollection<ReportBandViewModel> Bands { get; } = new ObservableCollection<ReportBandViewModel>();

    #endregion

    #region Methods

    public void AddBand()
    {
      _subBands.AddBand();
    }

    public void AddBand(ReportBandViewModel otherBand, InsertLocation location)
    {
      _subBands.AddBand(otherBand.Band, location);
    }

    public void RemoveBand(ReportBandViewModel subBand)
    {
      _subBands.RemoveBand(subBand.Band);
    }

    protected void OnSelectionChanged()
    {
      SelectionChanged?.Invoke(this, EventArgs.Empty);
    }

    #endregion

    #region Event Handlers

    private void ReportBandVM_SelectionChanged(object sender, EventArgs e)
    {
      SelectionChanged?.Invoke(sender, EventArgs.Empty);
    }

    protected virtual void SubBands_SubBandAdded(object sender, BandsEventArgs e)
    {
      var newReportBandVM = new ReportBandViewModel(e.Item.Band) { Parent = this };
      newReportBandVM.SelectionChanged += ReportBandVM_SelectionChanged;
      Bands.Insert(e.Item.Index, newReportBandVM);

      if (!ReportEditorViewModel.IsInitializing)
      {
        newReportBandVM.IsSelected = true;
      }
    }

    protected virtual void SubBands_SubBandRemoved(object sender, BandsEventArgs e)
    {
      var bandVM = Bands.FirstOrDefault(x => x.Band == e.Item.Band);
      if (bandVM != null)
      {
        bandVM.SelectionChanged -= ReportBandVM_SelectionChanged;
        Bands.Remove(bandVM);
        bandVM.Dispose();
      }
    }

    #endregion

    #region IDisposable

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);

      if (disposing)
      {
        foreach (var subBand in Bands)
        {
          subBand.SelectionChanged -= ReportBandVM_SelectionChanged;
          subBand.Dispose();
        }

        _subBands.SubBandAdded -= SubBands_SubBandAdded;
        _subBands.SubBandRemoved -= SubBands_SubBandRemoved;
      }
    }

    #endregion
  }
}
