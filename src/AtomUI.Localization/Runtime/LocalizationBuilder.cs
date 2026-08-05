namespace AtomUI.Localization;

internal sealed class LocalizationBuilder : ILocalizationBuilder
{
    private readonly List<LanguageCatalogDescriptor> _catalogs = [];
    private readonly List<TranslationBundleDescriptor> _bundles = [];
    private readonly List<LanguageDefinition> _definitions = [];
    private LanguageCatalogRegistry? _registry;
    private LanguageTag _defaultLanguage;
    private LanguageTag[]? _supportedLanguages;
    private bool _isFrozen;

    public void AddCatalog(LanguageCatalogDescriptor descriptor)
    {
        EnsureMutable();
        ArgumentNullException.ThrowIfNull(descriptor);
        _catalogs.Add(descriptor);
    }

    public void AddTranslationBundle(TranslationBundleDescriptor descriptor)
    {
        EnsureMutable();
        ArgumentNullException.ThrowIfNull(descriptor);
        _bundles.Add(descriptor);
    }

    public void AddLanguageDefinition(LanguageDefinition definition)
    {
        EnsureMutable();
        ArgumentNullException.ThrowIfNull(definition);
        _definitions.Add(definition);
    }

    public void ConfigureLanguages(
        LanguageTag defaultLanguage,
        IEnumerable<LanguageTag> supportedLanguages)
    {
        EnsureMutable();
        if (defaultLanguage == default)
        {
            throw new ArgumentException("A valid default language is required.", nameof(defaultLanguage));
        }

        ArgumentNullException.ThrowIfNull(supportedLanguages);
        _defaultLanguage = defaultLanguage;
        _supportedLanguages = supportedLanguages.ToArray();
    }

    internal LanguageCatalogRegistry FreezeRegistry()
    {
        if (_registry is not null)
        {
            return _registry;
        }

        _isFrozen = true;
        _registry = LanguageCatalogRegistry.Create(_catalogs, _bundles);
        return _registry;
    }

    internal IReadOnlyList<LanguageDefinition> Definitions => _definitions;

    internal LanguageTag ConfiguredDefaultLanguage => _defaultLanguage;

    internal IReadOnlyList<LanguageTag>? ConfiguredSupportedLanguages => _supportedLanguages;

    private void EnsureMutable()
    {
        if (_isFrozen)
        {
            throw new InvalidOperationException("The localization builder has already been frozen.");
        }
    }
}
