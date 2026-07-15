using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using AtomUI.Generator.Diagnostics;

namespace AtomUI.Generator;

internal class ControlTokenPropertyWalker : CSharpSyntaxWalker
{
    public const string NotTokenDefinitionAttribute = "NotTokenDefinition";
    public const string BaseControlTokenClass = "global::AtomUI.Theme.TokenSystem.AbstractControlDesignToken";
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
        // 检查是否有 getter 和 setter
        bool hasGetter = node.AccessorList?.Accessors.Any(a => a.Kind() == SyntaxKind.GetAccessorDeclaration) ?? false;
        bool hasSetter = node.AccessorList?.Accessors.Any(a => a.Kind() == SyntaxKind.SetAccessorDeclaration) ?? false;
        var isSkip = !hasGetter || !hasSetter ||
                     node.AttributeLists
                         .SelectMany(attrList => attrList.Attributes)
                         .Any(attr => attr.Name.ToString() == NotTokenDefinitionAttribute);
        if (!isSkip)
        {
            ControlTokenInfo.Tokens.Add(new TokenName(node.Identifier.Text, TokenResourceCatalog!));
        }
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
            AddBaseClassProperties(classDeclaredSymbol);
        }

        base.VisitClassDeclaration(node);
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
    
    /// <summary>
    /// 遍历基类（包括多层继承）中所有可继承的属性（public/protected/internal），
    /// 排除标记了 NotTokenDefinition 的属性，并添加到 Token 列表中。
    /// </summary>
    private void AddBaseClassProperties(ITypeSymbol classSymbol)
    {
        var baseType = classSymbol.BaseType;
        while (baseType != null && baseType.SpecialType != SpecialType.System_Object)
        {
            var baseTypeFullName = baseType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            if (baseTypeFullName == BaseControlTokenClass)
            {
                break;
            }
            // 获取该基类上的 TokenResourceCatalog（如果存在）
            string? baseCatalog = null;
            foreach (var attr in baseType.GetAttributes())
            {
                if (attr.ConstructorArguments.Any() && attr.ConstructorArguments[0].Value is string catalog)
                {
                    baseCatalog = catalog;
                    break;
                }
            }

            // 遍历基类中所有属性成员
            foreach (var member in baseType.GetMembers())
            {
                if (member is IPropertySymbol property &&
                    !property.IsStatic &&
                    property.GetMethod != null &&   // 必须有 getter
                    property.SetMethod != null &&
                    property.DeclaredAccessibility != Accessibility.Private) // 排除私有属性
                {
                    // 检查是否标记了 NotTokenDefinition
                    bool hasNotTokenDef = false;
                    foreach (var attr in property.GetAttributes())
                    {
                        // 比较特性名称（可以是简单名称或完整名称）
                        var attrName = attr.AttributeClass?.Name;
                        if (attrName == NotTokenDefinitionAttribute ||
                            attr.AttributeClass?.ToDisplayString() == NotTokenDefinitionAttribute)
                        {
                            hasNotTokenDef = true;
                            break;
                        }
                    }
                    if (!hasNotTokenDef)
                    {
                        // 添加到 Token 列表（基类的属性使用基类的 catalog）
                        ControlTokenInfo.Tokens.Add(new TokenName(property.Name, baseCatalog!));
                    }
                }
            }

            baseType = baseType.BaseType;
        }
    }
}
