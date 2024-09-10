using AtomUI.Theme.TokenSystem;
using Avalonia.Media;

namespace AtomUI.Theme.Styling;

public interface IThemeVariantCalculator
{
    public MapDesignToken Calculate(SeedDesignToken seedToken, MapDesignToken sourceToken);
    public Color ColorBgBase { get; }
    public Color ColorTextBase { get; }
}