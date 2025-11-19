using Avalonia.Media;

namespace AtomUI.Controls;

public abstract class IconPackage<TIconKind> : IIconPackage<TIconKind>
    where TIconKind : Enum
{
    protected IDictionary<TIconKind, Func<IconInfo>> _iconInfoPool;
    
    public ColorInfo DefaultColorInfo { get; set; }
    
    public TwoToneColorInfo DefaultTwoToneColorInfo { get; set; }
    
    public string Id { get; }
    
    public int Priority { get; set; } = 0;
    
    protected IDictionary<IconThemeType, (int, int)> IconThemeRanges { get; }

    public IconPackage(string id)
    {
        Id               = id;
        _iconInfoPool    = new Dictionary<TIconKind, Func<IconInfo>>();
        DefaultColorInfo = new ColorInfo(Color.FromRgb(85, 85, 85)); // #333
        DefaultTwoToneColorInfo = new TwoToneColorInfo
        {
            PrimaryColor   = Color.FromRgb(22, 119, 255),
            SecondaryColor = Color.FromRgb(230, 244, 255)
        };
        IconThemeRanges = new Dictionary<IconThemeType, (int, int)>();
    }
    
    public Icon? BuildIcon(TIconKind iconKind)
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
    
    public Icon? BuildIcon(TIconKind iconKind, ColorInfo colorInfo)
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
    
    public Icon? BuildIcon(TIconKind iconKind, TwoToneColorInfo twoToneColorInfo)
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
    
    public IconInfo? GetIconInfo(TIconKind iconKind)
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
    
    public IconInfo? GetIconInfo(TIconKind iconKind, ColorInfo colorInfo)
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
    
    public IconInfo? GetIconInfo(TIconKind iconKind, TwoToneColorInfo twoToneColorInfo)
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

    private IconThemeType GetIconThemeType(TIconKind kind)
    {
        var kindValue = Convert.ToInt32(kind);
        foreach (var entry in IconThemeRanges)
        {
            
            if (kindValue >= entry.Value.Item1 && kindValue <= entry.Value.Item2)
            {
                return entry.Key;
            }
        }
        throw new ArgumentException($"Unknown icon kind: {kind}");
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