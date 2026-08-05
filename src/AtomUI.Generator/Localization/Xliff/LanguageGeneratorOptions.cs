using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AtomUI.Generator.Localization.Xliff;

internal static class LanguageGeneratorOptions
{
    internal const string LanguageFileMetadata =
        "build_metadata.AdditionalFiles.AtomUILanguage";
    internal const string SourceKindMetadata =
        "build_metadata.AdditionalFiles.AtomUILanguageSourceKind";
    internal const string SourceIdentityMetadata =
        "build_metadata.AdditionalFiles.AtomUILanguageSourceIdentity";
    internal const string ModuleIdMetadata =
        "build_metadata.AdditionalFiles.AtomUILanguageModuleId";

    internal static bool IsLanguageFile(
        AdditionalText text,
        AnalyzerConfigOptionsProvider optionsProvider)
    {
        return optionsProvider.GetOptions(text)
                              .TryGetValue(LanguageFileMetadata, out var value) &&
               string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
    }

    internal static string GetModuleId(
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

    internal static string GetFileValue(
        AnalyzerConfigOptions options,
        string key,
        string fallback)
    {
        return TryGetNonEmpty(options, key, out var value) ? value : fallback;
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
