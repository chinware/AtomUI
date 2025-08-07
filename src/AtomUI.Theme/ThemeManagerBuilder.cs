using System.Globalization;
using AtomUI.Theme.Language;
using Avalonia;

namespace AtomUI.Theme;

internal class ThemeManagerBuilder : IThemeManagerBuilder
{
    public IList<Type> ControlDesignTokens { get; }
    public IList<BaseControlTheme> ControlThemes { get; }
    public IList<IThemeAssetPathProvider> ThemeAssetPathProviders { get; }
    public IList<IControlThemesProvider> ControlThemesProviders { get; }
    public IList<AbstractLanguageProvider> LanguageProviders { get; }
    public IList<EventHandler> InitializedHandlers { get; }

    public LanguageVariant LanguageVariant { get; private set; }
    public string ThemeId { get; private set; }
    public AppBuilder AppBuilder { get; }

    private HashSet<string> _registeredTokenTypes;
    private HashSet<string> _registeredControlThemesProviders;
    private HashSet<string> _registeredLanguageProviders;

    internal ThemeManagerBuilder(AppBuilder builder)
    {
        ControlDesignTokens               = new List<Type>();
        ControlThemes                     = new List<BaseControlTheme>();
        ThemeAssetPathProviders           = new List<IThemeAssetPathProvider>();
        ControlThemesProviders            = new List<IControlThemesProvider>();
        LanguageProviders                 = new List<AbstractLanguageProvider>();
        InitializedHandlers               = new List<EventHandler>();
        LanguageVariant                   = LanguageVariant.en_US;
        ThemeId                           = IThemeManager.DEFAULT_THEME_ID;
        _registeredTokenTypes             = new HashSet<string>();
        _registeredLanguageProviders      = new HashSet<string>();
        _registeredControlThemesProviders = new HashSet<string>();
        AppBuilder                        = builder;
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

    public void AddControlThemesProvider(IThemeAssetPathProvider themeAssetPathProvider)
    {
        if (!ThemeAssetPathProviders.Contains(themeAssetPathProvider))
        {
            ThemeAssetPathProviders.Add(themeAssetPathProvider);
        }
    }
    
    public void AddControlThemesProvider(IControlThemesProvider controlThemesProvider)
    {
        if (string.IsNullOrEmpty(controlThemesProvider.Id))
        {
            throw new ThemeResourceRegisterException($"Control theme provider '{controlThemesProvider.Id}' is invalid, maybe empty.");
        }
        if (_registeredControlThemesProviders.Contains(controlThemesProvider.Id))
        {
            throw new ThemeResourceRegisterException($"Control theme provider '{controlThemesProvider.Id}' is already registered.");
        }
        ControlThemesProviders.Add(controlThemesProvider);
        _registeredControlThemesProviders.Add(controlThemesProvider.Id);
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

    public void WithDefaultTheme(string themeId)
    {
        ThemeId = themeId;
    }

    public void WithDefaultCultureInfo(CultureInfo cultureInfo)
    {
        LanguageVariant = LanguageVariant.FromCultureInfo(cultureInfo);
    }
    
    public void WithDefaultLanguageVariant(LanguageVariant languageVariant)
    {
        LanguageVariant = languageVariant;
    }

    internal ThemeManager Build()
    {
        var themeManager = new ThemeManager();
        themeManager.DefaultThemeId = ThemeId;
        foreach (var controlThemesProvider in ControlThemesProviders)
        {
            themeManager.RegisterControlThemesProvider(controlThemesProvider);
        }

        foreach (var tokenType in ControlDesignTokens)
        {
            themeManager.RegisterControlTokenType(tokenType);
        }
        
        foreach (var themeAssetPathProvider in ThemeAssetPathProviders)
        {
            themeManager.RegisterControlThemesProvider(themeAssetPathProvider);
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