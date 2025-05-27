using System.Diagnostics;
using System.Globalization;
using AtomUI.Utils;
using Avalonia;

namespace AtomUI.Theme;

public class ThemeManagerBuilder : IThemeManagerBuilder
{
    public IList<Type> ControlDesignTokens { get; }
    public IList<BaseControlTheme> ControlThemes { get; }
    public IList<AbstractLanguageProvider> LanguageProviders { get; }
    public IList<EventHandler> InitializedHandlers { get; }

    public CultureInfo CultureInfo { get; private set; }
    public string ThemeId { get; private set; }
    public AppBuilder AppBuilder { get; private set; }

    private HashSet<string> _registeredTokenTypes;
    private HashSet<string> _registeredControlThemes;
    private HashSet<string> _registeredLanguageProviders;

    internal ThemeManagerBuilder(AppBuilder builder)
    {
        ControlDesignTokens          = new List<Type>();
        ControlThemes                = new List<BaseControlTheme>();
        LanguageProviders            = new List<AbstractLanguageProvider>();
        InitializedHandlers          = new List<EventHandler>();
        CultureInfo                  = new CultureInfo(LanguageCode.en_US);
        ThemeId                      = ThemeManager.DEFAULT_THEME_ID;
        _registeredTokenTypes        = new HashSet<string>();
        _registeredControlThemes     = new HashSet<string>();
        _registeredLanguageProviders = new HashSet<string>();
        AppBuilder                   = builder;
    }

    public void AddControlToken(Type tokenType)
    {
        var typeStr = tokenType.FullName!;
        if (_registeredTokenTypes.Contains(typeStr))
        {
            throw new ThemeResourceRegisterException($"Control design token '{typeStr}' is already registered.");
        }

        ControlDesignTokens.Add(tokenType);
        _registeredTokenTypes.Add(typeStr);
    }

    public void AddControlTheme(BaseControlTheme controlTheme)
    {
        var     resourceKey    = controlTheme.ThemeResourceKey();
        string? resourceKeyStr = null;
        if (resourceKey is Type typeKey)
        {
            resourceKeyStr = typeKey.FullName;
        }
        else
        {
            resourceKeyStr = resourceKey.ToString();
        }

        Debug.Assert(resourceKeyStr != null);
        if (_registeredControlThemes.Contains(resourceKeyStr))
        {
            throw new ThemeResourceRegisterException($"Control theme '{resourceKeyStr}' is already registered.");
        }

        ControlThemes.Add(controlTheme);
        _registeredControlThemes.Add(resourceKeyStr);
    }

    public void AddLanguageProviders(AbstractLanguageProvider languageProvider)
    {
        var id = languageProvider.GetType().FullName!;
        if (_registeredLanguageProviders.Contains(id))
        {
            throw new ThemeResourceRegisterException($"Language provider '{id}' is already registered.");
        }

        LanguageProviders.Add(languageProvider);
        _registeredLanguageProviders.Add(id);
    }

    public void UseTheme(string themeId)
    {
        ThemeId = themeId;
    }

    public void UseCultureInfo(CultureInfo cultureInfo)
    {
        CultureInfo = cultureInfo;
    }

    internal ThemeManager Build()
    {
        var themeManager = new ThemeManager();
        foreach (var controlTheme in ControlThemes)
        {
            themeManager.RegisterControlTheme(controlTheme);
        }

        foreach (var tokenType in ControlDesignTokens)
        {
            themeManager.RegisterControlTokenType(tokenType);
        }

        foreach (var languageProvider in LanguageProviders)
        {
            themeManager.RegisterLanguageProvider(languageProvider);
        }

        foreach (var handler in InitializedHandlers)
        {
            themeManager.Initialized += handler;
        }

        return themeManager;
    }
}