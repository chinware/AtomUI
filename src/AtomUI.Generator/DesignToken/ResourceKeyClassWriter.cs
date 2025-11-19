using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace AtomUI.Generator;

internal class ResourceKeyClassWriter
{
    private readonly SourceProductionContext _context;
    private readonly TokenInfo _tokenInfo;
    private readonly List<string> _usingInfos;

    public ResourceKeyClassWriter(SourceProductionContext context, TokenInfo tokenInfo)
    {
        _context    = context;
        _tokenInfo  = tokenInfo;
        _usingInfos = new ();
        SetupUsingInfos();
    }

    private void SetupUsingInfos()
    {
        _usingInfos.Add("AtomUI.Theme.TokenSystem");
    }

    public void Write()
    {
        var compilationUnitSyntax = BuildCompilationUnitSyntax();
        var sourceText = SourceText.From(
            compilationUnitSyntax.NormalizeWhitespace().ToFullString().Replace("\r\n", "\n"), 
            Encoding.UTF8
        );
        _context.AddSource("TokenResourceConst.g.cs", sourceText);
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

        var argumentTokenList = new List<SyntaxNodeOrToken>();
        argumentTokenList.Add(argument);

        if (!string.IsNullOrEmpty(tokenName.ResourceCatalog))
        {
            var nsArgument = SyntaxFactory.Argument(
                SyntaxFactory.LiteralExpression(
                    SyntaxKind.StringLiteralExpression,
                    SyntaxFactory.Literal($"{tokenName.ResourceCatalog}")));
            argumentTokenList.Add(SyntaxFactory.Token(SyntaxKind.CommaToken));
            argumentTokenList.Add(nsArgument);
        }

        var resourceKeyInstanceExpr = SyntaxFactory.ObjectCreationExpression(resourceKeyType)
            .WithArgumentList(SyntaxFactory.ArgumentList(
                SyntaxFactory.SeparatedList<ArgumentSyntax>(
                    argumentTokenList.ToArray())));

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

    private void AddDesignResourceKeyField(ref ClassDeclarationSyntax classSyntax)
    {
        var resourceKeyFields = new List<MemberDeclarationSyntax>();
        var tokenNames = _tokenInfo.Tokens.ToList().OrderBy(token => token.Name);
        foreach (var tokenName in tokenNames)
        {
            resourceKeyFields.Add(BuildResourceKeyFieldSyntax(tokenName));
        }

        classSyntax = classSyntax.AddMembers(resourceKeyFields.ToArray());
    }

    private ClassDeclarationSyntax BuildControlResourceKeyClassSyntax(ControlTokenInfo controlTokenInfo)
    {
        var className = controlTokenInfo.ControlName;
        var tokenId   = className.Replace("Token", "");
        className += "Key";

        var controlClassSyntax = BuildClassSyntax(className);
        var resourceKeyFields  = new List<MemberDeclarationSyntax>();
        var tokenNames = controlTokenInfo.Tokens.ToList().OrderBy(token => token.Name);
        foreach (var tokenName in tokenNames)
        {
            resourceKeyFields.Add(BuildResourceKeyFieldSyntax(tokenName, $"{controlTokenInfo.ControlNamespace}.{tokenId}.{tokenName.Name}"));
        }

        controlClassSyntax = controlClassSyntax.AddMembers(resourceKeyFields.ToArray());
        return controlClassSyntax;
    }

    private ClassDeclarationSyntax BuildDesignResourceKeyClassSyntax()
    {
        var sharedClassSyntax = BuildClassSyntax("SharedTokenKey");
        // 添加全局的 Token 定义
        AddDesignResourceKeyField(ref sharedClassSyntax);
        return sharedClassSyntax;
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

        var controlTokenInfos = new Dictionary<string, List<ControlTokenInfo>>();

        foreach (var tokenInfo in _tokenInfo.ControlTokenInfos)
        {
            var ns = $"{tokenInfo.ControlNamespace}.DesignTokens";
            if (!controlTokenInfos.TryGetValue(ns, out var tokenInfoList))
            {
                tokenInfoList = new List<ControlTokenInfo>();
                controlTokenInfos.Add(ns, tokenInfoList);
            }
            tokenInfoList.Add(tokenInfo);
        }

        // 添加全局 Design Token
        {
            if (_tokenInfo.Tokens.Count != 0)
            {
                var namespaceSyntax = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName("AtomUI.Theme.Styling"));
                namespaceSyntax = namespaceSyntax.AddMembers(BuildDesignResourceKeyClassSyntax());
                compilationUnit = compilationUnit.AddMembers(namespaceSyntax);
            }
        }
        
        // 添加控件 Design Token
        foreach (var entry in controlTokenInfos)
        {
            if (entry.Value.Count > 0)
            {
                var namespaceSyntax            = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(entry.Key));
                var controlInfoClassSyntaxList = new List<MemberDeclarationSyntax>();
                // 添加控件类成员
                foreach (var controlTokenInfo in entry.Value)
                {
                    if (controlTokenInfo.Tokens.Count > 0)
                    {
                        controlInfoClassSyntaxList.Add(BuildControlResourceKeyClassSyntax(controlTokenInfo));
                    }
                }

                namespaceSyntax = namespaceSyntax.AddMembers(controlInfoClassSyntaxList.ToArray());
                compilationUnit = compilationUnit.AddMembers(namespaceSyntax);
            }
        }
        
        return compilationUnit;
    }
}