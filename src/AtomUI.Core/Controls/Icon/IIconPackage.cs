namespace AtomUI.Controls;

public interface IIconPackage<TIconKind>
    where TIconKind : Enum
{
    public string Id { get; }
    public int Priority { get; set; }
    
    public IconInfo? GetIconInfo(TIconKind iconKind);
    public IconInfo? GetIconInfo(TIconKind iconKind, ColorInfo colorInfo);
    public IconInfo? GetIconInfo(TIconKind iconKind, TwoToneColorInfo twoToneColorInfo);

    public IEnumerable<IconInfo> GetIconInfos(IconThemeType? iconThemeType);
}