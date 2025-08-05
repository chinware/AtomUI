using System.Globalization;
using AtomUI.Utils;
using Avalonia;

namespace AtomUI.Theme;

public class ThemeManagerBuilder : IThemeManagerBuilder
{
    public IList<Type> ControlDesignTokens { get; }
    public IList<BaseControlTheme> ControlThemes { get; }
    public IList<IControlThemesProvider> ControlThemesProviders { get; }
    public IList<AbstractLanguageProvider> LanguageProviders { get; }
    public IList<EventHandler> InitializedHandlers { get; }

    public CultureInfo CultureInfo { get; private set; }
    public string ThemeId { get; private set; }
    public AppBuilder AppBuilder { get; }

    private HashSet<string> _registeredTokenTypes;
    private HashSet<string> _registeredControlThemesProviders;
    private HashSet<string> _registeredLanguageProviders;

    internal ThemeManagerBuilder(AppBuilder builder)
    {
        ControlDesignTokens               = new List<Type>();
        ControlThemes                     = new List<BaseControlTheme>();
        ControlThemesProviders            = new List<IControlThemesProvider>();
        LanguageProviders                 = new List<AbstractLanguageProvider>();
        InitializedHandlers               = new List<EventHandler>();
        CultureInfo                       = new CultureInfo(LanguageCode.en_US);
        ThemeId                           = ThemeManager.DEFAULT_THEME_ID;
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
        CultureInfo = cultureInfo;
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