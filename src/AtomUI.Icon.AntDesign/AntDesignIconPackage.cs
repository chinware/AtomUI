namespace AtomUI.Icon.AntDesign;

public partial class AntDesignIconPackage : IconPackage
{
    public AntDesignIconPackage()
        : base("AntDesign")
    {
        SetupIconPool();
    }

    private partial void SetupIconPool();

    public IconInfo? GetIcon(AntDesignIconKind iconKind)
    {
        return GetIconRaw((int)iconKind)!;
    }

    public IconInfo GetIcon(AntDesignIconKind iconKind, ColorInfo colorInfo)
    {
        return GetIconRaw((int)iconKind, colorInfo)!;
    }

    public IconInfo GetIcon(AntDesignIconKind iconKind, TwoToneColorInfo twoToneColorInfo)
    {
        return GetIconRaw((int)iconKind, twoToneColorInfo)!;
    }

    public override IconInfo? GetIcon(string iconKind, ColorInfo colorInfo)
    {
        AntDesignIconKind kind;
        if (Enum.TryParse(iconKind, out kind)) return GetIcon(kind, colorInfo);
        return null;
    }

    public override IconInfo? GetIcon(string iconKind, TwoToneColorInfo twoToneColorInfo)
    {
        AntDesignIconKind kind;
        if (Enum.TryParse(iconKind, out kind)) return GetIcon(kind, twoToneColorInfo);
        return null;
    }

    public override IconInfo? GetIcon(string iconKind)
    {
        AntDesignIconKind kind;
        if (Enum.TryParse(iconKind, out kind)) return GetIcon(kind);
        return null;
    }
}