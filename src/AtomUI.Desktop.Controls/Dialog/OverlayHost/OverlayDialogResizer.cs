using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace AtomUI.Desktop.Controls;

internal class OverlayDialogResizer : TemplatedControl
{
    public event EventHandler<OverlayDialogResizeEventArgs>? AboutToResize;
    public event EventHandler<OverlayDialogResizeEventArgs>? ResizeRequest;
    public event EventHandler<OverlayDialogResizeEventArgs>? ResizeCompleted;
    
    private Panel? _rootLayout;
    private IPointer? _capturedPointer;
    private Control? _captureOwner;
    private Point? _lastPoint;
    private Vector _lastDelta;
    private bool _dragging;
    private ResizeHandleLocation? _dragLocation;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        CompleteResize();
        if (_rootLayout != null)
        {
            foreach (var child in _rootLayout.Children)
            {
                child.PointerPressed  -= HandleResizeHandlePressed;
                child.PointerReleased -= HandleResizeHandleReleased;
                child.PointerMoved    -= HandleResizeHandleMoved;
                child.PointerCaptureLost -= HandleResizeHandleCaptureLost;
            }
        }
        _rootLayout = e.NameScope.Find<Panel>("PART_RootLayout");
        if (_rootLayout != null)
        {
            foreach (var child in _rootLayout.Children)
            {
                child.PointerPressed  += HandleResizeHandlePressed;
                child.PointerReleased += HandleResizeHandleReleased;
                child.PointerMoved    += HandleResizeHandleMoved;
                child.PointerCaptureLost += HandleResizeHandleCaptureLost;
            }
        }
    }
    
    private void HandleResizeHandlePressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.Properties.IsLeftButtonPressed && sender is Border border && border.Tag is ResizeHandleLocation location)
        {
            CompleteResize();
            e.Handled     = true;
            _dragLocation = location;
            _lastPoint    = e.GetPosition(TopLevel.GetTopLevel(this));
            _lastDelta    = default;
            _dragging     = false;
            _capturedPointer = e.Pointer;
            _captureOwner = border;
            e.Pointer.Capture(border);
            e.PreventGestureRecognition();
        }
    }

    private void HandleResizeHandleReleased(object? sender, PointerEventArgs e)
    {
        if (_lastPoint.HasValue && _dragLocation.HasValue)
        {
            _lastDelta = e.GetPosition(TopLevel.GetTopLevel(this)) - _lastPoint.Value;
            e.Handled = true;
            CompleteResize();
        }
    }

    private void HandleResizeHandleCaptureLost(object? sender, PointerCaptureLostEventArgs e)
    {
        CompleteResize();
    }

    private void HandleResizeHandleMoved(object? sender, PointerEventArgs e)
    {
        if (_lastPoint.HasValue && e.Properties.IsLeftButtonPressed)
        {
            _lastDelta = e.GetPosition(TopLevel.GetTopLevel(this)) - _lastPoint.Value;
            var manhattanDistance = Math.Abs(_lastDelta.X) + Math.Abs(_lastDelta.Y);
            if (manhattanDistance > Constants.DragThreshold)
            {
                if (_dragLocation is { } location)
                {
                    if (!_dragging)
                    {
                        AboutToResize?.Invoke(this, new OverlayDialogResizeEventArgs(location, 0, 0));
                    }
                    _dragging = true;
                    ResizeRequest?.Invoke(
                        this,
                        new OverlayDialogResizeEventArgs(location, _lastDelta.X, _lastDelta.Y));
                }
            }
        }
    }

    private void CompleteResize()
    {
        var pointer = _capturedPointer;
        var captureOwner = _captureOwner;
        var location = _dragLocation;
        var delta = _lastDelta;
        _capturedPointer = null;
        _captureOwner = null;
        _lastPoint = null;
        _lastDelta = default;
        _dragLocation = null;
        _dragging = false;
        if (pointer is not null && ReferenceEquals(pointer.Captured, captureOwner))
        {
            pointer.Capture(null);
        }
        if (location.HasValue)
        {
            ResizeCompleted?.Invoke(
                this,
                new OverlayDialogResizeEventArgs(location.Value, delta.X, delta.Y));
        }
    }
}
