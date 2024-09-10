using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AtomUI.Generator.Language;

public class LangResourceKeyClassSourceWriter
{
    private readonly SourceProductionContext _context;
    private readonly List<LanguageInfo> _languageInfos;
    private readonly List<string> _usingInfos;
    private readonly Dictionary<string, List<LanguageInfo>> _languagesById;

    public LangResourceKeyClassSourceWriter(SourceProductionContext context, List<LanguageInfo> languageInfos)
    {
        _context       = context;
        _languageInfos = languageInfos;
        _languagesById = new Dictionary<string, List<LanguageInfo>>();
        _usingInfos    = new List<string>();
        SetupUsingInfos();
        BuildLanguageKeys();
    }

    private void BuildLanguageKeys()
    {
        foreach (var languageInfo in _languageInfos)
        {
            if (!_languagesById.ContainsKey(languageInfo.LanguageId))
            {
                _languagesById[languageInfo.LanguageId] = new List<LanguageInfo>();
            }

            _languagesById[languageInfo.LanguageId].Add(languageInfo);
        }
    }

    private void SetupUsingInfos()
    {
        _usingInfos.Add("AtomUI.Theme");
    }

    public void Write()
    {
        var compilationUnitSyntax = BuildCompilationUnitSyntax();
        _context.AddSource("LanguageResourceConst.g.cs", compilationUnitSyntax.NormalizeWhitespace().ToFullString());
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

    private FieldDeclarationSyntax BuildResourceKeyFieldSyntax(string catalog, string name, string value)
    {
        var modifiers = new List<SyntaxToken>
        {
            SyntaxFactory.Token(SyntaxKind.PublicKeyword),
            SyntaxFactory.Token(SyntaxKind.StaticKeyword),
            SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword)
        };

        var resourceKeyType = SyntaxFactory.ParseTypeName("LanguageResourceKey");
        var argument = SyntaxFactory.Argument(
            SyntaxFactory.LiteralExpression(
                SyntaxKind.StringLiteralExpression,
                SyntaxFactory.Literal($"{value}")));

        var nsArgument = SyntaxFactory.Argument(
            SyntaxFactory.LiteralExpression(
                SyntaxKind.StringLiteralExpression,
                SyntaxFactory.Literal($"{catalog}")));

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
                                                                              SyntaxFactory.VariableDeclarator(name)
                                                                                  .WithInitializer(
                                                                                      SyntaxFactory.EqualsValueClause(
                                                                                          resourceKeyInstanceExpr)))))
                                       .AddModifiers(modifiers.ToArray());
        return fieldSyntax;
    }

    private NamespaceDeclarationSyntax BuildLanguageResourceKey(List<LanguageInfo> languages)
    {
        var targetNamespace = languages.First().Namespace;
        var catalog         = languages.First().ResourceCatalog;
        var languageId      = languages.First().LanguageId;

        var keys = new HashSet<string>();
        foreach (var languageInfo in languages)
        {
            keys.UnionWith(languageInfo.Items.Keys);
        }

        var namespaceSyntax = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(targetNamespace));

        var className = $"{languageId}LangResourceKey";

        var classSyntax       = BuildClassSyntax(className);
        var resourceKeyFields = new List<MemberDeclarationSyntax>();
        foreach (var key in keys)
        {
            resourceKeyFields.Add(BuildResourceKeyFieldSyntax(catalog, key, $"{languageId}.{key}"));
        }

        classSyntax     = classSyntax.AddMembers(resourceKeyFields.ToArray());
        namespaceSyntax = namespaceSyntax.AddMembers(classSyntax);
        return namespaceSyntax;
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

        foreach (var entry in _languagesById)
        {
            if (entry.Value.Count > 0)
            {
                var namespaceDeclSyntax = BuildLanguageResourceKey(entry.Value);
                compilationUnit = compilationUnit.AddMembers(namespaceDeclSyntax);
            }
        }

        return compilationUnit;
    }
}