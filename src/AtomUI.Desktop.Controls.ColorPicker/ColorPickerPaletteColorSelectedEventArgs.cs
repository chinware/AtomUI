using Avalonia.Media;

namespace AtomUI.Controls;

internal class ColorPickerPaletteColorSelectedEventArgs : EventArgs
{
    public ColorPickerPaletteColorSelectedEventArgs(Color color)
    {
        SelectedColor = color;
    }
    
    public Color SelectedColor { get; private set; }
}