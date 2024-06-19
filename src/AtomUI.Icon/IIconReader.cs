namespace AtomUI.Icon;

public interface IIconReader
{
   public IconInfo? GetIcon(string iconKind);
   public IconInfo? GetIcon(string iconKind, ColorInfo colorInfo);
   public IconInfo? GetIcon(string iconKind, TwoToneColorInfo twoToneColorInfo);
}