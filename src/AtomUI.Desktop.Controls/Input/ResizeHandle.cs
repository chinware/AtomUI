using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace AtomUI.Desktop.Controls;

internal class ResizeHandle : TemplatedControl
{
    internal TextArea? Owner { get; set; }
    
    private Point? _lastPoint;
    private bool _dragging;

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        e.Handled = true;
        _lastPoint    = e.GetPosition(TopLevel.GetTopLevel(this));
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (_lastPoint.HasValue)
        {
            Owner?.NotifyResizeCompleted();
            e.Handled     = true;
            _lastPoint    = null;
            _dragging     = false;
        }
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        if (_lastPoint.HasValue && e.Properties.IsLeftButtonPressed)
        {
            var delta             = e.GetPosition(TopLevel.GetTopLevel(this)) - _lastPoint.Value;
            var manhattanDistance = Math.Abs(delta.X) + Math.Abs(delta.Y);
            if (manhattanDistance > Constants.DragThreshold)
            {
                if (!_dragging)
                {
                    Owner?.NotifyAboutToResize();
                    _dragging = true;
                }
                Owner?.NotifyResizing(delta);
            }
        }
    }
}