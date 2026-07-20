namespace AtomUI.Theme.Definitions;

internal static class ThemeApplicationIdentity
{
    internal static string? ResolveDefault(Type? applicationType)
    {
        return applicationType?.Assembly.GetName().Name;
    }

    internal static void Validate(string applicationId, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(applicationId, parameterName);
        if (applicationId is "." or ".." ||
            applicationId.Any(static character => !IsAllowed(character)))
        {
            throw new ArgumentException(
                "Application id must contain only ASCII letters, digits, '.', '_' or '-'.",
                parameterName);
        }
    }

    private static bool IsAllowed(char character)
    {
        return character is >= 'a' and <= 'z' or
               >= 'A' and <= 'Z' or
               >= '0' and <= '9' or
               '.' or '_' or '-';
    }
}
