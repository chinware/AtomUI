namespace AtomUI.Theme.TokenSystem;

public readonly struct TokenResourceKey : IEquatable<TokenResourceKey>
{
    public const string DefaultResourceCatalog = $"{ResourceCatalogConstants.Root}.{ResourceCatalogConstants.Token}";

    public TokenResourceKey(string value, string catalog = DefaultResourceCatalog)
    {
        Value   = value;
        Catalog = catalog;
    }

    public string Value { get; }

    public string Catalog { get; }

    public bool Equals(TokenResourceKey other)
    {
        return Catalog == other.Catalog && Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        if (obj is TokenResourceKey other) return Equals(other);

        if (obj is string str) return Value == str;

        return false;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public static bool operator ==(TokenResourceKey left, TokenResourceKey right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(TokenResourceKey left, TokenResourceKey right)
    {
        return !left.Equals(right);
    }

    public static bool operator ==(TokenResourceKey left, string right)
    {
        return left.Equals(new TokenResourceKey(right));
    }

    public static bool operator !=(TokenResourceKey left, string right)
    {
        return !left.Equals(new TokenResourceKey(right));
    }

    public override string ToString()
    {
        return $"{Catalog}:{Value}";
    }

    public string UnQualifiedKey()
    {
        return Value;
    }
}