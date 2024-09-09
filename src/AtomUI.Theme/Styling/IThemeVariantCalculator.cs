using AtomUI.Theme.TokenSystem;
using Avalonia.Media;

namespace AtomUI.Theme.Styling;

public interface IThemeVariantCalculator
{
    public Color ColorBgBase { get; }
    public Color ColorTextBase { get; }
    public MapDesignToken Calculate(SeedDesignToken seedToken, MapDesignToken sourceToken);
}