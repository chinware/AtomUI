namespace AtomUI.Controls;

public interface IIconPackage
{
    public string Id { get; }
    public int Priority { get; set; }
    
    public IconInfo? GetIconInfo(string iconKind);
    public IconInfo? GetIconInfo(string iconKind, ColorInfo colorInfo);
    public IconInfo? GetIconInfo(string iconKind, TwoToneColorInfo twoToneColorInfo);

    public IEnumerable<IconInfo> GetIconInfos(IconThemeType? iconThemeType);
}