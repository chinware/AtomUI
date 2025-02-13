using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia.Data;
using Avalonia.LogicalTree;

namespace AtomUI.Controls;

using AvaloniaTextBlock = Avalonia.Controls.TextBlock;

public class TextBlock : AvaloniaTextBlock
{
    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        TokenResourceBinder.CreateTokenBinding(this, LineHeightProperty, SharedTokenKey.FontHeight, BindingPriority.Template);
    }
}