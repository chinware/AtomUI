namespace AtomUI.Generator.LinkedRegistration.Model;

internal static class RegistrationUnitId
{
    internal static string Create(
        string packageId,
        string? sourcePath,
        string? projectDirectory,
        string fallbackName,
        string? explicitUnit = null)
    {
        if (!string.IsNullOrWhiteSpace(explicitUnit))
        {
            return Qualify(packageId, explicitUnit!);
        }

        var relativePath = NormalizeProjectPath(sourcePath, projectDirectory);
        var separator = relativePath.IndexOf('/');
        if (separator > 0)
        {
            var directory = relativePath.Substring(0, separator);
            if (!string.Equals(directory, "Themes", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(directory, "Properties", StringComparison.OrdinalIgnoreCase))
            {
                return Qualify(packageId, directory);
            }
        }

        return Qualify(packageId, fallbackName);
    }

    internal static string NormalizeProjectPath(string? path, string? projectDirectory)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return string.Empty;
        }

        var normalized = path!.Replace('\\', '/').TrimStart('/');
        if (!string.IsNullOrWhiteSpace(projectDirectory))
        {
            var normalizedProject = projectDirectory!.Replace('\\', '/').TrimEnd('/').TrimStart('/');
            if (normalized.StartsWith(normalizedProject + "/", StringComparison.OrdinalIgnoreCase))
            {
                normalized = normalized.Substring(normalizedProject.Length + 1);
            }
        }

        return normalized.StartsWith("./", StringComparison.Ordinal)
            ? normalized.Substring(2)
            : normalized;
    }

    private static string Qualify(string packageId, string unitName)
    {
        var normalizedName = unitName.Replace('\\', '/').Trim('/');
        return normalizedName.StartsWith(packageId + "/", StringComparison.Ordinal)
            ? normalizedName
            : packageId + "/" + normalizedName;
    }
}
