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
    internal const string ContractValidationMetadata =
        "build_metadata.AdditionalFiles.AtomUILanguageContractValidation";
    internal const string SourceFingerprintMetadata =
        "build_metadata.AdditionalFiles.AtomUILanguageSourceFingerprint";

    internal static bool IsLanguageFile(
        AdditionalText text,
        AnalyzerConfigOptionsProvider optionsProvider)
    {
        var options = optionsProvider.GetOptions(text);
        if (!options.TryGetValue(LanguageFileMetadata, out var value) ||
            !string.Equals(value, "true", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        // A static language-pack project validates and packages XLIFF but does
        // not compile a Catalog or runtime registration. The same files are
        // consumed by the generator in the application that references the
        // resulting package.
        return !IsLanguagePackBuild(optionsProvider.GlobalOptions);
    }

    private static bool IsLanguagePackBuild(AnalyzerConfigOptions options)
    {
        return options.TryGetValue("build_property.AtomUIBuildLanguagePackage", out var value) &&
               string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
    }

    internal static string GetModuleId(
        AnalyzerConfigOptionsProvider optionsProvider,
        string fallbackAssemblyName)
    {
        if (TryGetNonEmpty(optionsProvider.GlobalOptions, "build_property.AtomUILanguageModuleId", out var moduleId))
        {
            return moduleId;
        }

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
