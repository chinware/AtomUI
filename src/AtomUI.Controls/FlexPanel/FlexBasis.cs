using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace AtomUI.Controls;

public readonly struct FlexBasis : IEquatable<FlexBasis>
{
    public double Value { get; }

    public FlexBasisKind Kind { get; }

    public FlexBasis(double value, FlexBasisKind kind)
    {
        if (value < 0 || double.IsNaN(value) || double.IsInfinity(value))
        {
            throw new ArgumentException($"Invalid basis value: {value}", nameof(value));
        }
        if (kind < FlexBasisKind.Auto || kind > FlexBasisKind.Relative)
        {
            throw new ArgumentException($"Invalid basis kind: {kind}", nameof(kind));
        }
        Value = value;
        Kind = kind;
    }

    public FlexBasis(double value) : this(value, FlexBasisKind.Absolute) { }

    public static FlexBasis Auto => new(0.0, FlexBasisKind.Auto);

    public bool IsAuto => Kind == FlexBasisKind.Auto;

    public bool IsAbsolute => Kind == FlexBasisKind.Absolute;

    public bool IsRelative => Kind == FlexBasisKind.Relative;

    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    public bool Equals(FlexBasis other) =>
        (IsAuto && other.IsAuto) || (Value == other.Value && Kind == other.Kind);

    public override bool Equals(object? obj) =>
        obj is FlexBasis other && Equals(other);

    public override int GetHashCode() =>
        (Value, Kind).GetHashCode();

    public static bool operator ==(FlexBasis left, FlexBasis right) =>
        left.Equals(right);

    public static bool operator !=(FlexBasis left, FlexBasis right) =>
        !left.Equals(right);

    public override string ToString()
    {
        return Kind switch
        {
            FlexBasisKind.Auto => "Auto",
            FlexBasisKind.Absolute => FormattableString.Invariant($"{Value:G17}"),
            FlexBasisKind.Relative => FormattableString.Invariant($"{Value * 100:G17}%"),
            _ => throw new InvalidOperationException(),
        };
    }

    public static FlexBasis Parse(string str)
    {
        if (str.Equals("AUTO", StringComparison.OrdinalIgnoreCase))
        {
            return Auto;
        }
        if (str.EndsWith("%", StringComparison.Ordinal))
        {
            return new FlexBasis(ParseDouble(str.AsSpan(0, str.Length - 1).TrimEnd()) / 100,
                FlexBasisKind.Relative);
        }
        return new FlexBasis(ParseDouble(str.AsSpan()), FlexBasisKind.Absolute);

        double ParseDouble(ReadOnlySpan<char> s) => double.Parse(s, CultureInfo.InvariantCulture);
    }
}
