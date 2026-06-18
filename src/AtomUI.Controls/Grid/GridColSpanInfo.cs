using System.ComponentModel;
using System.Globalization;

namespace AtomUI.Controls;

[TypeConverter(typeof(GridColSpanInfoConverter))]
public readonly record struct GridColSpanInfo
{
    public int ExtraSmall { get; init; }
    public int Small { get; init; }
    public int Medium { get; init; }
    public int Large { get; init; }
    public int ExtraLarge { get; init; }
    public int ExtraExtraLarge { get; init; }
    public int ExtraExtraExtraLarge { get; init; }

    public GridColSpanInfo(int span)
    {
        ValidateSpan(span);
        ExtraSmall = span;
        Small = span;
        Medium = span;
        Large = span;
        ExtraLarge = span;
        ExtraExtraLarge = span;
        ExtraExtraExtraLarge = span;
    }

    public GridColSpanInfo(int extraSmall, int small, int medium, int large, int extraLarge, int extraExtraLarge)
        : this(extraSmall, small, medium, large, extraLarge, extraExtraLarge, extraExtraLarge)
    {
    }

    public GridColSpanInfo(int extraSmall,
                           int small,
                           int medium,
                           int large,
                           int extraLarge,
                           int extraExtraLarge,
                           int extraExtraExtraLarge)
    {
        ValidateSpan(extraSmall);
        ValidateSpan(small);
        ValidateSpan(medium);
        ValidateSpan(large);
        ValidateSpan(extraLarge);
        ValidateSpan(extraExtraLarge);
        ValidateSpan(extraExtraExtraLarge);

        ExtraSmall = extraSmall;
        Small = small;
        Medium = medium;
        Large = large;
        ExtraLarge = extraLarge;
        ExtraExtraLarge = extraExtraLarge;
        ExtraExtraExtraLarge = extraExtraExtraLarge;
    }

    public static GridColSpanInfo Parse(string input)
    {
        if (int.TryParse(input.AsSpan().Trim(), out var singleSpan))
        {
            ValidateSpan(singleSpan);
            return new GridColSpanInfo(singleSpan);
        }

        var responsive = ResponsiveInt.Parse(input);
        return new GridColSpanInfo(
            responsive.Resolve(MediaBreakPoint.ExtraSmall, 0),
            responsive.Resolve(MediaBreakPoint.Small, 0),
            responsive.Resolve(MediaBreakPoint.Medium, 0),
            responsive.Resolve(MediaBreakPoint.Large, 0),
            responsive.Resolve(MediaBreakPoint.ExtraLarge, 0),
            responsive.Resolve(MediaBreakPoint.ExtraExtraLarge, 0),
            responsive.Resolve(MediaBreakPoint.ExtraExtraExtraLarge, 0));
    }

    public int GetValue(MediaBreakPoint breakPoint)
    {
        return breakPoint switch
        {
            MediaBreakPoint.ExtraSmall => ExtraSmall,
            MediaBreakPoint.Small => Small,
            MediaBreakPoint.Medium => Medium,
            MediaBreakPoint.Large => Large,
            MediaBreakPoint.ExtraLarge => ExtraLarge,
            MediaBreakPoint.ExtraExtraLarge => ExtraExtraLarge,
            _ => ExtraExtraExtraLarge
        };
    }

    private static void ValidateSpan(int span)
    {
        if (span < 0 || span > 24)
        {
            throw new FormatException($"Span must be between 0 and 24, got {span}.");
        }
    }

    public static implicit operator GridColSpanInfo(int span) => new(span);
}

public class GridColSpanInfoConverter : TypeConverter
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

    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
    {
        return destinationType == typeof(string);
    }

    public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value == null)
        {
            throw new NotSupportedException("Cannot convert null to GridColSpanInfo.");
        }

        if (value is GridColSpanInfo spanInfo)
        {
            return spanInfo;
        }

        if (value is string str)
        {
            return GridColSpanInfo.Parse(str);
        }

        if (value is IConvertible convertible)
        {
            var number = convertible.ToInt32(culture ?? CultureInfo.InvariantCulture);
            return new GridColSpanInfo(number);
        }

        throw new NotSupportedException($"Cannot convert value '{value}' to GridColSpanInfo.");
    }

    public override object ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (destinationType != typeof(string))
        {
            throw new NotSupportedException($"Cannot convert GridColSpanInfo to {destinationType}.");
        }

        if (value is GridColSpanInfo spanInfo)
        {
            return spanInfo.ExtraSmall.ToString(CultureInfo.InvariantCulture);
        }

        return string.Empty;
    }
}
