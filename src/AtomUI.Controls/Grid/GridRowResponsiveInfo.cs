using System.ComponentModel;
using System.Globalization;

namespace AtomUI.Controls;

[TypeConverter(typeof(GridRowJustifyInfoConverter))]
public readonly record struct GridRowJustifyInfo
{
    private readonly ResponsiveValueMap<RowJustify> _valueMap;

    public GridRowJustifyInfo(RowJustify value)
    {
        _valueMap = new ResponsiveValueMap<RowJustify>(value);
    }

    private GridRowJustifyInfo(ResponsiveValueMap<RowJustify> valueMap)
    {
        _valueMap = valueMap;
    }

    public static GridRowJustifyInfo Parse(string input)
    {
        return new GridRowJustifyInfo(ResponsiveValueParser.Parse(input, ParseJustify, ParseJustify));
    }

    public RowJustify Resolve(MediaBreakPoint breakPoint, RowJustify fallback)
    {
        return _valueMap.Resolve(breakPoint, fallback);
    }

    public static implicit operator GridRowJustifyInfo(RowJustify value) => new(value);

    private static RowJustify ParseJustify(ReadOnlySpan<char> input)
    {
        return RowJustifyConverter.Parse(input);
    }
}

[TypeConverter(typeof(GridRowAlignInfoConverter))]
public readonly record struct GridRowAlignInfo
{
    private readonly ResponsiveValueMap<RowAlign> _valueMap;

    public GridRowAlignInfo(RowAlign value)
    {
        _valueMap = new ResponsiveValueMap<RowAlign>(value);
    }

    private GridRowAlignInfo(ResponsiveValueMap<RowAlign> valueMap)
    {
        _valueMap = valueMap;
    }

    public static GridRowAlignInfo Parse(string input)
    {
        return new GridRowAlignInfo(ResponsiveValueParser.Parse(input, ParseAlign, ParseAlign));
    }

    public RowAlign Resolve(MediaBreakPoint breakPoint, RowAlign fallback)
    {
        return _valueMap.Resolve(breakPoint, fallback);
    }

    public static implicit operator GridRowAlignInfo(RowAlign value) => new(value);

    private static RowAlign ParseAlign(ReadOnlySpan<char> input)
    {
        return RowAlignConverter.Parse(input);
    }
}

public class GridRowJustifyInfoConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(string) || sourceType == typeof(RowJustify);
    }

    public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is GridRowJustifyInfo info)
        {
            return info;
        }

        if (value is RowJustify justify)
        {
            return new GridRowJustifyInfo(justify);
        }

        if (value is string text)
        {
            return GridRowJustifyInfo.Parse(text);
        }

        throw new NotSupportedException($"Cannot convert value '{value}' to GridRowJustifyInfo.");
    }
}

public class GridRowAlignInfoConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(string) || sourceType == typeof(RowAlign);
    }

    public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is GridRowAlignInfo info)
        {
            return info;
        }

        if (value is RowAlign align)
        {
            return new GridRowAlignInfo(align);
        }

        if (value is string text)
        {
            return GridRowAlignInfo.Parse(text);
        }

        throw new NotSupportedException($"Cannot convert value '{value}' to GridRowAlignInfo.");
    }
}
