using AtomUI.Generator.Localization;
using AtomUI.Generator.Localization.Catalog;
using AtomUI.Generator.Localization.Xliff;
using Microsoft.CodeAnalysis;

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
                                      LanguageGeneratorOptions.GetModuleId(
                                          input.Right,
                                          input.Left.Symbol.ContainingAssembly.Name),
                                      cancellationToken));
        var languageFiles = context.AdditionalTextsProvider
                                   .Combine(context.AnalyzerConfigOptionsProvider)
                                   .Where(static input => LanguageGeneratorOptions.IsLanguageFile(
                                       input.Left,
                                       input.Right))
                                   .Select(static (input, cancellationToken) =>
                                       AdditionalLanguageFileParser.Parse(
                                           input.Left,
                                           input.Right,
                                           cancellationToken));
        var compiledCatalogs = context.CompilationProvider
                                       .Combine(catalogs.Collect())
                                       .Combine(languageFiles.Collect())
                                       .Select(static (input, _) => new LocalizationGenerationResult(
                                           input.Left.Left.AssemblyName,
                                           LanguageCatalogCompiler.Compile(
                                               input.Left.Right,
                                               input.Right,
                                               input.Left.Left)));

        context.RegisterSourceOutput(catalogs, static (sourceContext, result) =>
        {
            foreach (var diagnostic in result.Diagnostics)
            {
                sourceContext.ReportDiagnostic(diagnostic);
            }
        });
        context.RegisterSourceOutput(languageFiles, static (sourceContext, result) =>
        {
            foreach (var diagnostic in result.Diagnostics)
            {
                sourceContext.ReportDiagnostic(diagnostic);
            }
        });
        context.RegisterSourceOutput(compiledCatalogs, static (sourceContext, result) =>
        {
            foreach (var diagnostic in result.Compilation.Diagnostics)
            {
                sourceContext.ReportDiagnostic(diagnostic);
            }

            if (!result.Compilation.Diagnostics.IsEmpty)
            {
                return;
            }

            foreach (var catalog in result.Compilation.Catalogs.Where(static catalog => catalog.OwnsCatalog))
            {
                LanguageCatalogSourceWriter.Write(sourceContext, catalog);
            }
            LanguageModuleSourceWriter.Write(
                sourceContext,
                result.AssemblyName,
                result.Compilation.Catalogs);
        });
    }

    private sealed class LocalizationGenerationResult
    {
        internal LocalizationGenerationResult(
            string? assemblyName,
            LanguageCatalogCompilationResult compilation)
        {
            AssemblyName = assemblyName;
            Compilation = compilation;
        }

        internal string? AssemblyName { get; }

        internal LanguageCatalogCompilationResult Compilation { get; }
    }
}
