using System.Collections.Immutable;
using AtomUI.Generator.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AtomUI.Generator.Localization.Catalog;

internal static class LanguageCatalogSymbolParser
{
    internal static LanguageCatalogParseResult Parse(
        LanguageCatalogTarget target,
        string moduleId,
        CancellationToken cancellationToken)
    {
        var symbol = target.Symbol;
        var diagnostics = ImmutableArray.CreateBuilder<Diagnostic>();
        var typeLocation = GetTypeIdentifierLocation(symbol, cancellationToken);
        if (symbol.TypeKind != TypeKind.Enum || symbol.IsGenericType)
        {
            diagnostics.Add(InvalidCatalog(
                symbol,
                typeLocation,
                "the target must be a non-generic enum"));
            return new LanguageCatalogParseResult(null, diagnostics.ToImmutable());
        }

        if (symbol.GetAttributes().Any(static attribute =>
                attribute.AttributeClass?.ToDisplayString() == "System.FlagsAttribute"))
        {
            diagnostics.Add(InvalidCatalog(
                symbol,
                typeLocation,
                "Flags Catalog enums are not supported"));
        }

        var contractVersion = GetContractVersion(target.Attribute);
        if (contractVersion <= 0)
        {
            diagnostics.Add(InvalidCatalog(
                symbol,
                typeLocation,
                "ContractVersion must be positive"));
        }

        var fields = symbol.GetMembers()
                           .OfType<IFieldSymbol>()
                           .Where(static field => !field.IsImplicitlyDeclared && field.HasConstantValue)
                           .ToArray();
        if (fields.Length == 0)
        {
            diagnostics.Add(InvalidCatalog(
                symbol,
                typeLocation,
                "the Catalog must declare at least one resource unit"));
        }

        var units = ImmutableArray.CreateBuilder<LanguageCatalogUnitInfo>();
        var ids = new HashSet<int>();
        foreach (var field in fields)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var declaration = field.DeclaringSyntaxReferences
                                   .Select(reference => reference.GetSyntax(cancellationToken))
                                   .OfType<EnumMemberDeclarationSyntax>()
                                   .FirstOrDefault();
            if (declaration?.EqualsValue is null)
            {
                diagnostics.Add(InvalidUnit(
                    symbol,
                    field,
                    declaration?.Identifier.GetLocation() ?? field.Locations.FirstOrDefault() ?? typeLocation,
                    "the unit must declare an explicit positive integer ID"));
                continue;
            }

            if (!TryGetPositiveInt32(field.ConstantValue, out var id))
            {
                diagnostics.Add(InvalidUnit(
                    symbol,
                    field,
                    declaration.EqualsValue.Value.GetLocation(),
                    "the explicit unit ID must be a positive Int32 value"));
                continue;
            }

            if (!ids.Add(id))
            {
                diagnostics.Add(InvalidUnit(
                    symbol,
                    field,
                    declaration.EqualsValue.Value.GetLocation(),
                    $"unit ID '{id}' is duplicated"));
                continue;
            }

            units.Add(new LanguageCatalogUnitInfo(id, field.Name, field.Locations[0]));
        }

        if (diagnostics.Count > 0)
        {
            return new LanguageCatalogParseResult(null, diagnostics.ToImmutable());
        }

        var namespaceName = symbol.ContainingNamespace.IsGlobalNamespace
            ? string.Empty
            : symbol.ContainingNamespace.ToDisplayString();
        var metadataName = GetMetadataName(symbol);
        var typeName = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var catalog = new LanguageCatalogInfo(
            moduleId,
            metadataName,
            namespaceName,
            typeName,
            contractVersion,
            units.OrderBy(static unit => unit.Id).ToImmutableArray(),
            typeLocation);
        return new LanguageCatalogParseResult(catalog, ImmutableArray<Diagnostic>.Empty);
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

    private static bool TryGetPositiveInt32(object? value, out int result)
    {
        long converted;
        switch (value)
        {
            case sbyte signedByte:
                converted = signedByte;
                break;
            case byte unsignedByte:
                converted = unsignedByte;
                break;
            case short signedShort:
                converted = signedShort;
                break;
            case ushort unsignedShort:
                converted = unsignedShort;
                break;
            case int signedInt:
                converted = signedInt;
                break;
            case uint unsignedInt:
                converted = unsignedInt;
                break;
            case long signedLong:
                converted = signedLong;
                break;
            case ulong unsignedLong when unsignedLong <= int.MaxValue:
                converted = (long)unsignedLong;
                break;
            default:
                result = default;
                return false;
        }

        if (converted is > 0 and <= int.MaxValue)
        {
            result = (int)converted;
            return true;
        }

        result = default;
        return false;
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
            : $"{symbol.ContainingNamespace.ToDisplayString()}.{typeName}";
    }

    private static Location GetTypeIdentifierLocation(
        INamedTypeSymbol symbol,
        CancellationToken cancellationToken)
    {
        var declaration = symbol.DeclaringSyntaxReferences
                                .Select(reference => reference.GetSyntax(cancellationToken))
                                .OfType<BaseTypeDeclarationSyntax>()
                                .FirstOrDefault();
        return declaration?.Identifier.GetLocation() ??
               symbol.Locations.FirstOrDefault() ??
               Location.None;
    }

    private static Diagnostic InvalidCatalog(
        INamedTypeSymbol catalog,
        Location location,
        string reason)
    {
        return Diagnostic.Create(
            AtomUIDiagnosticDescriptors.LocalizationInvalidCatalog,
            location,
            catalog.Name,
            reason);
    }

    private static Diagnostic InvalidUnit(
        INamedTypeSymbol catalog,
        IFieldSymbol field,
        Location location,
        string reason)
    {
        return Diagnostic.Create(
            AtomUIDiagnosticDescriptors.LocalizationInvalidCatalogUnit,
            location,
            field.Name,
            catalog.ToDisplayString(),
            reason);
    }
}

internal sealed class LanguageCatalogTarget
{
    internal LanguageCatalogTarget(INamedTypeSymbol symbol, AttributeData attribute)
    {
        Symbol = symbol;
        Attribute = attribute;
    }

    internal INamedTypeSymbol Symbol { get; }

    internal AttributeData Attribute { get; }
}
