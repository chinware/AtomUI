namespace AtomUI.Desktop.Controls;

internal static class DataGridIdentityValidation
{
    public static string Validate(string value, string parameterName)
    {
        ArgumentNullException.ThrowIfNull(value, parameterName);
        if (value.Length == 0)
        {
            throw new ArgumentException("The identity cannot be empty.", parameterName);
        }
        if (char.IsWhiteSpace(value[0]) || char.IsWhiteSpace(value[^1]))
        {
            throw new ArgumentException("The identity cannot have leading or trailing whitespace.", parameterName);
        }
        foreach (var character in value)
        {
            if (char.IsControl(character))
            {
                throw new ArgumentException("The identity cannot contain control characters.", parameterName);
            }
        }
        return value;
    }
}

public readonly struct DataGridFieldId : IEquatable<DataGridFieldId>
{
    private readonly string? _value;

    public DataGridFieldId(string value)
    {
        _value = DataGridIdentityValidation.Validate(value, nameof(value));
    }

    public string Value => _value ?? throw new InvalidOperationException("The field identity is invalid.");

    public bool IsValid => _value is not null;

    public bool Equals(DataGridFieldId other) =>
        string.Equals(_value, other._value, StringComparison.Ordinal);

    public override bool Equals(object? obj) => obj is DataGridFieldId other && Equals(other);

    public override int GetHashCode() =>
        _value is null ? 0 : StringComparer.Ordinal.GetHashCode(_value);

    public override string ToString() => _value ?? string.Empty;

    public static bool operator ==(DataGridFieldId left, DataGridFieldId right) => left.Equals(right);

    public static bool operator !=(DataGridFieldId left, DataGridFieldId right) => !left.Equals(right);
}

public readonly struct DataGridOperatorId : IEquatable<DataGridOperatorId>
{
    private readonly string? _value;

    public DataGridOperatorId(string value)
    {
        _value = DataGridIdentityValidation.Validate(value, nameof(value));
    }

    public string Value => _value ?? throw new InvalidOperationException("The operator identity is invalid.");

    public bool IsValid => _value is not null;

    public bool Equals(DataGridOperatorId other) =>
        string.Equals(_value, other._value, StringComparison.Ordinal);

    public override bool Equals(object? obj) => obj is DataGridOperatorId other && Equals(other);

    public override int GetHashCode() =>
        _value is null ? 0 : StringComparer.Ordinal.GetHashCode(_value);

    public override string ToString() => _value ?? string.Empty;

    public static bool operator ==(DataGridOperatorId left, DataGridOperatorId right) => left.Equals(right);

    public static bool operator !=(DataGridOperatorId left, DataGridOperatorId right) => !left.Equals(right);
}

public enum DataGridRowKeyKind : byte
{
    Invalid,
    String,
    SignedInteger,
    UnsignedInteger,
    Guid
}

public readonly struct DataGridRowKey : IEquatable<DataGridRowKey>, IComparable<DataGridRowKey>
{
    private readonly string? _stringValue;
    private readonly long _signedValue;
    private readonly ulong _unsignedValue;
    private readonly Guid _guidValue;

    private DataGridRowKey(
        DataGridRowKeyKind kind,
        string? stringValue = null,
        long signedValue = 0,
        ulong unsignedValue = 0,
        Guid guidValue = default)
    {
        Kind = kind;
        _stringValue = stringValue;
        _signedValue = signedValue;
        _unsignedValue = unsignedValue;
        _guidValue = guidValue;
    }

    public DataGridRowKeyKind Kind { get; }

    public bool IsValid => Kind != DataGridRowKeyKind.Invalid;

    public static DataGridRowKey FromString(string value) =>
        new(DataGridRowKeyKind.String, DataGridIdentityValidation.Validate(value, nameof(value)));

    public static DataGridRowKey FromInt64(long value) =>
        new(DataGridRowKeyKind.SignedInteger, signedValue: value);

    public static DataGridRowKey FromUInt64(ulong value) =>
        new(DataGridRowKeyKind.UnsignedInteger, unsignedValue: value);

    public static DataGridRowKey FromGuid(Guid value) =>
        new(DataGridRowKeyKind.Guid, guidValue: value);

    public bool Equals(DataGridRowKey other)
    {
        if (Kind != other.Kind)
        {
            return false;
        }
        return Kind switch
        {
            DataGridRowKeyKind.Invalid => true,
            DataGridRowKeyKind.String => string.Equals(
                _stringValue, other._stringValue, StringComparison.Ordinal),
            DataGridRowKeyKind.SignedInteger => _signedValue == other._signedValue,
            DataGridRowKeyKind.UnsignedInteger => _unsignedValue == other._unsignedValue,
            DataGridRowKeyKind.Guid => _guidValue == other._guidValue,
            _ => false
        };
    }

    public override bool Equals(object? obj) => obj is DataGridRowKey other && Equals(other);

    public override int GetHashCode() => Kind switch
    {
        DataGridRowKeyKind.Invalid => 0,
        DataGridRowKeyKind.String => HashCode.Combine(
            Kind, StringComparer.Ordinal.GetHashCode(_stringValue!)),
        DataGridRowKeyKind.SignedInteger => HashCode.Combine(Kind, _signedValue),
        DataGridRowKeyKind.UnsignedInteger => HashCode.Combine(Kind, _unsignedValue),
        DataGridRowKeyKind.Guid => HashCode.Combine(Kind, _guidValue),
        _ => 0
    };

    public int CompareTo(DataGridRowKey other)
    {
        var kind = Kind.CompareTo(other.Kind);
        if (kind != 0)
        {
            return kind;
        }
        return Kind switch
        {
            DataGridRowKeyKind.Invalid => 0,
            DataGridRowKeyKind.String => string.Compare(
                _stringValue, other._stringValue, StringComparison.Ordinal),
            DataGridRowKeyKind.SignedInteger => _signedValue.CompareTo(other._signedValue),
            DataGridRowKeyKind.UnsignedInteger => _unsignedValue.CompareTo(other._unsignedValue),
            DataGridRowKeyKind.Guid => _guidValue.CompareTo(other._guidValue),
            _ => throw new InvalidOperationException($"Unknown row key kind '{Kind}'.")
        };
    }

    public override string ToString() => Kind switch
    {
        DataGridRowKeyKind.String => _stringValue!,
        DataGridRowKeyKind.SignedInteger => _signedValue.ToString(System.Globalization.CultureInfo.InvariantCulture),
        DataGridRowKeyKind.UnsignedInteger => _unsignedValue.ToString(System.Globalization.CultureInfo.InvariantCulture),
        DataGridRowKeyKind.Guid => _guidValue.ToString("D"),
        _ => string.Empty
    };

    public static bool operator ==(DataGridRowKey left, DataGridRowKey right) => left.Equals(right);

    public static bool operator !=(DataGridRowKey left, DataGridRowKey right) => !left.Equals(right);
}

public readonly struct DataGridGroupKey : IEquatable<DataGridGroupKey>
{
    private readonly string? _value;

    public DataGridGroupKey(string value)
    {
        _value = DataGridIdentityValidation.Validate(value, nameof(value));
    }

    public string Value => _value ?? throw new InvalidOperationException("The group identity is invalid.");

    public bool IsValid => _value is not null;

    public bool Equals(DataGridGroupKey other) =>
        string.Equals(_value, other._value, StringComparison.Ordinal);

    public override bool Equals(object? obj) => obj is DataGridGroupKey other && Equals(other);

    public override int GetHashCode() =>
        _value is null ? 0 : StringComparer.Ordinal.GetHashCode(_value);

    public override string ToString() => _value ?? string.Empty;

    public static bool operator ==(DataGridGroupKey left, DataGridGroupKey right) => left.Equals(right);

    public static bool operator !=(DataGridGroupKey left, DataGridGroupKey right) => !left.Equals(right);
}

public readonly struct DataGridSnapshotId : IEquatable<DataGridSnapshotId>
{
    private readonly string? _value;

    public DataGridSnapshotId(string value)
    {
        _value = DataGridIdentityValidation.Validate(value, nameof(value));
    }

    public string Value => _value ?? throw new InvalidOperationException("The snapshot identity is invalid.");

    public bool IsValid => _value is not null;

    public bool Equals(DataGridSnapshotId other) =>
        string.Equals(_value, other._value, StringComparison.Ordinal);

    public override bool Equals(object? obj) => obj is DataGridSnapshotId other && Equals(other);

    public override int GetHashCode() =>
        _value is null ? 0 : StringComparer.Ordinal.GetHashCode(_value);

    public override string ToString() => _value ?? string.Empty;

    public static bool operator ==(DataGridSnapshotId left, DataGridSnapshotId right) => left.Equals(right);

    public static bool operator !=(DataGridSnapshotId left, DataGridSnapshotId right) => !left.Equals(right);
}
