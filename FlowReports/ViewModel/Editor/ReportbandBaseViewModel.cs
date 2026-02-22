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

namespace FlowReports.ViewModel.Editor
{
  public abstract class ReportbandBaseViewModel<T> : ViewModelBase, IItemContainerViewModel, IDropTarget where T : ReportBandBase
  {
    #region Events

    public event EventHandler ItemSelected;
    internal event EventHandler SelectionChanged;

    #endregion

    #region Fields

    private bool _isSelected;

    #endregion

    #region Constructor

    protected ReportbandBaseViewModel(T bandOwner)
    {
      Band = bandOwner;
      Items = new ObservableCollection<IEditorItemViewModel>();

      foreach (var item in bandOwner.Items)
      {
        var itemVM = ViewModelFactory.CreateItemViewModel(item, this);
        Items.Add(itemVM);
        itemVM.PropertyChanged += ItemVM_PropertyChanged;
      }

      bandOwner.ItemAdded += Band_ItemAdded;
      bandOwner.ItemRemoved += Band_ItemRemoved;

      SelectCommand = new ActionCommand(Select);
      DeselectItemsCommand = new ActionCommand(DeselectItems);
    }

    #endregion

    #region Properties

    public Guid ID => Band.ID;

    internal T Band { get; }

    public IBandParentViewModel Parent { get; internal set; }

    public ObservableCollection<IEditorItemViewModel> Items { get; }

    public ReportViewModel ReportVM
    {
      get => Parent switch
      {
        ReportViewModel reportVM => reportVM,
        ReportBandViewModel rbVM => rbVM.ReportVM,
        _ => null,
      };
    }

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

          OnSelectionChanged(this);
          OnPropertyChanged();
        }
      }
    }

    public double Height
    {
      get => Band.Height ?? ReportBandBase.DefaultHeight;
      set
      {
        if (Band.Height != value)
        {
          Band.Height = value;
          HeightAuto = false;
        }
      }
    }

    public bool HeightAuto
    {
      get => Band.Height == null;
      set
      {
        if (value && Band.Height != null)
        {
          Band.Height = null;
          OnPropertyChanged(nameof(Height));
          OnPropertyChanged();
        }
        else if (!value && Band.Height == null)
        {
          Band.Height = ReportBandBase.DefaultHeight;
          OnPropertyChanged(nameof(Height));
          OnPropertyChanged();
        }
      }
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

    private void Band_ItemAdded(object sender, ReportItemsEventArgs e)
    {
      var itemVM = ViewModelFactory.CreateItemViewModel(e.Item, this);
      Items.Add(itemVM);
      itemVM.PropertyChanged += ItemVM_PropertyChanged;
      itemVM.IsSelected = true;
      ReportVM.IsDirty = true;
    }

    private void Band_ItemRemoved(object sender, ReportItemsEventArgs e)
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

    #region Private / Protected Methods

    /// <summary>
    /// Selects the current band. Deselects all others, beginning at the report level.
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
    private static void SelectBand(IBandParentViewModel parentVM, IItemContainerViewModel reportBandVM)
    {
      if (parentVM != null)
      {
        foreach (var childBand in parentVM.Bands)
        {
          childBand.IsSelected = childBand == reportBandVM;
          SelectBand(childBand, reportBandVM);
        }

        if (parentVM is ReportBandViewModel bandViewModel)
        {
          if (bandViewModel.HeaderBand != null)
          {
            bandViewModel.HeaderBand.IsSelected = bandViewModel.HeaderBand == reportBandVM;
          }
          if (bandViewModel.FooterBand != null)
          {
            bandViewModel.FooterBand.IsSelected = bandViewModel.FooterBand == reportBandVM;
          }
        }
      }
    }

    protected void OnSelectionChanged(object sender)
    {
      SelectionChanged?.Invoke(sender, EventArgs.Empty);
    }

    #endregion

    #region IDropTarget

    void IDropTarget.DragOver(IDropInfo dropInfo)
    {
      ProtectedDragOver(dropInfo);
    }

    protected virtual void ProtectedDragOver(IDropInfo dropInfo)
    {
      var parentContainer = dropInfo.VisualTarget.GetParent<Border>(x => x.Name == "BandContainer" || x.Name == "EditorContainer");
      bool parentIsBandContainer = parentContainer?.Name == "BandContainer";
      bool parentIsEditorContainer = parentContainer?.Name == "EditorContainer";
      Debug.Assert(parentIsBandContainer || parentIsEditorContainer, $"Dropped data must be of type {nameof(DataSourceListViewModel)} or {nameof(DataSourceItemViewModel)}.");

      if (dropInfo.Data is DataSourceItemViewModel dataSourceItemViewModel && parentIsEditorContainer)
      {
        dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
        dropInfo.Effects = DragDropEffects.Copy;
      }
    }

    void IDropTarget.Drop(IDropInfo dropInfo)
    {
      ProtectedDrop(dropInfo);
    }

    protected virtual void ProtectedDrop(IDropInfo dropInfo)
    {
      Debug.Assert(dropInfo.Data is DataSourceListViewModel || dropInfo.Data is DataSourceItemViewModel, $"Dropped data must be of type {nameof(DataSourceListViewModel)} or {nameof(DataSourceItemViewModel)}.");

      var parentContainer = dropInfo.VisualTarget.GetParent<Border>(x => x.Name == "BandContainer" || x.Name == "EditorContainer");
      bool parentIsBandContainer = parentContainer?.Name == "BandContainer";
      bool parentIsEditorContainer = parentContainer?.Name == "EditorContainer";
      Debug.Assert(parentIsBandContainer || parentIsEditorContainer);

      if (dropInfo.Data is DataSourceItemViewModel dataSourceItemViewModel && parentIsEditorContainer)
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

    #region IDisposable

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);

      Band.ItemAdded -= Band_ItemAdded;
      Band.ItemRemoved -= Band_ItemRemoved;

      foreach (var itemVM in Items)
      {
        itemVM.PropertyChanged -= ItemVM_PropertyChanged;
        itemVM.Dispose();
      }
    }

    #endregion
  }
}
