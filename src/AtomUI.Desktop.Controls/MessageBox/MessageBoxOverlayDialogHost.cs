using AtomUI.Desktop.Controls.Primitives;
using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Desktop.Controls;

internal class MessageBoxOverlayDialogHost : OverlayDialogHost
{
    public static readonly StyledProperty<PathIcon?> StyleIconProperty =
        AvaloniaProperty.Register<MessageBoxOverlayDialogHost, PathIcon?>(nameof (StyleIcon));
    
    public static readonly StyledProperty<MessageBoxStyle> StyleProperty =
        MessageBox.StyleProperty.AddOwner<MessageBoxOverlayDialogHost>();
    
    public PathIcon? StyleIcon
    {
        get => GetValue(StyleIconProperty);
        set => SetValue(StyleIconProperty, value);
    }
    
    public MessageBoxStyle Style
    {
        get => GetValue(StyleProperty);
        set => SetValue(StyleProperty, value);
    }
    
    public MessageBoxOverlayDialogHost(DialogLayer dialogLayer, Dialog dialog)
        : base(dialogLayer, dialog)
    {
    }
}