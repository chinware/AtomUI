namespace AtomUI.Localization;

public interface ILocalizationBuilder
{
    void AddCatalog(LanguageCatalogDescriptor descriptor);

    void AddTranslationBundle(TranslationBundleDescriptor descriptor);

    void AddLanguageDefinition(LanguageDefinition definition);

    void ConfigureLanguages(
        LanguageTag defaultLanguage,
        IEnumerable<LanguageTag> supportedLanguages);
}
