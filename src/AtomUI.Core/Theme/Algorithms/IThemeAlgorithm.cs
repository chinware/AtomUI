using AtomUI.Theme.TokenSystem;
using Avalonia.Media;

namespace AtomUI.Theme.Algorithms;

public interface IThemeAlgorithm
{
    void Calculate(DesignToken designToken);

    Color ColorBgBase { get; }

    Color ColorTextBase { get; }
}
