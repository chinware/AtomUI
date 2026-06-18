using System.ComponentModel;
using System.Globalization;

namespace AtomUI.Controls;

[TypeConverter(typeof(ResponsiveGutterConverter))]
public readonly record struct ResponsiveGutter
{
    private readonly bool _hasExplicitHorizontal;
    private readonly bool _hasExplicitVertical;

    public ResponsiveGutter()
        : this(new ResponsiveDouble(0), new ResponsiveDouble(0))
    {
    }

    public ResponsiveGutter(ResponsiveDouble horizontal, ResponsiveDouble vertical)
        : this(horizontal, vertical, true, true)
    {
    }

    private ResponsiveGutter(ResponsiveDouble horizontal,
                             ResponsiveDouble vertical,
                             bool hasExplicitHorizontal,
                             bool hasExplicitVertical)
    {
        Horizontal             = horizontal;
        Vertical               = vertical;
        _hasExplicitHorizontal = hasExplicitHorizontal;
        _hasExplicitVertical   = hasExplicitVertical;
    }

    public ResponsiveDouble Horizontal { get; }

    public ResponsiveDouble Vertical { get; }

    public static ResponsiveGutter Parse(string input)
    {
        var trimmed = input.AsSpan().Trim();
        if (trimmed.IsEmpty)
        {
            throw new FormatException("Responsive gutter value cannot be empty.");
        }

        if (trimmed.IndexOf(';') >= 0)
        {
            if (!TrySplitTwoNonEmptySegments(trimmed, ';', out var horizontal, out var vertical))
            {
                throw new FormatException("Responsive gutter must have exactly two segments when using ';' separator.");
            }

            return new ResponsiveGutter(ResponsiveDouble.Parse(horizontal.ToString()),
                ResponsiveDouble.Parse(vertical.ToString()));
        }

        if (trimmed.IndexOf(':') >= 0)
        {
            return new ResponsiveGutter(ResponsiveDouble.Parse(trimmed.ToString()), new ResponsiveDouble(0), true, false);
        }

        if (TryParsePair(trimmed, out var horizontalValue, out var verticalValue))
        {
            return new ResponsiveGutter(new ResponsiveDouble(horizontalValue), new ResponsiveDouble(verticalValue));
        }

        var scalar = ResponsiveDouble.Parse(trimmed.ToString());
        return new ResponsiveGutter(scalar, scalar);
    }

    public (double Horizontal, double Vertical) Resolve(MediaBreakPoint breakPoint, (double Horizontal, double Vertical) fallback)
    {
        return (
            _hasExplicitHorizontal ? Horizontal.Resolve(breakPoint, fallback.Horizontal) : fallback.Horizontal,
            _hasExplicitVertical ? Vertical.Resolve(breakPoint, fallback.Vertical) : fallback.Vertical);
    }

    public bool TryResolve(MediaBreakPoint breakPoint,
                           (double Horizontal, double Vertical) fallback,
                           out (double Horizontal, double Vertical) value)
    {
        var horizontal = fallback.Horizontal;
        var vertical   = fallback.Vertical;
        var horizontalHit = _hasExplicitHorizontal && Horizontal.TryResolve(breakPoint, out horizontal);
        var verticalHit   = _hasExplicitVertical && Vertical.TryResolve(breakPoint, out vertical);
        if (!horizontalHit && !verticalHit)
        {
            value = fallback;
            return false;
        }

        value = (
            horizontalHit ? horizontal : fallback.Horizontal,
            verticalHit ? vertical : fallback.Vertical);
        return true;
    }

    private static bool TryParsePair(ReadOnlySpan<char> input, out double horizontal, out double vertical)
    {
        horizontal = 0;
        vertical = 0;

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
            throw new FormatException("Responsive gutter values must be >= 0.");
        }

        return true;
    }

    private static bool TrySplitTwoNonEmptySegments(ReadOnlySpan<char> input,
                                                    char separator,
                                                    out ReadOnlySpan<char> first,
                                                    out ReadOnlySpan<char> second)
    {
        first = default;
        second = default;
        var foundCount = 0;

        while (!input.IsEmpty)
        {
            var separatorIndex = input.IndexOf(separator);
            var segment = separatorIndex >= 0 ? input[..separatorIndex].Trim() : input.Trim();
            input = separatorIndex >= 0 ? input[(separatorIndex + 1)..] : ReadOnlySpan<char>.Empty;

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

public class ResponsiveGutterConverter : TypeConverter
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
        if (value is ResponsiveGutter gutter)
        {
            return gutter;
        }
        if (value is string text)
        {
            return ResponsiveGutter.Parse(text);
        }
        if (value is IConvertible convertible)
        {
            var scalar = new ResponsiveDouble(convertible.ToDouble(culture ?? CultureInfo.InvariantCulture));
            return new ResponsiveGutter(scalar, scalar);
        }

        throw new NotSupportedException($"Cannot convert value '{value}' to ResponsiveGutter.");
    }
}
