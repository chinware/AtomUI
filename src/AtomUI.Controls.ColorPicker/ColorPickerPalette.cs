using Avalonia.Media;

namespace AtomUI.Controls;

public class ColorPickerPalette
{
    public string Label { get; }
    public bool IsOpen { get; }
    public List<Color> Colors { get; }

    public ColorPickerPalette(string label, bool isOpen,  List<Color> colors)
    {
        Label = label;
        IsOpen = isOpen;
        Colors = colors;
    }
}