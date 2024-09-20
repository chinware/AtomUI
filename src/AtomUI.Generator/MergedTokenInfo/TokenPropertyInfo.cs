namespace AtomUI.Generator.MergedTokenInfo;

public struct TokenPropertyDef : IEquatable<TokenPropertyDef>
{
    public string TokenName { get; set; }
    public string DefText { get; set; }
    public List<string> Comments { get; set; }

    public bool Equals(TokenPropertyDef other)
    {
        return TokenName == other.TokenName;
    }

    public override bool Equals(object? obj)
    {
        if (obj is TokenPropertyDef other)
        {
            return Equals(other);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return TokenName.GetHashCode();
    }
}

public class TokenPropertyInfo
{
    public string ClassName { get; set; } = string.Empty;
    public int Priority { get; set; } = 10000;
    public List<string> Usings { get; set; } = new List<string>();
    public List<TokenPropertyDef> Definitions { get; set; } = new List<TokenPropertyDef>();
}