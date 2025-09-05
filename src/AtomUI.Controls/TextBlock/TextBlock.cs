using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia.LogicalTree;
using Avalonia.Media;

namespace AtomUI.Controls;

using AvaloniaTextBlock = Avalonia.Controls.TextBlock;

public class TextBlock : AvaloniaTextBlock
{
    private IDisposable? _disposable;
    static TextBlock()
    {
        FontStyleProperty.OverrideDefaultValue<TextBlock>(FontStyle.Normal);
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        _disposable = TokenResourceBinder.CreateTokenBinding(this, LineHeightProperty, SharedTokenKey.FontHeight);
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        _disposable?.Dispose();
    }
}