using System.Diagnostics;
using System.Windows;
using FlowReports.Model.ReportItems;
using GongSolutions.Wpf.DragDrop;

namespace FlowReports.UI.ViewModel
{
  internal class TextItemViewModel : EditorItemViewModel<TextItem>, IDropTarget
  {
    public TextItemViewModel(TextItem item, ReportBandViewModel bandVM)
      : base(item, bandVM)
    { }

    public string Text
    {
      get => _item.Text;
      set
      {
        if (_item.Text != value)
        {
          _item.Text = value;
          OnPropertyChanged();
        }
      }
    }

    public string Format
    {
      get => _item.Format;
      set
      {
        if (_item.Format != value)
        {
          _item.Format = value;
          OnPropertyChanged();
        }
      }
    }

    #region IDropTarget

    void IDropTarget.DragOver(IDropInfo dropInfo)
    {
      Debug.Assert(dropInfo.Data is DataSourceItemViewModel, $"Dropped data must be of type {nameof(DataSourceItemViewModel)}.");

      if (dropInfo.Data is DataSourceItemViewModel)
      {
        dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
        dropInfo.Effects = DragDropEffects.Move;
      }
      else
      {
        Debug.Fail($"Dropped data must be of type {nameof(DataSourceItemViewModel)}.");
      }
    }

    void IDropTarget.Drop(IDropInfo dropInfo)
    {
      Debug.Assert(dropInfo.Data is DataSourceItemViewModel, $"Dropped data must be of type {nameof(DataSourceItemViewModel)}.");

      if (dropInfo.Data is DataSourceItemViewModel dataSourceItemViewModel)
      {
        Text = $"[{dataSourceItemViewModel.Name}]";
      }
    }

    void IDropTarget.DragEnter(IDropInfo dropInfo)
    { }

    void IDropTarget.DragLeave(IDropInfo dropInfo)
    { }

    #endregion
  }
}
