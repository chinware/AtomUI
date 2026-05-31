using System.ComponentModel;
using System.Globalization;

namespace AtomUI.Controls;

[TypeConverter(typeof(GridGutterConverter))]
public record GridGutter
{
    public GridGutterInfo Horizontal { get; init; }
    public GridGutterInfo Vertical { get; init; }

    public GridGutter()
        : this(new GridGutterInfo(), new GridGutterInfo())
    {
    }

    public GridGutter(GridGutterInfo horizontal, GridGutterInfo vertical)
    {
        Horizontal = horizontal;
        Vertical   = vertical;
    }

    public static GridGutter Parse(string input)
    {
        var trimmed = input.AsSpan().Trim();
        if (trimmed.Length == 0)
        {
            throw new FormatException("Gutter value cannot be empty.");
        }

        if (trimmed.IndexOf(';') >= 0)
        {
            if (!TrySplitTwoNonEmptySegments(trimmed, ';', out var horizontalSegment, out var verticalSegment))
            {
                throw new FormatException("Gutter must have exactly two segments when using ';' separator.");
            }

            var horizontal = GridGutterInfo.Parse(horizontalSegment.Trim());
            var vertical   = GridGutterInfo.Parse(verticalSegment.Trim());
            return new GridGutter(horizontal, vertical);
        }

        if (trimmed.IndexOf(':') >= 0)
        {
            var horizontal = GridGutterInfo.Parse(trimmed);
            return new GridGutter(horizontal, new GridGutterInfo());
        }

        if (TryParsePair(trimmed, out var horizontalValue, out var verticalValue))
        {
            return new GridGutter(new GridGutterInfo(horizontalValue), new GridGutterInfo(verticalValue));
        }

        if (double.TryParse(trimmed, NumberStyles.Float, CultureInfo.InvariantCulture, out var single))
        {
            return new GridGutter(new GridGutterInfo(single), new GridGutterInfo());
        }

        throw new FormatException($"Invalid gutter format '{input}'.");
    }

    internal (double Horizontal, double Vertical) Resolve(MediaBreakPoint breakPoint)
    {
        return (Horizontal.GetValue(breakPoint), Vertical.GetValue(breakPoint));
    }

    private static bool TryParsePair(ReadOnlySpan<char> input, out double horizontal, out double vertical)
    {
        horizontal = 0;
        vertical   = 0;

        if (!TrySplitTwoNonEmptySegments(input, ',', out var horizontalSegment, out var verticalSegment))
        {
            return false;
        }

        if (!double.TryParse(horizontalSegment.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out horizontal))
        {
            return false;
        }

        if (!double.TryParse(verticalSegment.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out vertical))
        {
            return false;
        }

        if (horizontal < 0 || vertical < 0)
        {
            throw new FormatException("Gutter values must be >= 0.");
        }

        return true;
    }

    private static bool TrySplitTwoNonEmptySegments(ReadOnlySpan<char> input,
                                                    char separator,
                                                    out ReadOnlySpan<char> first,
                                                    out ReadOnlySpan<char> second)
    {
        first  = default;
        second = default;
        var foundCount = 0;

        while (!input.IsEmpty)
        {
            var separatorIndex = input.IndexOf(separator);
            var segment        = separatorIndex >= 0 ? input[..separatorIndex] : input;
            input              = separatorIndex >= 0 ? input[(separatorIndex + 1)..] : ReadOnlySpan<char>.Empty;

            if (segment.IsEmpty)
            {
                continue;
            }

            foundCount++;
            if (foundCount == 1)
            {
                first = segment;
            }
            else if (foundCount == 2)
            {
                second = segment;
            }
            else
            {
                return false;
            }
        }

        return foundCount == 2;
    }
}

public class GridGutterConverter : TypeConverter
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
            throw new NotSupportedException("Cannot convert null to GridGutter.");
        }

        if (value is GridGutter gutter)
        {
            return gutter;
        }

        if (value is string str)
        {
            return GridGutter.Parse(str);
        }

        if (value is IConvertible convertible)
        {
            var number = convertible.ToDouble(culture ?? CultureInfo.InvariantCulture);
            return new GridGutter(new GridGutterInfo(number), new GridGutterInfo());
        }

        throw new NotSupportedException($"Cannot convert value '{value}' to GridGutter.");
    }

    public override object ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (destinationType != typeof(string))
        {
            throw new NotSupportedException($"Cannot convert GridGutter to {destinationType}.");
        }

        if (value is GridGutter gutter)
        {
            if (gutter.Vertical == new GridGutterInfo())
            {
                return FormattableString.Invariant($"{gutter.Horizontal.ExtraSmall:G17}");
            }

            return FormattableString.Invariant($"{gutter.Horizontal.ExtraSmall:G17},{gutter.Vertical.ExtraSmall:G17}");
        }

        return string.Empty;
    }
}
