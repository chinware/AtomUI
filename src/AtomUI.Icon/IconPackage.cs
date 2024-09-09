using Avalonia.Media;

namespace AtomUI.Icon;

public abstract class IconPackage : IIconPackageProvider
{
    protected IDictionary<int, IconInfo> _iconPool;

    public IconPackage(string id)
    {
        Id               = id;
        _iconPool        = new Dictionary<int, IconInfo>();
        DefaultColorInfo = new ColorInfo(Color.FromRgb(51, 51, 51)); // #333
        DefaultTwoToneColorInfo = new TwoToneColorInfo
        {
            PrimaryColor   = Color.FromRgb(51, 51, 51),
            SecondaryColor = Color.FromRgb(217, 217, 217)
        };
    }

    public ColorInfo DefaultColorInfo { get; set; }
    public TwoToneColorInfo DefaultTwoToneColorInfo { get; set; }
    public string Id { get; }
    public int Priority { get; set; } = 0;

    public abstract IconInfo? GetIcon(string iconKind);
    public abstract IconInfo? GetIcon(string iconKind, ColorInfo colorInfo);
    public abstract IconInfo? GetIcon(string iconKind, TwoToneColorInfo twoToneColorInfo);

    public IEnumerable<IconInfo> GetIconInfos(IconThemeType? iconThemeType = null)
    {
        if (!iconThemeType.HasValue) return _iconPool.Values;

        return _iconPool.Values.Where(iconInfo => iconInfo.ThemeType == iconThemeType.Value);
    }

    protected IconInfo? GetIconRaw(int iconKind)
    {
        if (!_iconPool.ContainsKey(iconKind)) return null;
        var iconInfo = _iconPool[iconKind];
        if (!iconInfo.IsTwoTone)
            iconInfo = iconInfo with
            {
                ColorInfo = DefaultColorInfo
            };
        else
            iconInfo = iconInfo with
            {
                TwoToneColorInfo = DefaultTwoToneColorInfo
            };
        return iconInfo;
    }

    protected IconInfo? GetIconRaw(int iconKind, ColorInfo colorInfo)
    {
        var iconInfo = GetIconRaw(iconKind);
        if (iconInfo is null) return iconInfo;
        if (!iconInfo.IsTwoTone)
            iconInfo = iconInfo with
            {
                ColorInfo = colorInfo
            };
        return iconInfo;
    }

    protected IconInfo? GetIconRaw(int iconKind, TwoToneColorInfo twoToneColorInfo)
    {
        var iconInfo = GetIconRaw(iconKind);
        if (iconInfo is null) return iconInfo;

        if (iconInfo.IsTwoTone)
            iconInfo = iconInfo with
            {
                TwoToneColorInfo = twoToneColorInfo
            };

        return iconInfo;
    }
}