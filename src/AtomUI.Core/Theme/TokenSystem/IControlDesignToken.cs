using Avalonia.Controls;

namespace AtomUI.Theme.TokenSystem;

public interface IControlDesignToken : IDesignToken
{
    string GetId();
    void AssignSharedToken(DesignToken sharedToken);
    bool HasCustomTokenConfig();
    void SetHasCustomTokenConfig(bool value);
    IList<string> GetCustomTokens();
    void SetCustomTokens(IList<string> customTokens);
    bool HasToken(string tokenName);
    IResourceDictionary GetSharedResourceDeltaDictionary();
}