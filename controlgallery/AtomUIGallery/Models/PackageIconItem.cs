using AtomUI.Controls;

namespace AtomUIGallery.Models;

public record PackageIconItem
{
    public string IconName { get; set; }
    public Icon Icon { get; set; }

    public PackageIconItem(string iconName, Icon iconInfo)
    {
        IconName = iconName;
        Icon = iconInfo;
    }
}