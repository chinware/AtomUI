using System.Text;

namespace AtomUI.Generator.LinkedRegistration;

internal static class LinkedRegistrationFragmentName
{
    internal static string ForUnit(string unitId)
    {
        var separator = unitId.LastIndexOf('/');
        var unitName = separator >= 0 ? unitId.Substring(separator + 1) : unitId;
        return $"GeneratedRegistrationUnit_{ToIdentifier(unitName)}_{ComputeHash(unitId):X16}";
    }

    internal static string ForPackageShared(string packageId)
    {
        return $"GeneratedPackageSharedThemeFragment_{ComputeHash(packageId):X16}";
    }

    private static ulong ComputeHash(string value)
    {
        var hash = 14695981039346656037UL;
        foreach (var character in value)
        {
            hash ^= character;
            hash *= 1099511628211UL;
        }
        return hash;
    }

    private static string ToIdentifier(string value)
    {
        var builder = new StringBuilder(value.Length + 1);
        foreach (var character in value)
        {
            builder.Append(char.IsLetterOrDigit(character) || character == '_' ? character : '_');
        }
        if (builder.Length == 0 || char.IsDigit(builder[0]))
        {
            builder.Insert(0, '_');
        }
        return builder.ToString();
    }
}
