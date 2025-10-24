using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Controls;

internal class SelectResultOptionsBox : ItemsControl
{
    public static readonly StyledProperty<SelectMode> ModeProperty =
        AvaloniaProperty.Register<SelectResultOptionsBox, SelectMode>(nameof(Mode));
    
    public SelectMode Mode
    {
        get => GetValue(ModeProperty);
        set => SetValue(ModeProperty, value);
    }
}