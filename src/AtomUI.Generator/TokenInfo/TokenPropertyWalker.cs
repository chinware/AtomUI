using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AtomUI.Generator;

public class TokenPropertyWalker : CSharpSyntaxWalker
{
    public const string NotTokenDefinitionAttribute = "NotTokenDefinition";
    
    public HashSet<string> TokenNames { get; }
    public string? TokenResourceCatalog { get; set; }
    private readonly SemanticModel _semanticModel;

    public TokenPropertyWalker(SemanticModel semanticModel)
    {
        TokenNames     = new HashSet<string>();
        _semanticModel = semanticModel;
    }

    public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
    {
        var isSkip = node.AttributeLists
                         .SelectMany(attrList => attrList.Attributes)
                         .Any(attr => attr.Name.ToString() == NotTokenDefinitionAttribute);

        if (!isSkip)
        {
            TokenNames.Add(node.Identifier.Text);
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
}