using System.Collections.Immutable;
using AtomUI.Generator.Diagnostics;
using Microsoft.CodeAnalysis;

namespace AtomUI.Generator;

[Generator]
public sealed class ThemeAssetManifestGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var assets = context.AdditionalTextsProvider
                            .Where(static text => text.Path.EndsWith(".axaml", StringComparison.OrdinalIgnoreCase))
                            .Select(static (text, token) => ThemeAssetInfo.Create(text, token))
                            .Collect();
        var controls = context.SyntaxProvider.ForAttributeWithMetadataName(
            TargetMarkConstants.ControlDesignTokenAttribute,
            static (node, token) => true,
            static (attributeContext, token) =>
            {
                var walker = new ControlTokenPropertyWalker(attributeContext.SemanticModel);
                walker.Visit(attributeContext.TargetNode);
                return walker.ControlTokenInfo;
            }).Collect();
        var input = assets.Combine(controls).Combine(
            context.CompilationProvider.Select(static (compilation, token) => compilation.AssemblyName ?? "AtomUI"));

        context.RegisterSourceOutput(input, static (productionContext, value) =>
        {
            var assets = value.Left.Left;
            if (assets.Length == 0)
            {
                return;
            }
            var controls = value.Left.Right
                                .Where(static control => control.IsValid)
                                .Select(static control => control.ControlId!)
                                .ToImmutableHashSet(StringComparer.Ordinal);
            var valid = new List<ThemeAssetInfo>();
            foreach (var asset in assets.OrderBy(static asset => asset.AssetPath, StringComparer.Ordinal))
            {
                var diagnostics = asset.Validate(controls).ToArray();
                foreach (var diagnostic in diagnostics)
                {
                    productionContext.ReportDiagnostic(diagnostic);
                }
                if (diagnostics.Length == 0 && asset.UsesTokens && asset.Identities.Count == 1)
                {
                    valid.Add(asset);
                }
            }

            foreach (var group in valid.GroupBy(static asset => asset.AssetPath, StringComparer.Ordinal))
            {
                if (group.Count() <= 1)
                {
                    continue;
                }
                var asset = group.First();
                productionContext.ReportDiagnostic(Diagnostic.Create(
                    AtomUIDiagnosticDescriptors.ThemeAssetDuplicateUri,
                    asset.CreateLocation(),
                    asset.AssetPath));
            }

            var unique = valid.GroupBy(static asset => asset.AssetPath, StringComparer.Ordinal)
                              .Where(static group => group.Count() == 1)
                              .Select(static group => group.Single())
                              .ToArray();
            if (unique.Length != 0)
            {
                new ThemeAssetManifestWriter(
                    productionContext,
                    value.Right,
                    unique).Write();
            }
        });
    }
}
