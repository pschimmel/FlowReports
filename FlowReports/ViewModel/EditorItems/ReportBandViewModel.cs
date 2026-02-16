using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ES.Tools.Core.MVVM;
using ES.Tools.UI;
using FlowReports.Model;
using FlowReports.Model.Events;
using FlowReports.Model.ReportItems;
using GongSolutions.Wpf.DragDrop;

namespace FlowReports.ViewModel.EditorItems
{
  public class ReportBandViewModel : ReportbandBaseViewModel<ReportBand>, IBandParentViewModel
  {
    #region Fields

    private readonly ReportBandCollection _subBands;

    #endregion

    #region Constructor

    public ReportBandViewModel(ReportBand bandOwner)
      : base(bandOwner)
    {
      Band.HeaderChanged += Band_HeaderChanged;
      Band.FooterChanged += Band_FooterChanged;
      _subBands = bandOwner.Bands;
      _subBands.SubBandAdded += SubBands_SubBandAdded;
      _subBands.SubBandRemoved += SubBands_SubBandRemoved;

      foreach (var subBand in _subBands)
      {
        var newReportBandVM = new ReportBandViewModel(subBand) { Parent = this };
        newReportBandVM.SelectionChanged += ReportBandVM_SelectionChanged;
        Bands.Add(newReportBandVM);
      }

      if (bandOwner.HeaderBand != null)
      {
        HeaderBand = new HeaderBandViewModel(bandOwner.HeaderBand) { Parent = this };
      }

      if (bandOwner.FooterBand != null)
      {
        FooterBand = new FooterBandViewModel(bandOwner.FooterBand) { Parent = this };
      }

      EditDetailsCommand = new ActionCommand(EditDetails, CanEditDetails);
    }

    #endregion

    #region Properties

    public ObservableCollection<ReportBandViewModel> Bands { get; } = new ObservableCollection<ReportBandViewModel>();

    public HeaderBandViewModel HeaderBand { get; private set; }

    public FooterBandViewModel FooterBand { get; private set; }

    public string DataSource
    {
      get => Band.DataSource;
      set
      {
        Band.DataSource = value;
        OnPropertyChanged(nameof(FullDataSource));
        OnPropertyChanged();
      }
    }

    public string FullDataSource => Parent is ReportBandViewModel reportBandViewModel ? reportBandViewModel.FullDataSource + "." + DataSource : DataSource;

    #endregion

    #region Public Methods

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

    public void MoveBandUp(ReportBandViewModel band)
    {
      _subBands.MoveBandUp(band.Band);
    }

    public void MoveBandDown(ReportBandViewModel band)
    {
      _subBands.MoveBandDown(band.Band);
    }

    public bool CanMoveBandUp(ReportBandViewModel band)
    {
      return _subBands.CanMoveBandUp(band.Band);
    }

    public bool CanMoveBandDown(ReportBandViewModel band)
    {
      return _subBands.CanMoveBandDown(band.Band);
    }

    #endregion

    #region Commands

    #region Edit Details

    public ICommand EditDetailsCommand { get; }

    public void EditDetails()
    {
      string oldDataSource = DataSource;
      var v = ViewFactory.Instance.CreateView(this);
      if (v.ShowDialog() != true)
      {
        DataSource = oldDataSource;
      }
      else if (ReportVM != null)
      {
        ReportVM.IsDirty = true;
      }
    }

    private bool CanEditDetails()
    {
      return IsSelected;
    }

    #endregion

    #endregion

    #region Event Handlers

    private void Band_HeaderChanged(object sender, EventArgs e)
    {
      if (Band.HeaderBand != null)
      {
        HeaderBand = new HeaderBandViewModel(Band.HeaderBand) { Parent = this };
      }
      else
      {
        HeaderBand.Dispose();
        HeaderBand = null;
      }
      OnPropertyChanged(nameof(HeaderBand));
    }

    private void Band_FooterChanged(object sender, EventArgs e)
    {
      if (Band.FooterBand != null)
      {
        FooterBand = new FooterBandViewModel(Band.FooterBand) { Parent = this };
      }
      else
      {
        FooterBand.Dispose();
        FooterBand = null;
      }
      OnPropertyChanged(nameof(FooterBand));
    }

    private void ReportBandVM_SelectionChanged(object sender, EventArgs e)
    {
      OnSelectionChanged(sender);
    }

    private void SubBands_SubBandAdded(object sender, BandsEventArgs e)
    {
      var newReportBandVM = new ReportBandViewModel(e.Item.Band) { Parent = this };
      newReportBandVM.SelectionChanged += ReportBandVM_SelectionChanged;
      Bands.Insert(e.Item.Index, newReportBandVM);

      if (!ReportEditorViewModel.IsInitializing)
      {
        newReportBandVM.IsSelected = true;
      }
    }

    private void SubBands_SubBandRemoved(object sender, BandsEventArgs e)
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

    #region IDropTarget

    protected override void ProtectedDragOver(IDropInfo dropInfo)
    {
      var parentContainer = dropInfo.VisualTarget.GetParent<Border>(x => x.Name == "BandContainer" || x.Name == "EditorContainer");
      bool parentIsBandContainer = parentContainer?.Name == "BandContainer";
      bool parentIsEditorContainer = parentContainer?.Name == "EditorContainer";
      Debug.Assert(parentIsBandContainer || parentIsEditorContainer, $"Dropped data must be of type {nameof(DataSourceListViewModel)} or {nameof(DataSourceItemViewModel)}.");

      if (dropInfo.Data is DataSourceListViewModel dataSourceListViewModel && parentIsBandContainer)
      {
        dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
        dropInfo.Effects = DragDropEffects.Move;
      }
      else
      {
        base.ProtectedDragOver(dropInfo);
      }
    }

    protected override void ProtectedDrop(IDropInfo dropInfo)
    {
      Debug.Assert(dropInfo.Data is DataSourceListViewModel || dropInfo.Data is DataSourceItemViewModel, $"Dropped data must be of type {nameof(DataSourceListViewModel)} or {nameof(DataSourceItemViewModel)}.");

      var parentContainer = dropInfo.VisualTarget.GetParent<Border>(x => x.Name == "BandContainer" || x.Name == "EditorContainer");
      bool parentIsBandContainer = parentContainer?.Name == "BandContainer";
      bool parentIsEditorContainer = parentContainer?.Name == "EditorContainer";
      Debug.Assert(parentIsBandContainer || parentIsEditorContainer);

      if (dropInfo.Data is DataSourceListViewModel dataSourceListViewModel && parentIsBandContainer)
      {
        DataSource = dataSourceListViewModel.Name;
      }
      else
      {
        base.ProtectedDrop(dropInfo);
      }
    }

    #endregion

    #region IDisposable

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);

      if (disposing)
      {
        Band.HeaderChanged -= Band_HeaderChanged;
        Band.FooterChanged -= Band_FooterChanged;

        HeaderBand?.Dispose();
        FooterBand?.Dispose();

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
