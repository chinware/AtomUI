using AtomUI.Generator.Language;
using Microsoft.CodeAnalysis;

namespace AtomUI.Generator;

[Generator]
public class LanguageGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext initContext)
    {
        var languageProviders = initContext.SyntaxProvider.ForAttributeWithMetadataName(
            TargetMarkConstants.LanguageProviderAttribute,
            (node, token) => true,
            (context, token) =>
            {
                var walker = new LanguageProviderWalker(context.SemanticModel);
                walker.Visit(context.TargetNode);
                return walker.LanguageInfo;
            }).Collect();
        var compilationInfo = initContext.CompilationProvider.Select(static (compilation, _) =>
            (compilation.AssemblyName,
             HasLegacyLanguageRuntime: compilation.GetTypeByMetadataName(TargetMarkConstants.LanguageProvider) is not null));
        var generationInput = languageProviders.Combine(compilationInfo);
        initContext.RegisterImplementationSourceOutput(generationInput, (context, input) =>
        {
            var (providers, info) = input;
            var (assemblyName, hasLegacyLanguageRuntime) = info;
            if (!hasLegacyLanguageRuntime)
            {
                return;
            }

            var providerList = providers.ToList();
            {
                var classWriter = new LanguageProviderPoolClassSourceWriter(context, providerList, assemblyName);
                classWriter.Write();
            }

            if (providers.IsEmpty)
            {
                return;
            }

            {
                var classWriter = new LangResourceKeyClassSourceWriter(context, providerList);
                classWriter.Write();
            }
            {
                var classWriter = new LanguageProviderConstructorSourceWriter(context, providerList);
                classWriter.Write();
            }
        });
    }
}
