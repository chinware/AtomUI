using System.Collections.Immutable;
using AtomUI.Generator.Localization.Xliff;
using Microsoft.CodeAnalysis;

namespace AtomUI.Generator.Localization.Catalog;

internal static class ReferencedLanguageCatalogResolver
{
    private const string LanguageCatalogAttributeName =
        "AtomUI.Localization.LanguageCatalogAttribute";
    private const string AssemblyMetadataAttributeName =
        "System.Reflection.AssemblyMetadataAttribute";
    private const string LanguageModuleIdMetadataName = "AtomUILanguageModuleId";

    internal static bool TryResolve(
        Compilation compilation,
        AdditionalLanguageFile file,
        out LanguageCatalogInfo? catalog,
        out string error)
    {
        var symbol = compilation.GetTypeByMetadataName(file.Document.File.Id);
        if (symbol is null)
        {
            catalog = null;
            error = $"referenced Catalog '{file.Document.File.Id}' could not be found";
            return false;
        }

        var catalogAttribute = symbol.GetAttributes().FirstOrDefault(static attribute =>
            attribute.AttributeClass?.ToDisplayString() == LanguageCatalogAttributeName);
        if (symbol.TypeKind != TypeKind.Enum || symbol.IsGenericType || catalogAttribute is null)
        {
            catalog = null;
            error = $"referenced Catalog '{file.Document.File.Id}' is not a valid LanguageCatalog enum";
            return false;
        }

        if (symbol.GetAttributes().Any(static attribute =>
                attribute.AttributeClass?.ToDisplayString() == "System.FlagsAttribute"))
        {
            catalog = null;
            error = $"referenced Catalog '{file.Document.File.Id}' cannot be a Flags enum";
            return false;
        }

        var actualModuleId = GetModuleId(symbol.ContainingAssembly);
        if (!string.Equals(file.ModuleId, actualModuleId, StringComparison.Ordinal))
        {
            catalog = null;
            error = $"referenced Catalog '{file.Document.File.Id}' belongs to language module " +
                    $"'{actualModuleId}' instead of '{file.ModuleId}'";
            return false;
        }

        var contractVersion = GetContractVersion(catalogAttribute);
        if (file.ContractVersion != contractVersion)
        {
            catalog = null;
            error = $"language input ContractVersion '{file.ContractVersion}' does not match " +
                    $"referenced Catalog ContractVersion '{contractVersion}'";
            return false;
        }

        var units = ImmutableArray.CreateBuilder<LanguageCatalogUnitInfo>();
        foreach (var field in symbol.GetMembers().OfType<IFieldSymbol>()
                                    .Where(static field =>
                                        !field.IsImplicitlyDeclared && field.HasConstantValue))
        {
            units.Add(new LanguageCatalogUnitInfo(field.Name, Location.None));
        }

        if (units.Count == 0)
        {
            catalog = null;
            error = $"referenced Catalog '{file.Document.File.Id}' does not declare any units";
            return false;
        }

        var namespaceName = symbol.ContainingNamespace.IsGlobalNamespace
            ? string.Empty
            : symbol.ContainingNamespace.ToDisplayString();
        catalog = new LanguageCatalogInfo(
            actualModuleId,
            file.Document.File.Id,
            namespaceName,
            symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            contractVersion,
            units.OrderBy(static unit => unit.Key, StringComparer.Ordinal).ToImmutableArray(),
            Location.None);
        error = string.Empty;
        return true;
    }

    private static string GetModuleId(IAssemblySymbol assembly)
    {
        foreach (var attribute in assembly.GetAttributes())
        {
            if (attribute.AttributeClass?.ToDisplayString() != AssemblyMetadataAttributeName ||
                attribute.ConstructorArguments.Length != 2 ||
                attribute.ConstructorArguments[0].Value is not string key ||
                attribute.ConstructorArguments[1].Value is not string value ||
                key != LanguageModuleIdMetadataName ||
                string.IsNullOrWhiteSpace(value))
            {
                continue;
            }

            return value.Trim();
        }

        return assembly.Name;
    }

    private static int GetContractVersion(AttributeData attribute)
    {
        foreach (var argument in attribute.NamedArguments)
        {
            if (argument.Key == "ContractVersion" && argument.Value.Value is int value)
            {
                return value;
            }
        }

        return 1;
    }

}
