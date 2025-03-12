using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AtomUI.Generator.ControlTheme;

public class ControlThemeRegisterClassSourceWriter
{
    private readonly SourceProductionContext _context;
    private readonly ImmutableArray<string> _classes;
    private readonly List<string> _usingInfos;

    public ControlThemeRegisterClassSourceWriter(SourceProductionContext context, ImmutableArray<string> classes)
    {
        _context    = context;
        _classes    = classes.OrderBy(name => name).ToImmutableArray();
        _usingInfos = new List<string>();
        SetupUsingInfos();
    }

    private void SetupUsingInfos()
    {
    }

    public void Write()
    {
        var compilationUnitSyntax = BuildCompilationUnitSyntax();
        _context.AddSource("ControlThemeRegister.g.cs", compilationUnitSyntax.NormalizeWhitespace().ToFullString());
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

        var themeManagerClassDecl = SyntaxFactory.ClassDeclaration("ControlThemeRegister")
                                                 .AddModifiers(SyntaxFactory.Token(SyntaxKind.InternalKeyword))
                                                 .AddMembers(GenerateControlThemeCreateMethod());
        namespaceSyntax = namespaceSyntax.AddMembers(themeManagerClassDecl);
        compilationUnit = compilationUnit.AddMembers(namespaceSyntax);

        return compilationUnit;
    }

    private MethodDeclarationSyntax GenerateControlThemeCreateMethod()
    {
        List<ExpressionStatementSyntax> objectCreateStmts = new();
        foreach (var className in _classes)
        {
            var registerExprStmt =
                SyntaxFactory.ParseExpression($"ThemeManager.Current.RegisterControlTheme(new {className}())");
            var statement = SyntaxFactory.ExpressionStatement(registerExprStmt);
            objectCreateStmts.Add(statement);
        }

        return SyntaxFactory
               .MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)), "Register")
               .AddModifiers(SyntaxFactory.Token(SyntaxKind.InternalKeyword),
                   SyntaxFactory.Token(SyntaxKind.StaticKeyword))
               .WithBody(SyntaxFactory.Block(objectCreateStmts));
    }
}