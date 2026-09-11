using System.Collections.Immutable;
using AtomUI.Generator.Diagnostics;
using AtomUI.Generator.Localization.Catalog;
using AtomUI.Generator.Localization.Xliff;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AtomUI.Generator.Localization;

internal static class LocalizationPipeline
{
    internal static LanguageFileInputResult ParseLanguageFile(
        AdditionalText additionalText,
        AnalyzerConfigOptionsProvider optionsProvider,
        CancellationToken cancellationToken)
    {
        var parseResult = AdditionalLanguageFileParser.Parse(additionalText, cancellationToken);
        if (!parseResult.Errors.IsEmpty || parseResult.File is null)
        {
            var diagnostics = parseResult.Errors
                                         .Select(error => LocalizationDiagnosticFactory.InvalidXliff(
                                             parseResult.Path,
                                             parseResult.Text,
                                             error))
                                         .ToImmutableArray();
            return new LanguageFileInputResult(null, diagnostics);
        }

        return LanguageFileMetadataValidator.Validate(
            parseResult.File,
            optionsProvider.GetOptions(additionalText),
            LanguageGeneratorOptions.GetModuleId(optionsProvider, "Application"));
    }

    internal static LocalizationGenerationResult Compile(
        string? assemblyName,
        ImmutableArray<LanguageCatalogParseResult> catalogResults,
        ImmutableArray<LanguageFileInputResult> fileResults,
        Compilation? compilation,
        ImmutableArray<ApplicationLanguageHostInfo> applicationHosts)
    {
        var inputDiagnostics = catalogResults.SelectMany(static result => result.Diagnostics)
                                             .Concat(fileResults.SelectMany(static result => result.Diagnostics))
                                             .ToImmutableArray();
        if (!inputDiagnostics.IsEmpty)
        {
            return new LocalizationGenerationResult(
                assemblyName,
                LocalizationCompilationPlan.Empty,
                OrderDiagnostics(inputDiagnostics));
        }

        var catalogCompilation = CompileCatalogs(
            catalogResults.Select(static result => result.Catalog!)
                          .ToImmutableArray(),
            fileResults.Select(static result => result.Input!)
                       .ToImmutableArray(),
            compilation);
        if (!catalogCompilation.Diagnostics.IsEmpty)
        {
            return new LocalizationGenerationResult(
                assemblyName,
                LocalizationCompilationPlan.Empty,
                OrderDiagnostics(catalogCompilation.Diagnostics));
        }

        var applicationHostResult = SelectApplicationHost(
            catalogCompilation.Catalogs,
            applicationHosts);
        var plan = new LocalizationCompilationPlan(
            catalogCompilation.Catalogs,
            applicationHostResult.Host);
        return new LocalizationGenerationResult(
            assemblyName,
            plan,
            applicationHostResult.Diagnostics);
    }

    private static LanguageCatalogCompilationResult CompileCatalogs(
        ImmutableArray<LanguageCatalogInfo> catalogInputs,
        ImmutableArray<LanguageFileInput> fileInputs,
        Compilation? compilation)
    {
        var ownedCatalogs = catalogInputs.OrderBy(static catalog => catalog.CatalogId, StringComparer.Ordinal)
                                         .ToImmutableArray();
        var files = fileInputs.OrderBy(static file => file.ModuleId, StringComparer.Ordinal)
                              .ThenBy(static file => file.Document.File.Id, StringComparer.Ordinal)
                              .ThenBy(static file => GetLanguage(file), StringComparer.Ordinal)
                              .ThenBy(static file => file.SourceKind)
                              .ThenBy(static file => file.SourceIdentity, StringComparer.Ordinal)
                              .ThenBy(static file => file.Path, StringComparer.Ordinal)
                              .ToImmutableArray();
        var diagnostics = ImmutableArray.CreateBuilder<Diagnostic>();
        var symbolIndexResult = CatalogSymbolIndex.Create(
            ownedCatalogs,
            files,
            compilation);
        diagnostics.AddRange(symbolIndexResult.Diagnostics);
        var planningResult = CatalogCompilationPlanner.Plan(
            symbolIndexResult.Index,
            files);
        diagnostics.AddRange(planningResult.Diagnostics);

        var compiledCatalogs = ImmutableArray.CreateBuilder<CompiledLanguageCatalog>();
        foreach (var workItem in planningResult.WorkItems)
        {
            var validationResult = CatalogSemanticValidator.Validate(workItem);
            diagnostics.AddRange(validationResult.Diagnostics);
            if (validationResult.Input is not null)
            {
                compiledCatalogs.Add(TranslationBundleCompiler.Compile(validationResult.Input));
            }
        }

        var orderedDiagnostics = OrderDiagnostics(diagnostics);
        return new LanguageCatalogCompilationResult(
            orderedDiagnostics.IsEmpty
                ? compiledCatalogs.ToImmutable()
                : ImmutableArray<CompiledLanguageCatalog>.Empty,
            orderedDiagnostics);
    }

    private static ApplicationHostSelectionResult SelectApplicationHost(
        ImmutableArray<CompiledLanguageCatalog> compiledCatalogs,
        ImmutableArray<ApplicationLanguageHostInfo> applicationHosts)
    {
        if (compiledCatalogs.IsEmpty)
        {
            return ApplicationHostSelectionResult.Empty;
        }

        var concreteHosts = applicationHosts
                            .GroupBy(static host => host.MetadataName, StringComparer.Ordinal)
                            .Select(static group => group.First())
                            .Where(static host => !host.IsAbstract)
                            .OrderBy(static host => host.MetadataName, StringComparer.Ordinal)
                            .ToArray();
        if (concreteHosts.Length == 0)
        {
            return ApplicationHostSelectionResult.Empty;
        }

        if (concreteHosts.Length > 1)
        {
            return new ApplicationHostSelectionResult(
                null,
                concreteHosts.Select(host => InvalidApplicationHost(
                                 host,
                                 "more than one concrete Avalonia Application type is declared"))
                             .ToImmutableArray());
        }

        var application = concreteHosts[0];
        if (!application.IsTopLevel)
        {
            return InvalidApplicationHostResult(application, "the Application must be a top-level class");
        }
        if (application.IsGeneric)
        {
            return InvalidApplicationHostResult(application, "the Application must be non-generic");
        }
        if (!application.IsPartial)
        {
            return InvalidApplicationHostResult(application, "the Application must be declared partial");
        }
        if (string.IsNullOrEmpty(application.AccessibilityModifier))
        {
            return InvalidApplicationHostResult(application, "the Application must be public or internal");
        }

        return new ApplicationHostSelectionResult(
            application,
            ImmutableArray<Diagnostic>.Empty);
    }

    private static ApplicationHostSelectionResult InvalidApplicationHostResult(
        ApplicationLanguageHostInfo host,
        string reason)
    {
        return new ApplicationHostSelectionResult(
            null,
            [InvalidApplicationHost(host, reason)]);
    }

    private static Diagnostic InvalidApplicationHost(
        ApplicationLanguageHostInfo host,
        string reason)
    {
        return Diagnostic.Create(
            AtomUIDiagnosticDescriptors.LocalizationInvalidApplicationHost,
            host.Location,
            host.MetadataName,
            reason);
    }

    private static string GetLanguage(LanguageFileInput file)
    {
        return file.Document.TargetLanguage ?? file.Document.SourceLanguage;
    }

    private static ImmutableArray<Diagnostic> OrderDiagnostics(
        IEnumerable<Diagnostic> diagnostics)
    {
        return diagnostics.OrderBy(static diagnostic => diagnostic.Location.GetLineSpan().Path, StringComparer.Ordinal)
                          .ThenBy(static diagnostic =>
                              diagnostic.Location.GetLineSpan().StartLinePosition.Line)
                          .ThenBy(static diagnostic =>
                              diagnostic.Location.GetLineSpan().StartLinePosition.Character)
                          .ThenBy(static diagnostic => diagnostic.Id, StringComparer.Ordinal)
                          .ThenBy(static diagnostic => diagnostic.GetMessage(), StringComparer.Ordinal)
                          .ToImmutableArray();
    }

    private sealed class ApplicationHostSelectionResult
    {
        internal static readonly ApplicationHostSelectionResult Empty = new(
            null,
            ImmutableArray<Diagnostic>.Empty);

        internal ApplicationHostSelectionResult(
            ApplicationLanguageHostInfo? host,
            ImmutableArray<Diagnostic> diagnostics)
        {
            Host = host;
            Diagnostics = diagnostics;
        }

        internal ApplicationLanguageHostInfo? Host { get; }

        internal ImmutableArray<Diagnostic> Diagnostics { get; }
    }
}
