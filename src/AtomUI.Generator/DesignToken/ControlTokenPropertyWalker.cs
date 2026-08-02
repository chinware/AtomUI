using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using AtomUI.Generator.Diagnostics;

namespace AtomUI.Generator;

internal class ControlTokenPropertyWalker : CSharpSyntaxWalker
{
    public const string NotTokenDefinitionAttribute = "NotTokenDefinition";
    public const string BaseControlTokenClass = "global::AtomUI.Theme.DesignTokens.AbstractControlDesignToken";
    public ControlTokenInfo ControlTokenInfo { get; }
    private readonly SemanticModel _semanticModel;
    public ControlTokenPropertyWalker(SemanticModel semanticModel)
    {
        _semanticModel   = semanticModel;
        ControlTokenInfo = new ControlTokenInfo();
    }

    public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
    {
        // Properties are collected from symbols in VisitClassDeclaration so inherited
        // Token definitions and their concrete types use one deterministic path.
    }

    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        ControlTokenInfo.TokenName = node.Identifier.Text;
        ControlTokenInfo.DeclarationLocation = node.Identifier.GetLocation();
        if (node.Parent is FileScopedNamespaceDeclarationSyntax fileScopedNamespaceDecl)
        {
            ControlTokenInfo.TokenNamespace = fileScopedNamespaceDecl.Name.ToString();
        }
        else if (node.Parent is NamespaceDeclarationSyntax namespaceDecl)
        {
            ControlTokenInfo.TokenNamespace = namespaceDecl.Name.ToString();
        }

        var classDeclaredSymbol = _semanticModel.GetDeclaredSymbol(node);
        if (classDeclaredSymbol is not null)
        {
            ReadControlIdentity(node, classDeclaredSymbol);
            ValidateInheritance(node, classDeclaredSymbol);
            AddTokenProperties(classDeclaredSymbol);
        }

        base.VisitClassDeclaration(node);
    }

    private void ReadControlIdentity(ClassDeclarationSyntax node, INamedTypeSymbol classSymbol)
    {
        const string suffix = "Token";
        if (!classSymbol.Name.EndsWith(suffix, StringComparison.Ordinal) ||
            classSymbol.Name.Length == suffix.Length)
        {
            ControlTokenInfo.Diagnostics.Add(Diagnostic.Create(
                AtomUIDiagnosticDescriptors.ControlTokenInvalidName,
                node.Identifier.GetLocation(),
                classSymbol.Name));
            return;
        }

        ControlTokenInfo.ControlName = classSymbol.Name.Substring(0, classSymbol.Name.Length - suffix.Length);
    }

    private void ValidateInheritance(ClassDeclarationSyntax node, INamedTypeSymbol classSymbol)
    {
        if (classSymbol.BaseType is not { } baseType)
        {
            return;
        }

        var baseTypeName = baseType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        if (!string.Equals(baseTypeName, BaseControlTokenClass, StringComparison.Ordinal))
        {
            ControlTokenInfo.Diagnostics.Add(Diagnostic.Create(
                AtomUIDiagnosticDescriptors.ControlTokenInheritance,
                node.Identifier.GetLocation(),
                classSymbol.Name,
                baseType.Name));
        }
    }
    
    private void AddTokenProperties(ITypeSymbol classSymbol)
    {
        foreach (var property in classSymbol.GetMembers().OfType<IPropertySymbol>())
        {
            if (property.IsStatic ||
                property.GetMethod is null ||
                property.SetMethod is null ||
                property.DeclaredAccessibility == Accessibility.Private ||
                HasNotTokenDefinition(property))
            {
                continue;
            }

            ControlTokenInfo.Tokens.Add(new TokenName(property.Name, string.Empty));
            ControlTokenInfo.SchemaTokens.Add(new SchemaTokenInfo(
                property.Name,
                property.Type.ToDisplayString(GeneratorSymbolDisplay.FullyQualifiedType),
                ControlTokenInfo.GetFullyQualifiedTokenTypeName().StartsWith("global::", StringComparison.Ordinal)
                    ? ControlTokenInfo.GetFullyQualifiedTokenTypeName()
                    : $"global::{ControlTokenInfo.GetFullyQualifiedTokenTypeName()}",
                SchemaTokenStage.Control));
        }
    }

    private static bool HasNotTokenDefinition(IPropertySymbol property)
    {
        foreach (var attribute in property.GetAttributes())
        {
            if (attribute.AttributeClass?.ToDisplayString() == TargetMarkConstants.NotTokenDefinitionAttribute)
            {
                return true;
            }
        }

        return false;
    }

}
