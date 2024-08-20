namespace AtomUI.Generator;

public class ControlTokenInfo
{
   public string? ControlNamespace { get; set; }
   public string ControlName { get; set; }
   public HashSet<TokenName> Tokens { get; private set; }

   public ControlTokenInfo(string controlName, HashSet<TokenName> tokens)
   {
      ControlName = controlName;
      Tokens = tokens;
   }
   
   public ControlTokenInfo()
      : this(string.Empty, new HashSet<TokenName>())
   {
   }

   public void AddToken(TokenName tokenName)
   {
      Tokens.Add(tokenName);
   }
}

public class TokenInfo
{
   public HashSet<TokenName> Tokens { get; private set; }
   public List<ControlTokenInfo> ControlTokenInfos { get; private set; }

   public TokenInfo()
   {
      Tokens = new HashSet<TokenName>();
      ControlTokenInfos = new List<ControlTokenInfo>();
   }
}

public record TokenName
{
   public string Name { get; }
   public string ResourceNamespace { get; }

   public TokenName(string name, string resourceNamespace)
   {
      Name = name;
      ResourceNamespace = resourceNamespace;
   }
}