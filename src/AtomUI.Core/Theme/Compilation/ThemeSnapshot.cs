using AtomUI.Theme.Configuration;
using AtomUI.Theme.Definitions;
using AtomUI.Theme.Algorithms;
using AtomUI.Theme.Schema;

namespace AtomUI.Theme.Compilation;

internal sealed class ThemeSnapshot
{
    internal ThemeSnapshot(
        string themeId,
        BoundThemeDefinition definition,
        ThemeDefinitionRevision definitionRevision,
        ThemeContentFingerprint contentFingerprint,
        ThemeAppearance appearance,
        ThemeSchemaRevision registryRevision,
        ThemeSchemaRegistry registry,
        NormalizedThemeConfig effectiveConfig,
        TokenValueTable globalTokenValues,
        IReadOnlyDictionary<object, object?> globalResources,
        IReadOnlyDictionary<PresetPrimaryColor, PaletteInfo> presetColorPalettes,
        IReadOnlyList<ControlThemeSnapshot> controlSlots)
    {
        ThemeId = themeId;
        Definition = definition;
        DefinitionRevision = definitionRevision;
        ContentFingerprint = contentFingerprint;
        Appearance = appearance;
        RegistryRevision = registryRevision;
        Registry = registry;
        EffectiveConfig = effectiveConfig;
        GlobalTokenValues = globalTokenValues;
        GlobalResources = globalResources;
        PresetColorPalettes = presetColorPalettes;
        Controls = controlSlots;
        EstimatedRetainedBytes = ThemeRetainedBytesEstimator.EstimateSnapshot(this);
    }

    internal string ThemeId { get; }
    internal BoundThemeDefinition Definition { get; }
    internal ThemeDefinitionRevision DefinitionRevision { get; }
    internal ThemeContentFingerprint ContentFingerprint { get; }
    internal ThemeAppearance Appearance { get; }
    internal ThemeSchemaRevision RegistryRevision { get; }
    internal ThemeSchemaRegistry Registry { get; }
    internal NormalizedThemeConfig EffectiveConfig { get; }
    internal TokenValueTable GlobalTokenValues { get; }
    internal IReadOnlyDictionary<object, object?> GlobalResources { get; }
    internal IReadOnlyDictionary<PresetPrimaryColor, PaletteInfo> PresetColorPalettes { get; }
    internal IReadOnlyList<ControlThemeSnapshot> Controls { get; }
    internal long EstimatedRetainedBytes { get; }
}
