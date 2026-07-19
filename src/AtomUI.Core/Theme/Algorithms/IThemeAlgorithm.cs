using AtomUI.Theme.TokenSystem;

namespace AtomUI.Theme.Algorithms;

public interface IThemeAlgorithm
{
    void Evaluate(DesignToken effectiveSeed, DesignToken? previousMap, DesignToken nextMap);
}
