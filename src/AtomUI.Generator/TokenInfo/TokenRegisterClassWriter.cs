using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AtomUI.Generator;

public class TokenRegisterClassWriter
{
    private readonly SourceProductionContext _context;
    private readonly IEnumerable<string> _classes;
    private readonly List<string> _usingInfos;

    public TokenRegisterClassWriter(SourceProductionContext context, IEnumerable<string> classes)
    {
        _context    = context;
        _classes    = classes;
        _usingInfos = new List<string>();
        SetupUsingInfos();
    }

    private void SetupUsingInfos()
    {
    }

    public void Write()
    {
        var compilationUnitSyntax = BuildCompilationUnitSyntax();
        _context.AddSource("ControlTokenRegister.g.cs", compilationUnitSyntax.NormalizeWhitespace().ToFullString());
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

        var themeManagerClassDecl = SyntaxFactory.ClassDeclaration("ControlTokenTypeRegister")
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
                SyntaxFactory.ParseExpression($"ThemeManager.Current.RegisterControlTokenType(typeof({className}))");
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