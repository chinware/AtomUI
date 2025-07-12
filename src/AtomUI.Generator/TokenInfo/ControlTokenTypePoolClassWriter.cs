using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace AtomUI.Generator;

public class ControlTokenTypePoolClassWriter
{
    private readonly SourceProductionContext _context;
    private readonly IEnumerable<string> _classes;
    private readonly List<string> _usingInfos;

    public ControlTokenTypePoolClassWriter(SourceProductionContext context, IEnumerable<string> classes)
    {
        _context    = context;
        _classes    = classes.OrderBy(className => className).ToList();
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
        var sourceText = SourceText.From(
            compilationUnitSyntax.NormalizeWhitespace().ToFullString().Replace("\r\n", "\n"), 
            Encoding.UTF8
        );
        _context.AddSource("ControlTokenTypePool.g.cs", sourceText);
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

        var themeManagerClassDecl = SyntaxFactory.ClassDeclaration("ControlTokenTypePool")
                                                 .AddModifiers(SyntaxFactory.Token(SyntaxKind.InternalKeyword))
                                                 .AddMembers(GenerateGetControlTokenTypesMethod());
        namespaceSyntax = namespaceSyntax.AddMembers(themeManagerClassDecl);
        compilationUnit = compilationUnit.AddMembers(namespaceSyntax);

        return compilationUnit;
    }

    private MethodDeclarationSyntax GenerateGetControlTokenTypesMethod()
    {
        List<ExpressionStatementSyntax> objectCreateStmts = new();
        foreach (var className in _classes)
        {
            var registerExprStmt =
                SyntaxFactory.ParseExpression($"ThemeManager.Current.RegisterControlTokenType(typeof({className}))");
            var statement = SyntaxFactory.ExpressionStatement(registerExprStmt);
            objectCreateStmts.Add(statement);
        }

        var statements = new List<StatementSyntax>
        {
            // var themes = new List<BaseControlTheme>();
            SyntaxFactory.LocalDeclarationStatement(
                SyntaxFactory.VariableDeclaration(
                                 SyntaxFactory.GenericName(SyntaxFactory.Identifier("List"))
                                              .WithTypeArgumentList(SyntaxFactory.TypeArgumentList(
                                                  SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                                      SyntaxFactory.ParseTypeName("Type")))))
                             .WithVariables(SyntaxFactory.SingletonSeparatedList(
                                 SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier("tokenTypes"))
                                              .WithInitializer(SyntaxFactory.EqualsValueClause(
                                                  SyntaxFactory.ObjectCreationExpression(
                                                                   SyntaxFactory
                                                                       .GenericName(SyntaxFactory.Identifier("List"))
                                                                       .WithTypeArgumentList(
                                                                           SyntaxFactory.TypeArgumentList(
                                                                               SyntaxFactory
                                                                                   .SingletonSeparatedList<TypeSyntax>(
                                                                                       SyntaxFactory.ParseTypeName(
                                                                                           "Type")))))
                                                               .WithArgumentList(SyntaxFactory.ArgumentList()))))))
        };

        // 动态添加 themes.Add(typeof(XXX));
        foreach (var className in _classes)
        {
            var addStatement = SyntaxFactory.ExpressionStatement(
                SyntaxFactory.InvocationExpression(
                                 SyntaxFactory.MemberAccessExpression(
                                     SyntaxKind.SimpleMemberAccessExpression,
                                     SyntaxFactory.IdentifierName("tokenTypes"),
                                     SyntaxFactory.IdentifierName("Add")))
                             .WithArgumentList(SyntaxFactory.ArgumentList(
                                 SyntaxFactory.SingletonSeparatedList(
                                     SyntaxFactory.Argument(
                                         SyntaxFactory.TypeOfExpression(SyntaxFactory.ParseTypeName(className)))))));

            statements.Add(addStatement);
        }

        // return themes;
        statements.Add(
            SyntaxFactory.ReturnStatement(SyntaxFactory.IdentifierName("tokenTypes")));
        
        return SyntaxFactory.MethodDeclaration(
                                SyntaxFactory.GenericName(SyntaxFactory.Identifier("IList"))
                                             .WithTypeArgumentList(SyntaxFactory.TypeArgumentList(
                                                 SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                                     SyntaxFactory.ParseTypeName("Type")))),
                                SyntaxFactory.Identifier("GetTokenTypes"))
                            .WithModifiers(SyntaxFactory.TokenList(
                                SyntaxFactory.Token(SyntaxKind.InternalKeyword),
                                SyntaxFactory.Token(SyntaxKind.StaticKeyword)))
                            .WithBody(SyntaxFactory.Block(statements));
    }
}