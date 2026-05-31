using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace AtomUI.Controls;

[TypeConverter(typeof(GridColSizeConverter))]
public record GridColSize
{
    public int? Span { get; init; }
    public int? Offset { get; init; }
    public int? Order { get; init; }
    public int? Push { get; init; }
    public int? Pull { get; init; }

    public static GridColSize Parse(string input)
    {
        var trimmed = input.AsSpan().Trim();
        if (trimmed.Length == 0)
        {
            throw new FormatException("Grid column size cannot be empty.");
        }

        if (int.TryParse(trimmed, out var singleSpan))
        {
            ValidateColumnValue(singleSpan, "span");
            return new GridColSize { Span = singleSpan };
        }

        var result = new GridColSize();
        while (!trimmed.IsEmpty)
        {
            var commaIndex = trimmed.IndexOf(',');
            var segment    = commaIndex >= 0 ? trimmed[..commaIndex].Trim() : trimmed.Trim();
            trimmed = commaIndex >= 0 ? trimmed[(commaIndex + 1)..] : ReadOnlySpan<char>.Empty;
            if (segment.Length == 0)
            {
                continue;
            }

            var colonIndex = segment.IndexOf(':');
            if (colonIndex < 0)
            {
                throw new FormatException($"Missing ':' in segment '{segment}'.");
            }

            var key = segment[..colonIndex].Trim();
            var value = segment[(colonIndex + 1)..].Trim();
            if (key.Length == 0 || value.Length == 0)
            {
                throw new FormatException($"Invalid segment '{segment}'.");
            }

            if (key.Equals("span", StringComparison.OrdinalIgnoreCase))
            {
                var span = ParseInt(value, "span", allowNegative: false);
                ValidateColumnValue(span, "span");
                result = result with { Span = span };
            }
            else if (key.Equals("offset", StringComparison.OrdinalIgnoreCase))
            {
                var offset = ParseInt(value, "offset", allowNegative: false);
                ValidateColumnValue(offset, "offset");
                result = result with { Offset = offset };
            }
            else if (key.Equals("order", StringComparison.OrdinalIgnoreCase))
            {
                var order = ParseInt(value, "order", allowNegative: true);
                result = result with { Order = order };
            }
            else if (key.Equals("push", StringComparison.OrdinalIgnoreCase))
            {
                var push = ParseInt(value, "push", allowNegative: false);
                ValidateColumnValue(push, "push");
                result = result with { Push = push };
            }
            else if (key.Equals("pull", StringComparison.OrdinalIgnoreCase))
            {
                var pull = ParseInt(value, "pull", allowNegative: false);
                ValidateColumnValue(pull, "pull");
                result = result with { Pull = pull };
            }
            else
            {
                throw new FormatException($"Unknown grid column key '{key}'.");
            }
        }

        return result;
    }

    internal GridColLayout ApplyTo(GridColLayout layout)
    {
        return layout with
        {
            Span = Span ?? layout.Span,
            Offset = Offset ?? layout.Offset,
            Order = Order ?? layout.Order,
            Push = Push ?? layout.Push,
            Pull = Pull ?? layout.Pull
        };
    }

    private static int ParseInt(ReadOnlySpan<char> input, string name, bool allowNegative)
    {
        if (!int.TryParse(input, out var value))
        {
            throw new FormatException($"Invalid {name} value '{input.ToString()}'.");
        }

        if (!allowNegative && value < 0)
        {
            throw new FormatException($"{name} must be >= 0, got {value}.");
        }

        return value;
    }

    private static void ValidateColumnValue(int value, string name)
    {
        if (value < 0 || value > 24)
        {
            throw new FormatException($"{name} must be between 0 and 24, got {value}.");
        }
    }
}

internal readonly record struct GridColLayout(
    int Span,
    int Offset,
    int Order,
    int Push,
    int Pull);

public class GridColSizeConverter : TypeConverter
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
            throw new NotSupportedException("Cannot convert null to GridColSize.");
        }

        if (value is GridColSize size)
        {
            return size;
        }

        if (value is string str)
        {
            return GridColSize.Parse(str);
        }

        if (value is IConvertible convertible)
        {
            var number = convertible.ToInt32(culture ?? CultureInfo.InvariantCulture);
            return new GridColSize { Span = number };
        }

        throw new NotSupportedException($"Cannot convert value '{value}' to GridColSize.");
    }

    public override object ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (destinationType != typeof(string))
        {
            throw new NotSupportedException($"Cannot convert GridColSize to {destinationType}.");
        }

        if (value is GridColSize size)
        {
            if (size.Span.HasValue && size.Offset is null && size.Order is null && size.Push is null && size.Pull is null)
            {
                return size.Span.Value.ToString(CultureInfo.InvariantCulture);
            }

            var builder = new StringBuilder();
            if (size.Span.HasValue)
            {
                AppendPart(builder, "span", size.Span.Value);
            }
            if (size.Offset.HasValue)
            {
                AppendPart(builder, "offset", size.Offset.Value);
            }
            if (size.Order.HasValue)
            {
                AppendPart(builder, "order", size.Order.Value);
            }
            if (size.Push.HasValue)
            {
                AppendPart(builder, "push", size.Push.Value);
            }
            if (size.Pull.HasValue)
            {
                AppendPart(builder, "pull", size.Pull.Value);
            }
            return builder.ToString();
        }

        return string.Empty;
    }

    private static void AppendPart(StringBuilder builder, string name, int value)
    {
        if (builder.Length > 0)
        {
            builder.Append(',');
        }

        builder.Append(name);
        builder.Append(':');
        builder.Append(value.ToString(CultureInfo.InvariantCulture));
    }
}
