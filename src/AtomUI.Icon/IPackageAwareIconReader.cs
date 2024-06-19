namespace AtomUI.Icon;

public interface IPackageAwareIconReader : IIconReader
{
   public IconInfo? GetIcon(string package, string iconKind);
   public IconInfo? GetIcon(string package, string iconKind, ColorInfo colorInfo);
   public IconInfo? GetIcon(string package, string iconKind, TwoToneColorInfo twoToneColorInfo);
}