namespace AtomUI.Theme;

public readonly struct LanguageResourceKey : IEquatable<LanguageResourceKey>
{
   private readonly string _value;
   private readonly string _catalog;
   public const string DefaultResourceCatalog = $"{ResourceCatalogConstants.Root}.{ResourceCatalogConstants.Language}";

   public LanguageResourceKey(string value, string catalog = DefaultResourceCatalog)
   {
      _value = value;
      _catalog = catalog;
   }

   public string Value => _value;
   public string Catalog => _catalog;

   public bool Equals(LanguageResourceKey other)
   {
      return _catalog == other._catalog && _value == other._value;
   }

   public override bool Equals(object? obj)
   {
      if (obj is LanguageResourceKey other) {
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
      return $"{_catalog}:{_value}";
   }

   public string UnQualifiedKey()
   {
      return _value;
   }
}
