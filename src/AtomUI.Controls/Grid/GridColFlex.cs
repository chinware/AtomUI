using System.ComponentModel;
using System.Globalization;

namespace AtomUI.Controls;

[TypeConverter(typeof(GridColFlexConverter))]
public readonly record struct GridColFlex
{
    public GridColFlex(double grow)
        : this(grow, grow, null)
    {
    }

    private GridColFlex(double grow, double shrink, double? basis)
    {
        ValidateFlexValue(grow, nameof(grow));
        ValidateFlexValue(shrink, nameof(shrink));
        if (basis.HasValue)
        {
            ValidateBasis(basis.Value);
        }

        Grow   = grow;
        Shrink = shrink;
        Basis  = basis;
    }

    public double Grow { get; }
    public double Shrink { get; }
    public double? Basis { get; }

    public static GridColFlex Auto { get; } = new(1, 1, null);
    public static GridColFlex None { get; } = new(0, 0, null);

    public static GridColFlex Parse(string input)
    {
        var trimmed = input.AsSpan().Trim();
        if (trimmed.IsEmpty)
        {
            throw new FormatException("Grid column flex value cannot be empty.");
        }

        if (trimmed.Equals("auto".AsSpan(), StringComparison.OrdinalIgnoreCase))
        {
            return Auto;
        }

        if (trimmed.Equals("none".AsSpan(), StringComparison.OrdinalIgnoreCase))
        {
            return None;
        }

        if (trimmed.EndsWith("px".AsSpan(), StringComparison.OrdinalIgnoreCase))
        {
            var valueSpan = trimmed[..^2].Trim();
            if (!double.TryParse(valueSpan, NumberStyles.Float, CultureInfo.InvariantCulture, out var basis))
            {
                throw new FormatException($"Invalid flex basis value '{input}'.");
            }

            return new GridColFlex(0, 0, basis);
        }

        if (double.TryParse(trimmed, NumberStyles.Float, CultureInfo.InvariantCulture, out var grow))
        {
            return new GridColFlex(grow);
        }

        throw new FormatException($"Invalid grid column flex value '{input}'.");
    }

    public static implicit operator GridColFlex(double grow) => new(grow);
    public static implicit operator GridColFlex(int grow) => new(grow);

    private static void ValidateFlexValue(double value, string name)
    {
        if (double.IsNaN(value) || double.IsInfinity(value) || value < 0)
        {
            throw new FormatException($"Flex {name} must be >= 0, got {value}.");
        }
    }

    private static void ValidateBasis(double value)
    {
        if (double.IsNaN(value) || double.IsInfinity(value) || value < 0)
        {
            throw new FormatException($"Flex basis must be >= 0, got {value}.");
        }
    }
}

public class GridColFlexConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        var typeCode = Type.GetTypeCode(sourceType);
        return typeCode switch
        {
            TypeCode.String => true,
            TypeCode.Decimal => true,
            TypeCode.Single => true,
            TypeCode.Double => true,
            TypeCode.Int16 => true,
            TypeCode.Int32 => true,
            TypeCode.Int64 => true,
            TypeCode.UInt16 => true,
            TypeCode.UInt32 => true,
            TypeCode.UInt64 => true,
            _ => false
        };
    }

    public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is GridColFlex flex)
        {
            return flex;
        }

        if (value is string text)
        {
            return GridColFlex.Parse(text);
        }

        if (value is IConvertible convertible)
        {
            return new GridColFlex(convertible.ToDouble(culture ?? CultureInfo.InvariantCulture));
        }

        throw new NotSupportedException($"Cannot convert value '{value}' to GridColFlex.");
    }
}
