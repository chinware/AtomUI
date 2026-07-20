using System.Globalization;
using AtomUI.Theme.Configuration;
using AtomUI.Theme.Definitions;
using AtomUI.Theme.Language;
using AtomUI.Theme.Resources;
using AtomUI.Theme.Schema;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Theme;

internal sealed class ThemeManagerBuilder : IThemeManagerBuilder
{
    private readonly List<ControlTokenDescriptor> _controlTokenDescriptors = new();
    private readonly List<IControlThemesProvider> _controlThemesProviders = new();
    private readonly List<LanguageProvider> _languageProviders = new();
    private readonly List<IThemeDefinitionResolver> _themeDefinitionResolvers = new();
    private readonly List<Action<IThemeManager>> _initializers = new();
    private readonly HashSet<ControlTokenIdentity> _registeredControlTokenIdentities = new();
    private readonly HashSet<string> _registeredControlThemeProviders = new(StringComparer.Ordinal);
    private readonly HashSet<string> _registeredLanguageProviders = new(StringComparer.Ordinal);
    private readonly HashSet<string> _registeredThemeDefinitionResolvers = new(StringComparer.Ordinal);
    private bool _useUserThemeDirectory;
    private string? _userThemeDirectory;

    internal ThemeManagerBuilder(Application? application = null)
    {
        ApplicationId = ThemeApplicationIdentity.ResolveDefault(application?.GetType()) ??
                        typeof(ThemeManagerBuilder).Assembly.GetName().Name!;
        LanguageVariant = LanguageVariant.en_US;
        InitialRequest = new ThemeRequest(
            IThemeManager.DEFAULT_THEME_ID,
            null,
            ThemeTransitionReason.Startup);
        AddThemeDefinitionResolver(CoreThemeDefinitionResolver.Create());
    }

    internal LanguageVariant LanguageVariant { get; private set; }
    internal FontFamily? FontFamily { get; private set; }
    internal ThemeRequest InitialRequest { get; private set; }
    internal ThemeRequest? FollowSystemLightRequest { get; private set; }
    internal ThemeRequest? FollowSystemDarkRequest { get; private set; }
    internal IReadOnlyList<Action<IThemeManager>> Initializers => _initializers;
    internal IReadOnlyList<IThemeDefinitionResolver> ThemeDefinitionResolvers => _themeDefinitionResolvers;
    internal string? ApplicationId { get; private set; }
    internal bool UsesUserThemeDirectory => _useUserThemeDirectory;
    internal string? UserThemeDirectory => _userThemeDirectory;

    public void AddThemeDefinitionResolver(IThemeDefinitionResolver resolver)
    {
        ArgumentNullException.ThrowIfNull(resolver);
        if (string.IsNullOrWhiteSpace(resolver.Id))
        {
            throw new ArgumentException("Theme definition resolver id cannot be empty.", nameof(resolver));
        }
        if (!_registeredThemeDefinitionResolvers.Add(resolver.Id))
        {
            throw new ThemeResourceRegisterException(
                $"Theme definition resolver '{resolver.Id}' is already registered.");
        }

        _themeDefinitionResolvers.Add(resolver);
    }

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

    public void WithApplicationId(string applicationId)
    {
        ThemeApplicationIdentity.Validate(applicationId, nameof(applicationId));
        ApplicationId = applicationId;
    }

    public void UseUserThemeDirectory()
    {
        _useUserThemeDirectory = true;
        _userThemeDirectory = null;
    }

    public void UseUserThemeDirectory(string directory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directory);
        _useUserThemeDirectory = true;
        _userThemeDirectory = Path.GetFullPath(directory);
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
        var themeDefinitionResolvers = new List<IThemeDefinitionResolver>(_themeDefinitionResolvers);
        if (_useUserThemeDirectory)
        {
            if (themeDefinitionResolvers.Any(static resolver =>
                    string.Equals(
                        resolver.Id,
                        UserDirectoryThemeDefinitionResolver.ResolverId,
                        StringComparison.Ordinal)))
            {
                throw new ThemeResourceRegisterException(
                    $"Theme definition resolver '{UserDirectoryThemeDefinitionResolver.ResolverId}' is already registered.");
            }
            themeDefinitionResolvers.Add(new UserDirectoryThemeDefinitionResolver(_userThemeDirectory));
        }

        var themeManager = new ThemeManager
        {
            FontFamily = FontFamily
        };
        themeManager.ConfigureStartup(
            InitialRequest,
            FollowSystemLightRequest,
            FollowSystemDarkRequest);
        themeManager.ConfigureThemeDefinitions(
            themeDefinitionResolvers,
            ApplicationId!,
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
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
