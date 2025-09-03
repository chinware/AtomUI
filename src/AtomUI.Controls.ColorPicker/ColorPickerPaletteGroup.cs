using Avalonia;
using Avalonia.Controls.Primitives;

namespace AtomUI.Controls;

internal class ColorPickerPaletteGroup : TemplatedControl
{
    public static readonly StyledProperty<List<ColorPickerPalette>?> PaletteGroupProperty =
        AvaloniaProperty.Register<AbstractColorPickerView, List<ColorPickerPalette>?>(
            nameof(PaletteGroup));
    
    public List<ColorPickerPalette>? PaletteGroup
    {
        get => GetValue(PaletteGroupProperty);
        set => SetValue(PaletteGroupProperty, value);
    }
}