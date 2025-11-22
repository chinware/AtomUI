using Avalonia;
using Avalonia.Controls.Primitives;

namespace AtomUIGallery.Controls;

public class ColorItemControl : TemplatedControl
{
    public static readonly StyledProperty<string?> ColorNameProperty =
        AvaloniaProperty.Register<ColorItemControl, string?>(
            nameof(ColorName));
    
    public static readonly StyledProperty<string?> HexProperty = 
        AvaloniaProperty.Register<ColorItemControl, string?>(nameof(Hex));

    public string? ColorName
    {
        get => GetValue(ColorNameProperty);
        set => SetValue(ColorNameProperty, value);
    }

    public string? Hex
    {
        get => GetValue(HexProperty);
        set => SetValue(HexProperty, value);
    }
}