using System.Collections.Frozen;
using AtomUI.Theme.Algorithms;
using AtomUI.Theme.Configuration;
using AtomUI.Theme.Definitions;
using AtomUI.Theme.Schema;
using AtomUI.Theme.Tokens;

namespace AtomUI.Theme.Compilation;

internal sealed class ThemeCompiler
{
    private const string CompilerPath = "ThemeCompiler";
internal static NormalizedThemeConfig CreateDefinitionDefaults(
        BoundThemeDefinition definition,
        ThemeSchemaRegistry registry)
    {
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(registry);

        IReadOnlyList<ThemeAlgorithmDescriptor> algorithms = definition.Algorithms;
        if (algorithms.Count == 0)
        {
            registry.TryGetAlgorithm(nameof(ThemeAlgorithm.Default), out var defaultAlgorithm);
            algorithms =
            [
                defaultAlgorithm ??
                throw new InvalidOperationException("Default algorithm is not registered.")
            ];
        }

        var globalTokens = new NormalizedTokenValue[definition.Tokens.Count];
        for (var index = 0; index < globalTokens.Length; index++)
        {
            var token = definition.Tokens[index];
            globalTokens[index] = new NormalizedTokenValue(
                token.Descriptor,
                token.Value,
                token.Descriptor.Format(token.Value));
        }
        Array.Sort(
            globalTokens,
            static (left, right) => left.Descriptor.Slot.CompareTo(right.Descriptor.Slot));

        var controls = new NormalizedControlThemeConfig[definition.Controls.Count];
        for (var index = 0; index < controls.Length; index++)
        {
            var control = definition.Controls[index];
            controls[index] = new NormalizedControlThemeConfig(
                control.Descriptor.Identity,
                control.AlgorithmMode,
                control.Algorithms,
                NormalizeBoundTokens(control.GlobalTokens),
                NormalizeBoundTokens(control.OwnTokens));
        }
        return new NormalizedThemeConfig(
            false,
            true,
            algorithms,
            globalTokens,
            controls);
    }

    private static IReadOnlyList<NormalizedTokenValue> NormalizeBoundTokens(
        IReadOnlyList<BoundTokenValue> tokens)
    {
        var result = new NormalizedTokenValue[tokens.Count];
        for (var index = 0; index < result.Length; index++)
        {
            var token = tokens[index];
            result[index] = new NormalizedTokenValue(
                token.Descriptor,
                token.Value,
                token.Descriptor.Format(token.Value));
        }
        Array.Sort(
            result,
            static (left, right) => left.Descriptor.Slot.CompareTo(right.Descriptor.Slot));
        return result;
    }


    internal ThemeCompileResult Compile(ThemeCompileInput input)
    {
        return Compile(input, null);
    }

    internal ThemeCompileResult Compile(
        ThemeCompileInput input,
        ControlCompilationCache? controlCompilationCache)
    {
        ArgumentNullException.ThrowIfNull(input);

        var diagnostics = new List<ThemeDefinitionDiagnostic>();
        try
        {
            if (input.EffectiveConfig.Algorithms.Count == 0)
            {
                AddError(diagnostics, "ATMTHM5001", "A theme compilation requires at least one algorithm.");
                return Failed(diagnostics);
            }

            var appearance = ResolveAppearance(input.EffectiveConfig.Algorithms);
            var reuseGlobal = CanReuseGlobal(input, appearance);
            DesignToken globalBuilder;
            TokenValueTable globalValues;
            IReadOnlyDictionary<object, object?> globalResources;
            IReadOnlyDictionary<PresetPrimaryColor, PaletteInfo> palettes;
            if (reuseGlobal)
            {
                var parent = input.ReusableParent!;
                globalValues    = parent.GlobalTokenValues;
                globalResources = parent.GlobalResources;
                palettes        = parent.PresetColorPalettes;
                globalBuilder   = CreateBuilder(globalValues, palettes, input.Registry.GlobalTokens);
            }
            else
            {
                globalBuilder   = CompileGlobalBuilder(input);
                globalValues    = TokenValueTable.Freeze(globalBuilder, input.Registry.GlobalTokens);
                globalResources = BuildResourceMap(globalBuilder, input.Registry.GlobalTokens);
                palettes        = FreezePresetPalettes(globalBuilder);
            }

            var controls = CompileControls(
                input,
                appearance,
                globalBuilder,
                globalValues,
                globalResources,
                reuseGlobal,
                controlCompilationCache);
            var fingerprint = ThemeContentFingerprint.Compute(
                input.DefinitionRevision,
                input.EffectiveConfig,
                input.Registry.Revision,
                appearance);

            return new ThemeCompileResult(
                new ThemeSnapshot(
                    input.Definition.Id,
                    input.Definition,
                    input.DefinitionRevision,
                    fingerprint,
                    appearance,
                    input.Registry.Revision,
                    input.Registry,
                    input.EffectiveConfig,
                    globalValues,
                    globalResources,
                    palettes,
                    controls),
                CopyDiagnostics(diagnostics),
                null);
        }
        catch (Exception exception)
        {
            return new ThemeCompileResult(null, CopyDiagnostics(diagnostics), exception);
        }
    }

    private static bool CanReuseGlobal(ThemeCompileInput input, ThemeAppearance appearance)
    {
        var parent = input.ReusableParent;
        return parent is not null &&
               parent.DefinitionRevision == input.DefinitionRevision &&
               parent.RegistryRevision == input.Registry.Revision &&
               parent.Appearance == appearance &&
               AlgorithmSequenceEqual(
                   parent.EffectiveConfig.Algorithms,
                   input.EffectiveConfig.Algorithms) &&
               TokenSequenceEqual(
                   parent.EffectiveConfig.GlobalTokens,
                   input.EffectiveConfig.GlobalTokens);
    }

    private static IReadOnlyList<ControlThemeSnapshot> CompileControls(
        ThemeCompileInput input,
        ThemeAppearance globalAppearance,
        DesignToken globalBuilder,
        TokenValueTable globalValues,
        IReadOnlyDictionary<object, object?> globalResources,
        bool reuseGlobal,
        ControlCompilationCache? controlCompilationCache)
    {
        var controls = new ControlThemeSnapshot[input.Registry.Controls.Count];
        for (var slot = 0; slot < controls.Length; slot++)
        {
            var descriptor = input.Registry.Controls[slot];
            var config = FindControlConfig(input.EffectiveConfig.Controls, descriptor.Identity);
            if (reuseGlobal && CanReuseControl(input.ReusableParent!, descriptor, config))
            {
                controls[slot] = input.ReusableParent!.Controls[slot];
                continue;
            }

            ControlThemeSnapshot CompileCurrentControl()
            {
                return CompileControl(
                    descriptor,
                    config,
                    input.EffectiveConfig.Algorithms,
                    globalAppearance,
                    globalBuilder,
                    globalValues,
                    globalResources,
                    input.Registry.GlobalTokens);
            }

            controls[slot] = controlCompilationCache is null
                ? CompileCurrentControl()
                : controlCompilationCache.GetOrCompile(
                    input,
                    descriptor,
                    config,
                    globalAppearance,
                    CompileCurrentControl);
        }

        return Array.AsReadOnly(controls);
    }

    private static bool CanReuseControl(
        ThemeSnapshot parent,
        ControlTokenDescriptor descriptor,
        NormalizedControlThemeConfig? config)
    {
        if ((uint)descriptor.Slot >= (uint)parent.Controls.Count)
        {
            return false;
        }

        var parentConfig = FindControlConfig(parent.EffectiveConfig.Controls, descriptor.Identity);
        return config is null ? parentConfig is null : config.Equals(parentConfig);
    }

    private static ControlThemeSnapshot CompileControl(
        ControlTokenDescriptor descriptor,
        NormalizedControlThemeConfig? config,
        IReadOnlyList<ThemeAlgorithmDescriptor> globalAlgorithms,
        ThemeAppearance globalAppearance,
        DesignToken globalBuilder,
        TokenValueTable globalValues,
        IReadOnlyDictionary<object, object?> globalResources,
        IReadOnlyList<TokenDescriptor> globalDescriptors)
    {
        var effectiveGlobalBuilder = CopyBuilder(globalBuilder, globalDescriptors);
        var appearance = globalAppearance;
        if (config is not null)
        {
            switch (config.AlgorithmMode)
            {
                case ControlAlgorithmMode.Global:
                    effectiveGlobalBuilder = ReevaluateControlGlobal(
                        effectiveGlobalBuilder,
                        config.GlobalTokens,
                        globalAlgorithms,
                        globalDescriptors);
                    appearance = ResolveAppearance(globalAppearance, globalAlgorithms);
                    break;

                case ControlAlgorithmMode.Custom:
                    effectiveGlobalBuilder = ReevaluateControlGlobal(
                        effectiveGlobalBuilder,
                        config.GlobalTokens,
                        config.Algorithms,
                        globalDescriptors);
                    appearance = ResolveAppearance(globalAppearance, config.Algorithms);
                    break;

                case ControlAlgorithmMode.Unspecified:
                case ControlAlgorithmMode.Disabled:
                    ApplyAllValues(effectiveGlobalBuilder, config.GlobalTokens);
                    NormalizeFinalBehavior(effectiveGlobalBuilder);
                    break;

                default:
                    throw new InvalidOperationException(
                        $"Control '{descriptor.Identity}' has unsupported algorithm mode '{config.AlgorithmMode}'.");
            }
        }

        var effectiveGlobalValues = TokenValueTable.Freeze(effectiveGlobalBuilder, globalDescriptors);
        var effectiveGlobalResources = BuildResourceMap(effectiveGlobalBuilder, globalDescriptors);
        var globalDelta = BuildTokenDelta(globalValues, effectiveGlobalValues);
        var globalResourceDelta = BuildDenseResourceDelta(globalResources, effectiveGlobalResources);

        var controlBuilder = descriptor.CreateBuilder();
        controlBuilder.AssignSharedToken(effectiveGlobalBuilder);
        descriptor.Evaluate(controlBuilder, appearance);
        if (config is not null)
        {
            ApplyAllValues(controlBuilder, config.OwnTokens);
        }

        return new ControlThemeSnapshot(
            descriptor.Slot,
            appearance,
            globalDelta,
            globalResourceDelta,
            TokenValueTable.Freeze(controlBuilder, descriptor.OwnTokens),
            BuildResourceMap(controlBuilder, descriptor.OwnTokens));
    }

    private static DesignToken ReevaluateControlGlobal(
        DesignToken baseline,
        IReadOnlyList<NormalizedTokenValue> overrides,
        IReadOnlyList<ThemeAlgorithmDescriptor> algorithms,
        IReadOnlyList<TokenDescriptor> descriptors)
    {
        ApplyValues(baseline, overrides, TokenStage.Seed);
        var result = EvaluateAlgorithms(baseline, algorithms, descriptors);
        ApplyValues(result, overrides, TokenStage.Map);
        result.CalculateAliasTokenValues();
        ApplyValues(result, overrides, TokenStage.Alias);
        NormalizeFinalBehavior(result);
        return result;
    }

    private static ThemeAppearance ResolveAppearance(
        ThemeAppearance baseline,
        IReadOnlyList<ThemeAlgorithmDescriptor> algorithms)
    {
        var appearance = baseline;
        foreach (var algorithm in algorithms)
        {
            appearance = algorithm.AppearanceEffect switch
            {
                ThemeAppearanceEffect.Light => ThemeAppearance.Light,
                ThemeAppearanceEffect.Dark  => ThemeAppearance.Dark,
                _                           => appearance
            };
        }

        return appearance;
    }

    private static NormalizedControlThemeConfig? FindControlConfig(
        IReadOnlyList<NormalizedControlThemeConfig> controls,
        Schema.ControlTokenIdentity identity)
    {
        foreach (var control in controls)
        {
            if (control.Identity == identity)
            {
                return control;
            }
        }

        return null;
    }

    private static IReadOnlyDictionary<int, object?> BuildTokenDelta(
        TokenValueTable globalValues,
        TokenValueTable effectiveValues)
    {
        var delta = new Dictionary<int, object?>();
        for (var slot = 0; slot < effectiveValues.Count; slot++)
        {
            var value = effectiveValues.GetValue(slot);
            if (!globalValues.ValueEquals(slot, value))
            {
                delta.Add(slot, value);
            }
        }

        return delta.ToFrozenDictionary();
    }

    private static IReadOnlyDictionary<object, object?> BuildDenseResourceDelta(
        IReadOnlyDictionary<object, object?> globalResources,
        IReadOnlyDictionary<object, object?> effectiveResources)
    {
        var delta = new Dictionary<object, object?>();
        foreach (var resource in effectiveResources)
        {
            if (!globalResources.TryGetValue(resource.Key, out var globalValue) ||
                !Equals(globalValue, resource.Value))
            {
                delta.Add(resource.Key, resource.Value);
            }
        }

        return delta.ToFrozenDictionary();
    }

    private static bool AlgorithmSequenceEqual(
        IReadOnlyList<ThemeAlgorithmDescriptor> left,
        IReadOnlyList<ThemeAlgorithmDescriptor> right)
    {
        if (left.Count != right.Count)
        {
            return false;
        }

        for (var index = 0; index < left.Count; index++)
        {
            if (!string.Equals(left[index].Id, right[index].Id, StringComparison.Ordinal) ||
                left[index].Revision != right[index].Revision)
            {
                return false;
            }
        }

        return true;
    }

    private static bool TokenSequenceEqual(
        IReadOnlyList<NormalizedTokenValue> left,
        IReadOnlyList<NormalizedTokenValue> right)
    {
        if (left.Count != right.Count)
        {
            return false;
        }

        for (var index = 0; index < left.Count; index++)
        {
            if (!left[index].Equals(right[index]))
            {
                return false;
            }
        }

        return true;
    }

    private static ThemeAppearance ResolveAppearance(
        IReadOnlyList<ThemeAlgorithmDescriptor> algorithms)
    {
        var appearance = ThemeAppearance.Light;
        foreach (var algorithm in algorithms)
        {
            appearance = algorithm.AppearanceEffect switch
            {
                ThemeAppearanceEffect.Light => ThemeAppearance.Light,
                ThemeAppearanceEffect.Dark  => ThemeAppearance.Dark,
                _                           => appearance
            };
        }

        return appearance;
    }

    private static DesignToken CompileGlobalBuilder(ThemeCompileInput input)
    {
        var seed = new DesignToken();
        ApplyValues(seed, input.EffectiveConfig.GlobalTokens, TokenStage.Seed);
        var map = EvaluateAlgorithms(seed, input.EffectiveConfig.Algorithms, input.Registry.GlobalTokens);
        ApplyValues(map, input.EffectiveConfig.GlobalTokens, TokenStage.Map);
        map.CalculateAliasTokenValues();
        ApplyValues(map, input.EffectiveConfig.GlobalTokens, TokenStage.Alias);
        NormalizeFinalBehavior(map);
        return map;
    }

    private static DesignToken EvaluateAlgorithms(
        DesignToken effectiveSeed,
        IReadOnlyList<ThemeAlgorithmDescriptor> algorithms,
        IReadOnlyList<TokenDescriptor> descriptors)
    {
        DesignToken? previousMap = null;
        foreach (var algorithm in algorithms)
        {
            var nextMap = CopyBuilder(previousMap ?? effectiveSeed, descriptors);
            algorithm.Create().Evaluate(effectiveSeed, previousMap, nextMap);
            previousMap = nextMap;
        }

        return previousMap ?? throw new InvalidOperationException(
            "A theme compilation requires at least one algorithm.");
    }

    private static DesignToken CopyBuilder(
        DesignToken source,
        IReadOnlyList<TokenDescriptor> descriptors)
    {
        var copy = new DesignToken();
        foreach (var descriptor in descriptors)
        {
            var value = descriptor.GetValue(source);
            if (value is not null &&
                !value.GetType().IsValueType &&
                value is not string &&
                value is not Avalonia.Media.FontFamily)
            {
                value = descriptor.Parse(descriptor.Format(value));
            }

            descriptor.SetValue(copy, value);
        }

        foreach (var palette in source.ColorPalettes)
        {
            copy.ColorPalettes[palette.Key] = CopyColorMap(palette.Value);
        }

        return copy;
    }

    private static void ApplyValues(
        DesignToken builder,
        IReadOnlyList<NormalizedTokenValue> values,
        TokenStage stage)
    {
        foreach (var value in values)
        {
            if (value.Descriptor.Stage == stage)
            {
                value.Descriptor.SetValue(builder, value.Value);
            }
        }
    }

    private static void ApplyAllValues(
        AbstractDesignToken builder,
        IReadOnlyList<NormalizedTokenValue> values)
    {
        foreach (var value in values)
        {
            value.Descriptor.SetValue(builder, value.Value);
        }
    }

    private static DesignToken CreateBuilder(
        TokenValueTable values,
        IReadOnlyDictionary<PresetPrimaryColor, PaletteInfo> palettes,
        IReadOnlyList<TokenDescriptor> descriptors)
    {
        var builder = new DesignToken();
        foreach (var descriptor in descriptors)
        {
            descriptor.SetValue(builder, ThawValue(descriptor, values.GetValue(descriptor.Slot)));
        }

        foreach (var palette in palettes)
        {
            builder.ColorPalettes[palette.Key] = ColorMap.FromColors(palette.Value.ColorSequence);
        }

        return builder;
    }

    private static object? ThawValue(TokenDescriptor descriptor, object? value)
    {
        if (value is null || value.GetType().IsValueType || value is string)
        {
            return value;
        }

        return descriptor.Parse(descriptor.Format(value));
    }

    private static void NormalizeFinalBehavior(DesignToken builder)
    {
        if (!builder.EnableMotion)
        {
            builder.MotionDurationFast     = TimeSpan.Zero;
            builder.MotionDurationMid      = TimeSpan.Zero;
            builder.MotionDurationSlow     = TimeSpan.Zero;
            builder.MotionDurationVerySlow = TimeSpan.Zero;
        }

        if (!builder.EnableWaveSpirit)
        {
            builder.WaveAnimationRange = 0;
            builder.WaveStartOpacity   = 0;
        }
    }

    private static IReadOnlyDictionary<object, object?> BuildResourceMap(
        AbstractDesignToken builder,
        IReadOnlyList<TokenDescriptor> descriptors)
    {
        var resources = new Dictionary<object, object?>(descriptors.Count);
        foreach (var descriptor in descriptors)
        {
            resources.Add(descriptor.ResourceKey, descriptor.ProjectResourceValue(builder));
        }

        return resources.ToFrozenDictionary();
    }

    private static IReadOnlyDictionary<PresetPrimaryColor, PaletteInfo> FreezePresetPalettes(
        DesignToken builder)
    {
        var palettes = new Dictionary<PresetPrimaryColor, PaletteInfo>(
            builder.ColorPalettes.Count);
        foreach (var palette in builder.ColorPalettes)
        {
            var colors = GetColors(palette.Value);
            palettes.Add(
                palette.Key,
                new PaletteInfo(palette.Value.Color6, colors));
        }

        return palettes.ToFrozenDictionary();
    }

    private static ColorMap CopyColorMap(ColorMap source)
    {
        return ColorMap.FromColors(GetColors(source));
    }

    private static Avalonia.Media.Color[] GetColors(ColorMap source)
    {
        return
        [
            source.Color1,
            source.Color2,
            source.Color3,
            source.Color4,
            source.Color5,
            source.Color6,
            source.Color7,
            source.Color8,
            source.Color9,
            source.Color10
        ];
    }

    private static void AddError(List<ThemeDefinitionDiagnostic> diagnostics, string code, string message)
    {
        diagnostics.Add(new ThemeDefinitionDiagnostic(
            code,
            ThemeDefinitionDiagnosticSeverity.Error,
            string.Empty,
            0,
            0,
            CompilerPath,
            message));
    }

    private static ThemeCompileResult Failed(List<ThemeDefinitionDiagnostic> diagnostics)
    {
        return new ThemeCompileResult(null, CopyDiagnostics(diagnostics), null);
    }

    private static IReadOnlyList<ThemeDefinitionDiagnostic> CopyDiagnostics(
        List<ThemeDefinitionDiagnostic> diagnostics)
    {
        return Array.AsReadOnly(diagnostics.ToArray());
    }
}
