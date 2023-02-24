using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace FlowReports.UI.View
{
  internal class ReportItemAdorner : Adorner
  {
    public event Action<double> MoveRightBorder;
    public event Action<double> MoveLeftBorder;
    public event Action<double> MoveTopBorder;
    public event Action<double> MoveBottomBorder;

    private static Size _size = new(5, 5);
    private Point _resizeStartPosition;
    bool _moveLeft = false;
    bool _moveRight = false;
    bool _moveTop = false;
    bool _moveBottom = false;
    private readonly Lazy<Brush> _resizeThumbBackground;

    public ReportItemAdorner(UIElement adornedElement)
      : base(adornedElement)
    {
      _resizeThumbBackground = new Lazy<Brush>(() => (Brush)FindResource("ReportItem.ResizeThumb.Background"));
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
      base.OnRender(drawingContext);

      Debug.Assert(_resizeThumbBackground.Value != null);
      drawingContext.DrawRectangle(_resizeThumbBackground.Value, null, GetLeftTop());     // Left top
      drawingContext.DrawRectangle(_resizeThumbBackground.Value, null, GetRightTop());    // Right top
      drawingContext.DrawRectangle(_resizeThumbBackground.Value, null, GetLeftBottom());  // Left bottom
      drawingContext.DrawRectangle(_resizeThumbBackground.Value, null, GetRightBottom()); // Right bottom
    }

    private Rect GetLeftTop()
    {
      return new Rect(new Point(-_size.Width, -_size.Height), _size);
    }

    private Rect GetRightTop()
    {
      return new(new Point(ActualWidth, -_size.Height), _size);
    }

    private Rect GetLeftBottom()
    {
      return new(new Point(-_size.Width, ActualHeight), _size);
    }

    private Rect GetRightBottom()
    {
      return new(new Point(ActualWidth, ActualHeight), _size);
    }

    protected override void OnMouseLeave(MouseEventArgs e)
    {
      base.OnMouseLeave(e);
      Mouse.OverrideCursor = null;
    }

    protected override void OnPreviewMouseMove(MouseEventArgs e)
    {
      base.OnPreviewMouseMove(e);
      Point pos = e.GetPosition(AdornedElement);
      var leftTop = GetLeftTop();
      var rightTop = GetRightTop();
      var leftBottom = GetLeftBottom();
      var rightBottom = GetRightBottom();

      bool mouseIsDown = e.LeftButton == MouseButtonState.Pressed;

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

      if (mouseIsDown)
      {
        if (_moveRight)
        {
          var deltaRight = pos.X - _resizeStartPosition.X;
          _resizeStartPosition = new Point(_resizeStartPosition.X + deltaRight, _resizeStartPosition.Y);
          MoveRightBorder?.Invoke(deltaRight);
        }
        if (_moveBottom)
        {
          var deltaBottom = pos.Y - _resizeStartPosition.Y;
          _resizeStartPosition = new Point(_resizeStartPosition.X, _resizeStartPosition.Y + deltaBottom);
          MoveBottomBorder?.Invoke(deltaBottom);
        }
        if (_moveLeft)
        {
          var deltaLeft = pos.X - _resizeStartPosition.X;
          _resizeStartPosition = new Point(_resizeStartPosition.X + deltaLeft, _resizeStartPosition.Y);
          MoveLeftBorder?.Invoke(deltaLeft);
        }
        if (_moveTop)
        {
          var deltaTop = pos.Y - _resizeStartPosition.Y;
          _resizeStartPosition = new Point(_resizeStartPosition.X, _resizeStartPosition.Y + deltaTop);
          MoveTopBorder?.Invoke(deltaTop);
        }
      }
    }

    private static bool PointIsOver(Point point, Rect dragThumb)
    {
      dragThumb.Inflate(1, 1);
      return dragThumb.Contains(point);
      //      return point.X >= dragThumb.Left - 1 && point.X <= dragThumb.Right + 1 && point.Y >= dragThumb.Top - 1 && point.Y <= dragThumb.Bottom + 1;
    }

    protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      base.OnPreviewMouseLeftButtonDown(e);
      _resizeStartPosition = e.GetPosition(AdornedElement);
      CaptureMouse(); // Capture the mouse to receive all move events
      var leftTop = GetLeftTop();
      var rightTop = GetRightTop();
      var leftBottom = GetLeftBottom();
      var rightBottom = GetRightBottom();

      if (PointIsOver(_resizeStartPosition, leftTop))
      {
        _moveLeft = true;
        _moveTop = true;
      }
      else if (PointIsOver(_resizeStartPosition, rightBottom))
      {
        Mouse.OverrideCursor = Cursors.SizeNWSE;
        _moveRight = true;
        _moveBottom = true;
      }
      else if (PointIsOver(_resizeStartPosition, rightTop))
      {
        Mouse.OverrideCursor = Cursors.SizeNESW;
        _moveRight = true;
        _moveTop = true;
      }
      else if (PointIsOver(_resizeStartPosition, leftBottom))
      {
        Mouse.OverrideCursor = Cursors.SizeNESW;
        _moveLeft = true;
        _moveBottom = true;
      }
    }

    protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
    {
      base.OnPreviewMouseLeftButtonUp(e);
      _resizeStartPosition = default;
      ReleaseMouseCapture();
      _moveTop = false;
      _moveLeft = false;
      _moveRight = false;
      _moveBottom = false;
    }
  }
}