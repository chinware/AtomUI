using System.Collections.ObjectModel;
using AtomUI.Theme.Compilation;
using AtomUI.Theme.Definitions;
using AtomUI.Theme.Resources;
using AtomUI.Theme.TokenSystem;
using Avalonia.Platform;

namespace AtomUI.Theme.Catalog;

internal interface IThemeCatalogSource
{
    string Id { get; }
    string DefinitionFilePath { get; }
    bool IsBuiltIn { get; }
    bool IsRequiredBuiltInDefault { get; }
    int SourcePriority { get; }
    Stream OpenRead();
}

internal interface IThemeDefinitionStreamOpener
{
    Stream OpenRead(string definitionFilePath);
}

internal sealed class FileThemeDefinitionStreamOpener : IThemeDefinitionStreamOpener
{
    internal static readonly FileThemeDefinitionStreamOpener Instance = new();

    private FileThemeDefinitionStreamOpener()
    {
    }

    public Stream OpenRead(string definitionFilePath)
    {
        return File.OpenRead(definitionFilePath);
    }
}

internal sealed class AssetThemeDefinitionStreamOpener : IThemeDefinitionStreamOpener
{
    internal static readonly AssetThemeDefinitionStreamOpener Instance = new();

    private AssetThemeDefinitionStreamOpener()
    {
    }

    public Stream OpenRead(string definitionFilePath)
    {
        return AssetLoader.Open(new Uri(definitionFilePath));
    }
}

internal sealed class ThemeCatalogSource : IThemeCatalogSource
{
    private readonly IThemeDefinitionStreamOpener _streamOpener;

    internal ThemeCatalogSource(
        string id,
        string definitionFilePath,
        bool isBuiltIn,
        bool isRequiredBuiltInDefault,
        int sourcePriority,
        IThemeDefinitionStreamOpener streamOpener)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(definitionFilePath);
        ArgumentNullException.ThrowIfNull(streamOpener);

        Id                       = id;
        DefinitionFilePath       = definitionFilePath;
        IsBuiltIn                = isBuiltIn;
        IsRequiredBuiltInDefault = isRequiredBuiltInDefault;
        SourcePriority           = sourcePriority;
        _streamOpener            = streamOpener;
    }

    public string Id { get; }
    public string DefinitionFilePath { get; }
    public bool IsBuiltIn { get; }
    public bool IsRequiredBuiltInDefault { get; }
    public int SourcePriority { get; }

    public Stream OpenRead()
    {
        return _streamOpener.OpenRead(DefinitionFilePath);
    }
}

internal sealed class ThemeCatalog
{
    private const string DuplicateIdCode = "ATMTHM011";
    private const string SourceOpenCode = "ATMTHM012";
    private const string CatalogPath = "ThemeCatalog";

    private static readonly IReadOnlyDictionary<string, string> s_emptySharedOverrides =
        new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal));

    private static readonly IReadOnlyDictionary<ComponentTokenIdentity, ControlTokenConfigInfo> s_emptyComponentOverrides =
        new ReadOnlyDictionary<ComponentTokenIdentity, ControlTokenConfigInfo>(
            new Dictionary<ComponentTokenIdentity, ControlTokenConfigInfo>());

    private readonly Dictionary<string, ThemeDescriptor> _descriptors;
    private readonly IReadOnlyDictionary<string, IReadOnlySet<string>> _componentOwnTokenNames;
    private readonly IReadOnlyList<ControlTokenRegistration> _registrations;
    private readonly IReadOnlyList<ThemeDescriptor> _requiredBuiltInDescriptors;

    internal ThemeCatalog(
        IEnumerable<IThemeCatalogSource> sources,
        IReadOnlySet<string> sharedTokenNames,
        IReadOnlyDictionary<string, IReadOnlySet<string>> componentOwnTokenNames,
        IEnumerable<ControlTokenRegistration> registrations)
    {
        ArgumentNullException.ThrowIfNull(sources);
        ArgumentNullException.ThrowIfNull(sharedTokenNames);
        ArgumentNullException.ThrowIfNull(componentOwnTokenNames);
        ArgumentNullException.ThrowIfNull(registrations);

        _componentOwnTokenNames = CopyComponentOwnTokenNames(componentOwnTokenNames);
        _registrations          = Array.AsReadOnly(registrations.ToArray());
        _descriptors            = new Dictionary<string, ThemeDescriptor>(StringComparer.Ordinal);

        var requiredBuiltIns = new List<ThemeDescriptor>();
        var diagnostics = new List<ThemeDefinitionDiagnostic>();
        foreach (var source in sources.OrderBy(static source => source.SourcePriority)
                                      .ThenBy(static source => source.DefinitionFilePath, StringComparer.Ordinal))
        {
            var descriptor = ParseSource(source, sharedTokenNames, _componentOwnTokenNames);
            diagnostics.AddRange(descriptor.Diagnostics);
            if (source.IsRequiredBuiltInDefault)
            {
                requiredBuiltIns.Add(descriptor);
            }

            if (_descriptors.TryGetValue(descriptor.Id, out var winner))
            {
                var duplicateDiagnostic = CreateDuplicateDiagnostic(winner, descriptor);
                _descriptors[descriptor.Id] = winner.WithAdditionalDiagnostic(duplicateDiagnostic);
                diagnostics.Add(duplicateDiagnostic);
                continue;
            }

            _descriptors.Add(descriptor.Id, descriptor);
        }

        Descriptors = Array.AsReadOnly(
            _descriptors.Values
                        .OrderBy(static descriptor => descriptor.SourcePriority)
                        .ThenBy(static descriptor => descriptor.DefinitionFilePath, StringComparer.Ordinal)
                        .ToArray());
        Diagnostics = Array.AsReadOnly(diagnostics.ToArray());
        _requiredBuiltInDescriptors = Array.AsReadOnly(
            requiredBuiltIns
                .OrderBy(static descriptor => descriptor.SourcePriority)
                .ThenBy(static descriptor => descriptor.DefinitionFilePath, StringComparer.Ordinal)
                .ToArray());
    }

    public IReadOnlyList<ThemeDescriptor> Descriptors { get; }
    public IReadOnlyList<ThemeDefinitionDiagnostic> Diagnostics { get; }

    internal ThemeDescriptor? GetDescriptor(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        return _descriptors.GetValueOrDefault(id);
    }

    internal void EnsureRequiredBuiltInThemesAvailable()
    {
        if (_requiredBuiltInDescriptors.Count != 1 ||
            !string.Equals(
                _requiredBuiltInDescriptors[0].Id,
                IThemeManager.DEFAULT_THEME_ID,
                StringComparison.Ordinal))
        {
            throw new ThemeLoadException(
                $"Exactly one required Core '{IThemeManager.DEFAULT_THEME_ID}' theme source must be registered.");
        }

        foreach (var descriptor in _requiredBuiltInDescriptors)
        {
            if (!descriptor.IsAvailable)
            {
                throw new ThemeLoadException(
                    $"Required built-in theme '{descriptor.Id}' from '{descriptor.DefinitionFilePath}' is unavailable: {DescribeErrors(descriptor)}");
            }
        }
    }

    internal ThemeDescriptor ResolveDefaultDescriptor(string? explicitThemeId)
    {
        if (explicitThemeId is not null)
        {
            var explicitDescriptor = GetDescriptor(explicitThemeId);
            if (explicitDescriptor is null)
            {
                throw new ThemeLoadException($"Explicitly selected theme '{explicitThemeId}' was not found in the theme catalog.");
            }

            if (!explicitDescriptor.IsAvailable)
            {
                throw new ThemeLoadException(
                    $"Explicitly selected theme '{explicitThemeId}' is unavailable: {DescribeErrors(explicitDescriptor)}");
            }

            return explicitDescriptor;
        }

        var defaultDescriptors = Descriptors
            .Where(static descriptor => descriptor.IsAvailable && descriptor.Definition!.IsDefault)
            .OrderBy(static descriptor => descriptor.SourcePriority)
            .ThenBy(static descriptor => descriptor.DefinitionFilePath, StringComparer.Ordinal)
            .ToArray();
        if (defaultDescriptors.Length == 0)
        {
            throw new ThemeLoadException("No available theme definition declares IsDefault='true'.");
        }

        if (defaultDescriptors.Length > 1)
        {
            throw new ThemeLoadException(
                $"Multiple available theme definitions declare IsDefault='true': {string.Join(", ", defaultDescriptors.Select(static descriptor => descriptor.Id))}.");
        }

        return defaultDescriptors[0];
    }

    internal ThemeCompileRequest CreateCompileRequest(
        string themeId,
        IReadOnlyList<ThemeAlgorithm> algorithms,
        IReadOnlyDictionary<string, string>? runtimeOverrides = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(themeId);
        ArgumentNullException.ThrowIfNull(algorithms);

        var descriptor = GetDescriptor(themeId);
        if (descriptor is null)
        {
            throw new ThemeNotFoundException($"Theme '{themeId}' was not found in the theme catalog.");
        }

        if (!descriptor.IsAvailable)
        {
            throw new ThemeLoadException($"Theme '{themeId}' is unavailable: {DescribeErrors(descriptor)}");
        }

        return new ThemeCompileRequest(
            descriptor.Id,
            FilterUnavailableComponentDefinitions(descriptor.Definition!),
            null,
            Array.AsReadOnly(algorithms.ToArray()),
            s_emptySharedOverrides,
            s_emptyComponentOverrides,
            Array.AsReadOnly(_registrations.ToArray()),
            runtimeOverrides ?? s_emptySharedOverrides);
    }

    private static ThemeDescriptor ParseSource(
        IThemeCatalogSource source,
        IReadOnlySet<string> sharedTokenNames,
        IReadOnlyDictionary<string, IReadOnlySet<string>> componentOwnTokenNames)
    {
        ArgumentNullException.ThrowIfNull(source);
        try
        {
            using var stream = source.OpenRead();
            var result = ThemeDefinitionParser.Parse(new ThemeDefinitionParseRequest(
                stream,
                source.DefinitionFilePath,
                sharedTokenNames,
                componentOwnTokenNames));
            return new ThemeDescriptor(
                result.Definition?.Id ?? source.Id,
                source.DefinitionFilePath,
                source.IsBuiltIn,
                source.SourcePriority,
                result.Definition,
                result.Diagnostics);
        }
        catch (Exception exception)
        {
            return new ThemeDescriptor(
                source.Id,
                source.DefinitionFilePath,
                source.IsBuiltIn,
                source.SourcePriority,
                null,
                [new ThemeDefinitionDiagnostic(
                    SourceOpenCode,
                    ThemeDiagnosticSeverity.Error,
                    source.DefinitionFilePath,
                    0,
                    0,
                    CatalogPath,
                    $"Failed to open theme source: {exception.GetBaseException().Message}")]);
        }
    }

    private ThemeDefinition FilterUnavailableComponentDefinitions(ThemeDefinition definition)
    {
        var registeredComponents = new Dictionary<string, ThemeControlTokenDefinition>(StringComparer.Ordinal);
        foreach (var entry in definition.ControlTokens)
        {
            if (_componentOwnTokenNames.ContainsKey(entry.Key))
            {
                registeredComponents.Add(entry.Key, entry.Value);
            }
        }

        return new ThemeDefinition(
            definition.Id,
            definition.DisplayName,
            definition.IsDefault,
            definition.Algorithms,
            definition.SharedTokens,
            registeredComponents);
    }

    private static IReadOnlyDictionary<string, IReadOnlySet<string>> CopyComponentOwnTokenNames(
        IReadOnlyDictionary<string, IReadOnlySet<string>> componentOwnTokenNames)
    {
        var copied = new Dictionary<string, IReadOnlySet<string>>(
            componentOwnTokenNames.Count,
            StringComparer.Ordinal);
        foreach (var entry in componentOwnTokenNames)
        {
            copied.Add(entry.Key, new HashSet<string>(entry.Value, StringComparer.Ordinal));
        }

        return new ReadOnlyDictionary<string, IReadOnlySet<string>>(copied);
    }

    private static ThemeDefinitionDiagnostic CreateDuplicateDiagnostic(
        ThemeDescriptor winner,
        ThemeDescriptor duplicate)
    {
        return new ThemeDefinitionDiagnostic(
            DuplicateIdCode,
            ThemeDiagnosticSeverity.Warning,
            duplicate.DefinitionFilePath,
            0,
            0,
            CatalogPath,
            $"Theme id '{winner.Id}' from '{winner.DefinitionFilePath}' (priority {winner.SourcePriority}) wins over duplicate source '{duplicate.DefinitionFilePath}' (priority {duplicate.SourcePriority}).");
    }

    private static string DescribeErrors(ThemeDescriptor descriptor)
    {
        var errors = descriptor.Diagnostics
            .Where(static diagnostic => diagnostic.Severity == ThemeDiagnosticSeverity.Error)
            .Select(static diagnostic => diagnostic.Message)
            .ToArray();
        return errors.Length == 0 ? "No parse diagnostics were recorded." : string.Join(" ", errors);
    }
}
