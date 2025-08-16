using Avalonia.Media;

namespace AtomUI.Controls;

public class ColorPickerPresetGroup
{
    public string Label { get; }
    public bool IsOpen { get; }
    public List<Color> Colors { get; }

    public ColorPickerPresetGroup(string label, bool isOpen,  List<Color> colors)
    {
        Label = label;
        IsOpen = isOpen;
        Colors = colors;
    }
}