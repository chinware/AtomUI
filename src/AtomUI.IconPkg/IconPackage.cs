using Avalonia.Media;

namespace AtomUI.IconPkg;

public abstract class IconPackage : IIconPackage
{
    protected IDictionary<int, Lazy<IconInfo>> _iconInfoPool;
    
    public ColorInfo DefaultColorInfo { get; set; }
    
    public TwoToneColorInfo DefaultTwoToneColorInfo { get; set; }
    
    public string Id { get; }
    
    public int Priority { get; set; } = 0;

    public IconPackage(string id)
    {
        Id               = id;
        _iconInfoPool    = new Dictionary<int, Lazy<IconInfo>>();
        DefaultColorInfo = new ColorInfo(Color.FromRgb(85, 85, 85)); // #333
        DefaultTwoToneColorInfo = new TwoToneColorInfo
        {
            PrimaryColor   = Color.FromRgb(22, 119, 255),
            SecondaryColor = Color.FromRgb(230, 244, 255)
        };
    }
    
    public abstract IconInfo? GetIconInfo(string iconKind);
    public abstract IconInfo? GetIconInfo(string iconKind, ColorInfo colorInfo);
    public abstract IconInfo? GetIconInfo(string iconKind, TwoToneColorInfo twoToneColorInfo);
    
    public abstract Icon? BuildIcon(string iconKind);
    public abstract Icon? BuildIcon(string iconKind, ColorInfo colorInfo);
    public abstract Icon? BuildIcon(string iconKind, TwoToneColorInfo twoToneColorInfo);
    
    protected Icon? BuildIcon(int iconKind)
    {
        var iconInfo = GetIconInfo(iconKind);
        if (iconInfo is null)
        {
            return null;
        }

        return new Icon()
        {
            IconInfo = iconInfo
        };
    }
    
    protected Icon? BuildIcon(int iconKind, ColorInfo colorInfo)
    {
        var iconInfo = GetIconInfo(iconKind, colorInfo);
        if (iconInfo is null)
        {
            return null;
        }

        return new Icon()
        {
            IconInfo = iconInfo
        };
    }
    
    protected Icon? BuildIcon(int iconKind, TwoToneColorInfo twoToneColorInfo)
    {
        var iconInfo = GetIconInfo(iconKind, twoToneColorInfo);
        if (iconInfo is null)
        {
            return null;
        }

        return new Icon()
        {
            IconInfo = iconInfo
        };
    }
    
    protected IconInfo? GetIconInfo(int iconKind)
    {
        if (!_iconInfoPool.ContainsKey(iconKind))
        {
            return null;
        }

        var iconInfo = _iconInfoPool[iconKind].Value;
        if (!iconInfo.IsTwoTone)
        {
            iconInfo.ColorInfo = DefaultColorInfo;
        }
        else
        {
            iconInfo.TwoToneColorInfo = DefaultTwoToneColorInfo;
        }
        return iconInfo;
    }
    
    protected IconInfo? GetIconInfo(int iconKind, ColorInfo colorInfo)
    {
        var iconInfo = GetIconInfo(iconKind);
        if (iconInfo is null)
        {
            return null;
        }

        if (!iconInfo.IsTwoTone)
        {
            iconInfo.ColorInfo = colorInfo;
        }
        return iconInfo;
    }
    
    protected IconInfo? GetIconInfo(int iconKind, TwoToneColorInfo twoToneColorInfo)
    {
        var iconInfo = GetIconInfo(iconKind);
        if (iconInfo is null)
        {
            return null;
        }

        if (iconInfo.IsTwoTone)
        {
            iconInfo.TwoToneColorInfo = twoToneColorInfo;
        }

        return iconInfo;
    }

    public IEnumerable<IconInfo> GetIconInfos(IconThemeType? iconThemeType = null)
    {
        if (!iconThemeType.HasValue)
        {
            return _iconInfoPool.Values.Select(x =>
            {
                var iconInfo = x.Value;
                EnsureDefaultColorInfo(iconInfo);
                return iconInfo;
            });
        }

        return _iconInfoPool.Values.Where(iconInfo => iconInfo.Value.ThemeType == iconThemeType.Value)
                            .Select(x =>
                            {
                                var iconInfo = x.Value;
                                EnsureDefaultColorInfo(iconInfo);
                                return iconInfo;
                            });
    }

    private void EnsureDefaultColorInfo(IconInfo iconInfo)
    {
        if (!iconInfo.IsTwoTone)
        {
            iconInfo.ColorInfo = DefaultColorInfo;
        }
        else
        {
            iconInfo.TwoToneColorInfo = DefaultTwoToneColorInfo;
        }
    }
}