using Microsoft.CodeAnalysis;

namespace AtomUI.Generator;

[Generator]
public class TokenResourceKeyGenerator : IIncrementalGenerator
{
   public const string GlobalDesignTokenAttribute = "AtomUI.TokenSystem.GlobalDesignTokenAttribute";
   public const string ControlDesignTokenAttribute = "AtomUI.TokenSystem.ControlDesignTokenAttribute";
   
   public void Initialize(IncrementalGeneratorInitializationContext initContext)
   {
      var globalTokensProvider = initContext.SyntaxProvider.ForAttributeWithMetadataName(GlobalDesignTokenAttribute,
         ((node, token) => true),
         (context, token) =>
         {
            var walker = new TokenPropertyWalker(context.SemanticModel);
            walker.Visit(context.TargetNode);
            return walker.TokenNames;
         }).Collect().Select((array, token) =>
      {
         var mergedSet = new HashSet<string>();
         foreach (var set in array) {
            mergedSet.UnionWith(set);
         }

         return mergedSet;
      });
      
      var controlTokensProvider = initContext.SyntaxProvider.ForAttributeWithMetadataName(ControlDesignTokenAttribute,
         (node, token) => true,
         (context, token) =>
         {
            var walker = new ControlTokenPropertyWalker();
            walker.Visit(context.TargetNode);
            return walker.ControlTokenInfo;
         }).Collect();

      var tokensProvider = globalTokensProvider.Combine(controlTokensProvider);
      
      initContext.RegisterImplementationSourceOutput(tokensProvider, (context, combinedInfos) =>
      {
         var tokenInfo = new TokenInfo();
         tokenInfo.Tokens.UnionWith(combinedInfos.Left);
         foreach (var controlToken in combinedInfos.Right) {
            tokenInfo.ControlTokenInfos.Add(controlToken);
         }

         var classWriter = new ResourceKeyClassSourceWriter(context, tokenInfo);
         classWriter.Write();
      });
   }
}