using AtomUI.Theme.Styling;
using AtomUI.Theme.TokenSystem;
using Avalonia.Styling;

namespace AtomUI.Theme;

internal class TokenResourceLayer : Styles, ITokenResourceLayer
{
    private IThemeVariantCalculator? _themeVariantCalculator;
    
    public DesignToken SharedToken { get; protected set; }
    
    public Dictionary<string, IControlDesignToken> ControlTokens { get; protected set; }
    
    public IList<string> Algorithms { get; protected set; }
    
    public bool IsDarkMode { get; protected set; }

    public TokenResourceLayer()
    {
        ControlTokens = new Dictionary<string, IControlDesignToken>();
        Algorithms    = new List<string>();
        SharedToken   = new DesignToken();
    }

    public void Calculate()
    {
    }

    public void MountTokenResources(ThemeConfigProvider themeConfigProvider)
    {
        
    }
}