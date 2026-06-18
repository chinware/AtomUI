using System.ComponentModel;
using System.Globalization;

namespace AtomUI.Controls;

[TypeConverter(typeof(ResponsiveIntConverter))]
public readonly record struct ResponsiveInt
{
    private readonly ResponsiveValueMap<int> _valueMap;

    public ResponsiveInt(int value)
    {
        _valueMap = new ResponsiveValueMap<int>(value);
    }

    private ResponsiveInt(ResponsiveValueMap<int> valueMap)
    {
        _valueMap = valueMap;
    }

    public static ResponsiveInt Parse(string input)
    {
        return new ResponsiveInt(ResponsiveValueParser.Parse(input, ParseInt, ParseInt));
    }

    public int Resolve(MediaBreakPoint breakPoint, int fallback)
    {
        return _valueMap.Resolve(breakPoint, fallback);
    }

    public bool TryResolve(MediaBreakPoint breakPoint, out int value)
    {
        return _valueMap.TryResolve(breakPoint, out value);
    }

    public static implicit operator ResponsiveInt(int value) => new(value);

    private static int ParseInt(ReadOnlySpan<char> input)
    {
        if (!int.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value))
        {
            throw new FormatException($"Responsive integer value '{input.ToString()}' is not a valid integer.");
        }

        return value;
    }
}

public class ResponsiveIntConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        var typeCode = Type.GetTypeCode(sourceType);
        return typeCode switch
        {
            TypeCode.String => true,
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
        if (value is ResponsiveInt responsiveInt)
        {
            return responsiveInt;
        }
        if (value is string text)
        {
            return ResponsiveInt.Parse(text);
        }
        if (value is IConvertible convertible)
        {
            return new ResponsiveInt(convertible.ToInt32(culture ?? CultureInfo.InvariantCulture));
        }

        throw new NotSupportedException($"Cannot convert value '{value}' to ResponsiveInt.");
    }
}
