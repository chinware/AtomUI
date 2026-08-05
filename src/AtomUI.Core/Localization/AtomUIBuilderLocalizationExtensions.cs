using AtomUI.Localization;

namespace AtomUI;

public static class AtomUIBuilderLocalizationExtensions
{
    public static IAtomUIBuilder UseLanguages(
        this IAtomUIBuilder builder,
        LanguageTag defaultLanguage,
        IEnumerable<LanguageTag> supportedLanguages)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.Localization.ConfigureLanguages(defaultLanguage, supportedLanguages);
        return builder;
    }
}
