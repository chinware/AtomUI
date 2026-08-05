using AtomUI.Desktop.Controls;
using Avalonia.Controls;

namespace AtomUI.Toolkits.GalleryBase.Shell;

public interface IGallerySidebarNavMenuHost
{
    NavMenu SidebarNavMenu { get; }

    Control? SidebarHeaderAction { get; }
}
