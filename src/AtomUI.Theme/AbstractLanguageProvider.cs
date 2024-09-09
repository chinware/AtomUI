using System.Reflection;
using Avalonia.Controls;

namespace AtomUI.Theme;

public class AbstractLanguageProvider : ILanguageProvider
{
   private string _languageCode;
   private string _languageId;
   private string _resourceCatalog;

   public string LangCode => _languageCode;
   public string LangId => _languageId;
   public string ResourceCatalog => _resourceCatalog;

   public AbstractLanguageProvider()
   {
      var type = GetType();
      var languageProviderAttribute = type.GetCustomAttribute<LanguageProviderAttribute>();
      if (languageProviderAttribute is null) {
         throw new LanguageMetaInfoParseException("No annotations found LanguageProviderAttribute");
      }

      _languageCode = languageProviderAttribute.LanguageCode;
      _languageId = languageProviderAttribute.LanguageId;
      _resourceCatalog = languageProviderAttribute.ResourceCatalog;
   }

   public void BuildResourceDictionary(IResourceDictionary dictionary)
   {
      var type = GetType();
      var languageFields = type.GetFields(BindingFlags.Public |
                                          BindingFlags.Static |
                                          BindingFlags.FlattenHierarchy);
      foreach (var field in languageFields) {
         var languageKey = $"{_languageId}.{field.Name}";
         var languageText = field.GetValue(this);
         dictionary[new LanguageResourceKey(languageKey, _resourceCatalog)] = languageText;
      }
   }
}