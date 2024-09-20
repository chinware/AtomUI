using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AtomUI.Generator.MergedTokenInfo;

public class MergedGlobalTokenClassWriter
{
    private readonly SourceProductionContext _context;
    private readonly List<string> _usingInfos;
    private readonly List<TokenPropertyInfo> _tokenPropertyInfos;
    private const string MergedGlobalTokenClassName = "MergedGlobalToken";
    private Dictionary<string, int> _priorityMap;
    private const int DefaultPriority = 10000;
    
    public MergedGlobalTokenClassWriter(SourceProductionContext context, List<TokenPropertyInfo> tokenPropertyInfos)
    {
        _context            = context;
        _tokenPropertyInfos = tokenPropertyInfos;
        _usingInfos         = new List<string>();
        _priorityMap        = new Dictionary<string, int>();
        SetupUsingInfos();
        SetupPriorityMap();
    }
    
    private void SetupPriorityMap()
    {
        // 越小的排在前面，这样后面的优先级就高一些，出现一样的属性就覆盖前面的
        _priorityMap.Add("SeedDesignToken", 1);
        _priorityMap.Add("ColorNeutralMapDesignToken", 100);
        _priorityMap.Add("ColorPrimaryMapDesignToken", 101);
        _priorityMap.Add("ColorSuccessMapDesignToken", 102);
        _priorityMap.Add("ColorWarningMapDesignToken", 103);
        _priorityMap.Add("ColorErrorMapDesignToken", 104);
        _priorityMap.Add("ColorInfoMapDesignToken", 105);
        _priorityMap.Add("ColorLinkMapDesignToken", 106);
        _priorityMap.Add("ColorMapDesignToken", 199);
        _priorityMap.Add("StyleMapDesignToken", 299);
        _priorityMap.Add("SizeMapDesignToken", 399);
        _priorityMap.Add("HeightMapDesignToken", 499);
        _priorityMap.Add("FontMapDesignToken", 599);
        _priorityMap.Add("MapDesignToken", 999);
        _priorityMap.Add("AliasDesignToken", 1000);
    }
    
    private void SetupUsingInfos()
    {
        foreach (var info in _tokenPropertyInfos)
        {
            foreach (var usingDef in info.Usings)
            {
                if (!_usingInfos.Contains(usingDef))
                {
                    _usingInfos.Add(usingDef);
                }
            }
        }
    }

    public void Write()
    {
        var compilationUnitSyntax = BuildCompilationUnitSyntax();
        _context.AddSource("MergedGlobalToken.g.cs", compilationUnitSyntax.NormalizeWhitespace().ToFullString());
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
        var namespaceSyntax = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName("AtomUI.Theme.Styling"));
        var classSyntax     = BuildMergedTokenClassSyntax();
        
        namespaceSyntax = namespaceSyntax.AddMembers(classSyntax);
        compilationUnit = compilationUnit.AddMembers(namespaceSyntax)
                                         .WithLeadingTrivia(SyntaxFactory.Trivia(SyntaxFactory.NullableDirectiveTrivia(SyntaxFactory.Token(SyntaxKind.EnableKeyword), true)));

        return compilationUnit;
    }
    
    private ClassDeclarationSyntax BuildMergedTokenClassSyntax()
    {
        var classModifiers = new List<SyntaxToken>
        {
            SyntaxFactory.Token(SyntaxKind.PublicKeyword),
            SyntaxFactory.Token(SyntaxKind.PartialKeyword)
        };
        var mergedTokenClass = SyntaxFactory.ClassDeclaration(MergedGlobalTokenClassName)
                                            .AddModifiers(classModifiers.ToArray());
        
        var tokenPropertyDefs = new List<PropertyDeclarationSyntax>();
        var propertyDefs      = BuildTokenPropertyInfos();
        foreach (var def in propertyDefs)
        {
            var tokenPropertyDef = SyntaxFactory.ParseMemberDeclaration(def.DefText) as PropertyDeclarationSyntax;
            if (tokenPropertyDef != null)
            {
                var triviaList = new List<SyntaxTrivia>();
                foreach (var comment in def.Comments)
                {
                    triviaList.Add(SyntaxFactory.Comment(comment));
                }

                tokenPropertyDef = tokenPropertyDef.WithLeadingTrivia(triviaList.ToArray());
                tokenPropertyDefs.Add(tokenPropertyDef);
            }
        }

        mergedTokenClass = mergedTokenClass.AddMembers(tokenPropertyDefs.ToArray());
        return mergedTokenClass;
    }

    private List<TokenPropertyDef> BuildTokenPropertyInfos()
    {
        var defs = new List<TokenPropertyDef>();
        
        // 首先排序
        _tokenPropertyInfos.Sort((left, right) =>
        {
            var leftPriority = DefaultPriority;
            var rightPriority = DefaultPriority;
            if (_priorityMap.ContainsKey(left.ClassName))
            {
                leftPriority = _priorityMap[left.ClassName];   
            }

            if (_priorityMap.ContainsKey(right.ClassName))
            {
                rightPriority = _priorityMap[right.ClassName];
            }
            
            return leftPriority.CompareTo(rightPriority);
        });
        foreach (var info in _tokenPropertyInfos)
        {
            foreach (var def in info.Definitions)
            {
                if (!defs.Contains(def))
                {
                    defs.Add(def);
                }
                else
                {
                    Debug.WriteLine($"Already contain {def.DefText}");
                }
            }
        }

        return defs;
    }
}