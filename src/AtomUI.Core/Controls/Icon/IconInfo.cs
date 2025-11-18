using System.ComponentModel;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

public record  GeometryData
{
    public string PathData { get; }
    public bool IsPrimary { get; }

    public GeometryData(string pathData, bool isPrimary = true)
    {
        PathData  = pathData;
        IsPrimary = isPrimary;
    }
}

public class IconInfo
{
    public IList<GeometryData> Data { get; }
    public ColorInfo? ColorInfo { get; internal set; }
    public TwoToneColorInfo? TwoToneColorInfo { get; internal set; }
    public bool IsTwoTone => ThemeType == IconThemeType.TwoTone;
    public IconThemeType ThemeType { get; }

    public string Name { get; }
    public Rect ViewBox { get; init; }

    internal IconInfo()
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
        {
            throw new InvalidEnumArgumentException("ColorInfo does not support IconThemeType.TwoTone.");
        }

        Name      = name;
        
        Data      = data;
        ThemeType = themeType;
        ViewBox   = viewBox;
    }

    public IconInfo(string name, Rect viewBox, IList<GeometryData> data, TwoToneColorInfo? colorInfo = null)
    {
        Name             = name;
        Data             = data;
        ThemeType        = IconThemeType.TwoTone;
        ViewBox          = viewBox;
    }
}