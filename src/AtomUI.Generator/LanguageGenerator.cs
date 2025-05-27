using AtomUI.Generator.Language;
using Microsoft.CodeAnalysis;

namespace AtomUI.Generator;

[Generator]
public class LanguageGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext initContext)
    {
        var languageProvider = initContext.SyntaxProvider.ForAttributeWithMetadataName(
            TargetMarkConstants.LanguageProviderAttribute,
            (node, token) => true,
            (context, token) =>
            {
                var walker = new LanguageProviderWalker(context.SemanticModel);
                walker.Visit(context.TargetNode);
                return walker.LanguageInfo;
            }).Collect();
        initContext.RegisterImplementationSourceOutput(languageProvider, (context, languageProviders) =>
        {
            var providerList = languageProviders.ToList();
            {
                var classWriter = new LangResourceKeyClassSourceWriter(context, providerList);
                classWriter.Write();
            }
            {
                var classWriter = new LanguageProviderPoolClassSourceWriter(context, providerList);
                classWriter.Write();
            }
        });
    }
}