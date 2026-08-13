using AtomUI.Generator.Diagnostics;
using AtomUI.Generator.LinkedRegistration.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace AtomUI.Generator;

internal sealed class ControlThemeInfo
{
    internal ControlThemeInfo(
        string controlNamespace,
        string controlName,
        string controlMetadataName,
        string controlTypeName,
        string unitId,
        bool hasDescriptor,
        bool ownsControlMap,
        ControlTokenInfo? ownToken)
    {
        ControlNamespace = controlNamespace;
        ControlName = controlName;
        ControlMetadataName = controlMetadataName;
        ControlTypeName = controlTypeName;
        UnitId = unitId;
        HasDescriptor = hasDescriptor;
        OwnsControlMap = ownsControlMap;
        OwnToken = ownToken;
    }

    internal string ControlNamespace { get; }
    internal string ControlName { get; }
    internal string ControlMetadataName { get; }
    internal string ControlTypeName { get; }
    internal string UnitId { get; }
    internal bool HasDescriptor { get; }
    internal bool OwnsControlMap { get; }
    internal ControlTokenInfo? OwnToken { get; }
    internal bool HasOwnToken => OwnToken is not null;
    internal string TokenKindType => $"{ControlName}TokenKind";
    internal string TokenKeyType => $"{ControlName}TokenKey";
    internal string TokensType => $"{ControlName}Tokens";
    internal string ResourceExtensionType => $"{ControlName}TokenResourceExtension";
    internal IEnumerable<TokenName> OwnTokens => OwnToken is null
        ? Array.Empty<TokenName>()
        : OwnToken.Tokens;
    internal IEnumerable<SchemaTokenInfo> OwnSchemaTokens => OwnToken is null
        ? Array.Empty<SchemaTokenInfo>()
        : OwnToken.SchemaTokens;

}

internal sealed class ControlThemeSourceInfo
{
    private ControlThemeSourceInfo(
        string path,
        string assetPath,
        string? controlCandidate,
        string? explicitUnit,
        Location location)
    {
        Path = path;
        AssetPath = assetPath;
        ControlCandidate = controlCandidate;
        ExplicitUnit = explicitUnit;
        Location = location;
    }

    internal string Path { get; }
    internal string AssetPath { get; }
    internal string? ControlCandidate { get; }
    internal string? ExplicitUnit { get; }
    internal Location Location { get; }

    internal static ControlThemeSourceInfo Create(
        AdditionalText text,
        string? projectDirectory,
        string? link,
        string? explicitUnit,
        CancellationToken cancellationToken)
    {
        var source = text.GetText(cancellationToken) ?? SourceText.From(string.Empty);

        var span = new TextSpan(0, source.Length);
        var location = Location.Create(text.Path, span, source.Lines.GetLinePositionSpan(span));
        var assetPath = ThemeAssetInfo.NormalizeAssetPath(text.Path, projectDirectory, link);
        return new ControlThemeSourceInfo(
            text.Path,
            assetPath,
            GetControlCandidate(assetPath),
            explicitUnit,
            location);
    }

    private static string? GetControlCandidate(string path)
    {
        var fileName = System.IO.Path.GetFileNameWithoutExtension(path);
        const string suffix = "Theme";
        if (!fileName.EndsWith(suffix, StringComparison.Ordinal) ||
            fileName.EndsWith("Themes", StringComparison.Ordinal) ||
            fileName.Length == suffix.Length)
        {
            return null;
        }

        return fileName.Substring(0, fileName.Length - suffix.Length);
    }

}

internal static class ControlThemeModelBuilder
{
    private const string ControlBaseType = "global::Avalonia.Controls.Control";

    internal static IReadOnlyList<ControlThemeInfo> Build(
        Compilation compilation,
        IEnumerable<ControlTokenInfo> ownTokens,
        IEnumerable<ControlThemeSourceInfo> assets,
        ISet<string> globalTokenNames,
        string packageId,
        string? projectDirectory,
        AnalyzerConfigOptionsProvider optionsProvider,
        Action<Diagnostic> reportDiagnostic)
    {
        var controls = GetPublicControls(compilation.Assembly.GlobalNamespace).ToArray();
        var result = new Dictionary<string, ControlThemeInfo>(StringComparer.Ordinal);

        foreach (var ownToken in ownTokens)
        {
            foreach (var diagnostic in ownToken.Diagnostics)
            {
                reportDiagnostic(diagnostic);
            }
            if (!ownToken.IsValid)
            {
                continue;
            }

            var conflictingTokenNames = ownToken.Tokens
                                                .Select(static token => token.Name)
                                                .Where(globalTokenNames.Contains)
                                                .OrderBy(static name => name, StringComparer.Ordinal)
                                                .ToArray();
            foreach (var tokenName in conflictingTokenNames)
            {
                reportDiagnostic(Diagnostic.Create(
                    AtomUIDiagnosticDescriptors.ControlTokenGlobalNameConflict,
                    ownToken.DeclarationLocation,
                    ownToken.ControlName,
                    tokenName));
            }
            if (conflictingTokenNames.Length != 0)
            {
                continue;
            }

            var matches = FindMatchingControls(compilation, controls, ownToken).ToArray();
            if (matches.Length == 0)
            {
                reportDiagnostic(Diagnostic.Create(
                    AtomUIDiagnosticDescriptors.ControlTokenMissingControl,
                    ownToken.DeclarationLocation,
                    ownToken.TokenName,
                    ownToken.ControlName));
                continue;
            }
            if (matches.Length > 1)
            {
                reportDiagnostic(Diagnostic.Create(
                    AtomUIDiagnosticDescriptors.ControlTokenAmbiguousControl,
                    ownToken.DeclarationLocation,
                    ownToken.TokenName,
                    ownToken.ControlName));
                continue;
            }

            var control = matches[0];
            var info = CreateInfo(
                control,
                ownToken,
                hasDescriptor: true,
                ownsControlMap: SymbolEqualityComparer.Default.Equals(
                    control.ContainingAssembly,
                    compilation.Assembly),
                packageId,
                projectDirectory,
                optionsProvider);
            result[GetControlKey(control)] = info;
        }

        foreach (var asset in assets)
        {
            if (asset.ControlCandidate is not null)
            {
                var matches = FindPublicControlsByName(
                    compilation,
                    controls,
                    asset.ControlCandidate).ToArray();
                if (matches.Length == 1)
                {
                    var control = matches[0];
                    var key = GetControlKey(control);
                    if (!result.ContainsKey(key))
                    {
                        result.Add(key, CreateInfo(
                            control,
                            null,
                            hasDescriptor: true,
                            ownsControlMap: SymbolEqualityComparer.Default.Equals(
                                control.ContainingAssembly,
                                compilation.Assembly),
                            packageId,
                            projectDirectory,
                            optionsProvider,
                            asset.AssetPath,
                            asset.ExplicitUnit));
                    }
                }
                else if (matches.Length > 1)
                {
                    reportDiagnostic(Diagnostic.Create(
                        AtomUIDiagnosticDescriptors.ThemeAssetAmbiguousControl,
                        asset.Location,
                        asset.Path,
                        asset.ControlCandidate));
                }
            }

        }

        foreach (var control in controls)
        {
            var key = GetControlKey(control);
            if (result.ContainsKey(key))
            {
                continue;
            }

            var info = CreateInfo(
                control,
                null,
                hasDescriptor: false,
                ownsControlMap: true,
                packageId,
                projectDirectory,
                optionsProvider);
            result.Add(key, info);
        }

        return result.Values.OrderBy(static info => info.ControlName, StringComparer.Ordinal).ToArray();
    }

    private static IEnumerable<INamedTypeSymbol> FindMatchingControls(
        Compilation compilation,
        IReadOnlyList<INamedTypeSymbol> sourceControls,
        ControlTokenInfo ownToken)
    {
        var metadataName = string.IsNullOrWhiteSpace(ownToken.TokenNamespace)
            ? ownToken.ControlName!
            : $"{ownToken.TokenNamespace}.{ownToken.ControlName}";
        var exact = compilation.GetTypeByMetadataName(metadataName);
        if (exact is not null && IsPublicControl(exact))
        {
            return [exact];
        }

        return FindPublicControlsByName(compilation, sourceControls, ownToken.ControlName!);
    }

    internal static IEnumerable<INamedTypeSymbol> FindPublicControlsByName(
        Compilation compilation,
        IReadOnlyList<INamedTypeSymbol> sourceControls,
        string name)
    {
        var sourceMatches = sourceControls
            .Where(control => string.Equals(control.Name, name, StringComparison.Ordinal))
            .ToArray();
        if (sourceMatches.Length != 0)
        {
            return sourceMatches;
        }

        var referencedMatches = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
        foreach (var assembly in compilation.SourceModule.ReferencedAssemblySymbols)
        {
            AddPublicControlsByName(assembly.GlobalNamespace, name, referencedMatches);
        }
        var atomUIMatches = referencedMatches
            .Where(static type => ThemeGeneratorOptions.IsBuiltInControlCatalog(type.ContainingAssembly))
            .ToArray();
        if (atomUIMatches.Length != 0)
        {
            return atomUIMatches;
        }

        var result = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
        foreach (var namespaceName in new[]
                 {
                     "Avalonia.Controls",
                     "Avalonia.Controls.Primitives"
                 })
        {
            var type = compilation.GetTypeByMetadataName($"{namespaceName}.{name}");
            if (type is not null && IsPublicControl(type))
            {
                result.Add(type);
            }
        }

        return result.Count != 0 ? result : referencedMatches;
    }

    private static void AddPublicControlsByName(
        INamespaceSymbol ns,
        string name,
        ISet<INamedTypeSymbol> result)
    {
        foreach (var type in ns.GetTypeMembers(name))
        {
            if (IsPublicControl(type))
            {
                result.Add(type);
            }
        }

        foreach (var child in ns.GetNamespaceMembers())
        {
            AddPublicControlsByName(child, name, result);
        }
    }

    private static ControlThemeInfo CreateInfo(
        INamedTypeSymbol control,
        ControlTokenInfo? ownToken,
        bool hasDescriptor,
        bool ownsControlMap,
        string packageId,
        string? projectDirectory,
        AnalyzerConfigOptionsProvider optionsProvider,
        string? fallbackSourcePath = null,
        string? fallbackExplicitUnit = null)
    {
        var sourceTree = control.DeclaringSyntaxReferences.FirstOrDefault()?.SyntaxTree;
        string? explicitUnit = null;
        if (sourceTree is not null)
        {
            optionsProvider.GetOptions(sourceTree).TryGetValue(
                "build_metadata.Compile.AtomUIRegistrationUnit",
                out explicitUnit);
        }
        explicitUnit ??= fallbackExplicitUnit;
        var sourcePath = sourceTree?.FilePath;
        if (string.IsNullOrWhiteSpace(sourcePath))
        {
            sourcePath = fallbackSourcePath;
        }

        return new ControlThemeInfo(
            ownToken?.TokenNamespace ??
            (control.ContainingNamespace.IsGlobalNamespace
                ? string.Empty
                : control.ContainingNamespace.ToDisplayString()),
            control.Name,
            GetMetadataName(control),
            control.ToDisplayString(GeneratorSymbolDisplay.FullyQualifiedType),
            RegistrationUnitId.Create(
                packageId,
                sourcePath,
                projectDirectory,
                control.Name,
                explicitUnit),
            hasDescriptor,
            ownsControlMap,
            ownToken);
    }

    private static string GetMetadataName(INamedTypeSymbol symbol)
    {
        var typeNames = new Stack<string>();
        for (var current = symbol; current is not null; current = current.ContainingType)
        {
            typeNames.Push(current.MetadataName);
        }

        var typeName = string.Join("+", typeNames);
        return symbol.ContainingNamespace.IsGlobalNamespace
            ? typeName
            : symbol.ContainingNamespace.ToDisplayString() + "." + typeName;
    }

    internal static IEnumerable<INamedTypeSymbol> GetPublicControls(INamespaceSymbol ns)
    {
        foreach (var type in ns.GetTypeMembers())
        {
            if (IsPublicControl(type))
            {
                yield return type;
            }
        }

        foreach (var child in ns.GetNamespaceMembers())
        {
            foreach (var control in GetPublicControls(child))
            {
                yield return control;
            }
        }
    }

    internal static bool IsPublicControl(INamedTypeSymbol type)
    {
        if (type.DeclaredAccessibility != Accessibility.Public || type.Arity != 0)
        {
            return false;
        }

        for (var current = type; current is not null; current = current.BaseType)
        {
            if (string.Equals(
                    current.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                    ControlBaseType,
                    StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    private static string GetControlKey(INamedTypeSymbol control)
    {
        return control.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
    }
}
