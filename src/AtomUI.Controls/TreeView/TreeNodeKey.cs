namespace AtomUI.Controls;

public readonly struct TreeNodeKey : IEquatable<TreeNodeKey>
{
   private readonly string _value;

   public TreeNodeKey(string value)
   {
      _value = value;
   }

   public string Value => _value;

   public bool Equals(TreeNodeKey other)
   {
      return _value == other._value;
   }

   public override bool Equals(object? obj)
   {
      if (obj is TreeNodeKey other) {
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

   public static bool operator ==(TreeNodeKey left, TreeNodeKey right)
   {
      return left.Equals(right);
   }

   public static bool operator !=(TreeNodeKey left, TreeNodeKey right)
   {
      return !left.Equals(right);
   }

   public static bool operator ==(TreeNodeKey left, string right)
   {
      return left.Equals(new TreeNodeKey(right));
   }

   public static bool operator !=(TreeNodeKey left, string right)
   {
      return !left.Equals(new TreeNodeKey(right));
   }

   public override string ToString()
   {
      return _value;
   }
}