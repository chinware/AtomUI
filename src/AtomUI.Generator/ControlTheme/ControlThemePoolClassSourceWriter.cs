using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AtomUI.Generator.ControlTheme;

public class ControlThemePoolClassSourceWriter
{
    private readonly SourceProductionContext _context;
    private readonly ImmutableArray<string> _classes;
    private readonly List<string> _usingInfos;

    public ControlThemePoolClassSourceWriter(SourceProductionContext context, ImmutableArray<string> classes)
    {
        _context    = context;
        _classes    = classes.OrderBy(name => name).ToImmutableArray();
        _usingInfos = new List<string>();
        SetupUsingInfos();
    }

    private void SetupUsingInfos()
    {
        _usingInfos.Add("System.Collections.Generic");
        _usingInfos.Add("AtomUI.Theme");
    }

    public void Write()
    {
        var compilationUnitSyntax = BuildCompilationUnitSyntax();
        _context.AddSource("ControlThemePool.g.cs", compilationUnitSyntax.NormalizeWhitespace().ToFullString());
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
        var namespaceSyntax = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName("AtomUI.Theme"));

        var themeManagerClassDecl = SyntaxFactory.ClassDeclaration("ControlThemePool")
                                                 .AddModifiers(SyntaxFactory.Token(SyntaxKind.InternalKeyword))
                                                 .AddMembers(GenerateGetControlThemesMethod());
        namespaceSyntax = namespaceSyntax.AddMembers(themeManagerClassDecl);
        compilationUnit = compilationUnit.AddMembers(namespaceSyntax);

        return compilationUnit;
    }

    private MethodDeclarationSyntax GenerateGetControlThemesMethod()
    {
        var statements = new List<StatementSyntax>
        {
            // var themes = new List<BaseControlTheme>();
            SyntaxFactory.LocalDeclarationStatement(
                SyntaxFactory.VariableDeclaration(
                                 SyntaxFactory.GenericName(SyntaxFactory.Identifier("List"))
                                              .WithTypeArgumentList(SyntaxFactory.TypeArgumentList(
                                                  SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                                      SyntaxFactory.ParseTypeName("BaseControlTheme")))))
                             .WithVariables(SyntaxFactory.SingletonSeparatedList(
                                 SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier("themes"))
                                              .WithInitializer(SyntaxFactory.EqualsValueClause(
                                                  SyntaxFactory.ObjectCreationExpression(
                                                                   SyntaxFactory
                                                                       .GenericName(SyntaxFactory.Identifier("List"))
                                                                       .WithTypeArgumentList(
                                                                           SyntaxFactory.TypeArgumentList(
                                                                               SyntaxFactory
                                                                                   .SingletonSeparatedList<TypeSyntax>(
                                                                                       SyntaxFactory.ParseTypeName(
                                                                                           "BaseControlTheme")))))
                                                               .WithArgumentList(SyntaxFactory.ArgumentList()))))))
        };

        // 动态添加 themes.Add(new XXX());
        foreach (var className in _classes)
        {
            var addStatement = SyntaxFactory.ExpressionStatement(
                SyntaxFactory.InvocationExpression(
                                 SyntaxFactory.MemberAccessExpression(
                                     SyntaxKind.SimpleMemberAccessExpression,
                                     SyntaxFactory.IdentifierName("themes"),
                                     SyntaxFactory.IdentifierName("Add")))
                             .WithArgumentList(SyntaxFactory.ArgumentList(
                                 SyntaxFactory.SingletonSeparatedList(
                                     SyntaxFactory.Argument(
                                         SyntaxFactory.ObjectCreationExpression(
                                                          SyntaxFactory.ParseTypeName(className))
                                                      .WithArgumentList(SyntaxFactory.ArgumentList()))))));

            statements.Add(addStatement);
        }

        // return themes;
        statements.Add(
            SyntaxFactory.ReturnStatement(SyntaxFactory.IdentifierName("themes")));
        
        return SyntaxFactory.MethodDeclaration(
                                SyntaxFactory.GenericName(SyntaxFactory.Identifier("IList"))
                                             .WithTypeArgumentList(SyntaxFactory.TypeArgumentList(
                                                 SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                                     SyntaxFactory.ParseTypeName("BaseControlTheme")))),
                                SyntaxFactory.Identifier("GetControlThemes"))
                            .WithModifiers(SyntaxFactory.TokenList(
                                SyntaxFactory.Token(SyntaxKind.InternalKeyword),
                                SyntaxFactory.Token(SyntaxKind.StaticKeyword)))
                            .WithBody(SyntaxFactory.Block(statements));
    }
}