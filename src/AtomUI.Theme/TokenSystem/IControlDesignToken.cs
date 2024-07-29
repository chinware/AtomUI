namespace AtomUI.Theme.TokenSystem;

public interface IControlDesignToken : IDesignToken
{
   public string Id { get; }
   public void AssignGlobalToken(AliasDesignToken globalToken);
   public bool IsCustomTokenConfig { get; }
   public IList<string> CustomTokens { get; }
   public bool HasToken(string tokenName);
}