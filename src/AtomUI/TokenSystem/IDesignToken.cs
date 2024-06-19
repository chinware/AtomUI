using Avalonia.Controls;

namespace AtomUI.TokenSystem;

public interface IDesignToken
{
   public void BuildResourceDictionary(IResourceDictionary dictionary);
   public object? GetTokenValue(string key);
   public void SetTokenValue(string key, object value);
   public AbstractDesignToken Clone();
}