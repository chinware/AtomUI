using AtomUI.Theme.TokenSystem;
using Avalonia.Media;

namespace AtomUI.Theme.Styling;

public interface IThemeVariantCalculator
{
    public void Calculate(GlobalToken globalToken);
    public Color ColorBgBase { get; }
    public Color ColorTextBase { get; }
}