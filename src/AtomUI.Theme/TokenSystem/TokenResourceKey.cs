namespace AtomUI.Theme.TokenSystem;

public readonly struct TokenResourceKey : IEquatable<TokenResourceKey>
{
   private readonly string _value;
   private readonly string _catalog;
   public const string DefaultResourceCatalog = $"{ResourceCatalogConstants.Root}.{ResourceCatalogConstants.Token}";

   public TokenResourceKey(string value, string catalog = DefaultResourceCatalog)
   {
      _value = value;
      _catalog = catalog;
   }

   public string Value => _value;
   public string Catalog => _catalog;

   public bool Equals(TokenResourceKey other)
   {
      return _catalog == other._catalog && _value == other._value;
   }

   public override bool Equals(object? obj)
   {
      if (obj is TokenResourceKey other) {
         return Equals(other);
      }

      if (obj is string str) {
         return _value == str;
      }

      return false;
   }

   public override int GetHashCode()
   {
      return _value.GetHashCode();
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
      return $"{_catalog}:{_value}";
   }

   public string UnQualifiedKey()
   {
      return _value;
   }
}