using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AtomUI.Generator.Language;

internal class LanguageProviderWalker : CSharpSyntaxWalker
{
    public LanguageInfo LanguageInfo { get; }
    private readonly SemanticModel _semanticModel;

    public LanguageProviderWalker(SemanticModel semanticModel)
    {
        _semanticModel = semanticModel;
        LanguageInfo   = new LanguageInfo();
        ExtractLanguageMetaInfo();
    }
    
    private void ExtractLanguageMetaInfo()
    {
        // 获取编译对象中的程序集符号
        var assemblySymbol = _semanticModel.Compilation.Assembly;

        // 获取程序集上所有自定义特性
        foreach (var attributeData in assemblySymbol.GetAttributes())
        {
            // 检查特性类型
            var attributeClass = attributeData.AttributeClass;
            if (attributeClass?.Name == TargetMarkConstants.LanguageSgMetaInfoAttribute ||
                attributeClass?.ToDisplayString() == TargetMarkConstants.LanguageSgMetaInfoAttribute)
            {
                // 提取构造函数参数（位置参数）
                if (attributeData.ConstructorArguments.Length > 0)
                {
                    var arg = attributeData.ConstructorArguments[0];
                    if (arg.Value is string targetNamespace)
                    {
                        LanguageInfo.TargetNamespace = targetNamespace;
                    }
                }

                // 提取命名参数 TargetNamespace（如果有，会覆盖构造函数参数）
                foreach (var namedArg in attributeData.NamedArguments)
                {
                    if (namedArg.Key == "TargetNamespace" && namedArg.Value.Value is string ns)
                    {
                        LanguageInfo.TargetNamespace = ns;
                        break;
                    }
                }

                // 假设该特性只出现一次，找到后退出
                break;
            }
        }
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

            LanguageInfo.Items.Add(identifier.ToString()!, text);
        }
    }

    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        var classDeclaredSymbol = _semanticModel.GetDeclaredSymbol(node);
        if (classDeclaredSymbol is not null)
        {
            LanguageInfo.Accessibility = GetAccessibility(classDeclaredSymbol.DeclaredAccessibility);

            foreach (var attribute in classDeclaredSymbol.GetAttributes())
            {
                if (!IsLanguageProviderAttribute(attribute))
                {
                    continue;
                }

                if (attribute.ConstructorArguments.Length > 0)
                {
                    LanguageInfo.LanguageCode = GetLanguageCode(attribute.ConstructorArguments[0]) ?? string.Empty;
                }

                if (attribute.ConstructorArguments.Length > 1 &&
                    attribute.ConstructorArguments[1].Value is string languageId)
                {
                    LanguageInfo.LanguageId = languageId;
                }
                else
                {
                    LanguageInfo.LanguageId = "Default";
                }

                break;
            }
        }

        LanguageInfo.ClassName = node.Identifier.ToString();
        LanguageInfo.IsPartial = node.Modifiers.Any(SyntaxKind.PartialKeyword);
        LanguageInfo.HasParameterlessConstructor = node.Members
                                                      .OfType<ConstructorDeclarationSyntax>()
                                                      .Any(ctor => ctor.ParameterList.Parameters.Count == 0);
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

    private static bool IsLanguageProviderAttribute(AttributeData attribute)
    {
        var attributeClass = attribute.AttributeClass;
        if (attributeClass is null)
        {
            return false;
        }

        return attributeClass.Name == "LanguageProviderAttribute" ||
               attributeClass.ToDisplayString() == TargetMarkConstants.LanguageProviderAttribute;
    }

    private static string? GetLanguageCode(TypedConstant argument)
    {
        if (argument.Kind == TypedConstantKind.Enum &&
            argument.Type is INamedTypeSymbol enumType)
        {
            foreach (var member in enumType.GetMembers().OfType<IFieldSymbol>())
            {
                if (member.HasConstantValue &&
                    Equals(member.ConstantValue, argument.Value))
                {
                    return member.Name;
                }
            }
        }

        return argument.Value as string;
    }

    private static string GetAccessibility(Accessibility accessibility)
    {
        return accessibility switch
        {
            Accessibility.Public => "public",
            Accessibility.Internal => "internal",
            _ => "internal"
        };
    }
}
