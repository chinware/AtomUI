using AtomUI.Generator.Localization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AtomUI.Generator;

[Generator]
public sealed class LanguageTagsGenerator : IIncrementalGenerator
{
    private const string LanguageTagsDataMetadata =
        "build_metadata.AdditionalFiles.AtomUILanguageTagsData";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var languageData = context.AdditionalTextsProvider
                                  .Combine(context.AnalyzerConfigOptionsProvider)
                                  .Where(static pair => IsLanguageTagsData(pair.Left, pair.Right))
                                  .Select(static (pair, cancellationToken) =>
                                      LanguageDataFileParser.Parse(pair.Left, cancellationToken))
                                  .Collect();

        context.RegisterSourceOutput(languageData, static (sourceContext, results) =>
        {
            foreach (var result in results)
            {
                foreach (var diagnostic in result.Diagnostics)
                {
                    sourceContext.ReportDiagnostic(diagnostic);
                }
            }
        });
    }

    private static bool IsLanguageTagsData(
        AdditionalText additionalText,
        AnalyzerConfigOptionsProvider optionsProvider)
    {
        return optionsProvider.GetOptions(additionalText)
                              .TryGetValue(LanguageTagsDataMetadata, out var value) &&
               string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
    }
}
