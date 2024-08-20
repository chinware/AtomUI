using Avalonia.Controls;

namespace AtomUI.Theme;

public interface ILanguageProvider
{
   public void BuildResourceDictionary(IResourceDictionary dictionary);
}