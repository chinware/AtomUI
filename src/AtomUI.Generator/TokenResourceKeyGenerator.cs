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
                var walker = new ControlTokenPropertyWalker(context.SemanticModel, token);
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
            .Combine(initContext.AnalyzerConfigOptionsProvider)
            .Select(static (input, token) =>
            {
                input.Right.GlobalOptions.TryGetValue(
                    "build_property.AtomUIThemeAssetProjectDirectory",
                    out var projectDirectory);
                input.Right.GetOptions(input.Left).TryGetValue(
                    "build_metadata.AdditionalFiles.Link",
                    out var link);
                input.Right.GetOptions(input.Left).TryGetValue(
                    "build_metadata.AdditionalFiles.AtomUIRegistrationUnit",
                    out var explicitUnit);
                return ControlThemeSourceInfo.Create(
                    input.Left,
                    projectDirectory,
                    link,
                    explicitUnit,
                    token);
            })
            .Collect();

        var semanticControlsProvider = initContext.SyntaxProvider.ForAttributeWithMetadataName(
            TargetMarkConstants.SemanticPartAttribute,
            static (node, token) => true,
            static (context, token) => SemanticControlDeclaration.Create(context, token))
            .Collect();

        var semanticAssetsProvider = initContext.AdditionalTextsProvider
            .Where(static text => ThemeAssetInfo.IsThemeAssetPath(text.Path))
            .Combine(initContext.AnalyzerConfigOptionsProvider)
            .Select(static (input, token) =>
            {
                input.Right.GlobalOptions.TryGetValue(
                    "build_property.AtomUIThemeAssetProjectDirectory",
                    out var projectDirectory);
                input.Right.GetOptions(input.Left).TryGetValue(
                    "build_metadata.AdditionalFiles.Link",
                    out var link);
                input.Right.GetOptions(input.Left).TryGetValue(
                    "build_metadata.AdditionalFiles.AtomUIRegistrationUnit",
                    out var explicitUnit);
                return ThemeAssetInfo.Create(
                    input.Left,
                    projectDirectory,
                    link,
                    explicitUnit,
                    token);
            })
            .Collect();
        var tokensProvider = globalTokensProvider.Combine(controlTokensProvider)
                                               .Combine(algorithmsProvider)
                                               .Combine(themeAssetsProvider);
        var compilationProvider = initContext.CompilationProvider
                                             .Combine(initContext.AnalyzerConfigOptionsProvider)
                                             .Select(static (input, token) =>
                                                 CreateCompilationInfo(
                                                     input.Left,
                                                     input.Right));
        var generationProvider = tokensProvider.Combine(compilationProvider)
                                               .Combine(semanticControlsProvider)
                                               .Combine(semanticAssetsProvider);
        initContext.RegisterImplementationSourceOutput(generationProvider, (context, generationInfo) =>
        {
            var semanticGenerationInfo = generationInfo.Left;
            var themeGenerationInfo = semanticGenerationInfo.Left;
            var semanticDeclarations = semanticGenerationInfo.Right;
            var semanticAssets = generationInfo.Right;
            if (themeGenerationInfo.Right.InvalidRegistrationGranularity is not null)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    Diagnostics.AtomUIDiagnosticDescriptors.LinkedPackageDefinitionInvalid,
                    Location.None,
                    themeGenerationInfo.Right.PackageId,
                    $"AtomUIRegistrationGranularity '{themeGenerationInfo.Right.InvalidRegistrationGranularity}' is invalid; use 'Package' or 'Directory'"));
            }
            if (themeGenerationInfo.Right.Compilation.GetTypeByMetadataName(
                    "AtomUI.Theme.Resources.TokenResourceExtension`1") is null)
            {
                return;
            }
            var combinedInfos = themeGenerationInfo.Left;
            foreach (var issue in themeGenerationInfo.Right.EntryMethods.Issues)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    Diagnostics.AtomUIDiagnosticDescriptors.LinkedPackageEntryInvalid,
                    issue.Location,
                    issue.MethodDisplayName,
                    issue.Reason));
            }
            var semanticRuntimeAvailable = themeGenerationInfo.Right.Compilation.GetTypeByMetadataName(
                "AtomUI.Theme.Schema.ControlSemanticDescriptor") is not null;
            ThemeControlCatalogMetadataWriter.Write(
                context,
                themeGenerationInfo.Right.ControlCatalog);
            var tokenInfo = new TokenInfo();
            tokenInfo.Tokens.UnionWith(combinedInfos.Left.Left.Left.Tokens);
            tokenInfo.AvailableGlobalTokenNames.UnionWith(
                combinedInfos.Left.Left.Left.Tokens.Select(static token => token.Name));
            tokenInfo.AvailableGlobalTokenNames.UnionWith(themeGenerationInfo.Right.GlobalTokenNames);
            tokenInfo.SchemaTokens.UnionWith(combinedInfos.Left.Left.Left.SchemaTokens);
            var controlThemeInfos = ControlThemeModelBuilder.Build(
                themeGenerationInfo.Right.Compilation,
                combinedInfos.Left.Left.Right,
                combinedInfos.Right,
                tokenInfo.AvailableGlobalTokenNames,
                themeGenerationInfo.Right.PackageId,
                themeGenerationInfo.Right.RegistrationGranularity,
                themeGenerationInfo.Right.ProjectDirectory,
                themeGenerationInfo.Right.OptionsProvider,
                context.ReportDiagnostic);
            tokenInfo.ControlThemeInfos.AddRange(controlThemeInfos);
            if (tokenInfo.SchemaTokens.Count != 0 ||
                tokenInfo.ControlThemeInfos.Count != 0 ||
                combinedInfos.Left.Right.Length != 0)
            {
                var schemaWriter = new GeneratedThemeSchemaWriter(
                    context,
                    themeGenerationInfo.Right.AssemblyName,
                    themeGenerationInfo.Right.PackageId,
                    themeGenerationInfo.Right.EntryMethods.HasEntries,
                    themeGenerationInfo.Right.ControlCatalog,
                    tokenInfo.SchemaTokens,
                    tokenInfo.ControlThemeInfos,
                    combinedInfos.Left.Right);
                schemaWriter.Write();
            }
            if (tokenInfo.ControlThemeInfos.Count != 0 && combinedInfos.Right.Length != 0)
            {
                new ControlPackageRegistrationWriter(
                    context,
                    themeGenerationInfo.Right.AssemblyName,
                    themeGenerationInfo.Right.PackageId,
                    themeGenerationInfo.Right.RegistrationGranularity,
                    themeGenerationInfo.Right.EntryMethods.ManifestMethodMetadataNames,
 semanticRuntimeAvailable).Write();
            }

            if (semanticRuntimeAvailable)
            {
                var semanticControls = SemanticPartModelBuilder.Build(
                    themeGenerationInfo.Right.Compilation,
                    semanticDeclarations,
                    semanticAssets,
                    context.ReportDiagnostic);
                new SemanticPartManifestWriter(
                    context,
                    themeGenerationInfo.Right.AssemblyName,
                    themeGenerationInfo.Right.ControlCatalog,
                    semanticControls).Write();
            }

            {
                var classWriter = new ResourceKeyClassWriter(
                    context,
                    tokenInfo,
                    themeGenerationInfo.Right.ControlCatalog);
                classWriter.Write();
            }
        });
    }

    private static ThemeCompilationInfo CreateCompilationInfo(
        Compilation compilation,
        Microsoft.CodeAnalysis.Diagnostics.AnalyzerConfigOptionsProvider optionsProvider)
    {
        var names = compilation.GetTypeByMetadataName("AtomUI.Theme.Resources.SharedTokenKind")?
                               .GetMembers()
                               .OfType<IFieldSymbol>()
                               .Where(static field => field.HasConstantValue && field.Name != "value__")
                               .Select(static field => field.Name)
                               .OrderBy(static name => name, StringComparer.Ordinal)
                               .ToArray() ?? Array.Empty<string>();
        var assemblyName = compilation.AssemblyName ?? "AtomUI";
        optionsProvider.GlobalOptions.TryGetValue(
            "build_property.AtomUIThemeAssetProjectDirectory",
            out var projectDirectory);
        var registrationGranularity = LinkedRegistration.LinkedRegistrationOptions
            .GetRegistrationGranularity(optionsProvider, out var invalidRegistrationGranularity);
        return new ThemeCompilationInfo(
            compilation,
            assemblyName,
            LinkedRegistration.LinkedRegistrationOptions.GetPackageId(optionsProvider, assemblyName),
            registrationGranularity,
            invalidRegistrationGranularity,
            projectDirectory,
            ThemeGeneratorOptions.GetControlCatalog(optionsProvider),
            names,
            optionsProvider,
            LinkedRegistration.ControlPackageRegistrationEntryDiscovery.Discover(compilation));
    }
}
