using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AtomUI.Generator.MergedTokenInfo;

public class GlobalTokenPropertyDefWalker : CSharpSyntaxWalker
{
    public const string AbstractDesignTokenName = "AbstractDesignToken";
    public const string NotTokenDefinitionAttribute = "NotTokenDefinition";
    public TokenPropertyInfo TokenPropertyInfo { get; private set; }
    
    private readonly SemanticModel _semanticModel;
    
    public GlobalTokenPropertyDefWalker(SemanticModel semanticModel)
    {
        _semanticModel    = semanticModel;
        TokenPropertyInfo = new TokenPropertyInfo();
    }
    
    public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
    {
        var isSkip = node.AttributeLists
                               .SelectMany(attrList => attrList.Attributes)
                               .Any(attr => attr.Name.ToString() == NotTokenDefinitionAttribute);
        
        if (!isSkip)
        {
            var triviaList = node.GetLeadingTrivia();
            var comments   = new List<string>();
            foreach (var trivia in triviaList)
            {
                var triviaText = trivia.ToString().Trim();
                if (!string.IsNullOrEmpty(triviaText))
                {
                    comments.Add(triviaText);
                }
            }
            TokenPropertyInfo.Definitions.Add(new TokenPropertyDef
            {
                DefText = node.ToString(),
                Comments = comments,
                TokenName = node.Identifier.ToString(),
            });
        }
    }

    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        base.VisitClassDeclaration(node);
        TokenPropertyInfo.ClassName = node.Identifier.Text;
        var parent = node.Parent;
        while (parent is not null)
        {
            if (parent is CompilationUnitSyntax compilationUnit)
            {
                ParseUsingsInfo(compilationUnit);
                break;
            }
            parent = parent.Parent;
        }
    }

    private void ParseUsingsInfo(CompilationUnitSyntax compilationUnitSyntax)
    {
        foreach (var usingDef in compilationUnitSyntax.Usings)
        {
            // 暂收不支持别名
            TokenPropertyInfo.Usings.Add(usingDef.Name!.ToString());
        }
    }
}