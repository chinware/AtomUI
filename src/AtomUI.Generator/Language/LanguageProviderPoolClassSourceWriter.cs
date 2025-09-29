using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace AtomUI.Generator.Language;

internal class LanguageProviderPoolClassSourceWriter
{
    private readonly SourceProductionContext _context;
    private readonly List<LanguageInfo> _languageInfos;
    private readonly List<string> _usingInfos;

    public LanguageProviderPoolClassSourceWriter(SourceProductionContext context, List<LanguageInfo> classes)
    {
        _context       = context;
        _languageInfos = classes.OrderBy(info => info.Namespace).ThenBy(info => info.ClassName).ToList();
        _usingInfos    = new List<string>();
        SetupUsingInfos();
    }

    private void SetupUsingInfos()
    {
        _usingInfos.Add("System.Collections.Generic");
        _usingInfos.Add("AtomUI.Theme");
        _usingInfos.Add("AtomUI.Theme.Language");
    }

    public void Write()
    {
        var compilationUnitSyntax = BuildCompilationUnitSyntax();
        var sourceText = SourceText.From(
            compilationUnitSyntax.NormalizeWhitespace().ToFullString().Replace("\r\n", "\n"), 
            Encoding.UTF8
        );
        _context.AddSource("LanguageProviderPool.g.cs", sourceText);
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
        var namespaceSyntax = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName("AtomUI.Theme.Language"));

        var languageProviderPoolClassDecl = SyntaxFactory.ClassDeclaration("LanguageProviderPool")
                                                         .AddModifiers(SyntaxFactory.Token(SyntaxKind.InternalKeyword))
                                                         .AddMembers(GenerateGetLanguageProvidersMethod());
        namespaceSyntax = namespaceSyntax.AddMembers(languageProviderPoolClassDecl);
        compilationUnit = compilationUnit.AddMembers(namespaceSyntax);

        return compilationUnit;
    }

    private MethodDeclarationSyntax GenerateGetLanguageProvidersMethod()
    {
        var statements = new List<StatementSyntax>
        {
            // var themes = new List<BaseControlTheme>();
            SyntaxFactory.LocalDeclarationStatement(
                SyntaxFactory.VariableDeclaration(
                                 SyntaxFactory.GenericName(SyntaxFactory.Identifier("List"))
                                              .WithTypeArgumentList(SyntaxFactory.TypeArgumentList(
                                                  SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                                      SyntaxFactory.ParseTypeName("LanguageProvider")))))
                             .WithVariables(SyntaxFactory.SingletonSeparatedList(
                                 SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier("languageProviders"))
                                              .WithInitializer(SyntaxFactory.EqualsValueClause(
                                                  SyntaxFactory.ObjectCreationExpression(
                                                                   SyntaxFactory
                                                                       .GenericName(SyntaxFactory.Identifier("List"))
                                                                       .WithTypeArgumentList(
                                                                           SyntaxFactory.TypeArgumentList(
                                                                               SyntaxFactory
                                                                                   .SingletonSeparatedList<TypeSyntax>(
                                                                                       SyntaxFactory.ParseTypeName(
                                                                                           "LanguageProvider")))))
                                                               .WithArgumentList(SyntaxFactory.ArgumentList()))))))
        };

        // 动态添加 themes.Add(new XXX());
        foreach (var languageInfo in _languageInfos)
        {
            var addStatement = SyntaxFactory.ExpressionStatement(
                SyntaxFactory.InvocationExpression(
                                 SyntaxFactory.MemberAccessExpression(
                                     SyntaxKind.SimpleMemberAccessExpression,
                                     SyntaxFactory.IdentifierName("languageProviders"),
                                     SyntaxFactory.IdentifierName("Add")))
                             .WithArgumentList(SyntaxFactory.ArgumentList(
                                 SyntaxFactory.SingletonSeparatedList(
                                     SyntaxFactory.Argument(
                                         SyntaxFactory.ObjectCreationExpression(
                                                          SyntaxFactory.ParseTypeName($"{languageInfo.Namespace}.{languageInfo.ClassName}"))
                                                      .WithArgumentList(SyntaxFactory.ArgumentList()))))));

            statements.Add(addStatement);
        }

        // return themes;
        statements.Add(
            SyntaxFactory.ReturnStatement(SyntaxFactory.IdentifierName("languageProviders")));
        
        return SyntaxFactory.MethodDeclaration(
                                SyntaxFactory.GenericName(SyntaxFactory.Identifier("IList"))
                                             .WithTypeArgumentList(SyntaxFactory.TypeArgumentList(
                                                 SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                                     SyntaxFactory.ParseTypeName("LanguageProvider")))),
                                SyntaxFactory.Identifier("GetLanguageProviders"))
                            .WithModifiers(SyntaxFactory.TokenList(
                                SyntaxFactory.Token(SyntaxKind.InternalKeyword),
                                SyntaxFactory.Token(SyntaxKind.StaticKeyword)))
                            .WithBody(SyntaxFactory.Block(statements));
    }
}