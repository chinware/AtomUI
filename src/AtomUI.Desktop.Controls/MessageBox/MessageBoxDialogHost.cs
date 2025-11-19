using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Desktop.Controls;

internal class MessageBoxDialogHost : DialogHost
{
    public static readonly StyledProperty<Icon?> StyleIconProperty =
        AvaloniaProperty.Register<MessageBoxDialogHost, Icon?>(nameof (StyleIcon));
    
    public static readonly StyledProperty<MessageBoxStyle> StyleProperty =
        MessageBox.StyleProperty.AddOwner<MessageBoxDialogHost>();
    
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
    
    public MessageBoxDialogHost(TopLevel parent, Dialog dialog)
        : base(parent, dialog)
    {
    }
}