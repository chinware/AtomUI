using Microsoft.CodeAnalysis;

namespace AtomUI.Generator;

internal sealed class ThemeAlgorithmInfo
{
    private const string ThemeAlgorithmInterface = "global::AtomUI.Theme.Algorithms.IThemeAlgorithm";

    internal ThemeAlgorithmInfo(
        string id,
        int revision,
        string appearanceEffect,
        string typeName)
    {
        Id = id;
        Revision = revision;
        AppearanceEffect = appearanceEffect;
        TypeName = typeName;
    }

    public string Id { get; }
    public int Revision { get; }
    public string AppearanceEffect { get; }
    public string TypeName { get; }

    internal static ThemeAlgorithmInfo? Create(GeneratorAttributeSyntaxContext context)
    {
        if (context.TargetSymbol is not INamedTypeSymbol type ||
            type.IsAbstract ||
            type.IsGenericType ||
            !ImplementsThemeAlgorithm(type))
        {
            return null;
        }

        var attribute = context.Attributes.FirstOrDefault();
        if (attribute is null ||
            attribute.ConstructorArguments.Length != 3 ||
            attribute.ConstructorArguments[0].Value is not string id ||
            string.IsNullOrWhiteSpace(id) ||
            attribute.ConstructorArguments[1].Value is not int revision ||
            revision <= 0)
        {
            return null;
        }

        var appearanceValue = attribute.ConstructorArguments[2].Value switch
        {
            byte value  => value,
            short value => value,
            int value   => value,
            _           => -1
        };

        var appearance = appearanceValue switch
        {
            0 => "Preserve",
            1 => "Light",
            2 => "Dark",
            _ => null
        };
        if (appearance is null)
        {
            return null;
        }

        var parameterless = type.InstanceConstructors.FirstOrDefault(static constructor =>
            constructor.DeclaredAccessibility == Accessibility.Public && constructor.Parameters.Length == 0);
        if (parameterless is not null)
        {
            return new ThemeAlgorithmInfo(
                id,
                revision,
                appearance,
                type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
        }

        return null;
    }

    private static bool ImplementsThemeAlgorithm(ITypeSymbol type)
    {
        if (type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == ThemeAlgorithmInterface)
        {
            return true;
        }

        foreach (var interfaceType in type.AllInterfaces)
        {
            if (interfaceType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == ThemeAlgorithmInterface)
            {
                return true;
            }
        }

        return false;
    }
}
