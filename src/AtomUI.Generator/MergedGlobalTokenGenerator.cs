using AtomUI.Generator.MergedTokenInfo;
using Microsoft.CodeAnalysis;

namespace AtomUI.Generator;

[Generator]
public class MergedGlobalTokenGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext initContext)
    {
        var globalTokensProvider = initContext.SyntaxProvider.ForAttributeWithMetadataName(
            TargetMarkConstants.GlobalDesignTokenAttribute,
            (node, token) => true,
            (context, token) =>
            {
                var walker = new GlobalTokenPropertyDefWalker(context.SemanticModel);
                walker.Visit(context.TargetNode);
                return walker.TokenPropertyInfo;
            }).Collect();
        
        initContext.RegisterImplementationSourceOutput(globalTokensProvider, (context, infos) =>
        {
            var classWriter = new MergedGlobalTokenClassWriter(context, infos.ToList());
            classWriter.Write();
        });
        
    }
}