using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using AtomUI.Controls;
using AtomUI.Theme.Catalog;
using AtomUI.Theme.Compilation;
using AtomUI.Theme.Language;
using AtomUI.Theme.Styling;
using AtomUI.Theme.TokenSystem;
using AtomUI.Theme.Transitions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Styling;

namespace AtomUI.Theme;

/// <summary>
/// 当切换主题时候就是动态的换 ResourceDictionary 里面的东西
/// </summary>
internal class ThemeManager : Styles, IThemeManager
{
    public const string DEFAULT_THEME_RES_PATH = $"avares://AtomUI.Core/Assets/{THEME_DIR}";
    public const string DEFAULT_APP_NAME = "AtomUIApplication";
    public const string THEME_DIR = "Themes";

    private static readonly IReadOnlyList<ThemeAlgorithm>[] s_algorithmCombinations =
    {
        [ThemeAlgorithm.Default],
        [ThemeAlgorithm.Default, ThemeAlgorithm.Dark],
        [ThemeAlgorithm.Default, ThemeAlgorithm.Dark, ThemeAlgorithm.Compact],
        [ThemeAlgorithm.Default, ThemeAlgorithm.Compact]
    };

    #region 公共属性定义
    
    public static readonly StyledProperty<ThemeVariant> ThemeVariantProperty =
        IThemeManager.ThemeVariantProperty.AddOwner<ThemeManager>();
    
    public static readonly StyledProperty<LanguageVariant> LanguageVariantProperty = 
        LanguageVariant.LanguageVariantProperty.AddOwner<ThemeManager>();
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<ThemeManager>();
    
    public static readonly StyledProperty<bool> IsWaveSpiritEnabledProperty =
        AvaloniaProperty.Register<ThemeManager, bool>(nameof(IsWaveSpiritEnabled));
    
    public static readonly StyledProperty<bool> IsDarkThemeModeProperty =
        IThemeManager.IsDarkThemeModeProperty.AddOwner<ThemeManager>();
    
    public static readonly StyledProperty<bool> IsCompactThemeModeProperty =
        IThemeManager.IsCompactThemeModeProperty.AddOwner<ThemeManager>();
    
    public ThemeVariant ThemeVariant
    {
        get => GetValue(ThemeVariantProperty);
        set => SetValue(ThemeVariantProperty, value);
    }
    
    public LanguageVariant LanguageVariant
    {
        get => GetValue(LanguageVariantProperty);
        set => SetValue(LanguageVariantProperty, value);
    }
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    public bool IsWaveSpiritEnabled
    {
        get => GetValue(IsWaveSpiritEnabledProperty);
        set => SetValue(IsWaveSpiritEnabledProperty, value);
    }
    
    public bool IsDarkThemeMode
    {
        get => GetValue(IsDarkThemeModeProperty);
        set => SetValue(IsDarkThemeModeProperty, value);
    }
    
    public bool IsCompactThemeMode
    {
        get => GetValue(IsCompactThemeModeProperty);
        set => SetValue(IsCompactThemeModeProperty, value);
    }
    
    public IList<ThemeAlgorithm>? ActivatedThemeAlgorithms { get; internal set; }
    public AvaloniaObject BindingSource => this;

    #endregion
    
    public ITheme? ActivatedTheme => _activatedTheme;
    public IReadOnlyList<string> CustomThemeDirs => _customThemeDirs;
    public static ThemeManager? Current => AvaloniaLocator.Current.GetService(typeof(ThemeManager)) as ThemeManager;
    public string DefaultThemeId { get; set; }
    public FontFamily? FontFamily { get; internal set; }
    internal List<ControlTokenRegistration> ControlTokenTypes { get; set; }
    internal IThemeVariantCalculatorFactory? ThemeVariantCalculatorFactory { get; set; }
    internal bool HasExplicitDefaultTheme { get; set; }
    internal string? ExplicitDefaultThemeBaseId { get; set; }
    internal ThemeCoordinator ThemeCoordinator => _themeCoordinator;
    
    public event EventHandler<ThemeOperateEventArgs>? ThemeCreated;
    public event EventHandler<ThemeOperateEventArgs>? ThemeAboutToLoad;
    public event EventHandler<ThemeOperateEventArgs>? ThemeLoaded;
    public event EventHandler<ThemeOperateEventArgs>? ThemeLoadFailed;
    public event EventHandler<ThemeOperateEventArgs>? ThemeAboutToChange;
    public event EventHandler<ThemeChangedEventArgs>? ThemeChanged;
    public event EventHandler<LanguageVariantChangedEventArgs>? LanguageVariantChanged;

    public event EventHandler? Initialized;
    
    private Theme? _activatedTheme;
    private readonly Dictionary<ThemeVariant, Theme> _themePool;
    private readonly List<string> _customThemeDirs;
    private readonly List<string> _builtInThemeDirs;
    private IList<IControlThemesProvider> _controlThemesProviders;
    private IList<IThemeAssetPathProvider> _themeAssetPathProviders;
    private ThemeCatalog? _themeCatalog;
    private ThemeCompiler? _themeCompiler;
    private readonly ThemeCoordinator _themeCoordinator;
    
    private readonly Dictionary<LanguageVariant, ResourceDictionary> _languages;
    private List<ILanguageProvider>? _languageProviders;
    
    internal ThemeManager(Func<bool>? themeTransitionAccessCheck = null)
    {
        _themePool       = new Dictionary<ThemeVariant, Theme>();
        _customThemeDirs = new List<string>();
        var appName = Application.Current?.Name ?? DEFAULT_APP_NAME;
        _builtInThemeDirs = [Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), appName),
            THEME_DIR)];
        DefaultThemeId           = IThemeManager.DEFAULT_THEME_ID;
        _controlThemesProviders  = new List<IControlThemesProvider>();
        _themeAssetPathProviders = new List<IThemeAssetPathProvider>();
        ControlTokenTypes        = new List<ControlTokenRegistration>();
        _languageProviders       = new List<ILanguageProvider>();
        _languages               = new Dictionary<LanguageVariant, ResourceDictionary>();
        _themeCoordinator        = themeTransitionAccessCheck is null
            ? new ThemeCoordinator(this)
            : new ThemeCoordinator(this, themeTransitionAccessCheck);
    }

    internal void EnsureRegistrationCapacity(int controlTokenCount,
                                             int controlThemesProviderCount,
                                             int themeAssetPathProviderCount,
                                             int languageProviderCount)
    {
        EnsureListCapacity(ControlTokenTypes, controlTokenCount);
        if (_controlThemesProviders is List<IControlThemesProvider> controlThemesProviders)
        {
            EnsureListCapacity(controlThemesProviders, controlThemesProviderCount);
        }

        if (_themeAssetPathProviders is List<IThemeAssetPathProvider> themeAssetPathProviders)
        {
            EnsureListCapacity(themeAssetPathProviders, themeAssetPathProviderCount);
        }

        if (_languageProviders is not null)
        {
            EnsureListCapacity(_languageProviders, languageProviderCount);
        }
    }

    private static void EnsureListCapacity<T>(List<T> list, int capacity)
    {
        if (list.Capacity < capacity)
        {
            list.Capacity = capacity;
        }
    }

    public IReadOnlyCollection<ITheme> AvailableThemes
    {
        get
        {
            if (_themePool.Count == 0)
            {
                ScanThemes();
            }

            return _themePool.Values;
        }
    }

    internal Theme LoadTheme(
        ThemeVariant themeVariant,
        IReadOnlyDictionary<string, string>? runtimeOverrides = null)
    {
        ScanThemes();
        if (!_themePool.TryGetValue(themeVariant, out var theme))
        {
            throw new ThemeNotFoundException($"Theme {themeVariant} not found");
        }
        
        if (theme.IsLoaded)
        {
            return theme;
        }

        theme.NotifyAboutToLoad();
        NotifyThemeOperate(ThemeAboutToLoad, new ThemeOperateEventArgs(theme));
        try
        {
            theme.Load(runtimeOverrides);
            theme.NotifyLoaded();
            NotifyThemeOperate(ThemeLoaded, new ThemeOperateEventArgs(theme));
            return theme;
        }
        catch (Exception)
        {
            NotifyThemeOperate(ThemeLoadFailed, new ThemeOperateEventArgs(theme));
            throw;
        }
    }

    /// <summary>
    /// 取消主题在 avalonia 里面的 resource 资源
    /// </summary>
    /// <param name="themeVariant"></param>
    internal void UnLoadTheme(ThemeVariant themeVariant)
    {
        if (!_themePool.ContainsKey(themeVariant))
        {
            // TODO 需要记录一个日志
            return;
        }

        if (_activatedTheme != null && _activatedTheme.ThemeVariant == themeVariant)
        {
            // TODO 需要记录一个日志
            return;
        }
    }

    public Theme? SetActiveTheme(ThemeVariant themeVariant)
    {
        var oldTheme = _activatedTheme;
        _themeCoordinator.Request(CreateThemeRequest(themeVariant, ThemeTransitionReason.UserRequest));
        return oldTheme;
    }

    internal Theme? CommitActiveTheme(Theme theme)
    {
        var oldTheme = _activatedTheme;
        if (ReferenceEquals(oldTheme, theme))
        {
            return oldTheme;
        }

        if (oldTheme is not null)
        {
            oldTheme.NotifyAboutToDeActive();
        }
        
        theme.NotifyAboutToActive();
        NotifyThemeOperate(ThemeAboutToChange, new ThemeOperateEventArgs(oldTheme));
        _activatedTheme = theme;
        
        if (!Resources.ThemeDictionaries.ContainsKey(theme.ThemeVariant))
        {
            Resources.ThemeDictionaries.Add(theme.ThemeVariant, theme.ThemeResource);
        }

        if (oldTheme is not null)
        {
            oldTheme.NotifyDeActivated();
        }
        
        theme.NotifyActivated();
        ActivatedThemeAlgorithms = theme.Algorithms;
        SetCurrentValue(ThemeVariantProperty, theme.ThemeVariant);
        SetCurrentValue(IsDarkThemeModeProperty, theme.Algorithms.Contains(ThemeAlgorithm.Dark));
        SetCurrentValue(IsCompactThemeModeProperty, theme.Algorithms.Contains(ThemeAlgorithm.Compact));

        ConfigureThemeSwitchRuntimeResources(oldTheme);
        return oldTheme;
    }
    
    public void RegisterControlThemesProvider(IControlThemesProvider controlThemesProvider)
    {
        _controlThemesProviders.Add(controlThemesProvider);
    }

    public void RegisterControlThemesProvider(IThemeAssetPathProvider themeAssetPathProvider)
    {
        _themeAssetPathProviders.Add(themeAssetPathProvider);
    }

    public void RegisterLanguageProvider(ILanguageProvider languageProvider)
    {
        _languageProviders?.Add(languageProvider);
    }

    public void RegisterControlTokenType(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor |
                                    DynamicallyAccessedMemberTypes.PublicProperties |
                                    DynamicallyAccessedMemberTypes.NonPublicProperties)]
        Type tokenType)
    {
        ControlTokenTypes.Add(new ControlTokenRegistration(tokenType));
    }

    internal void ScanThemes()
    {
        if (_themeCatalog is not null)
        {
            return;
        }

        var componentTokenSchemas = CreateComponentTokenSchemas();
        var catalog = CreateThemeCatalog(componentTokenSchemas);
        catalog.EnsureRequiredBuiltInThemesAvailable();
        var defaultDescriptor = catalog.ResolveDefaultDescriptor(
            HasExplicitDefaultTheme ? ExplicitDefaultThemeBaseId : null);
        var compiler = new ThemeCompiler(ThemeVariantCalculatorFactory);
        var themes = new List<Theme>();
        foreach (var descriptor in catalog.Descriptors)
        {
            if (!descriptor.IsAvailable)
            {
                continue;
            }

            foreach (var algorithms in s_algorithmCombinations)
            {
                themes.Add(new Theme(descriptor, catalog, compiler, algorithms));
            }
        }

        if (!HasExplicitDefaultTheme)
        {
            DefaultThemeId = defaultDescriptor.Id;
        }

        _themeCatalog = catalog;
        _themeCompiler = compiler;
        foreach (var theme in themes)
        {
            _themePool.Add(theme.ThemeVariant, theme);
        }

        foreach (var theme in themes)
        {
            ThemeCreated?.Invoke(this, new ThemeOperateEventArgs(theme));
            theme.NotifyRegistered();
        }

        Debug.Assert(_themePool.Count > 0);
    }

    private ResourceDictionary? TryGetLanguageResource(LanguageVariant languageVariant)
    {
        if (_languages.TryGetValue(languageVariant, out var resource))
        {
            return resource;
        }
        return null;
    }
    
    private ResourceDictionary GetLanguageResourceOrDefault(LanguageVariant languageVariant, 
                                                            ResourceDictionary defaultResourceDictionary)
    {
        return _languages.GetValueOrDefault(languageVariant, defaultResourceDictionary);
    }

    public void AddCustomThemePaths(IList<string> paths)
    {
        foreach (var path in paths)
        {
            var fullPath = Path.GetFullPath(path);
            if (!_customThemeDirs.Contains(fullPath) && Directory.Exists(fullPath))
            {
                _customThemeDirs.Add(fullPath);
            }
        }
    }

    private ThemeCatalog CreateThemeCatalog(
        IReadOnlyDictionary<string, IReadOnlySet<string>> componentTokenSchemas)
    {
        var sources = new List<IThemeCatalogSource>();
        var sourcePriority = 0;
        AddDirectorySources(_customThemeDirs, false, sources, ref sourcePriority);
        AddDirectorySources(_builtInThemeDirs, false, sources, ref sourcePriority);

        foreach (var provider in _themeAssetPathProviders)
        {
            AddSources(provider.GetThemeFilePaths(), true, false, sources, ref sourcePriority);
        }

        AddSources(
            AssetLoader.GetAssets(new Uri(DEFAULT_THEME_RES_PATH), null)
                       .Select(static path => path.ToString()),
            true,
            true,
            sources,
            ref sourcePriority);

        return new ThemeCatalog(
            sources,
            CreateSharedTokenSchema(),
            componentTokenSchemas,
            ControlTokenTypes);
    }

    private static void AddDirectorySources(
        IEnumerable<string> directories,
        bool isBuiltIn,
        List<IThemeCatalogSource> sources,
        ref int sourcePriority)
    {
        foreach (var directory in directories)
        {
            if (!Directory.Exists(directory))
            {
                continue;
            }

            AddSources(
                Directory.GetFiles(directory, "*.xml"),
                isBuiltIn,
                false,
                sources,
                ref sourcePriority);
        }
    }

    private static void AddSources(
        IEnumerable<string> filePaths,
        bool isBuiltIn,
        bool isCoreAssets,
        List<IThemeCatalogSource> sources,
        ref int sourcePriority)
    {
        foreach (var filePath in filePaths.OrderBy(static path => path, StringComparer.Ordinal))
        {
            var id = Path.GetFileNameWithoutExtension(filePath);
            var isRequiredBuiltInDefault = isCoreAssets &&
                                           string.Equals(id, IThemeManager.DEFAULT_THEME_ID, StringComparison.Ordinal);
            IThemeDefinitionStreamOpener opener = filePath.StartsWith("avares://", StringComparison.OrdinalIgnoreCase)
                ? AssetThemeDefinitionStreamOpener.Instance
                : FileThemeDefinitionStreamOpener.Instance;
            sources.Add(new ThemeCatalogSource(
                id,
                filePath,
                isBuiltIn,
                isRequiredBuiltInDefault,
                sourcePriority++,
                opener));
        }
    }

    private static IReadOnlySet<string> CreateSharedTokenSchema()
    {
        var names = new HashSet<string>(
            DesignToken.GetTokenPropertyNames(DesignTokenKind.Seed),
            StringComparer.Ordinal);
        names.UnionWith(DesignToken.GetTokenPropertyNames(DesignTokenKind.Map));
        names.UnionWith(DesignToken.GetTokenPropertyNames(DesignTokenKind.Alias));
        return names;
    }

    internal IReadOnlyDictionary<string, IReadOnlySet<string>> CreateComponentTokenSchemas()
    {
        var schemas = new Dictionary<string, IReadOnlySet<string>>(StringComparer.Ordinal);
        var errors = new List<string>();
        foreach (var registration in ControlTokenTypes)
        {
            if (registration.TryGetIdentity(out var registeredIdentity))
            {
                if (!typeof(AbstractControlDesignToken).IsAssignableFrom(registration.TokenType))
                {
                    errors.Add(
                        $"Registration '{registration.TokenType.FullName}' does not create an {nameof(AbstractControlDesignToken)}.");
                    continue;
                }

                if (!schemas.TryAdd(
                        registeredIdentity.TokenId,
                        CreateComponentTokenSchema(registration.TokenType)))
                {
                    errors.Add($"Duplicate component token id '{registeredIdentity.TokenId}'.");
                }

                continue;
            }

            AbstractControlDesignToken? token;
            try
            {
                token = registration.Activate();
            }
            catch (Exception exception)
            {
                errors.Add(
                    $"Registration '{registration.TokenType.FullName}' activation failed: {exception.GetBaseException().Message}");
                continue;
            }

            if (token is null)
            {
                errors.Add(
                    $"Registration '{registration.TokenType.FullName}' does not create an {nameof(AbstractControlDesignToken)}.");
                continue;
            }

            if (!schemas.TryAdd(token.Id, CreateComponentTokenSchema(registration.TokenType)))
            {
                errors.Add($"Duplicate component token id '{token.Id}'.");
            }
        }

        if (errors.Count > 0)
        {
            throw new ThemeLoadException(
                $"Invalid control token registrations: {string.Join(" ", errors)}");
        }

        return schemas;
    }

    private static IReadOnlySet<string> CreateComponentTokenSchema(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
        Type tokenType)
    {
        var names = tokenType
                    .GetProperties(System.Reflection.BindingFlags.Instance |
                                   System.Reflection.BindingFlags.Public)
                    .Where(static property => property.SetMethod?.IsPublic == true)
                    .Select(static property => property.Name);
        return new HashSet<string>(names, StringComparer.Ordinal);
    }

    internal void Configure()
    {
        ScanThemes();
        foreach (var provider in _controlThemesProviders)
        {
            foreach (var resourceProvider in provider.ControlThemes)
            {
                Resources.MergedDictionaries.Add(resourceProvider);
            }
        }
        _controlThemesProviders.Clear();
        BuildLanguageResources();
        SwitchLanguageResource(null, LanguageVariant);
    }

    private void SwitchLanguageResource(LanguageVariant? oldVariant, LanguageVariant? newVariant)
    {
        if (oldVariant != null)
        {
            var oldResource = TryGetLanguageResource(oldVariant);
            if (oldResource != null)
            {
                Resources.MergedDictionaries.Remove(oldResource);
            }
        }

        newVariant ??= IThemeManager.DEFAULT_LANGUAGE;
        var languageResource = TryGetLanguageResource(newVariant);
        if (_languages.TryGetValue(IThemeManager.DEFAULT_LANGUAGE, out var defaultLang))
        {
            languageResource ??= defaultLang;
        }

        if (languageResource != null && !Resources.MergedDictionaries.Contains(languageResource))
        {
            Resources.MergedDictionaries.Add(languageResource);
        }
    }

    private void BuildLanguageResources()
    {
        if (_languageProviders is not null)
        {
            foreach (var languageProvider in _languageProviders)
            {
                var languageVariant = LanguageVariant.FromCode(languageProvider.LangCode);
                if (!_languages.TryGetValue(languageVariant, out var resourceDictionary))
                {
                    resourceDictionary           = new ResourceDictionary();
                    _languages[languageVariant] = resourceDictionary;
                }

                languageProvider.BuildResourceDictionary(resourceDictionary);
            }

            _languageProviders = null;
        }
    }

    internal virtual void NotifyInitialized()
    {
        Initialized?.Invoke(this, EventArgs.Empty);
    }

    internal virtual void NotifyAttachedToApplication()
    {
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == LanguageVariantProperty)
        {
            SwitchLanguageResource(change.OldValue as LanguageVariant, change.NewValue as LanguageVariant);
            NotifyLanguageVariantChanged();
            LanguageVariantChanged?.Invoke(this, new LanguageVariantChangedEventArgs(LanguageVariant, change.GetOldValue<LanguageVariant>()));
        }
        else if (change.Property == ThemeVariantProperty)
        {
            if (!_themeCoordinator.IsCommitting)
            {
                _themeCoordinator.Request(CreateThemeRequest(
                    ThemeVariant,
                    ThemeTransitionReason.ApplicationThemeVariantChanged));
            }
        }
        else if (change.Property == IsDarkThemeModeProperty ||
                 change.Property == IsCompactThemeModeProperty)
        {
            if (!_themeCoordinator.IsCommitting)
            {
                _themeCoordinator.Request(CreateThemeRequest(
                    ActivatedTheme?.Id ?? GetDefaultRequestThemeId(),
                    IsDarkThemeMode,
                    IsCompactThemeMode,
                    ThemeTransitionReason.PropertyChanged));
            }
        }
        else if (change.Property == IsMotionEnabledProperty)
        {
            ConfigureEnableMotion();
        }
        else if (change.Property == IsWaveSpiritEnabledProperty)
        {
            ConfigureEnableWaveSpirit();
        }
    }
    
    protected virtual void NotifyLanguageVariantChanged()
    {}

    internal IThemeVariantCalculator CreateThemeVariantCalculator(ThemeAlgorithm algorithm, IThemeVariantCalculator? baseCalculator)
    {
        if (ThemeVariantCalculatorFactory != null)
        {
            return ThemeVariantCalculatorFactory.Create(algorithm, baseCalculator);
        }

        if (algorithm == ThemeAlgorithm.Default)
        {
            return new DefaultThemeVariantCalculator();
        }
        if (algorithm == ThemeAlgorithm.Dark)
        {
            Debug.Assert(baseCalculator is not null);
            return new DarkThemeVariantCalculator(baseCalculator);
        } 
        if (algorithm == ThemeAlgorithm.Compact)
        {
            Debug.Assert(baseCalculator is not null);
            return new CompactThemeVariantCalculator(baseCalculator);
        }

        throw new ArgumentOutOfRangeException(nameof(algorithm), $"Unsupported theme variant algorithm: {algorithm}");
    }

    public void AttachApplication(Application application)
    {
        _themeCoordinator.AttachApplication(application);
        application.Styles.Add(this);
        NotifyAttachedToApplication();
        _themeCoordinator.Request(CreateThemeRequest(
            new ThemeVariant(DefaultThemeId, null),
            ThemeTransitionReason.Startup));
        this[!ThemeVariantProperty] = application[!Application.ActualThemeVariantProperty];
    }

    private void ConfigureThemeSwitchRuntimeResources(ITheme? oldTheme)
    {
        if (oldTheme is not null)
        {
            ConfigureEnableMotion();
            ConfigureEnableWaveSpirit();
            return;
        }

        if (TryGetResource(SharedTokenKind.EnableMotion, ThemeVariant, out var enableMotionResource) &&
            enableMotionResource is bool enableMotion)
        {
            SetCurrentValue(IsMotionEnabledProperty, enableMotion);
        }

        if (TryGetResource(SharedTokenKind.EnableWaveSpirit, ThemeVariant, out var enableWaveSpiritResource) &&
            enableWaveSpiritResource is bool enableWaveSpirit)
        {
            SetCurrentValue(IsWaveSpiritEnabledProperty, enableWaveSpirit);
        }
    }

    internal ThemeRequest CreateThemeRequest(
        ThemeVariant themeVariant,
        ThemeTransitionReason reason)
    {
        var variantName = themeVariant.Key?.ToString() ?? themeVariant.ToString();
        var baseThemeId = ExplicitDefaultThemeBaseId;
        if (baseThemeId is not null &&
            variantName.StartsWith(baseThemeId, StringComparison.Ordinal))
        {
            var suffix = variantName[baseThemeId.Length..];
            return CreateThemeRequest(
                baseThemeId,
                suffix.Contains($"-{nameof(ThemeAlgorithm.Dark)}", StringComparison.Ordinal),
                suffix.Contains($"-{nameof(ThemeAlgorithm.Compact)}", StringComparison.Ordinal),
                reason);
        }

        var themeId    = variantName;
        var hasCompact = TryTrimAlgorithmSuffix(ref themeId, ThemeAlgorithm.Compact);
        var hasDark    = TryTrimAlgorithmSuffix(ref themeId, ThemeAlgorithm.Dark);
        return CreateThemeRequest(themeId, hasDark, hasCompact, reason);
    }

    internal ThemeRequest CreateThemeRequest(
        string themeId,
        bool hasDark,
        bool hasCompact,
        ThemeTransitionReason reason)
    {
        var algorithms = new List<ThemeAlgorithm>
        {
            ThemeAlgorithm.Default
        };
        if (hasDark)
        {
            algorithms.Add(ThemeAlgorithm.Dark);
        }

        if (hasCompact)
        {
            algorithms.Add(ThemeAlgorithm.Compact);
        }

        return new ThemeRequest(themeId, algorithms, reason);
    }

    private string GetDefaultRequestThemeId()
    {
        if (ExplicitDefaultThemeBaseId is not null)
        {
            return ExplicitDefaultThemeBaseId;
        }

        var defaultThemeId = DefaultThemeId;
        TryTrimAlgorithmSuffix(ref defaultThemeId, ThemeAlgorithm.Compact);
        TryTrimAlgorithmSuffix(ref defaultThemeId, ThemeAlgorithm.Dark);
        return defaultThemeId;
    }

    private static bool TryTrimAlgorithmSuffix(ref string themeId, ThemeAlgorithm algorithm)
    {
        var suffix = $"-{algorithm}";
        if (!themeId.EndsWith(suffix, StringComparison.Ordinal))
        {
            return false;
        }

        themeId = themeId[..^suffix.Length];
        return true;
    }

    internal void NotifyThemeChanged(Theme newTheme, ITheme? oldTheme)
    {
        NotifyThemeChanged(new ThemeChangedEventArgs(newTheme, oldTheme));
    }

    private void NotifyThemeChanged(ThemeChangedEventArgs args)
    {
        var handlers = ThemeChanged;
        if (handlers is null)
        {
            return;
        }

        foreach (EventHandler<ThemeChangedEventArgs> handler in handlers.GetInvocationList())
        {
            try
            {
                handler(this, args);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }
        }
    }

    private void NotifyThemeOperate(
        EventHandler<ThemeOperateEventArgs>? handlers,
        ThemeOperateEventArgs args)
    {
        if (handlers is null)
        {
            return;
        }

        foreach (EventHandler<ThemeOperateEventArgs> handler in handlers.GetInvocationList())
        {
            try
            {
                handler(this, args);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }
        }
    }

    private void ConfigureEnableMotion()
    {
        var themeResource = Resources.ThemeDictionaries[ThemeVariant];
        if (themeResource is ResourceDictionary globalResourceDictionary)
        {
            globalResourceDictionary[SharedTokenKind.EnableMotion] = IsMotionEnabled;
        }
    }
    
    private void ConfigureEnableWaveSpirit()
    {
        var themeResource = Resources.ThemeDictionaries[ThemeVariant];
        if (themeResource is ResourceDictionary globalResourceDictionary)
        {
            globalResourceDictionary[SharedTokenKind.EnableWaveSpirit] = IsWaveSpiritEnabled;
        }
    }
}

public class ThemeOperateEventArgs : EventArgs
{
    public ITheme? Theme { get; }

    public ThemeOperateEventArgs(ITheme? theme)
    {
        Theme = theme;
    }
}

public class ThemeChangedEventArgs : EventArgs
{
    public ITheme? OldTheme { get; }
    public ITheme NewTheme { get; }

    public ThemeChangedEventArgs(ITheme newTheme, ITheme? oldTheme)
    {
        NewTheme = newTheme;
        OldTheme = oldTheme;
    }
}
