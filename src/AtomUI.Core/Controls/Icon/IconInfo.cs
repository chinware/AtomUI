using System.ComponentModel;
using Avalonia;

namespace AtomUI.Controls;

public record GeometryData
{
    public string PathData { get; }
    public bool IsPrimary { get; }
    public string? Transform { get; }

    public GeometryData(string pathData, string? transform = null, bool isPrimary = true)
    {
        PathData  = pathData;
        IsPrimary = isPrimary;
        Transform = transform;
    }
}

public class IconInfo
{
    public IList<GeometryData> Data { get; init; }
    public IList<DrawingInstruction>? Instructions { get; init; }
    
    public bool IsTwoTone => ThemeType == IconThemeType.TwoTone;
    public IconThemeType ThemeType { get; init; }

    public string Name { get; init; }
    public Rect ViewBox { get; init; }

    internal IconInfo()
        : this(
            string.Empty,
            IconThemeType.Filled,
            new Rect(),
            Array.Empty<GeometryData>())
    {
    }

    public IconInfo(string name, IconThemeType themeType,
                    Rect viewBox, IList<GeometryData> data)
    {
        if (themeType == IconThemeType.TwoTone)
        {
            throw new InvalidEnumArgumentException("ColorInfo does not support IconThemeType.TwoTone.");
        }

        Name      = name;
        Data      = data;
        ThemeType = themeType;
        ViewBox   = viewBox;
    }
    
    public IconInfo(string name, IconThemeType themeType,
                    Rect viewBox, IList<DrawingInstruction> instructions)
    {
        if (themeType == IconThemeType.TwoTone)
        {
            throw new InvalidEnumArgumentException("ColorInfo does not support IconThemeType.TwoTone.");
        }

        Name         = name;
        Data         = Array.Empty<GeometryData>();
        ThemeType    = themeType;
        Instructions = instructions;
        ViewBox      = viewBox;
    }

    public IconInfo(string name, Rect viewBox, IList<GeometryData> data)
    {
        Name             = name;
        Data             = data;
        ThemeType        = IconThemeType.TwoTone;
        ViewBox          = viewBox;
    }
    
    public IconInfo(string name, Rect viewBox, IList<DrawingInstruction> instructions)
    {
        Name         = name;
        Data         = Array.Empty<GeometryData>();
        Instructions = instructions;
        ThemeType    = IconThemeType.TwoTone;
        ViewBox      = viewBox;
    }
}