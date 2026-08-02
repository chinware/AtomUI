using AtomUI.Generator.Diagnostics;
using Microsoft.CodeAnalysis;

namespace AtomUI.Generator;

[Generator]
public sealed class ThemeAssetManifestGenerator : IIncrementalGenerator
{
    private const string ControlThemeType = "global::Avalonia.Styling.ControlTheme";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var assets = context.AdditionalTextsProvider
                            .Where(static text => ThemeAssetInfo.IsThemeAssetPath(text.Path))
                            .Combine(context.AnalyzerConfigOptionsProvider)
                            .Select(static (input, token) =>
                            {
                                input.Right.GlobalOptions.TryGetValue(
                                    "build_property.AtomUIThemeAssetProjectDirectory",
                                    out var projectDirectory);
                                input.Right.GetOptions(input.Left).TryGetValue(
                                    "build_metadata.AdditionalFiles.Link",
                                    out var link);
                                return ThemeAssetInfo.Create(
                                    input.Left,
                                    projectDirectory,
                                    link,
                                    token);
                            })
                            .Collect();
        var input = assets.Combine(context.CompilationProvider)
                          .Combine(context.AnalyzerConfigOptionsProvider);

        context.RegisterSourceOutput(input, static (productionContext, value) =>
        {
            var compilation = value.Left.Right;
            var assemblyName = compilation.AssemblyName ?? "AtomUI";
            var controlCatalog = ThemeGeneratorOptions.GetControlCatalog(value.Right);
            var globalTokenNames = compilation.GetTypeByMetadataName("AtomUI.Theme.Resources.SharedTokenKind")?
                                         .GetMembers()
                                         .OfType<IFieldSymbol>()
                                         .Where(static field => field.HasConstantValue && field.Name != "value__")
                                         .OrderBy(static field => Convert.ToInt64(
                                             field.ConstantValue,
                                             System.Globalization.CultureInfo.InvariantCulture))
                                         .ThenBy(static field => field.Name, StringComparer.Ordinal)
                                         .Select(static field => field.Name)
                                         .ToArray() ?? Array.Empty<string>();
            var sourceControls = ControlThemeModelBuilder
                                 .GetPublicControls(compilation.Assembly.GlobalNamespace)
                                 .ToArray();
            var resolved = new List<ResolvedThemeAssetInfo>();
            var auxiliary = new List<ThemeAssetInfo>();
            var uniqueAssets = new List<ThemeAssetInfo>();

            foreach (var group in value.Left.Left.GroupBy(static item => item.AssetPath, StringComparer.Ordinal))
            {
                if (group.Count() != 1)
                {
                    var asset = group.First();
                    productionContext.ReportDiagnostic(Diagnostic.Create(
                        AtomUIDiagnosticDescriptors.ThemeAssetDuplicateUri,
                        asset.CreateLocation(),
                        asset.AssetPath));
                    continue;
                }
                uniqueAssets.Add(group.Single());
            }

            foreach (var asset in uniqueAssets.OrderBy(static item => item.AssetPath, StringComparer.Ordinal))
            {
                var resolution = Resolve(
                    compilation,
                    sourceControls,
                    asset,
                    controlCatalog,
                    productionContext.ReportDiagnostic,
                    out var resolutionFailed);
                if (resolution is not null)
                {
                    resolved.Add(resolution);
                }
                else if (!resolutionFailed)
                {
                    auxiliary.Add(asset);
                }
            }

            new ThemeAssetManifestWriter(
                productionContext,
                assemblyName,
                resolved,
                auxiliary,
                globalTokenNames).Write();
        });
    }

    private static ResolvedThemeAssetInfo? Resolve(
        Compilation compilation,
        IReadOnlyList<INamedTypeSymbol> sourceControls,
        ThemeAssetInfo asset,
        string controlCatalog,
        Action<Diagnostic> reportDiagnostic,
        out bool failed)
    {
        failed = false;
        var owner = ResolveOwner(
            compilation,
            sourceControls,
            asset,
            reportDiagnostic,
            out var semanticPropertyName,
            out var ownerResolutionFailed);
        if (ownerResolutionFailed || owner is null)
        {
            failed = ownerResolutionFailed;
            return null;
        }

        ThemeAssetSemanticPartInfo? semanticPart = null;
        INamedTypeSymbol? semanticTarget = null;
        if (semanticPropertyName is not null)
        {
            semanticTarget = ResolveSemanticTarget(compilation, sourceControls, asset);
            if (semanticTarget is null || !ControlThemeModelBuilder.IsPublicControl(semanticTarget))
            {
                reportDiagnostic(Diagnostic.Create(
                    AtomUIDiagnosticDescriptors.ThemeAssetSemanticPartTargetMismatch,
                    asset.CreateLocation(),
                    asset.AssetPath,
                    semanticTarget?.ToDisplayString() ?? asset.TargetTypes.FirstOrDefault()?.Value ?? "<missing>"));
                failed = true;
                return null;
            }

            semanticPart = new ThemeAssetSemanticPartInfo(
                semanticPropertyName,
                semanticTarget.ToDisplayString(GeneratorSymbolDisplay.FullyQualifiedType));
        }

        var referenced = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default)
        {
            owner
        };
        if (semanticTarget is not null)
        {
            referenced.Add(semanticTarget);
        }
        foreach (var family in asset.ControlTokenFamilies)
        {
            var matches = FindPublicControlsByName(compilation, sourceControls, family).ToArray();
            if (matches.Length == 1)
            {
                referenced.Add(matches[0]);
            }
        }

        return new ResolvedThemeAssetInfo(
            asset,
            new ThemeAssetControlIdentityInfo(controlCatalog, owner.Name),
            referenced.Select(control => CreateIdentity(
                                  compilation,
                                  control,
                                  owner,
                                  controlCatalog))
                      .OrderBy(static identity => identity.Catalog, StringComparer.Ordinal)
                      .ThenBy(static identity => identity.Id, StringComparer.Ordinal)
                      .ToArray(),
            semanticPart);
    }

    private static ThemeAssetControlIdentityInfo CreateIdentity(
        Compilation compilation,
        INamedTypeSymbol control,
        INamedTypeSymbol owner,
        string currentControlCatalog)
    {
        if (SymbolEqualityComparer.Default.Equals(control, owner))
        {
            return new ThemeAssetControlIdentityInfo(currentControlCatalog, control.Name);
        }

        var catalog = SymbolEqualityComparer.Default.Equals(
            control.ContainingAssembly,
            compilation.Assembly)
            ? currentControlCatalog
            : ThemeGeneratorOptions.GetReferencedControlCatalog(control.ContainingAssembly);
        return new ThemeAssetControlIdentityInfo(catalog, control.Name);
    }

    private static INamedTypeSymbol? ResolveOwner(
        Compilation compilation,
        IReadOnlyList<INamedTypeSymbol> sourceControls,
        ThemeAssetInfo asset,
        Action<Diagnostic> reportDiagnostic,
        out string? semanticPropertyName,
        out bool failed)
    {
        semanticPropertyName = null;
        failed = false;

        if (asset.ControlCandidate is not null)
        {
            var exact = FindPublicControlsByName(compilation, sourceControls, asset.ControlCandidate).ToArray();
            if (exact.Length > 1)
            {
                ReportAmbiguousOwner(asset, asset.ControlCandidate, reportDiagnostic);
                failed = true;
                return null;
            }
            if (exact.Length == 1)
            {
                return exact[0];
            }
        }

        var semanticOwners = sourceControls
            .Where(control => HasSemanticPartThemeProperty(control, asset.FileName))
            .ToArray();
        if (semanticOwners.Length > 1)
        {
            ReportAmbiguousOwner(asset, asset.FileName, reportDiagnostic);
            failed = true;
            return null;
        }
        if (semanticOwners.Length == 1)
        {
            semanticPropertyName = asset.FileName;
            return semanticOwners[0];
        }

        foreach (var candidate in asset.DirectoryCandidates)
        {
            var matches = FindPublicControlsByName(compilation, sourceControls, candidate).ToArray();
            if (matches.Length > 1)
            {
                ReportAmbiguousOwner(asset, candidate, reportDiagnostic);
                failed = true;
                return null;
            }
            if (matches.Length == 1)
            {
                return matches[0];
            }
        }

        var familyOwners = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
        foreach (var family in asset.ControlTokenFamilies)
        {
            foreach (var match in FindPublicControlsByName(compilation, sourceControls, family))
            {
                familyOwners.Add(match);
            }
        }
        if (familyOwners.Count == 1)
        {
            return familyOwners.Single();
        }

        return null;
    }

    private static INamedTypeSymbol? ResolveSemanticTarget(
        Compilation compilation,
        IReadOnlyList<INamedTypeSymbol> sourceControls,
        ThemeAssetInfo asset)
    {
        foreach (var target in asset.TargetTypes)
        {
            if (!TryParseTypeReference(target.Value, out var prefix, out var typeName))
            {
                continue;
            }

            if (target.Namespaces.TryGetValue(prefix, out var namespaceName) &&
                namespaceName.StartsWith("using:", StringComparison.Ordinal))
            {
                return compilation.GetTypeByMetadataName(
                    $"{namespaceName.Substring("using:".Length)}.{typeName}");
            }

            var matches = FindPublicControlsByName(compilation, sourceControls, typeName).ToArray();
            if (matches.Length == 1)
            {
                return matches[0];
            }
        }

        return null;
    }

    private static IEnumerable<INamedTypeSymbol> FindPublicControlsByName(
        Compilation compilation,
        IReadOnlyList<INamedTypeSymbol> sourceControls,
        string name)
    {
        return ControlThemeModelBuilder.FindPublicControlsByName(compilation, sourceControls, name);
    }

    private static bool TryParseTypeReference(
        string value,
        out string prefix,
        out string typeName)
    {
        var reference = value.Trim();
        if (reference.StartsWith("{x:Type", StringComparison.Ordinal) &&
            reference.EndsWith("}", StringComparison.Ordinal))
        {
            reference = reference.Substring("{x:Type".Length, reference.Length - "{x:Type".Length - 1).Trim();
        }

        var colon = reference.IndexOf(':');
        if (colon < 0)
        {
            prefix = string.Empty;
            typeName = reference;
            return typeName.Length != 0;
        }

        prefix = reference.Substring(0, colon);
        typeName = reference.Substring(colon + 1);
        return prefix.Length != 0 && typeName.Length != 0;
    }

    private static bool HasSemanticPartThemeProperty(INamedTypeSymbol control, string propertyName)
    {
        return control.GetMembers(propertyName)
                      .OfType<IPropertySymbol>()
                      .Any(static property =>
                          !property.IsStatic &&
                          property.DeclaredAccessibility == Accessibility.Public &&
                          string.Equals(
                              property.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                              ControlThemeType,
                              StringComparison.Ordinal));
    }

    private static void ReportAmbiguousOwner(
        ThemeAssetInfo asset,
        string candidate,
        Action<Diagnostic> reportDiagnostic)
    {
        reportDiagnostic(Diagnostic.Create(
            AtomUIDiagnosticDescriptors.ThemeAssetAmbiguousControl,
            asset.CreateLocation(),
            asset.Path,
            candidate));
    }

}
