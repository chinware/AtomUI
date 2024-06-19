namespace AtomUI.Icon;

public interface IIconPackageProvider : IIconReader
{
   public string Id { get; }
   public int Priority { get; set; }

   public IEnumerable<IconInfo> GetIconInfos(IconThemeType? iconThemeType);
}