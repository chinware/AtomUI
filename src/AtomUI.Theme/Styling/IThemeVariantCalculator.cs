using AtomUI.TokenSystem;
using Avalonia.Media;

namespace AtomUI.Styling;

public interface IThemeVariantCalculator
{
   public MapDesignToken Calculate(SeedDesignToken seedToken, MapDesignToken sourceToken);
   public Color ColorBgBase { get; }
   public Color ColorTextBase { get; }
}