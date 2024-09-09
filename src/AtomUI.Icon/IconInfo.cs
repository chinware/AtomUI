using System.ComponentModel;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Icon;

public record class GeometryData
{
    public GeometryData(string pathData, bool isPrimary = true)
    {
        PathData  = pathData;
        IsPrimary = isPrimary;
    }

    public string PathData { get; }
    public bool IsPrimary { get; }
}


public record class IconInfo
{
    public IconInfo()
        : this(
            string.Empty,
            IconThemeType.Filled,
            new Rect(),
            new List<GeometryData> { new(string.Empty) },
            new ColorInfo(Colors.Black))
    {
    }

    public IconInfo(string name, IconThemeType themeType,
        Rect viewBox, IList<GeometryData> data,
        ColorInfo? colorInfo = null)
    {
        if (themeType == IconThemeType.TwoTone)
            throw new InvalidEnumArgumentException("ColorInfo does not support IconThemeType.TwoTone.");

        Name      = name;
        ColorInfo = colorInfo;
        Data      = data;
        ThemeType = themeType;
        ViewBox   = viewBox;
    }

    public IconInfo(string name, Rect viewBox, IList<GeometryData> data, TwoToneColorInfo? colorInfo = null)
    {
        Name             = name;
        TwoToneColorInfo = colorInfo;
        Data             = data;
        ThemeType        = IconThemeType.TwoTone;
        ViewBox          = viewBox;
    }

    public IList<GeometryData> Data { get; }
    public ColorInfo? ColorInfo { get; init; }
    public TwoToneColorInfo? TwoToneColorInfo { get; init; }
    public bool IsTwoTone => ThemeType == IconThemeType.TwoTone;
    public IconThemeType ThemeType { get; }

    public string Name { get; }
    public Rect ViewBox { get; init; }
}