using System.ComponentModel;
using System.Globalization;

namespace AtomUI.Controls;

[TypeConverter(typeof(RowJustifyConverter))]
public enum RowJustify
{
    Start,
    Center,
    End,
    SpaceBetween,
    SpaceAround,
    SpaceEvenly
}

[TypeConverter(typeof(RowAlignConverter))]
public enum RowAlign
{
    Top,
    Middle,
    Bottom,
    Stretch
}

public class RowJustifyConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(string);
    }

    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
    {
        return destinationType == typeof(string);
    }

    public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string text)
        {
            var normalized = text.AsSpan().Trim();
            if (normalized.Equals("start".AsSpan(), StringComparison.OrdinalIgnoreCase) ||
                normalized.Equals("flex-start".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                return RowJustify.Start;
            }
            if (normalized.Equals("center".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                return RowJustify.Center;
            }
            if (normalized.Equals("end".AsSpan(), StringComparison.OrdinalIgnoreCase) ||
                normalized.Equals("flex-end".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                return RowJustify.End;
            }
            if (normalized.Equals("space-between".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                return RowJustify.SpaceBetween;
            }
            if (normalized.Equals("space-around".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                return RowJustify.SpaceAround;
            }
            if (normalized.Equals("space-evenly".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                return RowJustify.SpaceEvenly;
            }
            return Enum.Parse(typeof(RowJustify), text, ignoreCase: true);
        }

        throw new NotSupportedException($"Cannot convert value '{value}' to RowJustify.");
    }

    public override object ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (destinationType != typeof(string))
        {
            throw new NotSupportedException($"Cannot convert RowJustify to {destinationType}.");
        }

        if (value is RowJustify justify)
        {
            return justify switch
            {
                RowJustify.Start => "start",
                RowJustify.Center => "center",
                RowJustify.End => "end",
                RowJustify.SpaceBetween => "space-between",
                RowJustify.SpaceAround => "space-around",
                RowJustify.SpaceEvenly => "space-evenly",
                _ => string.Empty
            };
        }

        return string.Empty;
    }
}

public class RowAlignConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(string);
    }

    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
    {
        return destinationType == typeof(string);
    }

    public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string text)
        {
            var normalized = text.AsSpan().Trim();
            if (normalized.Equals("top".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                return RowAlign.Top;
            }
            if (normalized.Equals("middle".AsSpan(), StringComparison.OrdinalIgnoreCase) ||
                normalized.Equals("center".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                return RowAlign.Middle;
            }
            if (normalized.Equals("bottom".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                return RowAlign.Bottom;
            }
            if (normalized.Equals("stretch".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                return RowAlign.Stretch;
            }
            return Enum.Parse(typeof(RowAlign), text, ignoreCase: true);
        }

        throw new NotSupportedException($"Cannot convert value '{value}' to RowAlign.");
    }

    public override object ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (destinationType != typeof(string))
        {
            throw new NotSupportedException($"Cannot convert RowAlign to {destinationType}.");
        }

        if (value is RowAlign align)
        {
            return align switch
            {
                RowAlign.Top => "top",
                RowAlign.Middle => "middle",
                RowAlign.Bottom => "bottom",
                RowAlign.Stretch => "stretch",
                _ => string.Empty
            };
        }

        return string.Empty;
    }
}
