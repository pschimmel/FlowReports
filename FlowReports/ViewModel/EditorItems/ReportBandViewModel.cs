using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ES.Tools.Core.MVVM;
using ES.Tools.UI;
using FlowReports.Model.DataSources;
using FlowReports.Model.Events;
using FlowReports.Model.ReportItems;
using GongSolutions.Wpf.DragDrop;

namespace FlowReports.ViewModel.EditorItems
{
  public class ReportBandViewModel : BandContainerViewModel, IDropTarget
  {
    #region Event

    public event EventHandler ItemSelected;

    #endregion

    #region Fields

    private bool _isSelected;

    #endregion

    #region Constructor

    public ReportBandViewModel(ReportBand band)
      : base(band.SubBands)
    {
      Band = band;
      Items = new ObservableCollection<IEditorItemViewModel>();

      foreach (var item in band.Items)
      {
        var itemVM = ViewModelFactory.CreateItemViewModel(item, this);
        Items.Add(itemVM);
        itemVM.PropertyChanged += ItemVM_PropertyChanged;
      }

      band.ItemAdded += Band_ItemAdded;
      band.ItemRemoved += Band_ItemRemoved;

      SelectCommand = new ActionCommand(Select);
      DeselectItemsCommand = new ActionCommand(DeselectItems);
      EditDetailsCommand = new ActionCommand(EditDetails, CanEditDetails);
    }

    #endregion

    #region Properties

    public Guid ID => Band.ID;

    internal ReportBand Band { get; private set; }

    public double Height
    {
      get => Band.Height;
      set => Band.Height = value;
    }

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


    public bool IsSelected
    {
      get => _isSelected;
      set
      {
        if (_isSelected != value)
        {
          _isSelected = value;
          if (_isSelected)
          {
            SelectBand();
          }
          else
          {
            foreach (var item in Items)
            {
              item.IsSelected = false;
            }
          }

          OnSelectionChanged();
          OnPropertyChanged();
        }
      }
    }

    public IBandParentViewModel Parent { get; internal set; }

    public ReportViewModel ReportVM
    {
      get => Parent switch
      {
        ReportViewModel reportVM => reportVM,
        ReportBandViewModel rbVM => rbVM.ReportVM,
        _ => null,
      };
    }

    public ObservableCollection<IEditorItemViewModel> Items { get; }

    #endregion

    #region Commands

    #region Select

    public ICommand SelectCommand { get; }

    public void Select()
    {
      IsSelected = true;
    }

    #endregion

    #region Deselect Items

    public ICommand DeselectItemsCommand { get; }

    private void DeselectItems()
    {
      IsSelected = true;
      foreach (var item in Items)
      {
        item.IsSelected = false;
      }
    }

    #endregion

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

    #region Public Methods

    public IEditorItemViewModel AddTextItem()
    {
      Band.AddTextItem();
      Debug.Assert(Items.Last() is EditorTextItemViewModel);
      return Items.Last();
    }

    public IEditorItemViewModel AddBooleanItem()
    {
      Band.AddBooleanItem();
      Debug.Assert(Items.Last() is EditorBooleanItemViewModel);
      return Items.Last();
    }

    public IEditorItemViewModel AddImageItem()
    {
      Band.AddImageItem();
      Debug.Assert(Items.Last() is EditorImageItemViewModel);
      return Items.Last();
    }

    public void RemoveItem(IEditorItemViewModel itemVM)
    {
      Band.RemoveItem(itemVM?.Item);
    }

    #endregion

    #region Event Handlers

    private void ItemVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(IEditorItemViewModel.IsSelected) && sender is IEditorItemViewModel viewModel)
      {
        if (viewModel.IsSelected)
        {
          foreach (var item in Items)
          {
            if (item != sender)
            {
              item.IsSelected = false;
            }
          }

          ItemSelected?.Invoke(sender, EventArgs.Empty);
        }
      }
      else
      {
        ReportVM.IsDirty = true;
      }
    }

    private void Band_ItemAdded(object sender, ItemsEventArgs e)
    {
      var itemVM = ViewModelFactory.CreateItemViewModel(e.Item, this);
      Items.Add(itemVM);
      itemVM.PropertyChanged += ItemVM_PropertyChanged;
      itemVM.IsSelected = true;
      ReportVM.IsDirty = true;
    }

    private void Band_ItemRemoved(object sender, ItemsEventArgs e)
    {
      foreach (var itemVM in Items.ToList())
      {
        if (itemVM.Item == e.Item)
        {
          Items.Remove(itemVM);
          itemVM.PropertyChanged -= ItemVM_PropertyChanged;
          ReportVM.IsDirty = true;
        }
      }
    }

    #endregion

    #region Private Methods

    private void SelectItem(IEditorItemViewModel itemVM)
    {
      IsSelected = true;
      SelectItem(ReportVM, itemVM);
    }

    private static void SelectItem(IBandParentViewModel parentVM, IEditorItemViewModel itemVM)
    {
      if (parentVM != null)
      {
        foreach (var childBand in parentVM.Bands)
        {
          foreach (var item in childBand.Items)
          {
            item.IsSelected = item == itemVM;
          }

          SelectItem(childBand, itemVM);
        }
      }
    }

    /// <summary>
    /// Selects the current band. Deselects all others, beginning at the report.
    /// </summary>
    private void SelectBand()
    {
      var parent = Parent;
      while (parent is ReportBandViewModel reportBandViewModel)
      {
        parent = reportBandViewModel.Parent;
      }

      Debug.Assert(parent is ReportViewModel);
      SelectBand(parent, this);
    }

    /// <summary>
    /// Selects the given band. Deselects all children.
    /// </summary>
    private static void SelectBand(IBandParentViewModel parentVM, ReportBandViewModel reportBandVM)
    {
      if (parentVM != null)
      {
        foreach (var childBand in parentVM.Bands)
        {
          childBand.IsSelected = childBand == reportBandVM;
          SelectBand(childBand, reportBandVM);
        }
      }
    }

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);

      if (disposing)
      {
        Band.ItemAdded -= Band_ItemAdded;
        Band.ItemRemoved -= Band_ItemRemoved;

        foreach (var itemVM in Items)
        {
          itemVM.PropertyChanged -= ItemVM_PropertyChanged;
          itemVM.Dispose();
        }
      }
    }

    #endregion

    #region IDropTarget

    void IDropTarget.DragOver(IDropInfo dropInfo)
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
      else if (dropInfo.Data is DataSourceItemViewModel dataSourceItemViewModel && parentIsEditorContainer)
      {
        dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
        dropInfo.Effects = DragDropEffects.Copy;
      }
    }

    void IDropTarget.Drop(IDropInfo dropInfo)
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
      else if (dropInfo.Data is DataSourceItemViewModel dataSourceItemViewModel && parentIsEditorContainer)
      {
        var reportItem = dataSourceItemViewModel.Item.GetReportItem(dropInfo.DropPosition.X, dropInfo.DropPosition.Y);
        Band.AddReportItem(reportItem);
      }
    }

    void IDropTarget.DragEnter(IDropInfo dropInfo)
    { }

    void IDropTarget.DragLeave(IDropInfo dropInfo)
    { }

    #endregion
  }
}
