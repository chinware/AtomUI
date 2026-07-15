using AtomUI.Theme.Catalog;
using AtomUI.Theme.Compilation;
using AtomUI.Theme.Resources;
using AtomUI.Theme.Styling;
using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;

namespace AtomUI.Theme;

/// <summary>
/// Compatibility facade over a catalog descriptor and its compiled snapshot.
/// </summary>
internal class Theme : AvaloniaObject, ITheme
{
    protected bool Loaded;
    protected bool LoadedStatus = true;
    protected bool Activated;

    protected ResourceDictionary ResourceDictionary;
    protected Dictionary<string, IControlDesignToken> ControlTokens;

    private readonly ThemeDescriptor _descriptor;
    private readonly ThemeCatalog _catalog;
    private readonly ThemeCompiler _compiler;
    private readonly ThemeSnapshotCache _snapshotCache;
    private readonly string _id;
    private readonly ThemeVariant _themeVariant;
    private readonly List<ThemeAlgorithm> _algorithms;
    private string? _loadErrorMsg;
    private DesignToken _sharedToken;
    private bool _isPrimary;
    private ThemeSnapshot? _snapshot;
    private IDisposable? _activeSnapshotPin;

    internal Theme(
        ThemeDescriptor descriptor,
        ThemeCatalog catalog,
        ThemeCompiler compiler,
        ThemeSnapshotCache snapshotCache,
        IReadOnlyList<ThemeAlgorithm> algorithms)
    {
        ArgumentNullException.ThrowIfNull(descriptor);
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(compiler);
        ArgumentNullException.ThrowIfNull(snapshotCache);
        ArgumentNullException.ThrowIfNull(algorithms);

        _descriptor         = descriptor;
        _catalog            = catalog;
        _compiler           = compiler;
        _snapshotCache      = snapshotCache;
        _id                 = descriptor.Id;
        _themeVariant       = BuildThemeVariant(_id, algorithms);
        _algorithms         = new List<ThemeAlgorithm>(algorithms);
        _sharedToken        = new DesignToken();
        ResourceDictionary  = new ResourceDictionary();
        ControlTokens       = new Dictionary<string, IControlDesignToken>();
        DefinitionFilePath  = descriptor.DefinitionFilePath;
        _isPrimary          = IsPrimaryAlgorithmSet(descriptor.Definition, algorithms);
    }

    internal Theme(
        ThemeDescriptor descriptor,
        ThemeCatalog catalog,
        ThemeCompiler compiler,
        IReadOnlyList<ThemeAlgorithm> algorithms)
        : this(descriptor, catalog, compiler, new ThemeSnapshotCache(), algorithms)
    {
    }

    public string DefinitionFilePath { get; }
    public string Id => _id;
    public string DisplayName => _descriptor.Definition?.DisplayName ?? _id;
    public bool LoadStatus => LoadedStatus;
    public string? LoadErrorMsg => _loadErrorMsg;
    public bool IsLoaded => Loaded;
    public ThemeVariant ThemeVariant => _themeVariant;
    public ResourceDictionary ThemeResource => ResourceDictionary;
    public bool IsDarkMode { get; private set; }
    public bool IsActivated => Activated;
    public bool IsBuiltIn => _descriptor.IsBuiltIn;
    public bool IsPrimary => _isPrimary;
    public DesignToken SharedToken => _sharedToken;
    public IList<ThemeAlgorithm> Algorithms => _algorithms;
    internal ThemeSnapshot Snapshot =>
        _snapshot ?? throw new InvalidOperationException($"Theme '{_id}' has not been loaded.");

    public List<string> ThemeResourceKeys
    {
        get
        {
            var keys = new List<string>(ResourceDictionary.Keys.Count);
            foreach (var key in ResourceDictionary.Keys)
            {
                keys.Add(key.ToString()!);
            }

            return keys;
        }
    }

    internal void Load(IReadOnlyDictionary<string, string>? runtimeOverrides = null)
    {
        if (Loaded)
        {
            throw new InvalidOperationException($"Theme: {_id} already loaded");
        }

        try
        {
            var request = _catalog.CreateCompileRequest(_id, _algorithms, runtimeOverrides);
            var result = Compile(request);
            if (!result.Success)
            {
                throw CreateCompilationException(result);
            }

            Hydrate(result.Snapshot!);
            _loadErrorMsg = null;
            LoadedStatus = true;
            Loaded       = true;
        }
        catch (Exception exception)
        {
            _loadErrorMsg = exception.Message;
            LoadedStatus  = false;
            throw;
        }
    }

    private void Hydrate(ThemeSnapshot snapshot)
    {
        var resources = new ResourceDictionary();
        var controlTokens = new Dictionary<string, IControlDesignToken>(StringComparer.Ordinal);
        foreach (var resource in snapshot.SharedResources)
        {
            resources[resource.Key] = resource.Value;
        }

        foreach (var component in snapshot.Components.Values)
        {
            foreach (var resource in component.ControlResources)
            {
                resources[resource.Key] = resource.Value;
            }

            var controlToken = component.ControlToken;
            controlTokens.Add(controlToken.Id, controlToken);
        }

        resources.MergedDictionaries.Add(new ThemeTokenResourceProvider(snapshot));

        ResourceDictionary = resources;
        ControlTokens      = controlTokens;
        _sharedToken       = DesignTokenClone.DeepClone(snapshot.SharedTokenCore);
        IsDarkMode         = snapshot.IsDark;
        _isPrimary         = IsPrimaryAlgorithmSet(_descriptor.Definition, snapshot.Algorithms);
        _snapshot          = snapshot;
    }

    protected virtual ThemeCompileResult Compile(ThemeCompileRequest request)
    {
        return _snapshotCache.GetOrCompile(request, _compiler);
    }

    private static ThemeLoadException CreateCompilationException(ThemeCompileResult result)
    {
        if (result.Exception is not null)
        {
            return new ThemeLoadException("Theme compilation failed.", result.Exception);
        }

        var errors = result.Diagnostics
                           .Where(static diagnostic => diagnostic.Severity == Definitions.ThemeDiagnosticSeverity.Error)
                           .Select(static diagnostic => diagnostic.Message)
                           .ToArray();
        var message = errors.Length == 0
            ? "Theme compilation failed."
            : $"Theme compilation failed: {string.Join(" ", errors)}";
        return new ThemeLoadException(message);
    }

    private static bool IsPrimaryAlgorithmSet(
        ThemeDefinition? definition,
        IReadOnlyList<ThemeAlgorithm> algorithms)
    {
        return definition is not null &&
               algorithms.Count == definition.Algorithms.Count &&
               algorithms.All(definition.Algorithms.Contains);
    }

    internal static ThemeVariant BuildThemeVariant(string id, IList<ThemeAlgorithm> algorithms)
    {
        var hasDark    = algorithms.Contains(DarkThemeVariantCalculator.Algorithm);
        var hasCompact = algorithms.Contains(CompactThemeVariantCalculator.Algorithm);
        return BuildThemeVariant(id, hasDark, hasCompact);
    }

    internal static ThemeVariant BuildThemeVariant(string id, IReadOnlyList<ThemeAlgorithm> algorithms)
    {
        var hasDark    = algorithms.Contains(DarkThemeVariantCalculator.Algorithm);
        var hasCompact = algorithms.Contains(CompactThemeVariantCalculator.Algorithm);
        return BuildThemeVariant(id, hasDark, hasCompact);
    }

    internal static ThemeVariant BuildThemeVariant(string id, bool hasDark, bool hasCompact)
    {
        return new ThemeVariant(BuildThemeVariantName(id, hasDark, hasCompact), null);
    }

    internal static string BuildThemeVariantName(string id, bool hasDark, bool hasCompact)
    {
        var variantName = id;
        if (hasDark)
        {
            variantName += $"-{nameof(ThemeAlgorithm.Dark)}";
        }

        if (hasCompact)
        {
            variantName += $"-{nameof(ThemeAlgorithm.Compact)}";
        }

        return variantName;
    }

    internal static ISet<ThemeAlgorithm> CheckAlgorithmNames(IList<string> algorithmNames)
    {
        var algorithms = new HashSet<ThemeAlgorithm>(algorithmNames.Count);
        foreach (var algorithmName in algorithmNames)
        {
            if (!Enum.TryParse<ThemeAlgorithm>(algorithmName, out var algorithm))
            {
                throw new ThemeLoadException(
                    $"Algorithm: {algorithm} is not supported. Supported algorithms are: {ThemeAlgorithm.Default}, {ThemeAlgorithm.Dark}, {ThemeAlgorithm.Compact}.");
            }
            algorithms.Add(algorithm);
        }
        return algorithms;
    }

    public IControlDesignToken? GetControlToken(string tokenId)
    {
        return ControlTokens.GetValueOrDefault(tokenId);
    }

    internal virtual void NotifyAboutToActive()
    {
    }

    internal virtual void NotifyActivated()
    {
        if (_snapshot is not null && _activeSnapshotPin is null)
        {
            _activeSnapshotPin = _snapshotCache.Pin(_snapshot);
        }
        Activated = true;
    }

    internal virtual void NotifyAboutToDeActive()
    {
    }

    internal virtual void NotifyDeActivated()
    {
        _activeSnapshotPin?.Dispose();
        _activeSnapshotPin = null;
        Activated = false;
    }

    internal virtual void NotifyAboutToLoad()
    {
    }

    internal virtual void NotifyLoaded()
    {
    }

    internal virtual void NotifyAboutToUnload()
    {
    }

    internal virtual void NotifyUnloaded()
    {
    }

    internal virtual void NotifyRegistered()
    {
    }
}
