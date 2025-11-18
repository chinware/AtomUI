using AtomUI.Desktop.Controls.Primitives;
using AtomUI.IconPkg;
using Avalonia;

namespace AtomUI.Desktop.Controls;

internal class MessageBoxOverlayDialogHost : OverlayDialogHost
{
    public static readonly StyledProperty<Icon?> StyleIconProperty =
        AvaloniaProperty.Register<MessageBoxOverlayDialogHost, Icon?>(nameof (StyleIcon));
    
    public static readonly StyledProperty<MessageBoxStyle> StyleProperty =
        MessageBox.StyleProperty.AddOwner<MessageBoxOverlayDialogHost>();
    
    public Icon? StyleIcon
    {
        get => GetValue(StyleIconProperty);
        set => SetValue(StyleIconProperty, value);
    }
    
    public MessageBoxStyle Style
    {
        get => GetValue(StyleProperty);
        set => SetValue(StyleProperty, value);
    }
    
    protected override Type StyleKeyOverride { get; } = typeof(MessageBoxOverlayDialogHost);
    
    public MessageBoxOverlayDialogHost(DialogLayer dialogLayer, Dialog dialog)
        : base(dialogLayer, dialog)
    {
    }
}