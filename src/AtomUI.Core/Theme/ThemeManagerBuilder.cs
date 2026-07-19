using System.Globalization;
using AtomUI.Theme.Configuration;
using AtomUI.Theme.Language;
using AtomUI.Theme.Schema;
using Avalonia.Media;

namespace AtomUI.Theme;

internal sealed class ThemeManagerBuilder : IThemeManagerBuilder
{
    private readonly List<ControlTokenDescriptor> _controlTokenDescriptors = new();
    private readonly List<IControlThemesProvider> _controlThemesProviders = new();
    private readonly List<LanguageProvider> _languageProviders = new();
    private readonly List<Action<IThemeManager>> _initializers = new();
    private readonly HashSet<ControlTokenIdentity> _registeredControlTokenIdentities = new();
    private readonly HashSet<string> _registeredControlThemeProviders = new(StringComparer.Ordinal);
    private readonly HashSet<string> _registeredLanguageProviders = new(StringComparer.Ordinal);

    internal ThemeManagerBuilder()
    {
        LanguageVariant = LanguageVariant.en_US;
        InitialRequest = new ThemeRequest(
            IThemeManager.DEFAULT_THEME_ID,
            null,
            ThemeTransitionReason.Startup);
    }

    internal LanguageVariant LanguageVariant { get; private set; }
    internal FontFamily? FontFamily { get; private set; }
    internal ThemeRequest InitialRequest { get; private set; }
    internal ThemeRequest? FollowSystemLightRequest { get; private set; }
    internal ThemeRequest? FollowSystemDarkRequest { get; private set; }
    internal IReadOnlyList<Action<IThemeManager>> Initializers => _initializers;

    public void AddControlToken(ControlTokenDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);
        if (!_registeredControlTokenIdentities.Add(descriptor.Identity))
        {
            throw new ThemeResourceRegisterException(
                $"Control Token descriptor '{descriptor.Identity}' is already registered.");
        }

        _controlTokenDescriptors.Add(descriptor);
    }

    public void AddControlThemesProvider(IControlThemesProvider controlThemesProvider)
    {
        ArgumentNullException.ThrowIfNull(controlThemesProvider);
        if (string.IsNullOrWhiteSpace(controlThemesProvider.Id))
        {
            throw new ThemeResourceRegisterException("Control theme provider id cannot be empty.");
        }
        if (!_registeredControlThemeProviders.Add(controlThemesProvider.Id))
        {
            throw new ThemeResourceRegisterException(
                $"Control theme provider '{controlThemesProvider.Id}' is already registered.");
        }

        _controlThemesProviders.Add(controlThemesProvider);
    }

    public void AddLanguageProviders(LanguageProvider languageProvider)
    {
        ArgumentNullException.ThrowIfNull(languageProvider);
        var id = languageProvider.GetType().FullName ?? languageProvider.GetType().Name;
        if (!_registeredLanguageProviders.Add(id))
        {
            throw new ThemeResourceRegisterException(
                $"Language provider '{id}' is already registered.");
        }

        _languageProviders.Add(languageProvider);
    }

    public void AddInitializer(Action<IThemeManager> initializer)
    {
        ArgumentNullException.ThrowIfNull(initializer);
        _initializers.Add(initializer);
    }

    public void WithInitialTheme(string themeId, ThemeConfig? config = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(themeId);
        InitialRequest = new ThemeRequest(themeId, config, ThemeTransitionReason.Startup);
        FollowSystemLightRequest = null;
        FollowSystemDarkRequest = null;
    }

    public void WithFollowSystemThemes(ThemeRequest light, ThemeRequest dark)
    {
        ArgumentNullException.ThrowIfNull(light);
        ArgumentNullException.ThrowIfNull(dark);
        FollowSystemLightRequest = light with { Reason = ThemeTransitionReason.FollowSystem };
        FollowSystemDarkRequest = dark with { Reason = ThemeTransitionReason.FollowSystem };
        InitialRequest = FollowSystemLightRequest;
    }

    public void WithDefaultFontFamily(FontFamily fontFamily)
    {
        FontFamily = fontFamily;
    }

    public void WithDefaultFontFamily(string fontFamily)
    {
        FontFamily = FontFamily.Parse(fontFamily);
    }

    public void WithDefaultCultureInfo(CultureInfo cultureInfo)
    {
        ArgumentNullException.ThrowIfNull(cultureInfo);
        LanguageVariant = LanguageVariant.FromCultureInfo(cultureInfo);
    }

    public void WithDefaultLanguageVariant(LanguageVariant languageVariant)
    {
        LanguageVariant = languageVariant;
    }

    internal ThemeManager Build()
    {
        var themeManager = new ThemeManager
        {
            FontFamily = FontFamily
        };
        themeManager.ConfigureStartup(
            InitialRequest,
            FollowSystemLightRequest,
            FollowSystemDarkRequest);
        themeManager.EnsureRegistrationCapacity(
            _controlTokenDescriptors.Count,
            _controlThemesProviders.Count,
            _languageProviders.Count);

        foreach (var provider in _controlThemesProviders)
        {
            themeManager.RegisterControlThemesProvider(provider);
        }
        foreach (var descriptor in _controlTokenDescriptors)
        {
            themeManager.RegisterControlTokenDescriptor(descriptor);
        }
        foreach (var provider in _languageProviders)
        {
            themeManager.RegisterLanguageProvider(provider);
        }

        return themeManager;
    }
}
