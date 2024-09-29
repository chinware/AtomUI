using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AtomUI.Generator;

public class ResourceKeyClassWriter
{
    private readonly SourceProductionContext _context;
    private readonly TokenInfo _tokenInfo;
    private readonly List<string> _usingInfos;

    public ResourceKeyClassWriter(SourceProductionContext context, TokenInfo tokenInfo)
    {
        _context    = context;
        _tokenInfo  = tokenInfo;
        _usingInfos = new List<string>();
        SetupUsingInfos();
    }

    private void SetupUsingInfos()
    {
        _usingInfos.Add("AtomUI.Theme.TokenSystem");
    }

    public void Write()
    {
        var compilationUnitSyntax = BuildCompilationUnitSyntax();
        _context.AddSource("TokenResourceConst.g.cs", compilationUnitSyntax.NormalizeWhitespace().ToFullString());
    }

    private ClassDeclarationSyntax BuildClassSyntax(string className)
    {
        var modifiers = new List<SyntaxToken>
        {
            SyntaxFactory.Token(SyntaxKind.PublicKeyword),
            SyntaxFactory.Token(SyntaxKind.StaticKeyword)
        };
        var classSyntax = SyntaxFactory.ClassDeclaration(className)
                                       .AddModifiers(modifiers.ToArray());
        return classSyntax;
    }

    private FieldDeclarationSyntax BuildResourceKeyFieldSyntax(TokenName tokenName, string? value = null)
    {
        value ??= tokenName.Name;
        var modifiers = new List<SyntaxToken>
        {
            SyntaxFactory.Token(SyntaxKind.PublicKeyword),
            SyntaxFactory.Token(SyntaxKind.StaticKeyword),
            SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword)
        };

        var resourceKeyType = SyntaxFactory.ParseTypeName("TokenResourceKey");
        var argument = SyntaxFactory.Argument(
            SyntaxFactory.LiteralExpression(
                SyntaxKind.StringLiteralExpression,
                SyntaxFactory.Literal($"{value}")));

        var nsArgument = SyntaxFactory.Argument(
            SyntaxFactory.LiteralExpression(
                SyntaxKind.StringLiteralExpression,
                SyntaxFactory.Literal($"{tokenName.ResourceCatalog}")));

        var resourceKeyInstanceExpr = SyntaxFactory.ObjectCreationExpression(resourceKeyType)
                                                   .WithArgumentList(SyntaxFactory.ArgumentList(
                                                       SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                                           new SyntaxNodeOrToken[]
                                                           {
                                                               argument,
                                                               SyntaxFactory.Token(SyntaxKind.CommaToken),
                                                               nsArgument
                                                           })));

        var fieldSyntax = SyntaxFactory.FieldDeclaration(SyntaxFactory.VariableDeclaration(resourceKeyType)
                                                                      .WithVariables(
                                                                          SyntaxFactory.SingletonSeparatedList(
                                                                              SyntaxFactory
                                                                                  .VariableDeclarator(tokenName.Name)
                                                                                  .WithInitializer(
                                                                                      SyntaxFactory.EqualsValueClause(
                                                                                          resourceKeyInstanceExpr)))))
                                       .AddModifiers(modifiers.ToArray());
        return fieldSyntax;
    }

    private void AddGlobalResourceKeyField(ref ClassDeclarationSyntax classSyntax)
    {
        var resourceKeyFields = new List<MemberDeclarationSyntax>();
        foreach (var tokenName in _tokenInfo.Tokens)
        {
            resourceKeyFields.Add(BuildResourceKeyFieldSyntax(tokenName));
        }

        classSyntax = classSyntax.AddMembers(resourceKeyFields.ToArray());
    }

    private ClassDeclarationSyntax BuildControlResourceKeyClassSyntax(ControlTokenInfo controlTokenInfo)
    {
        var className = controlTokenInfo.ControlName;
        var tokenId   = className.Replace("Token", "");
        className = className + "ResourceKey";

        var controlClassSyntax = BuildClassSyntax(className);
        var resourceKeyFields  = new List<MemberDeclarationSyntax>();
        foreach (var tokenName in controlTokenInfo.Tokens)
        {
            resourceKeyFields.Add(BuildResourceKeyFieldSyntax(tokenName, $"{tokenId}.{tokenName.Name}"));
        }

        controlClassSyntax = controlClassSyntax.AddMembers(resourceKeyFields.ToArray());
        return controlClassSyntax;
    }

    private ClassDeclarationSyntax BuildGlobalResourceKeyClassSyntax()
    {
        var globalClassSyntax = BuildClassSyntax("GlobalTokenResourceKey");
        // 添加全局的 Token 定义
        AddGlobalResourceKeyField(ref globalClassSyntax);
        return globalClassSyntax;
    }

    private CompilationUnitSyntax BuildCompilationUnitSyntax()
    {
        var compilationUnit = SyntaxFactory.CompilationUnit();

        var usingSyntaxList = new List<UsingDirectiveSyntax>();

        foreach (var usingInfo in _usingInfos)
        {
            var usingSyntax = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(usingInfo));
            usingSyntaxList.Add(usingSyntax);
        }

        compilationUnit = compilationUnit.AddUsings(usingSyntaxList.ToArray());

        // 添加命名空间
        var namespaceSyntax = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName("AtomUI.Theme.Styling"));
        if (_tokenInfo.Tokens.Count != 0)
        {
            namespaceSyntax = namespaceSyntax.AddMembers(BuildGlobalResourceKeyClassSyntax());
        }

        var controlInfoClassSyntaxList = new List<MemberDeclarationSyntax>();
        // 添加控件类成员
        foreach (var controlTokenInfo in _tokenInfo.ControlTokenInfos)
        {
            if (controlTokenInfo.Tokens.Count > 0)
            {
                controlInfoClassSyntaxList.Add(BuildControlResourceKeyClassSyntax(controlTokenInfo));
            }
        }

        namespaceSyntax = namespaceSyntax.AddMembers(controlInfoClassSyntaxList.ToArray());
        compilationUnit = compilationUnit.AddMembers(namespaceSyntax);

        return compilationUnit;
    }
}