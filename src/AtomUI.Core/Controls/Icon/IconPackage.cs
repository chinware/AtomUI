using Avalonia.Media;

namespace AtomUI.Controls;

public abstract class IconPackage : IIconPackage
{
    protected IDictionary<int, Func<IconInfo>> _iconInfoPool;
    
    public ColorInfo DefaultColorInfo { get; set; }
    
    public TwoToneColorInfo DefaultTwoToneColorInfo { get; set; }
    
    public string Id { get; }
    
    public int Priority { get; set; } = 0;
    
    protected IDictionary<IconThemeType, (int, int)> IconThemeRanges { get; }

    public IconPackage(string id)
    {
        Id               = id;
        _iconInfoPool    = new Dictionary<int, Func<IconInfo>>();
        DefaultColorInfo = new ColorInfo(Color.FromRgb(85, 85, 85)); // #333
        DefaultTwoToneColorInfo = new TwoToneColorInfo
        {
            PrimaryColor   = Color.FromRgb(22, 119, 255),
            SecondaryColor = Color.FromRgb(230, 244, 255)
        };
        IconThemeRanges = new Dictionary<IconThemeType, (int, int)>();
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

        var iconInfo = _iconInfoPool[iconKind]();
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
                var iconInfo = x();
                EnsureDefaultColorInfo(iconInfo);
                return iconInfo;
            });
        }
        return _iconInfoPool.Where(entry => GetIconThemeType(entry.Key) == iconThemeType.Value)
                            .Select(entry =>
                            {
                                var iconInfo = entry.Value();
                                EnsureDefaultColorInfo(iconInfo);
                                return iconInfo;
                            });
    }

    private IconThemeType GetIconThemeType(int id)
    {
        foreach (var entry in IconThemeRanges)
        {
            if (id >= entry.Value.Item1 && id <= entry.Value.Item2)
            {
                return entry.Key;
            }
        }
        throw new ArgumentException($"Unknown icon kind: {id}");
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