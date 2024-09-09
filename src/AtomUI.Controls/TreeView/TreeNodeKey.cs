namespace AtomUI.Controls;

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
        if (obj is TreeNodeKey other) return Equals(other);

        if (obj is string str) return Value == str;

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

    public override string ToString()
    {
        return Value;
    }
}