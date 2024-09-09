namespace AtomUI.Theme.TokenSystem;

public interface IControlDesignToken : IDesignToken
{
    public string Id { get; }
    public bool IsCustomTokenConfig { get; }
    public IList<string> CustomTokens { get; }
    public void AssignGlobalToken(AliasDesignToken globalToken);
    public bool HasToken(string tokenName);
}