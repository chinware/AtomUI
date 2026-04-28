using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace AtomUI.Generator.Language;

internal class LangResourceKeyClassSourceWriter
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
        _usingInfos.Add("AtomUI.Theme.Language");
    }

    public void Write()
    {
        var compilationUnitSyntax = BuildCompilationUnitSyntax();
        var sourceText = SourceText.From(
            compilationUnitSyntax.NormalizeWhitespace().ToFullString().Replace("\r\n", "\n"), 
            Encoding.UTF8
        );
        _context.AddSource("LanguageResourceConst.g.cs", sourceText);
    }

    private ClassDeclarationSyntax BuildClassSyntax(string className)
    {
        var modifiers = new List<SyntaxToken>
        {
            SyntaxFactory.Token(SyntaxKind.InternalKeyword),
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

    private NamespaceDeclarationSyntax BuildLanguageResourceInfo(List<LanguageInfo> languages)
    {
        var targetNamespace = languages.First().TargetNamespace ?? languages.First().Namespace;
        var languageId      = languages.First().LanguageId;

        var keys = new HashSet<string>();
        foreach (var languageInfo in languages)
        {
            keys.UnionWith(languageInfo.Items.Keys);
        }

        var namespaceSyntax = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(targetNamespace));
        
        var enumName = $"{languageId}LangResourceKind";
        var controlEnumDecl = SyntaxFactory.EnumDeclaration(enumName)
                                           .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
        
        var resourceKindFields = new List<EnumMemberDeclarationSyntax>();
        // 排序，不然生成结果不稳定
        var sortedKeys = keys.ToList().OrderBy(key => key);
        foreach (var key in sortedKeys)
        {
            resourceKindFields.Add(SyntaxFactory.EnumMemberDeclaration(key));
        }
        
        var lanugageMarkupExtensionClass = GenerateLanguageResourceMarkupExtensionClass($"{languageId}LangResourceExtension", enumName);
        
        controlEnumDecl = controlEnumDecl.AddMembers(resourceKindFields.ToArray());
        namespaceSyntax = namespaceSyntax.AddMembers(controlEnumDecl);
        namespaceSyntax = namespaceSyntax.AddMembers(lanugageMarkupExtensionClass);
        
        return namespaceSyntax;
    }
    
       private static ClassDeclarationSyntax GenerateLanguageResourceMarkupExtensionClass(string className, string genericArgType)
    {
        var genericName = SyntaxFactory.GenericName("LanguageResourceExtension")
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

        foreach (var entry in _languagesById.OrderBy(e => e.Key))
        {
            if (entry.Value.Count > 0)
            {
                var namespaceDeclSyntax = BuildLanguageResourceInfo(entry.Value);
                compilationUnit = compilationUnit.AddMembers(namespaceDeclSyntax);
            }
        }

        return compilationUnit;
    }
    
}