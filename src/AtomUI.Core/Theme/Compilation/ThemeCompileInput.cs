using AtomUI.Theme.Configuration;
using AtomUI.Theme.Definitions;
using AtomUI.Theme.Schema;

namespace AtomUI.Theme.Compilation;

internal sealed record ThemeCompileInput
{
    internal ThemeCompileInput(
        BoundThemeDefinition definition,
        ThemeDefinitionRevision definitionRevision,
        NormalizedThemeConfig effectiveConfig,
        ThemeSchemaRegistry registry,
        ThemeSnapshot? reusableParent = null)
    {
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(effectiveConfig);
        ArgumentNullException.ThrowIfNull(registry);

        Definition         = definition;
        DefinitionRevision = definitionRevision;
        EffectiveConfig    = effectiveConfig;
        Registry           = registry;
        ReusableParent     = reusableParent;
    }

    internal BoundThemeDefinition Definition { get; }
    internal ThemeDefinitionRevision DefinitionRevision { get; }
    internal NormalizedThemeConfig EffectiveConfig { get; }
    internal ThemeSchemaRegistry Registry { get; }
    internal ThemeSnapshot? ReusableParent { get; }
}
