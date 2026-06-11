using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AtomUI.Generator;

internal class ControlTokenTypePoolClassWriter
{
    private readonly SourceProductionContext _context;
    private readonly IReadOnlyList<string> _classes;
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
        _usingInfos.Add("System.Diagnostics.CodeAnalysis");
        _usingInfos.Add("AtomUI.Theme");
    }

    public void Write()
    {
        var compilationUnitSyntax = BuildCompilationUnitSyntax();
        _context.AddSource("ControlTokenTypePool.g.cs",
            GeneratedSourceText.From(compilationUnitSyntax.NormalizeWhitespace().ToFullString()));
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
        var statements = new List<StatementSyntax>
        {
            SyntaxFactory.ParseStatement($"List<ControlTokenRegistration> tokenTypes = new List<ControlTokenRegistration>({_classes.Count});")
        };

        // 动态添加 themes.Add(typeof(XXX));
        foreach (var className in _classes)
        {
            var addStatement = SyntaxFactory.ParseStatement(
                $"tokenTypes.Add(new ControlTokenRegistration(typeof({className})));");

            statements.Add(addStatement);
        }

        // return themes;
        statements.Add(
            SyntaxFactory.ReturnStatement(SyntaxFactory.IdentifierName("tokenTypes")));
        
        var methodDecl = SyntaxFactory.MethodDeclaration(
                                             SyntaxFactory.GenericName(SyntaxFactory.Identifier("IList"))
                                                          .WithTypeArgumentList(SyntaxFactory.TypeArgumentList(
                                                              SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                                                  SyntaxFactory.ParseTypeName("ControlTokenRegistration")))),
                                             SyntaxFactory.Identifier("GetTokenTypes"))
                                         .WithModifiers(SyntaxFactory.TokenList(
                                             SyntaxFactory.Token(SyntaxKind.InternalKeyword),
                                             SyntaxFactory.Token(SyntaxKind.StaticKeyword)))
                                         .WithBody(SyntaxFactory.Block(statements));

        foreach (var className in _classes)
        {
            methodDecl = methodDecl.AddAttributeLists(GenerateDynamicDependencyAttributeList(className));
        }

        return methodDecl;
    }

    private static AttributeListSyntax GenerateDynamicDependencyAttributeList(string className)
    {
        var attribute = SyntaxFactory.Attribute(SyntaxFactory.IdentifierName("DynamicDependency"))
                                     .WithArgumentList(SyntaxFactory.AttributeArgumentList(
                                         SyntaxFactory.SeparatedList<AttributeArgumentSyntax>(
                                             new SyntaxNodeOrToken[]
                                             {
                                                 SyntaxFactory.AttributeArgument(SyntaxFactory.ParseExpression(
                                                     "DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties")),
                                                 SyntaxFactory.Token(SyntaxKind.CommaToken),
                                                 SyntaxFactory.AttributeArgument(SyntaxFactory.TypeOfExpression(
                                                     SyntaxFactory.ParseTypeName(className)))
                                             })));

        return SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(attribute));
    }
}
