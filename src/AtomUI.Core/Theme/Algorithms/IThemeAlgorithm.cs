using AtomUI.Theme.Tokens;

namespace AtomUI.Theme.Algorithms;

public interface IThemeAlgorithm
{
    void Evaluate(DesignToken effectiveSeed, DesignToken? previousMap, DesignToken nextMap);
}
