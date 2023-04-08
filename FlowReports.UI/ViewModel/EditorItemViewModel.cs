using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using ES.Tools.Core.MVVM;
using FlowReports.Model;
using FlowReports.Model.ReportItems;
using GongSolutions.Wpf.DragDrop;

namespace FlowReports.UI.ViewModel
{
  internal abstract class EditorItemViewModel<T> : ViewModelBase, IEditorItemViewModel, IDropTarget where T : ReportItem
  {
    #region Fields

    private const double MoveDelta = 5.0;
    protected readonly T _item;
    private readonly ReportBandViewModel _bandVM;
    private bool _isSelected;

    #endregion

    #region Constructor

    protected EditorItemViewModel(T item, ReportBandViewModel bandVM)
    {
      _item = item;
      _bandVM = bandVM;
      SelectCommand = new ActionCommand(Select);
      DeleteCommand = new ActionCommand(Delete, CanDelete);
      MoveLeftCommand = new ActionCommand(MoveLeft, CanMoveLeft);
      MoveRightCommand = new ActionCommand(MoveRight, CanMoveRight);
      MoveUpCommand = new ActionCommand(MoveUp, CanMoveUp);
      MoveDownCommand = new ActionCommand(MoveDown, CanMoveDown);
    }

    #endregion

    #region Properties

    public ReportItem Item => _item;

    public double Left
    {
      get => _item.Left;
      set
      {
        if (_item.Left != value)
        {
          _item.Left = value;
          OnPropertyChanged();
        }
      }
    }

    public double Top
    {
      get => _item.Top;
      set
      {
        if (_item.Top != value)
        {
          _item.Top = value;
          OnPropertyChanged();
        }
      }
    }

    public double Width
    {
      get => _item.Width;
      set
      {
        if (_item.Width != value && value > 0.0)
        {
          _item.Width = value;
          OnPropertyChanged();
        }
      }
    }

    public double Height
    {
      get => _item.Height;
      set
      {
        if (_item.Height != value && value > 0.0)
        {
          _item.Height = value;
          OnPropertyChanged();
        }
      }
    }

    public Point Location => new(Left, Top);

    public Size Size => new(Width, Height);

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
            _bandVM.Select();
          }
          OnPropertyChanged();
        }
      }
    }

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

    #endregion

    #region Commmands

    #region Select

    public ICommand SelectCommand { get; }

    private void Select()
    {
      IsSelected = true;
    }

    #endregion

    #region Delete

    public ICommand DeleteCommand { get; }

    private void Delete()
    {
      _bandVM.RemoveItem(this);
    }

    private bool CanDelete()
    {
      return IsSelected;
    }

    #endregion

    #region Move Left

    public ICommand MoveLeftCommand { get; }

    private void MoveLeft()
    {
      Left -= Math.Min(Left, MoveDelta);
    }

    private bool CanMoveLeft()
    {
      return IsSelected && Left > 0;
    }

    #endregion

    #region Move Right

    public ICommand MoveRightCommand { get; }

    private void MoveRight()
    {
      Left += MoveDelta;
    }

    private bool CanMoveRight()
    {
      return IsSelected;
    }

    #endregion

    #region Move Up

    public ICommand MoveUpCommand { get; }

    private void MoveUp()
    {
      Top -= Math.Min(Top, MoveDelta);
    }

    private bool CanMoveUp()
    {
      return IsSelected && Top > 0;
    }

    #endregion

    #region Move Down

    public ICommand MoveDownCommand { get; }

    private void MoveDown()
    {
      Top += MoveDelta;
    }

    private bool CanMoveDown()
    {
      return IsSelected;
    }

    #endregion

    #endregion

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
