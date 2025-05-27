using Microsoft.CodeAnalysis;

namespace AtomUI.Generator;

[Generator]
public class TokenResourceKeyGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext initContext)
    {
        var globalTokensProvider = initContext.SyntaxProvider.ForAttributeWithMetadataName(
            TargetMarkConstants.GlobalDesignTokenAttribute,
            (node, token) => true,
            (context, token) =>
            {
                var walker = new TokenPropertyWalker(context.SemanticModel);
                walker.Visit(context.TargetNode);
                return (walker.TokenResourceCatalog, walker.TokenNames);
            }).Collect().Select((array, token) =>
        {
            var mergedSet = new HashSet<TokenName>();
            foreach (var entry in array)
            {
                var ns = entry.TokenResourceCatalog!;
                foreach (var tokenName in entry.TokenNames)
                {
                    mergedSet.Add(new TokenName(tokenName, ns));
                }
            }

            return mergedSet;
        });

        var controlTokensProvider = initContext.SyntaxProvider.ForAttributeWithMetadataName(
            TargetMarkConstants.ControlDesignTokenAttribute,
            (node, token) => true,
            (context, token) =>
            {
                var walker = new ControlTokenPropertyWalker(context.SemanticModel);
                walker.Visit(context.TargetNode);
                return walker.ControlTokenInfo;
            }).Collect();

        var tokensProvider = globalTokensProvider.Combine(controlTokensProvider);

        initContext.RegisterImplementationSourceOutput(tokensProvider, (context, combinedInfos) =>
        {
            var tokenInfo = new TokenInfo();
            tokenInfo.Tokens.UnionWith(combinedInfos.Left);
            foreach (var controlToken in combinedInfos.Right)
            {
                tokenInfo.ControlTokenInfos.Add(controlToken);
            }

            {
                var classWriter = new ResourceKeyClassWriter(context, tokenInfo);
                classWriter.Write();
            }

            {
                var tokenClassNames = tokenInfo.ControlTokenInfos.Select(info =>
                {
                    if (info.ControlNamespace is not null)
                    {
                        return $"{info.ControlNamespace}.{info.ControlName}";
                    }

                    return info.ControlName;
                }).ToList();
                if (tokenClassNames.Any())
                {
                    {
                        var classWriter = new ControlTokenTypePoolClassWriter(context, tokenClassNames);
                        classWriter.Write();
                    }
                }
            }
        });
    }
}