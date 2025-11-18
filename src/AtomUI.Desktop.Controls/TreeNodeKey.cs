using System.Globalization;
using AtomUI.Utils;

namespace AtomUI.Desktop.Controls;

public readonly struct TreeNodeKey : IEquatable<TreeNodeKey>
{
    public TreeNodeKey(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public bool Equals(TreeNodeKey other)
    {
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        if (obj is TreeNodeKey other)
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

    public static implicit operator TreeNodeKey(string value)
    {
        return new TreeNodeKey(value);
    }
    
    public override string ToString()
    {
        return Value;
    }
    
    public static TreeNodeKey Parse(string s)
    {
        using (var tokenizer = new SpanStringTokenizer(s, CultureInfo.InvariantCulture, exceptionMessage: "Invalid TreeNodeKey."))
        {
            return new TreeNodeKey(
                tokenizer.ReadString()
            );
        }
    }
}