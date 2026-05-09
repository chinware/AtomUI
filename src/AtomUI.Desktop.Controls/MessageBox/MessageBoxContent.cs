using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Desktop.Controls;

internal class MessageBoxContent : ContentControl
{
    public static readonly StyledProperty<PathIcon?> StyleIconProperty =
        AvaloniaProperty.Register<MessageBoxContent, PathIcon?>(nameof(StyleIcon));

    public static readonly StyledProperty<MessageBoxStyle> StyleProperty =
        MessageBox.StyleProperty.AddOwner<MessageBoxContent>();

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
}
