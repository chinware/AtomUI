namespace AtomUI.Localization;

internal static class StandardLanguageDefinitions
{
    internal static bool TryCreate(LanguageTag tag, out LanguageDefinition? definition)
    {
        if (tag == default)
        {
            definition = null;
            return false;
        }

        return GeneratedStandardLanguageDefinitions.TryCreate(tag, out definition);
    }
}
