using System.Windows;
using FlowReports.Model.ReportItems;

namespace FlowReports.UI.ViewModel
{
  internal abstract class ItemViewModel<T> : IItemViewModel where T : ReportItem
  {
    #region Fields

    protected readonly T _item;
    protected readonly object _data;
    private readonly double _deltaY;

    #endregion

    #region Constructor

    protected ItemViewModel(T item, object data, double deltaY)
    {
      _item = item;
      _data = data;
      _deltaY = deltaY;
    }

    #endregion

    #region Properties

    public double Left => _item.Left;

    public double Top => _deltaY + _item.Top;

    public double Width => _item.Width;

    public double Height => _item.Height;

    public Point Location => new(Left, Top);

    public Size Size => new(Width, Height);

    #endregion
  }
}
