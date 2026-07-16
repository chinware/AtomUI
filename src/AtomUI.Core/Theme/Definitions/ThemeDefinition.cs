using AtomUI.Theme.Configuration;
using AtomUI.Theme.Schema;

namespace AtomUI.Theme.Definitions;

internal sealed record BoundTokenValue(
    TokenDescriptor Descriptor,
    object? Value,
    ThemeSourceLocation Location);

internal sealed class ControlThemeDefinition
{
    internal ControlThemeDefinition(
        ControlTokenDescriptor descriptor,
        ControlAlgorithmMode algorithmMode,
        IEnumerable<ThemeAlgorithmDescriptor> algorithms,
        IEnumerable<BoundTokenValue> globalTokens,
        IEnumerable<BoundTokenValue> ownTokens,
        ThemeSourceLocation location)
    {
        Descriptor    = descriptor;
        AlgorithmMode = algorithmMode;
        Algorithms    = Array.AsReadOnly(algorithms.ToArray());
        GlobalTokens  = Array.AsReadOnly(globalTokens.ToArray());
        OwnTokens     = Array.AsReadOnly(ownTokens.ToArray());
        Location      = location;
    }

    public ControlTokenDescriptor Descriptor { get; }
    public ControlAlgorithmMode AlgorithmMode { get; }
    public IReadOnlyList<ThemeAlgorithmDescriptor> Algorithms { get; }
    public IReadOnlyList<BoundTokenValue> GlobalTokens { get; }
    public IReadOnlyList<BoundTokenValue> OwnTokens { get; }
    public ThemeSourceLocation Location { get; }
}

internal sealed class BoundThemeDefinition
{
    internal BoundThemeDefinition(
        string id,
        string name,
        ThemeAppearance declaredAppearance,
        ThemeAppearance effectiveAppearance,
        bool isDefault,
        IEnumerable<ThemeAlgorithmDescriptor> algorithms,
        IEnumerable<BoundTokenValue> tokens,
        IEnumerable<ControlThemeDefinition> controls,
        ThemeSourceLocation location)
    {
        Id                  = id;
        Name                = name;
        DeclaredAppearance  = declaredAppearance;
        EffectiveAppearance = effectiveAppearance;
        IsDefault           = isDefault;
        Algorithms          = Array.AsReadOnly(algorithms.ToArray());
        Tokens              = Array.AsReadOnly(tokens.ToArray());
        Controls            = Array.AsReadOnly(controls.ToArray());
        Location            = location;
    }

    public string Id { get; }
    public string Name { get; }
    public ThemeAppearance DeclaredAppearance { get; }
    public ThemeAppearance EffectiveAppearance { get; }
    public bool IsDefault { get; }
    public IReadOnlyList<ThemeAlgorithmDescriptor> Algorithms { get; }
    public IReadOnlyList<BoundTokenValue> Tokens { get; }
    public IReadOnlyList<ControlThemeDefinition> Controls { get; }
    public ThemeSourceLocation Location { get; }
}

internal sealed class ThemeDefinitionBindResult
{
    internal ThemeDefinitionBindResult(
        BoundThemeDefinition? definition,
        IEnumerable<ThemeDefinitionDiagnostic> diagnostics)
    {
        Definition  = definition;
        Diagnostics = Array.AsReadOnly(diagnostics.ToArray());
    }

    public BoundThemeDefinition? Definition { get; }
    public IReadOnlyList<ThemeDefinitionDiagnostic> Diagnostics { get; }

    public bool Success =>
        Definition is not null &&
        Diagnostics.All(static diagnostic => diagnostic.Severity != ThemeDiagnosticSeverity.Error);
}
