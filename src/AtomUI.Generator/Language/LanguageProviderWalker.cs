using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AtomUI.Generator.Language;

public class LanguageProviderWalker : CSharpSyntaxWalker
{
    public LanguageInfo LanguageInfo { get; }
    private readonly SemanticModel _semanticModel;

    public LanguageProviderWalker(SemanticModel semanticModel)
    {
        _semanticModel = semanticModel;
        LanguageInfo   = new LanguageInfo();
    }

    public override void VisitFieldDeclaration(FieldDeclarationSyntax fieldNode)
    {
        var variableDecl = fieldNode.Declaration.Variables.FirstOrDefault();
        var identifier   = variableDecl?.Identifier;
        if (identifier is not null)
        {
            var initializer = variableDecl?.Initializer;
            var text        = string.Empty;
            if (initializer is not null)
            {
                text = initializer.Value.ToString();
            }

            LanguageInfo.Items.Add(identifier.ToString(), text);
        }
    }

    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        var classDeclaredSymbol = _semanticModel.GetDeclaredSymbol(node);
        if (classDeclaredSymbol is not null)
        {
            foreach (var attribute in classDeclaredSymbol.GetAttributes())
            {
                if (attribute.ConstructorArguments.Any() &&
                    attribute.ConstructorArguments[0].Value is string languageCode)
                {
                    LanguageInfo.LanguageCode = languageCode;
                }

                if (attribute.ConstructorArguments.Any() &&
                    attribute.ConstructorArguments[1].Value is string languageId)
                {
                    LanguageInfo.LanguageId = languageId;
                }

                if (attribute.ConstructorArguments.Any() &&
                    attribute.ConstructorArguments[2].Value is string resourceCatalog)
                {
                    LanguageInfo.ResourceCatalog = resourceCatalog;
                }
            }
        }

        LanguageInfo.ClassName = node.Identifier.ToString();
        var ns = string.Empty;
        if (node.Parent is FileScopedNamespaceDeclarationSyntax fileScopedNamespaceDecl)
        {
            ns = fileScopedNamespaceDecl.Name.ToString();
        }
        else if (node.Parent is NamespaceDeclarationSyntax namespaceDecl)
        {
            ns = namespaceDecl.Name.ToString();
        }

        LanguageInfo.Namespace = ns;

        base.VisitClassDeclaration(node);
    }
}