using AtomUI.Generator.Localization.Catalog;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AtomUI.Generator;

[Generator]
public sealed class LocalizationGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var targets = context.SyntaxProvider.ForAttributeWithMetadataName(
            "AtomUI.Localization.LanguageCatalogAttribute",
            static (_, _) => true,
            static (attributeContext, _) => new LanguageCatalogTarget(
                (INamedTypeSymbol)attributeContext.TargetSymbol,
                attributeContext.Attributes[0]));
        var catalogs = targets.Combine(context.AnalyzerConfigOptionsProvider)
                              .Select(static (input, cancellationToken) =>
                                  LanguageCatalogSymbolParser.Parse(
                                      input.Left,
                                      GetModuleId(input.Right, input.Left.Symbol.ContainingAssembly.Name),
                                      cancellationToken));

        context.RegisterSourceOutput(catalogs, static (sourceContext, result) =>
        {
            foreach (var diagnostic in result.Diagnostics)
            {
                sourceContext.ReportDiagnostic(diagnostic);
            }
        });
    }

    private static string GetModuleId(
        AnalyzerConfigOptionsProvider optionsProvider,
        string fallbackAssemblyName)
    {
        if (TryGetNonEmpty(optionsProvider.GlobalOptions, "build_property.PackageId", out var packageId))
        {
            return packageId;
        }

        return TryGetNonEmpty(
            optionsProvider.GlobalOptions,
            "build_property.AssemblyName",
            out var assemblyName)
            ? assemblyName
            : fallbackAssemblyName;
    }

    private static bool TryGetNonEmpty(
        AnalyzerConfigOptions options,
        string key,
        out string value)
    {
        if (options.TryGetValue(key, out var candidate) &&
            !string.IsNullOrWhiteSpace(candidate))
        {
            value = candidate.Trim();
            return true;
        }

        value = string.Empty;
        return false;
    }
}
