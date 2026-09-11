using System.Diagnostics.CodeAnalysis;

namespace AtomUI.Localization;

internal static class StandardLanguageDefinitions
{
    internal static bool TryCreate(
        LanguageTag tag,
        [NotNullWhen(true)] out LanguageDefinition? definition)
    {
        if (tag == default)
        {
            definition = null;
            return false;
        }

        return GeneratedStandardLanguageDefinitions.TryCreate(tag, out definition);
    }
}
