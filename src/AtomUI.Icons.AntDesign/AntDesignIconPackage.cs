using AtomUI.Controls;

namespace AtomUI.Icons.AntDesign;

public partial class AntDesignIconPackage : IconPackage
{
    public static AntDesignIconPackage Current { get; }

    static AntDesignIconPackage()
    {
        Current = new AntDesignIconPackage();
    }
    
    public AntDesignIconPackage()
        : base("AntDesign")
    {
        SetupIconPool();
    }

    private partial void SetupIconPool();
    
    public IconInfo GetIconInfo(AntDesignIconKind iconKind)
    {
        return GetIconInfo((int)iconKind)!;
    }

    public IconInfo GetIconInfo(AntDesignIconKind iconKind, ColorInfo colorInfo)
    {
        return GetIconInfo((int)iconKind, colorInfo)!;
    }

    public IconInfo GetIconInfo(AntDesignIconKind iconKind, TwoToneColorInfo twoToneColorInfo)
    {
        return GetIconInfo((int)iconKind, twoToneColorInfo)!;
    }

    public override IconInfo? GetIconInfo(string iconKind)
    {
        if (Enum.TryParse(iconKind, out AntDesignIconKind kind))
        {
            return GetIconInfo(kind);
        }

        return null;
    }

    public override IconInfo? GetIconInfo(string iconKind, ColorInfo colorInfo)
    {
        if (Enum.TryParse(iconKind, out AntDesignIconKind kind))
        {
            return GetIconInfo(kind, colorInfo);
        }

        return null;
    }

    public override IconInfo? GetIconInfo(string iconKind, TwoToneColorInfo twoToneColorInfo)
    {
        if (Enum.TryParse(iconKind, out AntDesignIconKind kind))
        {
            return GetIconInfo(kind, twoToneColorInfo);
        }

        return null;
    }
    
    public Icon BuildIcon(AntDesignIconKind iconKind)
    {
        return BuildIcon((int)iconKind)!;
    }

    public Icon BuildIcon(AntDesignIconKind iconKind, ColorInfo colorInfo)
    {
        return BuildIcon((int)iconKind, colorInfo)!;
    }

    public Icon BuildIcon(AntDesignIconKind iconKind, TwoToneColorInfo twoToneColorInfo)
    {
        return BuildIcon((int)iconKind, twoToneColorInfo)!;
    }

    public override Icon? BuildIcon(string iconKind)
    {
        if (Enum.TryParse(iconKind, out AntDesignIconKind kind))
        {
            return BuildIcon(kind);
        }

        return null;
    }

    public override Icon? BuildIcon(string iconKind, ColorInfo colorInfo)
    {
        if (Enum.TryParse(iconKind, out AntDesignIconKind kind))
        {
            return BuildIcon(kind, colorInfo);
        }

        return null;
    }

    public override Icon? BuildIcon(string iconKind, TwoToneColorInfo twoToneColorInfo)
    {
        if (Enum.TryParse(iconKind, out AntDesignIconKind kind))
        {
            return BuildIcon(kind, twoToneColorInfo);
        }

        return null;
    }
}