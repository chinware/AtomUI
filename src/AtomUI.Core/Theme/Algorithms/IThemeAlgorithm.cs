using AtomUI.Theme.DesignTokens;

namespace AtomUI.Theme.Algorithms;

public interface IThemeAlgorithm
{
    void Evaluate(DesignToken effectiveSeed, DesignToken? previousMap, DesignToken nextMap);
}
