using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace FlowReports.UI.View.Adorners
{
  internal class ReportItemAdorner : Adorner
  {
    #region Events

    public event Action<Rect> ChangeLocation;

    #endregion

    #region Fields

    private static Size _resizeGripSize = new(5, 5);
    private const double _borderThickness = 2;
    private Point _resizeMouseStartPosition;
    private double _left;
    private double _top;
    private double _width;
    private double _height;
    private Move _move = Move.None;
    private readonly Lazy<Brush> _resizeThumbBackgroundBrush;
    private readonly Lazy<Brush> _resizeBorderBrush;
    private readonly Lazy<Brush> _resizeBorderBackgroundBrush;

    [Flags]
    private enum Move
    {
      None = 0,
      Left = 1,
      Right = 2,
      Top = 4,
      Bottom = 8,
      LeftTop = Left | Top,
      LeftBottom = Left | Bottom,
      RightTop = Right | Top,
      RightBottom = Right | Bottom,
      All = LeftBottom | RightTop
    }

    #endregion

    #region Constructor

    public ReportItemAdorner(FrameworkElement adornedElement)
      : base(adornedElement)
    {
      _resizeThumbBackgroundBrush = new Lazy<Brush>(() => (Brush)FindResource("ReportItem.ResizeAdorner.Thumb.Background"));
      _resizeBorderBrush = new Lazy<Brush>(() => (Brush)FindResource("ReportItem.ResizeAdorner.Border"));
      _resizeBorderBackgroundBrush = new Lazy<Brush>(() => (Brush)FindResource("ReportItem.ResizeAdorner.Background"));
      _left = 0;
      _top = 0;
      _width = adornedElement.Width;
      _height = adornedElement.Height;
    }

    #endregion

    #region Overwritten Methods

    protected override void OnRender(DrawingContext drawingContext)
    {
      base.OnRender(drawingContext);

      Debug.Assert(_resizeThumbBackgroundBrush.Value != null);
      Debug.Assert(_resizeBorderBrush.Value != null);
      Debug.Assert(_resizeBorderBackgroundBrush.Value != null);

      if (_move != Move.None)
      {
        drawingContext.DrawRectangle(_resizeBorderBackgroundBrush.Value, new Pen(_resizeBorderBrush.Value, _borderThickness), new Rect(_left, _top, _width, _height));
      }

      drawingContext.DrawRectangle(_resizeThumbBackgroundBrush.Value, null, GetLeftTopGripper());     // Left top
      drawingContext.DrawRectangle(_resizeThumbBackgroundBrush.Value, null, GetRightTopGripper());    // Right top
      drawingContext.DrawRectangle(_resizeThumbBackgroundBrush.Value, null, GetLeftBottomGripper());  // Left bottom
      drawingContext.DrawRectangle(_resizeThumbBackgroundBrush.Value, null, GetRightBottomGripper()); // Right bottom
      drawingContext.DrawRectangle(Brushes.Transparent, null, GetBackground()); // Moving
    }

    protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      base.OnPreviewMouseLeftButtonDown(e);

      var background = GetBackground();
      var leftTop = GetLeftTopGripper();
      var rightTop = GetRightTopGripper();
      var leftBottom = GetLeftBottomGripper();
      var rightBottom = GetRightBottomGripper();

      _resizeMouseStartPosition = e.GetPosition(AdornedElement);

      if (PointIsOver(_resizeMouseStartPosition, leftTop))
      {
        _move = Move.LeftTop;
      }
      else if (PointIsOver(_resizeMouseStartPosition, rightBottom))
      {
        _move = Move.RightBottom;
      }
      else if (PointIsOver(_resizeMouseStartPosition, rightTop))
      {
        _move = Move.RightTop;
      }
      else if (PointIsOver(_resizeMouseStartPosition, leftBottom))
      {
        _move = Move.LeftBottom;
      }
      else if (PointIsOver(_resizeMouseStartPosition, background))
      {
        _move = Move.All;
      }

      if (_move != Move.None)
      {
        CaptureMouse(); // Capture the mouse to receive all move events
      }
    }

    protected override void OnPreviewMouseMove(MouseEventArgs e)
    {
      base.OnPreviewMouseMove(e);

      var pos = e.GetPosition(AdornedElement);
      var background = GetBackground();
      var leftTop = GetLeftTopGripper();
      var rightTop = GetRightTopGripper();
      var leftBottom = GetLeftBottomGripper();
      var rightBottom = GetRightBottomGripper();

      if (PointIsOver(pos, leftTop))
      {
        Mouse.OverrideCursor = Cursors.SizeNWSE;
      }
      else if (PointIsOver(pos, rightBottom))
      {
        Mouse.OverrideCursor = Cursors.SizeNWSE;
      }
      else if (PointIsOver(pos, rightTop))
      {
        Mouse.OverrideCursor = Cursors.SizeNESW;
      }
      else if (PointIsOver(pos, leftBottom))
      {
        Mouse.OverrideCursor = Cursors.SizeNESW;
      }
      else if (PointIsOver(pos, background))
      {
        Mouse.OverrideCursor = Cursors.SizeAll;
      }

      bool mouseIsDown = e.LeftButton == MouseButtonState.Pressed;
      if (mouseIsDown)
      {
        double deltaX = pos.X - _resizeMouseStartPosition.X;
        double deltaY = pos.Y - _resizeMouseStartPosition.Y;

        if (_move == Move.All)
        {
          _left = deltaX;
          _top = deltaY;
        }
        else
        {
          if (_move.HasFlag(Move.Left))
          {
            _left = deltaX;
            _width = Math.Max((AdornedElement as FrameworkElement).Width - deltaX, 0);
          }

          if (_move.HasFlag(Move.Right))
          {
            _width = Math.Max((AdornedElement as FrameworkElement).Width + deltaX, 0);
          }

          if (_move.HasFlag(Move.Top))
          {
            _top = deltaY;
            _height = Math.Max((AdornedElement as FrameworkElement).Height - deltaY, 0);
          }

          if (_move.HasFlag(Move.Bottom))
          {
            _height = Math.Max((AdornedElement as FrameworkElement).Height + deltaY, 0);
          }
        }

        if (_move != Move.None)
        {
          InvalidateVisual();
        }
      }
    }

    protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
    {
      base.OnPreviewMouseLeftButtonUp(e);

      if (_move != Move.None)
      {
        ChangeLocation?.Invoke(new Rect(_left, _top, _width, _height));
        _resizeMouseStartPosition = new Point();
        ReleaseMouseCapture();
        _left = 0;  // Triggering the event will move the actual item,
        _top = 0;   // hence the location of the adorner needs to be reset
        InvalidateVisual();
      }

      _move = Move.None;
    }

    protected override void OnMouseLeave(MouseEventArgs e)
    {
      base.OnMouseLeave(e);
      Mouse.OverrideCursor = null;
    }

    #endregion

    #region Private Methods

    private Rect GetBackground()
    {
      return new Rect(_left, _top, _width, _height);
    }

    private Rect GetLeftTopGripper()
    {
      return new Rect(new Point(_left - _resizeGripSize.Width, _top - _resizeGripSize.Height), _resizeGripSize);
    }

    private Rect GetRightTopGripper()
    {
      return new(new Point(_left + _width, _top - _resizeGripSize.Height), _resizeGripSize);
    }

    private Rect GetLeftBottomGripper()
    {
      return new(new Point(_left - _resizeGripSize.Width, _top + _height), _resizeGripSize);
    }

    private Rect GetRightBottomGripper()
    {
      return new(new Point(_left + _width, _top + _height), _resizeGripSize);
    }

    private static bool PointIsOver(Point point, Rect dragThumb)
    {
      dragThumb.Inflate(1, 1);
      return dragThumb.Contains(point);
    }

    #endregion
  }
}