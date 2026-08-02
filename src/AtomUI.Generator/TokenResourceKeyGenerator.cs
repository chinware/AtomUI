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

        var themeAssetsProvider = initContext.AdditionalTextsProvider
            .Where(static text => ThemeAssetInfo.IsThemeAssetPath(text.Path))
            .Select(static (text, token) => ControlThemeSourceInfo.Create(text, token))
            .Collect();

        var tokensProvider = globalTokensProvider.Combine(controlTokensProvider)
                                               .Combine(algorithmsProvider)
                                               .Combine(themeAssetsProvider);
        var compilationProvider = initContext.CompilationProvider
                                             .Combine(initContext.AnalyzerConfigOptionsProvider)
                                             .Select(static (input, token) =>
                                                 CreateCompilationInfo(
                                                     input.Left,
                                                     ThemeGeneratorOptions.GetControlCatalog(input.Right)));
        var generationProvider = tokensProvider.Combine(compilationProvider);

        initContext.RegisterImplementationSourceOutput(generationProvider, (context, generationInfo) =>
        {
            var combinedInfos = generationInfo.Left;
            ThemeControlCatalogMetadataWriter.Write(
                context,
                generationInfo.Right.ControlCatalog);
            var tokenInfo = new TokenInfo();
            tokenInfo.Tokens.UnionWith(combinedInfos.Left.Left.Left.Tokens);
            tokenInfo.AvailableGlobalTokenNames.UnionWith(
                combinedInfos.Left.Left.Left.Tokens.Select(static token => token.Name));
            tokenInfo.AvailableGlobalTokenNames.UnionWith(generationInfo.Right.GlobalTokenNames);
            tokenInfo.SchemaTokens.UnionWith(combinedInfos.Left.Left.Left.SchemaTokens);
            tokenInfo.ControlThemeInfos.AddRange(ControlThemeModelBuilder.Build(
                generationInfo.Right.Compilation,
                combinedInfos.Left.Left.Right,
                combinedInfos.Right,
                tokenInfo.AvailableGlobalTokenNames,
                context.ReportDiagnostic));

            if (tokenInfo.SchemaTokens.Count != 0 ||
                tokenInfo.ControlThemeInfos.Count != 0 ||
                combinedInfos.Left.Right.Length != 0)
            {
                var schemaWriter = new GeneratedThemeSchemaWriter(
                    context,
                    generationInfo.Right.AssemblyName,
                    generationInfo.Right.ControlCatalog,
                    tokenInfo.SchemaTokens,
                    tokenInfo.ControlThemeInfos,
                    combinedInfos.Left.Right);
                schemaWriter.Write();
            }

            if (tokenInfo.ControlThemeInfos.Count != 0 && combinedInfos.Right.Length != 0)
            {
                new ControlPackageRegistrationWriter(
                    context,
                    generationInfo.Right.AssemblyName).Write();
            }

            {
                var classWriter = new ResourceKeyClassWriter(
                    context,
                    tokenInfo,
                    generationInfo.Right.ControlCatalog);
                classWriter.Write();
            }

        });
    }

    private static ThemeCompilationInfo CreateCompilationInfo(
        Compilation compilation,
        string controlCatalog)
    {
        var names = compilation.GetTypeByMetadataName("AtomUI.Theme.Resources.SharedTokenKind")?
                               .GetMembers()
                               .OfType<IFieldSymbol>()
                               .Where(static field => field.HasConstantValue && field.Name != "value__")
                               .Select(static field => field.Name)
                               .OrderBy(static name => name, StringComparer.Ordinal)
                               .ToArray() ?? Array.Empty<string>();
        return new ThemeCompilationInfo(
            compilation,
            compilation.AssemblyName ?? "AtomUI",
            controlCatalog,
            names);
    }
}
