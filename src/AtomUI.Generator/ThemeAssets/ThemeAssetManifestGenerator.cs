using AtomUI.Generator.Diagnostics;
using AtomUI.Generator.LinkedRegistration.Model;
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
                                input.Right.GetOptions(input.Left).TryGetValue(
                                    "build_metadata.AdditionalFiles.AtomUIRegistrationUnit",
                                    out var explicitUnit);
                                return ThemeAssetInfo.Create(
                                    input.Left,
                                    projectDirectory,
                                    link,
                                    explicitUnit,
                                    token);
                            })
                            .Collect();
        var input = assets.Combine(context.CompilationProvider)
                          .Combine(context.AnalyzerConfigOptionsProvider);

        context.RegisterSourceOutput(input, static (productionContext, value) =>
        {
            if (value.Left.Left.IsEmpty)
            {
                return;
            }

            var compilation = value.Left.Right;
            var assemblyName = compilation.AssemblyName ?? "AtomUI";
            var controlCatalog = ThemeGeneratorOptions.GetControlCatalog(value.Right);
            var packageId = LinkedRegistration.LinkedRegistrationOptions.GetPackageId(
                value.Right,
                assemblyName);
            value.Right.GlobalOptions.TryGetValue(
                "build_property.AtomUIThemeAssetProjectDirectory",
                out var projectDirectory);
            var packageSharedPaths = LinkedRegistration.LinkedRegistrationOptions
                .GetPackageSharedThemePaths(value.Right);
            var reportFallbackDiagnostics = LinkedRegistration.LinkedRegistrationOptions
                                                .IsLinkedPublish(value.Right) ||
                                            LinkedRegistration.LinkedRegistrationOptions
                                                .IsRegistrationStrict(value.Right);
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
            var knownUnitIds = new HashSet<string>(
                sourceControls.Select(control => GetControlUnitId(
                    control,
                    packageId,
                    projectDirectory,
                    value.Right)),
                StringComparer.Ordinal);
            var resolved = new List<ResolvedThemeAssetInfo>();
            var unitOwned = new List<UnitOwnedThemeAssetInfo>();
            var packageShared = new List<ThemeAssetInfo>();
            var unknown = new List<ThemeAssetInfo>();
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
                if (packageSharedPaths.Contains(asset.AssetPath))
                {
                    if (asset.ControlTokenFamilies.Count != 0)
                    {
                        productionContext.ReportDiagnostic(Diagnostic.Create(
                            AtomUIDiagnosticDescriptors.LinkedPackageDefinitionInvalid,
                            asset.CreateLocation(),
                            packageId,
                            $"PackageShared theme '{asset.AssetPath}' references Control-owned token resources"));
                        continue;
                    }

                    packageShared.Add(asset);
                    continue;
                }

                var resolution = Resolve(
                    compilation,
                    sourceControls,
                    asset,
                    controlCatalog,
                    packageId,
                    projectDirectory,
                    value.Right,
                    productionContext.ReportDiagnostic,
                    out var resolutionFailed);
                if (resolution is not null)
                {
                    resolved.Add(resolution);
                }
                else if (!resolutionFailed)
                {
                    var unitId = RegistrationUnitId.Create(
                        packageId,
                        asset.AssetPath,
                        projectDirectory,
                        asset.ControlCandidate ?? asset.FileName,
                        asset.ExplicitUnit);
                    if (knownUnitIds.Contains(unitId))
                    {
                        unitOwned.Add(new UnitOwnedThemeAssetInfo(asset, unitId));
                    }
                    else
                    {
                        unknown.Add(asset);
                        if (reportFallbackDiagnostics)
                        {
                            productionContext.ReportDiagnostic(Diagnostic.Create(
                                AtomUIDiagnosticDescriptors.LinkedDynamicUsageWidened,
                                asset.CreateLocation(),
                                asset.AssetPath,
                                packageId));
                        }
                    }
                }
            }

            foreach (var missingSharedPath in packageSharedPaths.Except(
                         uniqueAssets.Select(static asset => asset.AssetPath),
                         StringComparer.Ordinal))
            {
                productionContext.ReportDiagnostic(Diagnostic.Create(
                    AtomUIDiagnosticDescriptors.LinkedPackageDefinitionInvalid,
                    Location.None,
                    packageId,
                    $"PackageShared theme '{missingSharedPath}' is not an AvaloniaXaml input"));
            }

            new ThemeAssetManifestWriter(
                productionContext,
                assemblyName,
                packageId,
                resolved,
                unitOwned,
                packageShared,
                unknown,
                globalTokenNames).Write();
        });
    }

    private static ResolvedThemeAssetInfo? Resolve(
        Compilation compilation,
        IReadOnlyList<INamedTypeSymbol> sourceControls,
        ThemeAssetInfo asset,
        string controlCatalog,
        string packageId,
        string? projectDirectory,
        Microsoft.CodeAnalysis.Diagnostics.AnalyzerConfigOptionsProvider optionsProvider,
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
        var unitDependencies = new HashSet<INamedTypeSymbol>(
            referenced,
            SymbolEqualityComparer.Default);
        foreach (var elementType in asset.ElementTypes)
        {
            var matches = ResolveCurrentAssemblyElementControls(
                compilation,
                sourceControls,
                elementType).ToArray();
            if (matches.Length == 1)
            {
                unitDependencies.Add(matches[0]);
            }
        }

        var ownerUnitId = GetUnitId(
            owner,
            asset,
            packageId,
            projectDirectory,
            optionsProvider,
            useAssetOverride: true);
        var referencedUnitIds = unitDependencies
            .Where(control => SymbolEqualityComparer.Default.Equals(
                control.ContainingAssembly,
                compilation.Assembly))
            .Select(control => GetUnitId(
                control,
                asset,
                packageId,
                projectDirectory,
                optionsProvider,
                useAssetOverride: SymbolEqualityComparer.Default.Equals(control, owner)))
            .Where(unitId => !string.Equals(unitId, ownerUnitId, StringComparison.Ordinal))
            .Distinct(StringComparer.Ordinal)
            .OrderBy(static unitId => unitId, StringComparer.Ordinal)
            .ToArray();

        return new ResolvedThemeAssetInfo(
            asset,
            new ThemeAssetControlIdentityInfo(controlCatalog, owner.Name),
            ownerUnitId,
            referenced.Select(control => CreateIdentity(
                                  compilation,
                                  control,
                                  owner,
                                  controlCatalog))
                      .OrderBy(static identity => identity.Catalog, StringComparer.Ordinal)
                      .ThenBy(static identity => identity.Id, StringComparer.Ordinal)
                      .ToArray(),
            referencedUnitIds,
            semanticPart);
    }

    private static string GetUnitId(
        INamedTypeSymbol control,
        ThemeAssetInfo asset,
        string packageId,
        string? projectDirectory,
        Microsoft.CodeAnalysis.Diagnostics.AnalyzerConfigOptionsProvider optionsProvider,
        bool useAssetOverride)
    {
        var sourceTree = control.DeclaringSyntaxReferences.FirstOrDefault()?.SyntaxTree;
        var explicitUnit = GetExplicitUnit(sourceTree, optionsProvider);
        if (useAssetOverride && string.IsNullOrWhiteSpace(explicitUnit))
        {
            explicitUnit = asset.ExplicitUnit;
        }

        return RegistrationUnitId.Create(
            packageId,
            sourceTree?.FilePath ?? asset.AssetPath,
            projectDirectory,
            control.Name,
            explicitUnit);
    }

    private static string GetControlUnitId(
        INamedTypeSymbol control,
        string packageId,
        string? projectDirectory,
        Microsoft.CodeAnalysis.Diagnostics.AnalyzerConfigOptionsProvider optionsProvider)
    {
        var sourceTree = control.DeclaringSyntaxReferences.FirstOrDefault()?.SyntaxTree;
        return RegistrationUnitId.Create(
            packageId,
            sourceTree?.FilePath,
            projectDirectory,
            control.Name,
            GetExplicitUnit(sourceTree, optionsProvider));
    }

    private static string? GetExplicitUnit(
        SyntaxTree? sourceTree,
        Microsoft.CodeAnalysis.Diagnostics.AnalyzerConfigOptionsProvider optionsProvider)
    {
        if (sourceTree is null)
        {
            return null;
        }

        optionsProvider.GetOptions(sourceTree).TryGetValue(
            "build_metadata.Compile.AtomUIRegistrationUnit",
            out var explicitUnit);
        return explicitUnit;
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

    private static IEnumerable<INamedTypeSymbol> ResolveCurrentAssemblyElementControls(
        Compilation compilation,
        IReadOnlyList<INamedTypeSymbol> sourceControls,
        ThemeAssetElementTypeReference reference)
    {
        var namespaces = GetCurrentAssemblyClrNamespaces(compilation.Assembly, reference.NamespaceUri);
        if (namespaces.Count == 0)
        {
            return Array.Empty<INamedTypeSymbol>();
        }

        return sourceControls.Where(control =>
            string.Equals(control.Name, reference.LocalName, StringComparison.Ordinal) &&
            namespaces.Contains(
                control.ContainingNamespace.IsGlobalNamespace
                    ? string.Empty
                    : control.ContainingNamespace.ToDisplayString()));
    }

    private static ISet<string> GetCurrentAssemblyClrNamespaces(
        IAssemblySymbol assembly,
        string xmlNamespace)
    {
        var namespaces = new HashSet<string>(StringComparer.Ordinal);
        if (TryGetDirectClrNamespace(xmlNamespace, assembly.Name, out var directNamespace))
        {
            namespaces.Add(directNamespace);
            return namespaces;
        }

        foreach (var attribute in assembly.GetAttributes())
        {
            if (attribute.AttributeClass?.ToDisplayString() !=
                    "Avalonia.Metadata.XmlnsDefinitionAttribute" ||
                attribute.ConstructorArguments.Length < 2 ||
                attribute.ConstructorArguments[0].Value is not string declaredXmlNamespace ||
                attribute.ConstructorArguments[1].Value is not string clrNamespace ||
                !string.Equals(declaredXmlNamespace, xmlNamespace, StringComparison.Ordinal))
            {
                continue;
            }

            namespaces.Add(clrNamespace);
        }

        return namespaces;
    }

    private static bool TryGetDirectClrNamespace(
        string xmlNamespace,
        string assemblyName,
        out string clrNamespace)
    {
        const string usingPrefix = "using:";
        const string clrNamespacePrefix = "clr-namespace:";
        var prefixLength = xmlNamespace.StartsWith(usingPrefix, StringComparison.Ordinal)
            ? usingPrefix.Length
            : xmlNamespace.StartsWith(clrNamespacePrefix, StringComparison.Ordinal)
                ? clrNamespacePrefix.Length
                : 0;
        if (prefixLength == 0)
        {
            clrNamespace = string.Empty;
            return false;
        }

        var value = xmlNamespace.Substring(prefixLength);
        var assemblySeparator = value.IndexOf(';');
        if (assemblySeparator >= 0)
        {
            var qualifier = value.Substring(assemblySeparator + 1);
            const string assemblyPrefix = "assembly=";
            if (!qualifier.StartsWith(assemblyPrefix, StringComparison.Ordinal) ||
                !string.Equals(
                    qualifier.Substring(assemblyPrefix.Length),
                    assemblyName,
                    StringComparison.Ordinal))
            {
                clrNamespace = string.Empty;
                return false;
            }
            value = value.Substring(0, assemblySeparator);
        }

        clrNamespace = value;
        return clrNamespace.Length != 0;
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
