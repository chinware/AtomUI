using AtomUI.Controls;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace AtomUI.Desktop.Controls;

internal class WindowResizer : TemplatedControl
{
    public Window? TargetWindow { get; set; }
    private Panel? _rootLayout;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        if (_rootLayout != null)
        {
            foreach (var child in _rootLayout.Children)
            {
                child.PointerPressed -= HandleResizeHandlePressed;
            }
        }

        _rootLayout = e.NameScope.Find<Panel>("PART_RootLayout");
        if (_rootLayout != null)
        {
            foreach (var child in _rootLayout.Children)
            {
                child.PointerPressed += HandleResizeHandlePressed;
            }
        }
    }

    private void HandleResizeHandlePressed(object? sender, PointerPressedEventArgs e)
    {
        if (TargetWindow == null || !TargetWindow.CanResize || TargetWindow.WindowState != WindowState.Normal)
        {
            return;
        }

        if (sender is not Border border || border.Tag is not ResizeHandleLocation location)
        {
            return;
        }

        var windowEdge = location switch
        {
            ResizeHandleLocation.North     => WindowEdge.North,
            ResizeHandleLocation.South     => WindowEdge.South,
            ResizeHandleLocation.West      => WindowEdge.West,
            ResizeHandleLocation.East      => WindowEdge.East,
            ResizeHandleLocation.NorthWest => WindowEdge.NorthWest,
            ResizeHandleLocation.NorthEast => WindowEdge.NorthEast,
            ResizeHandleLocation.SouthWest => WindowEdge.SouthWest,
            ResizeHandleLocation.SouthEast => WindowEdge.SouthEast,
            _                              => throw new ArgumentOutOfRangeException()
        };

        TargetWindow.BeginResizeDrag(windowEdge, e);
        e.Handled = true;
    }
}
