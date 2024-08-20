using AtomUI.Generator.Language;
using Microsoft.CodeAnalysis;

namespace AtomUI.Generator;

[Generator]
public class LanguageGenerator : IIncrementalGenerator
{
   public const string LanguageProviderAttribute = "AtomUI.Theme.LanguageProviderAttribute";
   
   public void Initialize(IncrementalGeneratorInitializationContext initContext)
   {
      var languageProvider = initContext.SyntaxProvider.ForAttributeWithMetadataName(LanguageProviderAttribute,
         ((node, token) => true),
         (context, token) =>
         {
            var walker = new LanguageProviderWalker(context.SemanticModel);
            walker.Visit(context.TargetNode);
            return walker.LanguageInfo;
         }).Collect();
      initContext.RegisterImplementationSourceOutput(languageProvider, (context, languageProviders) =>
      {
         var classWriter = new LangResourceKeyClassSourceWriter(context, languageProviders.ToList());
         classWriter.Write();
      });
   }
}