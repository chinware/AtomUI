namespace AtomUI.Theme.TokenSystem;

public readonly struct TokenResourceKey : IEquatable<TokenResourceKey>
{
   private readonly string _value;
   private readonly string _namespace;
   public const string DefaultNamespace = $"{ResourceNamespace.Root}.{ResourceNamespace.Token}";

   public TokenResourceKey(string value, string ns = DefaultNamespace)
   {
      _value = value;
      _namespace = ns;
   }

   public string Value => _value;

   public bool Equals(TokenResourceKey other)
   {
      return _namespace == other._namespace && _value == other._value;
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
      return $"{_namespace}:{_value}";
   }

   public string UnQualifiedKey()
   {
      return _value;
   }
}