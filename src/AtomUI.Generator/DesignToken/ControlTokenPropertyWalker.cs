using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using AtomUI.Generator.Diagnostics;

namespace AtomUI.Generator;

internal class ControlTokenPropertyWalker : CSharpSyntaxWalker
{
    public const string NotTokenDefinitionAttribute = "NotTokenDefinition";
    public const string BaseControlTokenClass = "global::AtomUI.Theme.Tokens.AbstractControlDesignToken";
    public ControlTokenInfo ControlTokenInfo { get; }
    private readonly SemanticModel _semanticModel;
    public string? TokenResourceCatalog { get; set; }

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

            ControlTokenInfo.ResourceCatalog = TokenResourceCatalog;
            ReadControlId(node, classDeclaredSymbol);
        }
        
        if (classDeclaredSymbol is not null)
        {
            AddTokenProperties(classDeclaredSymbol, includeCurrentType: true);
        }
    }

    private void ReadControlId(ClassDeclarationSyntax node, INamedTypeSymbol classSymbol)
    {
        var idMember = classSymbol.GetMembers("ID")
                                  .OfType<IFieldSymbol>()
                                  .FirstOrDefault();
        if (idMember is null)
        {
            ControlTokenInfo.Diagnostics.Add(Diagnostic.Create(
                AtomUIDiagnosticDescriptors.ControlTokenMissingId,
                node.Identifier.GetLocation(),
                classSymbol.Name));
            return;
        }

        if (idMember.DeclaredAccessibility != Accessibility.Public ||
            !idMember.IsConst ||
            idMember.Type.SpecialType != SpecialType.System_String ||
            idMember.ConstantValue is not string id ||
            string.IsNullOrWhiteSpace(id))
        {
            ControlTokenInfo.Diagnostics.Add(Diagnostic.Create(
                AtomUIDiagnosticDescriptors.ControlTokenInvalidId,
                idMember.Locations.FirstOrDefault() ?? node.Identifier.GetLocation(),
                classSymbol.Name));
            return;
        }

        ControlTokenInfo.ControlId = id;
    }
    
    private void AddTokenProperties(ITypeSymbol classSymbol, bool includeCurrentType)
    {
        var current = includeCurrentType ? classSymbol : classSymbol.BaseType;
        while (current != null && current.SpecialType != SpecialType.System_Object)
        {
            var typeName = current.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            if (typeName == BaseControlTokenClass)
            {
                break;
            }

            string? propertyCatalog = null;
            foreach (var attr in current.GetAttributes())
            {
                if (attr.ConstructorArguments.Any() && attr.ConstructorArguments[0].Value is string declaredCatalog)
                {
                    propertyCatalog = declaredCatalog;
                    break;
                }
            }

            foreach (var property in current.GetMembers().OfType<IPropertySymbol>())
            {
                if (property.IsStatic ||
                    property.GetMethod is null ||
                    property.SetMethod is null ||
                    property.DeclaredAccessibility == Accessibility.Private ||
                    HasNotTokenDefinition(property))
                {
                    continue;
                }

                ControlTokenInfo.Tokens.Add(new TokenName(property.Name, propertyCatalog!));
                ControlTokenInfo.SchemaTokens.Add(new SchemaTokenInfo(
                    property.Name,
                    property.Type.ToDisplayString(GeneratorSymbolDisplay.FullyQualifiedType),
                    ControlTokenInfo.GetFullyQualifiedTypeName().StartsWith("global::", StringComparison.Ordinal)
                        ? ControlTokenInfo.GetFullyQualifiedTypeName()
                        : $"global::{ControlTokenInfo.GetFullyQualifiedTypeName()}",
                    SchemaTokenStage.Control));
            }

            current = current.BaseType;
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
