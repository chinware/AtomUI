using Microsoft.CodeAnalysis;

namespace AtomUI.Generator.LinkedRegistration;

internal static class ControlPackageRegistrationEntryDiscovery
{
    private const string BuilderMetadataName = "AtomUI.IAtomUIBuilder";

    internal static ControlPackageRegistrationEntrySet Discover(Compilation compilation)
    {
        var attributeType = compilation.GetTypeByMetadataName(
            TargetMarkConstants.ControlPackageRegistrationEntryAttribute);
        if (attributeType is null)
        {
            return ControlPackageRegistrationEntrySet.Empty;
        }

        var builderType = compilation.GetTypeByMetadataName(BuilderMetadataName);
        var methods = new List<IMethodSymbol>();
        CollectNamespace(
            compilation.Assembly.GlobalNamespace,
            attributeType,
            methods);

        var entries = new HashSet<string>(StringComparer.Ordinal);
        var issues = new List<ControlPackageRegistrationEntryIssue>();
        foreach (var method in methods.OrderBy(
                     LinkedRegistrationSymbolName.GetMethodMetadataName,
                     StringComparer.Ordinal).ThenBy(
                     static method => GetSourcePath(method),
                     StringComparer.Ordinal).ThenBy(
                     static method => GetSourceStart(method)))
        {
            var reason = Validate(compilation, method, builderType);
            if (reason is not null)
            {
                issues.Add(new ControlPackageRegistrationEntryIssue(
                    method.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat),
                    reason,
                    GetLocation(method, attributeType)));
                continue;
            }

            entries.Add(LinkedRegistrationSymbolName.GetMethodMetadataName(method));
        }

        return new ControlPackageRegistrationEntrySet(
            entries.OrderBy(static entry => entry, StringComparer.Ordinal).ToArray(),
            issues.ToArray());
    }

    private static void CollectNamespace(
        INamespaceSymbol namespaceSymbol,
        INamedTypeSymbol attributeType,
        ICollection<IMethodSymbol> methods)
    {
        foreach (var type in namespaceSymbol.GetTypeMembers().OrderBy(
                     static type => type.MetadataName,
                     StringComparer.Ordinal))
        {
            CollectType(type, attributeType, methods);
        }
        foreach (var child in namespaceSymbol.GetNamespaceMembers().OrderBy(
                     static child => child.Name,
                     StringComparer.Ordinal))
        {
            CollectNamespace(child, attributeType, methods);
        }
    }

    private static void CollectType(
        INamedTypeSymbol type,
        INamedTypeSymbol attributeType,
        ICollection<IMethodSymbol> methods)
    {
        foreach (var method in type.GetMembers().OfType<IMethodSymbol>())
        {
            if (method.GetAttributes().Any(attribute =>
                    SymbolEqualityComparer.Default.Equals(
                        attribute.AttributeClass,
                        attributeType)))
            {
                methods.Add(method);
            }
        }
        foreach (var nested in type.GetTypeMembers().OrderBy(
                     static nested => nested.MetadataName,
                     StringComparer.Ordinal))
        {
            CollectType(nested, attributeType, methods);
        }
    }

    private static string? Validate(
        Compilation compilation,
        IMethodSymbol method,
        INamedTypeSymbol? builderType)
    {
        if (method.DeclaredAccessibility != Accessibility.Public ||
            method.ContainingType.DeclaredAccessibility != Accessibility.Public)
        {
            return "the method and containing type must be public";
        }
        if (!method.IsStatic || !method.ContainingType.IsStatic)
        {
            return "the method must be declared in a static class";
        }
        if (method.IsGenericMethod)
        {
            return "the method must be non-generic";
        }
        if (!method.IsExtensionMethod ||
            method.Parameters.Length == 0 ||
            builderType is null ||
            !SymbolEqualityComparer.Default.Equals(method.Parameters[0].Type, builderType))
        {
            return "the method must be an extension method whose first parameter is IAtomUIBuilder";
        }
        if (!compilation.ClassifyCommonConversion(method.ReturnType, builderType).IsImplicit)
        {
            return "the return type must be assignable to IAtomUIBuilder";
        }
        if (method.ContainingType.GetMembers(method.Name)
                  .OfType<IMethodSymbol>()
                  .Count(static candidate => !candidate.IsImplicitlyDeclared) != 1)
        {
            return "the entry method name must not be overloaded";
        }
        return null;
    }

    private static Location GetLocation(
        IMethodSymbol method,
        INamedTypeSymbol attributeType)
    {
        var attributeLocation = method.GetAttributes()
                                      .FirstOrDefault(attribute =>
                                          SymbolEqualityComparer.Default.Equals(
                                              attribute.AttributeClass,
                                              attributeType))
                                      ?.ApplicationSyntaxReference
                                      ?.GetSyntax()
                                      .GetLocation();
        return attributeLocation ?? method.Locations.FirstOrDefault(static location => location.IsInSource) ?? Location.None;
    }

    private static string GetSourcePath(IMethodSymbol method)
    {
        return method.Locations.FirstOrDefault(static location => location.IsInSource)?
                     .GetLineSpan().Path ?? string.Empty;
    }

    private static int GetSourceStart(IMethodSymbol method)
    {
        return method.Locations.FirstOrDefault(static location => location.IsInSource)?
                     .SourceSpan.Start ?? 0;
    }
}

internal sealed class ControlPackageRegistrationEntrySet
{
    internal static ControlPackageRegistrationEntrySet Empty { get; } = new([], []);

    internal ControlPackageRegistrationEntrySet(
        IReadOnlyList<string> entries,
        IReadOnlyList<ControlPackageRegistrationEntryIssue> issues)
    {
        Entries = entries;
        Issues = issues;
    }

    internal IReadOnlyList<string> Entries { get; }
    internal IReadOnlyList<ControlPackageRegistrationEntryIssue> Issues { get; }
    internal bool HasEntries => Entries.Count != 0;
    internal string ManifestMethodMetadataNames => string.Join(";", Entries);
}

internal sealed class ControlPackageRegistrationEntryIssue
{
    internal ControlPackageRegistrationEntryIssue(
        string methodDisplayName,
        string reason,
        Location location)
    {
        MethodDisplayName = methodDisplayName;
        Reason = reason;
        Location = location;
    }

    internal string MethodDisplayName { get; }
    internal string Reason { get; }
    internal Location Location { get; }
}
