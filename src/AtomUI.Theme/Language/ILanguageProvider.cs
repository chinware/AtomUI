using Avalonia.Controls;

namespace AtomUI.Theme.Language;

public interface ILanguageProvider
{
    public LanguageCode LangCode { get; }
    public string LangId { get; }
    public string ResourceCatalog { get; }
    public void BuildResourceDictionary(IResourceDictionary dictionary);
}