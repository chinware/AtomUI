using Avalonia.Media;

namespace AtomUI.Controls;

public class ColorPickerPalette
{
    public string Label { get; }
    public bool IsOpen { get; }
    public IEnumerable<Color> Colors { get; }

    public ColorPickerPalette(string label, bool isOpen,  IEnumerable<Color> colors)
    {
        Label = label;
        IsOpen = isOpen;
        Colors = colors;
    }
}