using System.Collections.Immutable;
using AtomUI.Generator.Localization.Catalog;
using Microsoft.CodeAnalysis;

namespace AtomUI.Generator.Localization;

internal sealed class LocalizationGenerationResult
{
    internal LocalizationGenerationResult(
        string? assemblyName,
        LocalizationCompilationPlan plan,
        ImmutableArray<Diagnostic> diagnostics)
    {
        AssemblyName = assemblyName;
        Plan = plan;
        Diagnostics = diagnostics;
    }

    internal string? AssemblyName { get; }

    internal LocalizationCompilationPlan Plan { get; }

    internal ImmutableArray<Diagnostic> Diagnostics { get; }
}

internal sealed class LocalizationCompilationPlan
{
    internal static readonly LocalizationCompilationPlan Empty = new(
        ImmutableArray<CompiledLanguageCatalog>.Empty,
        applicationHost: null);

    internal LocalizationCompilationPlan(
        ImmutableArray<CompiledLanguageCatalog> catalogs,
        ApplicationLanguageHostInfo? applicationHost)
    {
        Catalogs = catalogs;
        ApplicationHost = applicationHost;
    }

    internal ImmutableArray<CompiledLanguageCatalog> Catalogs { get; }

    internal ApplicationLanguageHostInfo? ApplicationHost { get; }
}
