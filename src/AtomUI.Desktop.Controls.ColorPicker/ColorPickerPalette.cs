using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

public class ColorPickerPalette : IReadOnlyHeadered
{
    public string Label { get; }
    public bool IsOpen { get; }
    public IEnumerable<Color> Colors { get; }

    public object Header => Label;

    public ColorPickerPalette(string label, bool isOpen,  IEnumerable<Color> colors)
    {
        Label = label;
        IsOpen = isOpen;
        Colors = colors;
    }
}