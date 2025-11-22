using Avalonia.Media;

namespace AtomUIGallery.Models;

public record PaletteColorInfo
{
    public IBrush Brush { get; }
    public IBrush TextBrush { get; }
    public string DisplayName { get; }
    public string Hex { get; }

    public PaletteColorInfo(string displayName, IBrush brush, bool light, int index)
    {
        DisplayName = displayName;
        Brush       = brush;
        Hex         = brush.ToString()?.ToUpperInvariant() ?? "Invalid Color";
        if ((light && index < 5) || (!light && index >= 5))
        {
            TextBrush = Brushes.Black;
        }
        else
        {
            TextBrush = Brushes.White;
        }
    }
}