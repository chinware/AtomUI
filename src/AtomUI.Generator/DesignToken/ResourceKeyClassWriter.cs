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
        _usingInfos.Add("AtomUI.Theme");
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
    
    private EnumDeclarationSyntax BuildControlResourceKeyEnumSyntax(ControlTokenInfo controlTokenInfo)
    {
        var enumName = $"{controlTokenInfo.ControlName}Kind";
        var controlEnumDecl = SyntaxFactory.EnumDeclaration(enumName)
                                           .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
        var tokenNames = controlTokenInfo.Tokens.ToList().OrderBy(token => token.Name);
        var enumMembers = new List<EnumMemberDeclarationSyntax>();
        foreach (var tokenName in tokenNames)
        {
            enumMembers.Add(SyntaxFactory.EnumMemberDeclaration(tokenName.Name));
        }
        controlEnumDecl = controlEnumDecl.AddMembers(enumMembers.ToArray());
        return controlEnumDecl;
    }
    
    private EnumDeclarationSyntax BuildDesignResourceKeyEnumSyntax()
    {
        var controlEnumDecl = SyntaxFactory.EnumDeclaration("SharedTokenKind")
                                           .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
        var enumMembers = new List<EnumMemberDeclarationSyntax>();
        var tokenNames  = _tokenInfo.Tokens.ToList().OrderBy(token => token.Name);
        foreach (var tokenName in tokenNames)
        {
            enumMembers.Add(SyntaxFactory.EnumMemberDeclaration(tokenName.Name));
        }
        controlEnumDecl = controlEnumDecl.AddMembers(enumMembers.ToArray());
        return controlEnumDecl;
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
                namespaceSyntax = namespaceSyntax.AddMembers(BuildDesignResourceKeyEnumSyntax());
                compilationUnit = compilationUnit.AddMembers(namespaceSyntax);
            }
        }
        
        // 添加控件 Design Token
        foreach (var entry in controlTokenInfos.OrderBy(e => e.Key))
        {
            if (entry.Value.Count > 0)
            {
                var namespaceSyntax            = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(entry.Key));
                var controlTokenKindSyntaxList = new List<MemberDeclarationSyntax>();
                var controlTokenMarkupExtensionSyntaxList = new List<MemberDeclarationSyntax>();
                // 添加控件类成员
                foreach (var controlTokenInfo in entry.Value.OrderBy(info => info.ControlName))
                {
                    if (controlTokenInfo.Tokens.Count > 0)
                    {
                        controlTokenKindSyntaxList.Add(BuildControlResourceKeyEnumSyntax(controlTokenInfo));
                        controlTokenMarkupExtensionSyntaxList.Add(GenerateTokenResourceMarkupExtensionClass(controlTokenInfo));
                    }
                }
                
                namespaceSyntax = namespaceSyntax.AddMembers(controlTokenKindSyntaxList.ToArray());
                namespaceSyntax = namespaceSyntax.AddMembers(controlTokenMarkupExtensionSyntaxList.ToArray());
                compilationUnit = compilationUnit.AddMembers(namespaceSyntax);
            }
        }
        
        return compilationUnit;
    }

    private static ClassDeclarationSyntax GenerateTokenResourceMarkupExtensionClass(ControlTokenInfo controlTokenInfo)
    {
        var className = $"{controlTokenInfo.ControlName}ResourceExtension";
        var tokenKindType = $"{controlTokenInfo.ControlName}Kind";
        return GenerateTokenResourceMarkupExtensionClass(className, tokenKindType);
    }
    
    private static ClassDeclarationSyntax GenerateTokenResourceMarkupExtensionClass(string className, string genericArgType)
    {
        var genericName = SyntaxFactory.GenericName("TokenResourceExtension")
                                       .WithTypeArgumentList(
                                           SyntaxFactory.TypeArgumentList(
                                               SyntaxFactory.SingletonSeparatedList(
                                                   SyntaxFactory.ParseTypeName(genericArgType))));
        
        var ctor1 = SyntaxFactory.ConstructorDeclaration(className)
                                 .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                                 .WithBody(SyntaxFactory.Block());
        
        var parameter = SyntaxFactory.Parameter(SyntaxFactory.Identifier("kind"))
                                     .WithType(SyntaxFactory.ParseTypeName(genericArgType));
    
        var baseConstructorCall = SyntaxFactory.ConstructorInitializer(SyntaxKind.BaseConstructorInitializer)
                                               .AddArgumentListArguments(
                                                   SyntaxFactory.Argument(SyntaxFactory.IdentifierName("kind")));
    
        var ctor2 = SyntaxFactory.ConstructorDeclaration(className)
                                 .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                                 .AddParameterListParameters(parameter)
                                 .WithInitializer(baseConstructorCall)
                                 .WithBody(SyntaxFactory.Block());
        
        var classDeclaration = SyntaxFactory.ClassDeclaration(className)
                                            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                                            .AddBaseListTypes(SyntaxFactory.SimpleBaseType(genericName))
                                            .AddMembers(ctor1, ctor2)
                                            .NormalizeWhitespace();

        return classDeclaration;
    }
}