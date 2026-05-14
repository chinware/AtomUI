using AtomUI.Controls;

namespace AtomUIGallery.Models;

public record PackageIconItem
{
    public string IconName { get; set; }
    public Icon? Icon { get; set; }
    public Type IconType { get; set; }
    public Func<Icon>? Creator { get; set; }

    public PackageIconItem(string iconName, Type iconType, Func<Icon>? creator)
    {
        IconName = iconName;
        IconType = iconType;
        Creator  = creator;
    }
}
