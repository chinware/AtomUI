using AtomUI.Theme.Styling;
using AtomUI.Theme.TokenSystem;
using Avalonia.Styling;

namespace AtomUI.Theme;

internal class TokenResourceLayer : Styles, ITokenResourceLayer
{
    private DesignToken _designToken;
    private Dictionary<string, IControlDesignToken> _controlTokens;
    private IList<string> _algorithms;
    private IList<ControlTokenConfigInfo> _controlTokenConfigInfos;
    private IThemeVariantCalculator? _themeVariantCalculator;
    private bool _darkMode;

    public TokenResourceLayer()
    {
        _controlTokens           = new Dictionary<string, IControlDesignToken>();
        _controlTokenConfigInfos = new List<ControlTokenConfigInfo>();
        _algorithms              = new List<string>();
        _designToken             = new DesignToken();
    }
}