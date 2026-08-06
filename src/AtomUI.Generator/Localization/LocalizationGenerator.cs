using System.Collections.Immutable;
using AtomUI.Generator.Localization;
using AtomUI.Generator.Localization.Catalog;
using AtomUI.Generator.Localization.Xliff;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
                                      cancellationToken))
                              .WithTrackingName("LocalizationCatalogs");
        var languageFiles = context.AdditionalTextsProvider
                                   .Combine(context.AnalyzerConfigOptionsProvider)
                                   .Where(static input => LanguageGeneratorOptions.IsLanguageFile(
                                       input.Left,
                                       input.Right))
                                   .Select(static (input, cancellationToken) =>
                                       AdditionalLanguageFileParser.Parse(
                                           input.Left,
                                           input.Right,
                                           cancellationToken))
                                   .WithTrackingName("LocalizationLanguageFiles");
        var applicationHosts = context.SyntaxProvider.CreateSyntaxProvider(
                                          static (node, _) => node is ClassDeclarationSyntax { BaseList: not null },
                                          static (syntaxContext, cancellationToken) =>
                                              ApplicationLanguageHostInfo.TryCreate(
                                                  syntaxContext,
                                                  cancellationToken))
                                      .Where(static host => host is not null)
                                      .Select(static (host, _) => host!)
                                      .WithTrackingName("LocalizationApplicationHosts");
        var compiledCatalogs = context.CompilationProvider
                                       .Combine(catalogs.Collect())
                                       .Combine(languageFiles.Collect())
                                       .Combine(applicationHosts.Collect())
                                       .Select(static (input, _) => new LocalizationGenerationResult(
                                           input.Left.Left.Left.AssemblyName,
                                           LanguageCatalogCompiler.Compile(
                                               input.Left.Left.Right,
                                               input.Left.Right,
                                               input.Left.Left.Left),
                                           input.Right))
                                       .WithTrackingName("LocalizationCompilation");

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
                LanguageCatalogSourceWriter.Write(sourceContext, result.AssemblyName, catalog);
            }
            LanguageModuleSourceWriter.Write(
                sourceContext,
                result.AssemblyName,
                result.Compilation.Catalogs);
            ApplicationLanguageBootstrapWriter.Write(
                sourceContext,
                result.AssemblyName,
                result.Compilation.Catalogs,
                result.ApplicationHosts);
        });
    }

    private sealed class LocalizationGenerationResult
    {
        internal LocalizationGenerationResult(
            string? assemblyName,
            LanguageCatalogCompilationResult compilation,
            ImmutableArray<ApplicationLanguageHostInfo> applicationHosts)
        {
            AssemblyName = assemblyName;
            Compilation = compilation;
            ApplicationHosts = applicationHosts;
        }

        internal string? AssemblyName { get; }

        internal LanguageCatalogCompilationResult Compilation { get; }

        internal ImmutableArray<ApplicationLanguageHostInfo> ApplicationHosts { get; }
    }
}
