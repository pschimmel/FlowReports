using System.Diagnostics;
using System.Windows;
using FlowReports.Model;
using FlowReports.Model.ReportItems;
using GongSolutions.Wpf.DragDrop;

namespace FlowReports.UI.ViewModel
{
  internal class BooleanItemViewModel : EditorItemViewModel<BooleanItem>, IDropTarget
  {
    public BooleanItemViewModel(BooleanItem item, ReportBandViewModel bandVM)
      : base(item, bandVM)
    { }

    public string DataSource
    {
      get => _item.DataSource;
      set
      {
        if (_item.DataSource != value)
        {
          _item.DataSource = value;
          OnPropertyChanged();
        }
      }
    }

    #region IDropTarget

    void IDropTarget.DragOver(IDropInfo dropInfo)
    {
      if (dropInfo.Data is DataSourceItemViewModel)
      {
        dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
        dropInfo.Effects = DragDropEffects.Move;
      }
    }

    void IDropTarget.Drop(IDropInfo dropInfo)
    {
      Debug.Assert(dropInfo.Data is DataSourceItemViewModel, $"Dropped data must be of type {nameof(DataSourceItemViewModel)}.");

      if (dropInfo.Data is DataSourceItemViewModel dataSourceItemViewModel)
      {
        DataSource = $"{Settings.DATASOURCE_OPENING_BRACKET}{dataSourceItemViewModel.Name}{Settings.DATASOURCE_CLOSING_BRACKET}";
      }
    }

    void IDropTarget.DragEnter(IDropInfo dropInfo)
    { }

    void IDropTarget.DragLeave(IDropInfo dropInfo)
    { }

    #endregion
  }
}
