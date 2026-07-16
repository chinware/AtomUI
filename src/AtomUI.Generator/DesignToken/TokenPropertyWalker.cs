using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AtomUI.Generator;

internal class TokenPropertyWalker : CSharpSyntaxWalker
{
    public HashSet<string> TokenNames { get; }
    public HashSet<SchemaTokenInfo> SchemaTokens { get; }
    public string? TokenResourceCatalog { get; set; }
    private readonly SemanticModel _semanticModel;

    public TokenPropertyWalker(SemanticModel semanticModel)
    {
        TokenNames     = new HashSet<string>();
        SchemaTokens   = new HashSet<SchemaTokenInfo>();
        _semanticModel = semanticModel;
    }

    public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
    {
        var property = _semanticModel.GetDeclaredSymbol(node);
        if (property is null)
        {
            return;
        }

        var isSkip = HasAttribute(property, TargetMarkConstants.NotTokenDefinitionAttribute);

        if (!isSkip)
        {
            TokenNames.Add(node.Identifier.Text);
            if (TryGetStage(property, out var stage))
            {
                SchemaTokens.Add(new SchemaTokenInfo(
                    property.Name,
                    property.Type.ToDisplayString(GeneratorSymbolDisplay.FullyQualifiedType),
                    property.ContainingType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                    stage));
            }
        }
    }

    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        base.VisitClassDeclaration(node);
        var classDeclaredSymbol = _semanticModel.GetDeclaredSymbol(node);
        if (classDeclaredSymbol is not null)
        {
            foreach (var attribute in classDeclaredSymbol.GetAttributes())
            {
                if (attribute.ConstructorArguments.Any() && attribute.ConstructorArguments[0].Value is string catalog)
                {
                    TokenResourceCatalog = catalog;
                }
            }
        }
    }

    private static bool TryGetStage(IPropertySymbol property, out SchemaTokenStage stage)
    {
        foreach (var attribute in property.GetAttributes())
        {
            if (attribute.AttributeClass?.ToDisplayString() != TargetMarkConstants.DesignTokenKindAttribute ||
                attribute.ConstructorArguments.Length != 1 ||
                attribute.ConstructorArguments[0].Value is not int value)
            {
                continue;
            }

            stage = value switch
            {
                0 => SchemaTokenStage.Seed,
                1 => SchemaTokenStage.Map,
                2 => SchemaTokenStage.Alias,
                _ => default
            };
            return value is >= 0 and <= 2;
        }

        stage = default;
        return false;
    }

    private static bool HasAttribute(ISymbol symbol, string metadataName)
    {
        foreach (var attribute in symbol.GetAttributes())
        {
            if (attribute.AttributeClass?.ToDisplayString() == metadataName)
            {
                return true;
            }
        }

        return false;
    }
}
