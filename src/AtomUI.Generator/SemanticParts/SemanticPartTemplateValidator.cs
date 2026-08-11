using AtomUI.Generator.Diagnostics;
using Microsoft.CodeAnalysis;

namespace AtomUI.Generator;

internal sealed class SemanticPartTemplateValidator
{
    private readonly IReadOnlyList<ThemeAssetInfo> _assets;
    private readonly SemanticPartTypeResolver _typeResolver;
    private readonly Action<Diagnostic> _reportDiagnostic;

    internal SemanticPartTemplateValidator(
        IReadOnlyList<ThemeAssetInfo> assets,
        SemanticPartTypeResolver typeResolver,
        Action<Diagnostic> reportDiagnostic)
    {
        _assets = assets;
        _typeResolver = typeResolver;
        _reportDiagnostic = reportDiagnostic;
    }

    internal bool Validate(
        SemanticControlDeclaration control,
        IReadOnlyList<SemanticPartDeclaration> parts)
    {
        var staticParts = parts.Where(static part => !part.RuntimeCreated).ToArray();
        if (staticParts.Length == 0)
        {
            return true;
        }

        var templates = ResolveTemplates(control.ControlType);
        if (templates.Count == 0)
        {
            _reportDiagnostic(Diagnostic.Create(
                AtomUIDiagnosticDescriptors.SemanticPartMissingControlTemplate,
                control.Location,
                control.ControlType.ToDisplayString()));
            return false;
        }

        var declaredClasses = new HashSet<string>(
            staticParts.Select(static part => part.SelectorClass!),
            StringComparer.Ordinal);
        var valid = true;
        foreach (var entry in templates)
        {
            var templateName = $"{entry.Asset.AssetPath}#{entry.Template.Index}";
            var relevantMarkers = entry.Template.Markers
                                       .Where(marker => declaredClasses.Contains(marker.SelectorClass))
                                       .ToArray();
            foreach (var marker in relevantMarkers.Where(static marker => !marker.IsStaticallyEnabled))
            {
                _reportDiagnostic(Diagnostic.Create(
                    AtomUIDiagnosticDescriptors.SemanticPartTemplateMarkerNotStatic,
                    entry.Asset.CreateLocation(),
                    control.ControlType.ToDisplayString(),
                    templateName,
                    marker.SelectorClass,
                    marker.TypeName));
                valid = false;
            }

            var validMarkers = relevantMarkers.Where(static marker => marker.IsStaticallyEnabled).ToArray();
            foreach (var nodeMarkers in validMarkers.GroupBy(static marker => marker.NodeId))
            {
                var selectorClasses = nodeMarkers.Select(static marker => marker.SelectorClass)
                                                 .Distinct(StringComparer.Ordinal)
                                                 .OrderBy(static selectorClass => selectorClass, StringComparer.Ordinal)
                                                 .ToArray();
                if (selectorClasses.Length <= 1)
                {
                    continue;
                }

                _reportDiagnostic(Diagnostic.Create(
                    AtomUIDiagnosticDescriptors.SemanticPartTemplateNodeConflict,
                    entry.Asset.CreateLocation(),
                    control.ControlType.ToDisplayString(),
                    templateName,
                    nodeMarkers.First().TypeName,
                    string.Join(", ", selectorClasses)));
                valid = false;
            }

            foreach (var part in staticParts)
            {
                var markers = validMarkers
                              .Where(marker => string.Equals(
                                  marker.SelectorClass,
                                  part.SelectorClass,
                                  StringComparison.Ordinal))
                              .ToArray();
                if (!MatchesCardinality(part.Cardinality, markers.Length))
                {
                    _reportDiagnostic(Diagnostic.Create(
                        AtomUIDiagnosticDescriptors.SemanticPartTemplateCardinalityMismatch,
                        entry.Asset.CreateLocation(),
                        control.ControlType.ToDisplayString(),
                        templateName,
                        markers.Length,
                        part.Name,
                        DescribeCardinality(part.Cardinality)));
                    valid = false;
                }

                if (part.ContractType is not INamedTypeSymbol contractType)
                {
                    continue;
                }

                foreach (var marker in markers)
                {
                    var markerType = _typeResolver.ResolveMarkerType(marker);
                    if (markerType is null ||
                        !SemanticPartTypeResolver.IsAssignableTo(markerType, contractType))
                    {
                        _reportDiagnostic(Diagnostic.Create(
                            AtomUIDiagnosticDescriptors.SemanticPartTemplateContractTypeMismatch,
                            entry.Asset.CreateLocation(),
                            control.ControlType.ToDisplayString(),
                            templateName,
                            part.Name,
                            markerType?.ToDisplayString() ?? marker.TypeName,
                            contractType.ToDisplayString()));
                        valid = false;
                    }
                }
            }
        }

        return valid;
    }

    private IReadOnlyList<(
        ThemeAssetInfo Asset,
        ThemeAssetSemanticTemplateInfo Template)> ResolveTemplates(INamedTypeSymbol controlType)
    {
        var themes = _assets.OrderBy(static asset => asset.AssetPath, StringComparer.Ordinal)
                            .SelectMany(static asset => asset.SemanticThemes.Select(theme =>
                                (Asset: asset, Theme: theme)))
                            .ToArray();
        var resolvedTargets = themes.ToDictionary(
            static entry => entry.Theme,
            entry => _typeResolver.ResolveTargetType(entry.Theme.TargetType));
        var templates = new List<(
            ThemeAssetInfo Asset,
            ThemeAssetSemanticTemplateInfo Template)>();
        var visited = new HashSet<ThemeAssetSemanticThemeInfo>();

        foreach (var theme in themes.Where(entry =>
                     SymbolEqualityComparer.Default.Equals(resolvedTargets[entry.Theme], controlType)))
        {
            CollectTemplates(theme);
        }
        return templates;

        void CollectTemplates((ThemeAssetInfo Asset, ThemeAssetSemanticThemeInfo Theme) entry)
        {
            if (!visited.Add(entry.Theme))
            {
                return;
            }

            foreach (var template in entry.Theme.Templates)
            {
                templates.Add((entry.Asset, template));
            }

            if (entry.Theme.OverridesBaseTemplate ||
                entry.Theme.BasedOn is null ||
                _typeResolver.ResolveTargetType(entry.Theme.BasedOn) is not { } baseType)
            {
                return;
            }

            foreach (var baseTheme in themes.Where(candidate =>
                         SymbolEqualityComparer.Default.Equals(resolvedTargets[candidate.Theme], baseType)))
            {
                CollectTemplates(baseTheme);
            }
        }
    }

    private static bool MatchesCardinality(int cardinality, int count)
    {
        return cardinality switch
        {
            1 => count <= 1,
            2 => count >= 1,
            _ => count == 1
        };
    }

    private static string DescribeCardinality(int cardinality)
    {
        return cardinality switch
        {
            1 => "zero or one",
            2 => "one or more",
            _ => "exactly one"
        };
    }
}
