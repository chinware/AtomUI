using AtomUI.Controls.Themes;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace AtomUI.Controls;

internal class OverlayDialogResizer : TemplatedControl
{
    public OverlayDialogHost? TargetDialog { get; set; }
    private Panel? _rootLayout;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _rootLayout = e.NameScope.Find<Panel>(WindowResizerThemeConstants.RootLayoutPart);
        ConfigureResizeHandles(true);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        ConfigureResizeHandles(true);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        ConfigureResizeHandles(false);
    }

    private void ConfigureResizeHandles(bool isAttach)
    {
        if (_rootLayout != null)
        {
            foreach (var child in _rootLayout.Children)
            {
                if (isAttach)
                {
                    child.PointerPressed += HandleResizeHandlePressed;
                }
                else
                {
                    child.PointerPressed -= HandleResizeHandlePressed;
                }
            }
        }
    }

    private void HandleResizeHandlePressed(object? sender, PointerPressedEventArgs e)
    {
        if (TargetDialog == null ||
            !TargetDialog.IsResizable || 
            TargetDialog.WindowState != OverlayDialogState.Normal)
        {
            return;
        }

        if (sender is not Border border || border.Tag is not ResizeHandleLocation location)
        {
            return;
        }
        
        e.Handled = true;
    }
}