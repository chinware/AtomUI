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
                return (walker.TokenResourceCatalog, walker.TokenNames, walker.SchemaTokens);
            }).Collect().Select((array, token) =>
        {
            var merged = new GlobalTokenGenerationInfo();
            foreach (var entry in array)
            {
                var ns = entry.TokenResourceCatalog!;
                foreach (var tokenName in entry.TokenNames)
                {
                    merged.Tokens.Add(new TokenName(tokenName, ns));
                }

                foreach (var schemaToken in entry.SchemaTokens)
                {
                    merged.SchemaTokens.Add(schemaToken);
                }
            }

            return merged;
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

        var algorithmsProvider = initContext.SyntaxProvider.ForAttributeWithMetadataName(
            TargetMarkConstants.ThemeAlgorithmAttribute,
            static (node, token) => true,
            static (context, token) => ThemeAlgorithmInfo.Create(context))
            .Where(static info => info is not null)
            .Select(static (info, token) => info!)
            .Collect();

        var tokensProvider = globalTokensProvider.Combine(controlTokensProvider).Combine(algorithmsProvider);
        var generationProvider = tokensProvider.Combine(
            initContext.CompilationProvider.Select(static (compilation, token) => compilation.AssemblyName));

        initContext.RegisterImplementationSourceOutput(generationProvider, (context, generationInfo) =>
        {
            var combinedInfos = generationInfo.Left;
            var tokenInfo = new TokenInfo();
            tokenInfo.Tokens.UnionWith(combinedInfos.Left.Left.Tokens);
            tokenInfo.SchemaTokens.UnionWith(combinedInfos.Left.Left.SchemaTokens);
            foreach (var controlToken in combinedInfos.Left.Right)
            {
                foreach (var diagnostic in controlToken.Diagnostics)
                {
                    context.ReportDiagnostic(diagnostic);
                }

                if (controlToken.IsValid)
                {
                    tokenInfo.ControlTokenInfos.Add(controlToken);
                }
            }

            if (tokenInfo.SchemaTokens.Count != 0 ||
                tokenInfo.ControlTokenInfos.Count != 0 ||
                combinedInfos.Right.Length != 0)
            {
                var schemaWriter = new GeneratedThemeSchemaWriter(
                    context,
                    generationInfo.Right,
                    tokenInfo.SchemaTokens,
                    tokenInfo.ControlTokenInfos,
                    combinedInfos.Right);
                schemaWriter.Write();
            }

            {
                var classWriter = new ResourceKeyClassWriter(context, tokenInfo);
                classWriter.Write();
            }

        });
    }
}
