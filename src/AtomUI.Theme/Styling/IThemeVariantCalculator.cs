using AtomUI.Theme.TokenSystem;
using Avalonia.Media;

namespace AtomUI.Theme.Styling;

public interface IThemeVariantCalculator
{
    public void Calculate(DesignToken designToken);
    public Color ColorBgBase { get; }
    public Color ColorTextBase { get; }
}