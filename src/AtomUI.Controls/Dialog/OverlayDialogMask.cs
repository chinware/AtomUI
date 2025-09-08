using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.LogicalTree;

namespace AtomUI.Controls;

internal class OverlayDialogMask : Border
{
    private IDisposable? _tokenBindingDisposable;
    
    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        _tokenBindingDisposable = TokenResourceBinder.CreateTokenBinding(this, BackgroundProperty, SharedTokenKey.ColorBgMask);
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        _tokenBindingDisposable?.Dispose();
    }
}