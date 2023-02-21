using System.Windows;
using System.Windows.Input;
using ES.Tools.Core.MVVM;
using FlowReports.Model.ReportItems;

namespace FlowReports.UI.ViewModel
{
  internal abstract class EditorItemViewModel<T> : ViewModelBase, IItemViewModel where T : ReportItem
  {
    #region Fields

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

    #endregion

    #region Commmands

    #region SelectCommand

    public ICommand SelectCommand { get; }

    public void Select()
    {
      IsSelected = !IsSelected;
    }

    #endregion

    #endregion
  }
}
