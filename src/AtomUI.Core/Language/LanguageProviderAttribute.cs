namespace AtomUI.Theme.Language;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class LanguageProviderAttribute : Attribute
{
    public const string DefaultLanguageId = "Default";
    public const string DefaultResourceCatalog = $"{ResourceCatalogConstants.Root}.{ResourceCatalogConstants.Language}";
    public LanguageCode LanguageCode { get; }
    public string LanguageId { get; }
    public string ResourceCatalog { get; }

    public LanguageProviderAttribute(LanguageCode languageCode, 
                                     string languageId = DefaultLanguageId,
                                     string resourceCatalog = DefaultResourceCatalog)
    {
        LanguageCode    = languageCode;
        LanguageId      = languageId;
        ResourceCatalog = resourceCatalog;
    }
}