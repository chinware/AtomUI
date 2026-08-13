using Microsoft.CodeAnalysis.Diagnostics;

namespace AtomUI.Generator.LinkedRegistration;

internal static class LinkedRegistrationOptions
{
    private const string PackageIdProperty = "build_property.AtomUIRegistrationPackageId";
    private const string RegistrationEntriesProperty = "build_property.AtomUIRegistrationEntries";
    private const string LinkedPublishProperty = "build_property.AtomUILinkedPublish";
    private const string RegistrationStrictProperty = "build_property.AtomUIRegistrationStrict";
    private const string RegistrationPlanOwnerProperty =
        "build_property.AtomUIRegistrationPlanOwner";
    private const string PackageSharedThemePathsProperty =
        "build_property.AtomUIPackageSharedThemePaths";
    private const string ProjectDirProperty = "build_property.ProjectDir";
    private const string MSBuildProjectDirectoryProperty =
        "build_property.MSBuildProjectDirectory";

    internal static string GetPackageId(
        AnalyzerConfigOptionsProvider optionsProvider,
        string assemblyName)
    {
        return optionsProvider.GlobalOptions.TryGetValue(PackageIdProperty, out var value) &&
               !string.IsNullOrWhiteSpace(value)
            ? value.Trim()
            : assemblyName;
    }

    internal static string GetDeclaredPackageId(
        AnalyzerConfigOptionsProvider optionsProvider)
    {
        return TryGetNonEmptyValue(optionsProvider, PackageIdProperty, out var packageId)
            ? packageId
            : string.Empty;
    }

    internal static string GetRegistrationEntries(
        AnalyzerConfigOptionsProvider optionsProvider)
    {
        if (!optionsProvider.GlobalOptions.TryGetValue(RegistrationEntriesProperty, out var value) ||
            string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return string.Join(
            ";",
            value.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                 .Select(static entry => entry.Trim())
                 .Where(static entry => entry.Length != 0)
                 .Distinct(StringComparer.Ordinal)
                 .OrderBy(static entry => entry, StringComparer.Ordinal));
    }

    internal static ISet<string> GetPackageSharedThemePaths(
        AnalyzerConfigOptionsProvider optionsProvider)
    {
        if (!optionsProvider.GlobalOptions.TryGetValue(PackageSharedThemePathsProperty, out var value) ||
            string.IsNullOrWhiteSpace(value))
        {
            return new HashSet<string>(StringComparer.Ordinal);
        }

        return new HashSet<string>(
            value.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                 .Select(static path => path.Replace('\\', '/').Trim().TrimStart('/'))
                 .Where(static path => path.Length != 0),
            StringComparer.Ordinal);
    }

    internal static bool IsLinkedPublish(AnalyzerConfigOptionsProvider optionsProvider)
    {
        return GetBoolean(optionsProvider, LinkedPublishProperty);
    }

    internal static bool IsRegistrationStrict(AnalyzerConfigOptionsProvider optionsProvider)
    {
        return GetBoolean(optionsProvider, RegistrationStrictProperty);
    }

    internal static bool IsRegistrationPlanOwner(AnalyzerConfigOptionsProvider optionsProvider)
    {
        return GetBoolean(optionsProvider, RegistrationPlanOwnerProperty);
    }

    internal static string GetProjectDirectory(AnalyzerConfigOptionsProvider optionsProvider)
    {
        if (TryGetNonEmptyValue(optionsProvider, ProjectDirProperty, out var projectDirectory) ||
            TryGetNonEmptyValue(
                optionsProvider,
                MSBuildProjectDirectoryProperty,
                out projectDirectory))
        {
            return projectDirectory.Replace('\\', '/').TrimEnd('/') + "/";
        }
        return string.Empty;
    }

    private static bool TryGetNonEmptyValue(
        AnalyzerConfigOptionsProvider optionsProvider,
        string propertyName,
        out string value)
    {
        if (optionsProvider.GlobalOptions.TryGetValue(propertyName, out var rawValue) &&
            !string.IsNullOrWhiteSpace(rawValue))
        {
            value = rawValue.Trim();
            return true;
        }
        value = string.Empty;
        return false;
    }

    private static bool GetBoolean(
        AnalyzerConfigOptionsProvider optionsProvider,
        string propertyName)
    {
        return optionsProvider.GlobalOptions.TryGetValue(propertyName, out var value) &&
               string.Equals(value?.Trim(), "true", StringComparison.OrdinalIgnoreCase);
    }
}
