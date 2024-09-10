using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AtomUI.Generator;

public class ControlTokenPropertyWalker : CSharpSyntaxWalker
{
    private readonly SemanticModel _semanticModel;

    public ControlTokenPropertyWalker(SemanticModel semanticModel)
    {
        _semanticModel   = semanticModel;
        ControlTokenInfo = new ControlTokenInfo();
    }

    public ControlTokenInfo ControlTokenInfo { get; }
    public string? TokenResourceCatalog { get; set; }

    public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
    {
        ControlTokenInfo.Tokens.Add(new TokenName(node.Identifier.Text, TokenResourceCatalog!));
    }

    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        ControlTokenInfo.ControlName = node.Identifier.Text;
        if (node.Parent is FileScopedNamespaceDeclarationSyntax fileScopedNamespaceDecl)
        {
            ControlTokenInfo.ControlNamespace = fileScopedNamespaceDecl.Name.ToString();
        }
        else if (node.Parent is NamespaceDeclarationSyntax namespaceDecl)
        {
            ControlTokenInfo.ControlNamespace = namespaceDecl.Name.ToString();
        }

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

        base.VisitClassDeclaration(node);
    }
}