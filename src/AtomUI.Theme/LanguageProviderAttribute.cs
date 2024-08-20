namespace AtomUI.Theme;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class LanguageProviderAttribute : Attribute
{
   public const string DefaultLanguageId = "Default";
   public const string DefaultResourceCatalog = $"{ResourceCatalogConstants.Root}.{ResourceCatalogConstants.Token}";
   public string LanguageCode { get; }
   public string LanguageId { get; }
   public string ResourceCatalog { get; }

   public LanguageProviderAttribute(string languageCode, string languageId = DefaultLanguageId, 
                                    string resourceCatalog = DefaultResourceCatalog)
   {
      LanguageCode = languageCode;
      LanguageId = languageId;
      ResourceCatalog = resourceCatalog;
   }
}