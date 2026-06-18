using System.ComponentModel;
using System.Globalization;

namespace AtomUI.Controls;

[TypeConverter(typeof(ResponsiveDoubleConverter))]
public readonly record struct ResponsiveDouble
{
    private readonly ResponsiveValueMap<double> _valueMap;

    public ResponsiveDouble(double value)
    {
        Validate(value);
        _valueMap = new ResponsiveValueMap<double>(value);
    }

    private ResponsiveDouble(ResponsiveValueMap<double> valueMap)
    {
        _valueMap = valueMap;
    }

    public static ResponsiveDouble Parse(string input)
    {
        return new ResponsiveDouble(ResponsiveValueParser.Parse(input, ParseDouble, ParseDouble));
    }

    public double Resolve(MediaBreakPoint breakPoint, double fallback)
    {
        return _valueMap.Resolve(breakPoint, fallback);
    }

    public bool TryResolve(MediaBreakPoint breakPoint, out double value)
    {
        return _valueMap.TryResolve(breakPoint, out value);
    }

    public static implicit operator ResponsiveDouble(double value) => new(value);

    private static double ParseDouble(ReadOnlySpan<char> input)
    {
        if (!double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
        {
            throw new FormatException($"Responsive number value '{input.ToString()}' is not a valid number.");
        }

        Validate(value);
        return value;
    }

    private static void Validate(double value)
    {
        if (double.IsNaN(value) || double.IsInfinity(value) || value < 0)
        {
            throw new FormatException($"Responsive number value must be >= 0, got {value}.");
        }
    }
}

public class ResponsiveDoubleConverter : TypeConverter
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
        if (value is ResponsiveDouble responsiveDouble)
        {
            return responsiveDouble;
        }
        if (value is string text)
        {
            return ResponsiveDouble.Parse(text);
        }
        if (value is IConvertible convertible)
        {
            return new ResponsiveDouble(convertible.ToDouble(culture ?? CultureInfo.InvariantCulture));
        }

        throw new NotSupportedException($"Cannot convert value '{value}' to ResponsiveDouble.");
    }
}
