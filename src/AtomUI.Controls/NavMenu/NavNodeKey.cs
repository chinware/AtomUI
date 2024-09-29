namespace AtomUI.Controls;

public readonly struct NavNodeKey : IEquatable<NavNodeKey>
{
    public NavNodeKey(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public bool Equals(NavNodeKey other)
    {
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        if (obj is NavNodeKey other)
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

    public static bool operator ==(NavNodeKey left, NavNodeKey right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(NavNodeKey left, NavNodeKey right)
    {
        return !left.Equals(right);
    }

    public static bool operator ==(NavNodeKey left, string right)
    {
        return left.Equals(new NavNodeKey(right));
    }

    public static bool operator !=(NavNodeKey left, string right)
    {
        return !left.Equals(new NavNodeKey(right));
    }

    public override string ToString()
    {
        return Value;
    }
}