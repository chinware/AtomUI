using Microsoft.CodeAnalysis;

namespace AtomUI.Generator;

[Generator]
public class TokenValueConverterRegistrationGenerator : IIncrementalGenerator
{
    private const string TokenValueConverterInterface = "global::AtomUI.Theme.TokenSystem.ITokenValueConverter";

    public void Initialize(IncrementalGeneratorInitializationContext initContext)
    {
        var converterTypesProvider = initContext.SyntaxProvider.ForAttributeWithMetadataName(
            TargetMarkConstants.TokenValueConverterAttribute,
            static (node, token) => true,
            static (context, token) => GetConverterTypeName(context))
            .Where(static typeName => !string.IsNullOrWhiteSpace(typeName))
            .Collect()
            .Select(static (typeNames, token) => typeNames.Distinct().OrderBy(typeName => typeName).ToArray());

        initContext.RegisterImplementationSourceOutput(converterTypesProvider, static (context, converterTypes) =>
        {
            if (converterTypes.Length == 0)
            {
                return;
            }

            var classWriter = new TokenValueConverterRegistryClassWriter(context, converterTypes!);
            classWriter.Write();
        });
    }

    private static string? GetConverterTypeName(GeneratorAttributeSyntaxContext context)
    {
        if (context.TargetSymbol is not INamedTypeSymbol typeSymbol)
        {
            return null;
        }

        if (typeSymbol.TypeKind != TypeKind.Class ||
            typeSymbol.IsAbstract ||
            typeSymbol.IsGenericType ||
            !ImplementsTokenValueConverter(typeSymbol))
        {
            return null;
        }

        return typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
    }

    private static bool ImplementsTokenValueConverter(INamedTypeSymbol typeSymbol)
    {
        foreach (var interfaceSymbol in typeSymbol.AllInterfaces)
        {
            if (interfaceSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == TokenValueConverterInterface)
            {
                return true;
            }
        }

        return false;
    }
}
