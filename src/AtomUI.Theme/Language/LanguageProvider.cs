using System.Reflection;
using Avalonia.Controls;

namespace AtomUI.Theme.Language;

public abstract class LanguageProvider : ILanguageProvider
{
    public LanguageCode LangCode { get; }

    public string LangId { get; }

    public string ResourceCatalog { get; }

    public LanguageProvider()
    {
        var type                      = GetType();
        var languageProviderAttribute = type.GetCustomAttribute<LanguageProviderAttribute>();
        if (languageProviderAttribute is null)
        {
            throw new LanguageMetaInfoParseException("No annotations found LanguageProviderAttribute");
        }

        LangCode        = languageProviderAttribute.LanguageCode;
        LangId          = languageProviderAttribute.LanguageId;
        ResourceCatalog = languageProviderAttribute.ResourceCatalog;
    }

    public void BuildResourceDictionary(IResourceDictionary dictionary)
    {
        var type = GetType();
        var languageFields = type.GetFields(BindingFlags.Public |
                                            BindingFlags.Static |
                                            BindingFlags.FlattenHierarchy);
        foreach (var field in languageFields)
        {
            var languageKey  = $"{LangId}.{field.Name}";
            var languageText = field.GetValue(this);
            dictionary[new LanguageResourceKey(languageKey, ResourceCatalog)] = languageText;
        }
    }
}