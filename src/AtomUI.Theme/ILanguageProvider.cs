using Avalonia.Controls;

namespace AtomUI.Theme;

public interface ILanguageProvider
{
   public string LangCode { get; }
   public string LangId { get; }
   public string ResourceCatalog { get; }
   
   public void BuildResourceDictionary(IResourceDictionary dictionary);
}