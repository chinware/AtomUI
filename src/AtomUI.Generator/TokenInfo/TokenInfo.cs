namespace AtomUI.Generator;

public class ControlTokenInfo
{
   public string ControlName { get; set; }
   public HashSet<string> Tokens { get; private set; }

   public ControlTokenInfo(string controlNam, HashSet<string> tokens)
   {
      ControlName = controlNam;
      Tokens = tokens;
   }
   
   public ControlTokenInfo()
      : this(string.Empty, new HashSet<string>())
   {
   }

   public void AddToken(string token)
   {
      Tokens.Add(token);
   }
}

public class TokenInfo
{
   public HashSet<string> Tokens { get; private set; }
   public List<ControlTokenInfo> ControlTokenInfos { get; private set; }

   public TokenInfo()
   {
      Tokens = new HashSet<string>();
      ControlTokenInfos = new List<ControlTokenInfo>();
   }
}