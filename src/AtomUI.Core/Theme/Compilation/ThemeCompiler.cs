using System.Collections.ObjectModel;
using System.Threading;
using AtomUI.Theme.Definitions;
using AtomUI.Theme.Resources;
using AtomUI.Theme.Styling;
using AtomUI.Theme.TokenSystem;
using Avalonia.Controls;

namespace AtomUI.Theme.Compilation;

internal sealed class ThemeCompiler
{
    private const string CompilerPath = "ThemeCompiler";
    private static long s_nextVersion;

    private readonly IThemeVariantCalculatorFactory? _calculatorFactory;

    internal ThemeCompiler(IThemeVariantCalculatorFactory? calculatorFactory = null)
    {
        _calculatorFactory = calculatorFactory;
    }

    internal ThemeCompileResult Compile(ThemeCompileRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var diagnostics = new List<ThemeDefinitionDiagnostic>();
        try
        {
            if (!ValidateRequest(request, diagnostics, out var registrations))
            {
                return Failed(diagnostics);
            }

            var algorithms = ResolveEffectiveAlgorithms(request);
            var componentConfigs = MergeComponentConfigs(request);
            var calculator = CreateCalculator(algorithms);
            var sharedConfig = MergeSharedConfigs(request);
            var sharedToken = CreateSharedToken();
            ApplySharedConfig(sharedToken, sharedConfig, calculator);
            FreezeColorPalettes(sharedToken);

            var sharedResources = BuildResourceMap(sharedToken);
            var components = BuildComponents(
                request,
                registrations,
                componentConfigs,
                algorithms,
                calculator,
                sharedToken,
                sharedResources,
                diagnostics);
            if (diagnostics.Any(static diagnostic => diagnostic.Severity == ThemeDiagnosticSeverity.Error))
            {
                return Failed(diagnostics);
            }

            return new ThemeCompileResult(
                new ThemeSnapshot(
                    request.ThemeId,
                    Interlocked.Increment(ref s_nextVersion),
                    CopyAlgorithms(algorithms),
                    algorithms.Contains(ThemeAlgorithm.Dark),
                    sharedToken,
                    sharedResources,
                    ReadOnly(components),
                    componentConfigs,
                    sharedConfig),
                CopyDiagnostics(diagnostics),
                null);
        }
        catch (Exception exception)
        {
            return new ThemeCompileResult(null, CopyDiagnostics(diagnostics), exception);
        }
    }

    private bool ValidateRequest(
        ThemeCompileRequest request,
        List<ThemeDefinitionDiagnostic> diagnostics,
        out Dictionary<ComponentTokenIdentity, AbstractControlDesignToken> registrations)
    {
        var isValid = true;
        var sharedTokenNames = GetSharedTokenNames();
        isValid &= ValidateSharedNames(request.Definition.SharedTokens, sharedTokenNames, diagnostics);
        isValid &= ValidateSharedNames(request.SharedOverrides, sharedTokenNames, diagnostics);
        isValid &= ValidateSharedNames(request.RuntimeOverrides, sharedTokenNames, diagnostics);

        registrations = new Dictionary<ComponentTokenIdentity, AbstractControlDesignToken>();
        foreach (var registration in request.Registrations)
        {
            AbstractControlDesignToken? token;
            try
            {
                token = registration.Activate();
            }
            catch (Exception exception)
            {
                AddError(
                    diagnostics,
                    "THEME001",
                    $"Registration '{registration.TokenType.FullName}' activation failed: {exception.GetBaseException().Message}");
                isValid = false;
                continue;
            }

            if (token is null)
            {
                AddError(diagnostics, "THEME001", $"Registration '{registration.TokenType.FullName}' does not create an {nameof(AbstractControlDesignToken)}.");
                isValid = false;
                continue;
            }

            var identity = registration.GetIdentity(token);
            if (!registrations.TryAdd(identity, token))
            {
                AddError(diagnostics, "THEME002", $"Duplicate component token identity '{identity}'.");
                isValid = false;
            }
        }

        foreach (var definition in request.Definition.ControlTokens.Values)
        {
            var identity = new ComponentTokenIdentity(null, definition.TokenId);
            if (!registrations.TryGetValue(identity, out var token))
            {
                AddError(diagnostics, "THEME003", $"Unknown component token identity '{identity}'.");
                isValid = false;
                continue;
            }

            isValid &= ValidateComponentConfig(definition.Tokens, definition.SharedTokens, token, sharedTokenNames, diagnostics);
        }

        foreach (var overrideEntry in request.ComponentOverrides)
        {
            if (!registrations.TryGetValue(overrideEntry.Key, out var token))
            {
                AddError(diagnostics, "THEME003", $"Unknown component token identity '{overrideEntry.Key}'.");
                isValid = false;
                continue;
            }

            isValid &= ValidateComponentConfig(
                overrideEntry.Value.Tokens,
                overrideEntry.Value.SharedTokens,
                token,
                sharedTokenNames,
                diagnostics);
        }

        return isValid;
    }

    private Dictionary<ComponentTokenIdentity, ComponentThemeSnapshot> BuildComponents(
        ThemeCompileRequest request,
        IReadOnlyDictionary<ComponentTokenIdentity, AbstractControlDesignToken> registrations,
        IReadOnlyDictionary<ComponentTokenIdentity, ControlTokenConfigInfo> componentConfigs,
        IReadOnlyList<ThemeAlgorithm> algorithms,
        IThemeVariantCalculator calculator,
        DesignToken globalToken,
        IReadOnlyDictionary<object, object?> globalResources,
        List<ThemeDefinitionDiagnostic> diagnostics)
    {
        var components = new Dictionary<ComponentTokenIdentity, ComponentThemeSnapshot>(registrations.Count);
        foreach (var (identity, controlToken) in registrations)
        {
            componentConfigs.TryGetValue(identity, out var config);
            var effectiveToken = CloneDesignToken(globalToken);
            if (config is not null)
            {
                ApplyComponentSharedConfig(effectiveToken, config.SharedTokens, config.EnableAlgorithm, calculator);
            }
            FreezeColorPalettes(effectiveToken);

            controlToken.AssignSharedToken(effectiveToken);
            controlToken.SetHasCustomTokenConfig(config is not null);
            controlToken.SetCustomTokens(config is null ? Array.Empty<string>() : config.Tokens.Keys.ToArray());
            controlToken.CalculateTokenValues(algorithms.Contains(ThemeAlgorithm.Dark));
            if (config is not null)
            {
                controlToken.LoadConfig(config.Tokens);
            }

            var effectiveResources = BuildResourceMap(effectiveToken);
            controlToken.BuildSharedResourceDeltaDictionary(globalToken);
            components.Add(
                identity,
                new ComponentThemeSnapshot(
                    effectiveToken,
                    BuildResourceDelta(globalResources, effectiveResources),
                    controlToken,
                    BuildResourceMap(controlToken)));
        }

        return components;
    }

    private static IReadOnlyList<ThemeAlgorithm> ResolveEffectiveAlgorithms(ThemeCompileRequest request)
    {
        if (request.Algorithms.Count != 0)
        {
            return request.Algorithms;
        }

        if (request.Parent is not null)
        {
            return request.Parent.Algorithms;
        }

        return [ThemeAlgorithm.Default];
    }

    private static IReadOnlyDictionary<ComponentTokenIdentity, ControlTokenConfigInfo> MergeComponentConfigs(
        ThemeCompileRequest request)
    {
        var configs = new Dictionary<ComponentTokenIdentity, ControlTokenConfigInfo>();
        if (request.Parent is not null)
        {
            foreach (var (identity, parentConfig) in request.Parent.ComponentConfigs)
            {
                configs.Add(identity, parentConfig.Clone());
            }
        }

        foreach (var definition in request.Definition.ControlTokens.Values)
        {
            var identity = new ComponentTokenIdentity(null, definition.TokenId);
            MergeComponentConfig(
                configs,
                identity,
                definition.EnableAlgorithm,
                definition.Tokens,
                definition.SharedTokens);
        }

        foreach (var (identity, overrideConfig) in request.ComponentOverrides)
        {
            MergeComponentConfig(
                configs,
                identity,
                overrideConfig.EnableAlgorithm,
                overrideConfig.Tokens,
                overrideConfig.SharedTokens);
        }

        return ReadOnly(configs);
    }

    private static void MergeComponentConfig(
        IDictionary<ComponentTokenIdentity, ControlTokenConfigInfo> configs,
        ComponentTokenIdentity identity,
        bool enableAlgorithm,
        IEnumerable<KeyValuePair<string, string>> tokens,
        IEnumerable<KeyValuePair<string, string>> sharedTokens)
    {
        if (!configs.TryGetValue(identity, out var config))
        {
            config = new ControlTokenConfigInfo
            {
                TokenId = identity.TokenId
            };
            configs.Add(identity, config);
        }

        config.EnableAlgorithm = enableAlgorithm;
        MergeInto(config.Tokens, tokens);
        MergeInto(config.SharedTokens, sharedTokens);
    }

    private static void ApplyComponentSharedConfig(
        DesignToken token,
        IDictionary<string, string> config,
        bool enableAlgorithm,
        IThemeVariantCalculator calculator)
    {
        var buckets = CreateBuckets(config);
        token.LoadConfig(buckets.Seed);
        if (enableAlgorithm)
        {
            calculator.Calculate(token);
        }

        token.LoadConfig(buckets.Map);
        if (enableAlgorithm)
        {
            token.ColorBgBase   = calculator.ColorBgBase;
            token.ColorTextBase = calculator.ColorTextBase;
            token.CalculateAliasTokenValues();
        }

        token.LoadConfig(buckets.Alias);
    }

    private static void ApplySharedConfig(
        DesignToken token,
        IReadOnlyDictionary<string, string> config,
        IThemeVariantCalculator calculator)
    {
        var buckets = CreateBuckets(config);
        token.LoadConfig(buckets.Seed);
        calculator.Calculate(token);
        token.LoadConfig(buckets.Map);
        token.ColorBgBase   = calculator.ColorBgBase;
        token.ColorTextBase = calculator.ColorTextBase;
        token.CalculateAliasTokenValues();
        token.LoadConfig(buckets.Alias);
    }

    private static TokenConfigBuckets CreateBuckets(IEnumerable<KeyValuePair<string, string>> config)
    {
        var buckets = new TokenConfigBuckets();
        var seedNames = DesignToken.GetTokenPropertyNames(DesignTokenKind.Seed);
        var mapNames = DesignToken.GetTokenPropertyNames(DesignTokenKind.Map);
        var aliasNames = DesignToken.GetTokenPropertyNames(DesignTokenKind.Alias);
        foreach (var entry in config)
        {
            buckets.AddByTokenName(entry.Key, entry.Value, seedNames, mapNames, aliasNames);
        }

        return buckets;
    }

    private static IReadOnlyDictionary<string, string> MergeSharedConfigs(ThemeCompileRequest request)
    {
        var result = new Dictionary<string, string>(StringComparer.Ordinal);
        if (request.Parent is not null)
        {
            MergeInto(result, request.Parent.SharedConfig);
        }

        MergeInto(result, request.Definition.SharedTokens);
        MergeInto(result, request.SharedOverrides);
        MergeInto(result, request.RuntimeOverrides);
        return new ReadOnlyDictionary<string, string>(result);
    }

    private IThemeVariantCalculator CreateCalculator(IReadOnlyList<ThemeAlgorithm> algorithms)
    {
        IThemeVariantCalculator? calculator = null;
        foreach (var algorithm in algorithms)
        {
            calculator = _calculatorFactory?.Create(algorithm, calculator) ?? CreateDefaultCalculator(algorithm, calculator);
        }

        return calculator ?? throw new InvalidOperationException("A theme compilation requires at least one algorithm.");
    }

    private static IThemeVariantCalculator CreateDefaultCalculator(
        ThemeAlgorithm algorithm,
        IThemeVariantCalculator? baseCalculator)
    {
        return algorithm switch
        {
            ThemeAlgorithm.Default => new DefaultThemeVariantCalculator(),
            ThemeAlgorithm.Dark when baseCalculator is not null => new DarkThemeVariantCalculator(baseCalculator),
            ThemeAlgorithm.Compact when baseCalculator is not null => new CompactThemeVariantCalculator(baseCalculator),
            _ => throw new InvalidOperationException($"Algorithm '{algorithm}' requires a preceding calculator.")
        };
    }

    private static DesignToken CreateSharedToken()
    {
        return new DesignToken();
    }

    private static DesignToken CloneDesignToken(DesignToken source)
    {
        var clone = (DesignToken)source.Clone();
        var palettes = new Dictionary<Palette.PresetPrimaryColor, ColorMap>(source.ColorPalettes.Count);
        foreach (var palette in source.ColorPalettes)
        {
            palettes.Add(palette.Key, CloneColorMap(palette.Value));
        }

        clone.ColorPalettes = palettes;
        return clone;
    }

    private static ColorMap CloneColorMap(ColorMap source)
    {
        return new ColorMap
        {
            Color1 = source.Color1,
            Color2 = source.Color2,
            Color3 = source.Color3,
            Color4 = source.Color4,
            Color5 = source.Color5,
            Color6 = source.Color6,
            Color7 = source.Color7,
            Color8 = source.Color8,
            Color9 = source.Color9,
            Color10 = source.Color10
        };
    }

    private static void FreezeColorPalettes(DesignToken token)
    {
        var palettes = new Dictionary<Palette.PresetPrimaryColor, ColorMap>(token.ColorPalettes.Count);
        foreach (var palette in token.ColorPalettes)
        {
            palettes.Add(palette.Key, palette.Value);
        }

        token.ColorPalettes = new ReadOnlyDictionary<Palette.PresetPrimaryColor, ColorMap>(palettes);
    }

    private static IReadOnlyDictionary<object, object?> BuildResourceMap(AbstractDesignToken token)
    {
        var temporary = new ResourceDictionary();
        token.BuildResourceDictionary(temporary);
        return CopyResourceMap(temporary);
    }

    private static IReadOnlyDictionary<object, object?> BuildResourceDelta(
        IReadOnlyDictionary<object, object?> globalResources,
        IReadOnlyDictionary<object, object?> effectiveResources)
    {
        var delta = new Dictionary<object, object?>();
        foreach (var entry in effectiveResources)
        {
            if (!globalResources.TryGetValue(entry.Key, out var globalValue) || !Equals(globalValue, entry.Value))
            {
                delta.Add(entry.Key, entry.Value);
            }
        }

        return ReadOnly(delta);
    }

    private static IReadOnlyDictionary<object, object?> CopyResourceMap(ResourceDictionary source)
    {
        var copy = new Dictionary<object, object?>(source.Count);
        foreach (var key in source.Keys)
        {
            copy.Add(key, source[key]);
        }

        return ReadOnly(copy);
    }

    private static bool ValidateSharedNames(
        IEnumerable<KeyValuePair<string, string>> config,
        IReadOnlySet<string> knownNames,
        List<ThemeDefinitionDiagnostic> diagnostics)
    {
        var valid = true;
        foreach (var entry in config)
        {
            if (!knownNames.Contains(entry.Key))
            {
                AddError(diagnostics, "THEME004", $"Unknown shared token '{entry.Key}'.");
                valid = false;
            }
        }

        return valid;
    }

    private static bool ValidateComponentConfig(
        IEnumerable<KeyValuePair<string, string>> tokenConfig,
        IEnumerable<KeyValuePair<string, string>> sharedConfig,
        AbstractControlDesignToken token,
        IReadOnlySet<string> sharedTokenNames,
        List<ThemeDefinitionDiagnostic> diagnostics)
    {
        var valid = ValidateSharedNames(sharedConfig, sharedTokenNames, diagnostics);
        foreach (var entry in tokenConfig)
        {
            if (!token.HasToken(entry.Key))
            {
                AddError(diagnostics, "THEME005", $"Unknown token '{entry.Key}' for component '{token.Id}'.");
                valid = false;
            }
        }

        return valid;
    }

    private static IReadOnlySet<string> GetSharedTokenNames()
    {
        var names = new HashSet<string>(DesignToken.GetTokenPropertyNames(DesignTokenKind.Seed), StringComparer.Ordinal);
        names.UnionWith(DesignToken.GetTokenPropertyNames(DesignTokenKind.Map));
        names.UnionWith(DesignToken.GetTokenPropertyNames(DesignTokenKind.Alias));
        return names;
    }

    private static void AddError(List<ThemeDefinitionDiagnostic> diagnostics, string code, string message)
    {
        diagnostics.Add(new ThemeDefinitionDiagnostic(code, ThemeDiagnosticSeverity.Error, string.Empty, 0, 0, CompilerPath, message));
    }

    private static ThemeCompileResult Failed(List<ThemeDefinitionDiagnostic> diagnostics)
    {
        return new ThemeCompileResult(null, CopyDiagnostics(diagnostics), null);
    }

    private static IReadOnlyList<ThemeDefinitionDiagnostic> CopyDiagnostics(List<ThemeDefinitionDiagnostic> diagnostics)
    {
        return Array.AsReadOnly(diagnostics.ToArray());
    }

    private static IReadOnlyList<ThemeAlgorithm> CopyAlgorithms(IReadOnlyList<ThemeAlgorithm> algorithms)
    {
        return Array.AsReadOnly(algorithms.ToArray());
    }

    private static IReadOnlyDictionary<TKey, TValue> ReadOnly<TKey, TValue>(Dictionary<TKey, TValue> source)
        where TKey : notnull
    {
        return new ReadOnlyDictionary<TKey, TValue>(source);
    }

    private static void MergeInto(
        IDictionary<string, string> destination,
        IEnumerable<KeyValuePair<string, string>> source)
    {
        foreach (var entry in source)
        {
            destination[entry.Key] = entry.Value;
        }
    }
}
