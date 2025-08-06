namespace AtomUI.Theme.Language;

public readonly struct LanguageResourceKey : IEquatable<LanguageResourceKey>
{
    public const string DefaultResourceCatalog = $"{ResourceCatalogConstants.Root}.{ResourceCatalogConstants.Language}";

    public LanguageResourceKey(string value, string catalog = DefaultResourceCatalog)
    {
        Value   = value;
        Catalog = catalog;
    }

    public string Value { get; }

    public string Catalog { get; }

    public bool Equals(LanguageResourceKey other)
    {
        return Catalog == other.Catalog && Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        if (obj is LanguageResourceKey other)
        {
            return Equals(other);
        }

        if (obj is string str)
        {
            return Value == str;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public static bool operator ==(LanguageResourceKey left, LanguageResourceKey right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(LanguageResourceKey left, LanguageResourceKey right)
    {
        return !left.Equals(right);
    }

    public static bool operator ==(LanguageResourceKey left, string right)
    {
        return left.Equals(new LanguageResourceKey(right));
    }

    public static bool operator !=(LanguageResourceKey left, string right)
    {
        return !left.Equals(new LanguageResourceKey(right));
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