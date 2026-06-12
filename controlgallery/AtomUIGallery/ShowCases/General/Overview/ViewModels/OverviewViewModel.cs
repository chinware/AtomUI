using AtomUI.Controls;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Overview;

public class OverviewViewModel : ReactiveObject, IRoutableViewModel
{
    private const string DesktopControlsPackageName = "AtomUI.Desktop.Controls";

    public static EntityKey ID = "Overview";

    public IScreen HostScreen { get; }

    public string UrlPathSegment { get; } = ID.ToString();

    public string DotNetCliInstallCommand =>
        $"dotnet add package {DesktopControlsPackageName} --version {GalleryVersionInfo.Version}";

    public string PackageReferenceInstallCommand =>
        $"<PackageReference Include=\"{DesktopControlsPackageName}\" Version=\"{GalleryVersionInfo.Version}\" />";

    public string PackageManagerInstallCommand =>
        $"Install-Package {DesktopControlsPackageName} -Version {GalleryVersionInfo.Version}";

    public OverviewViewModel(IScreen screen)
    {
        HostScreen = screen;
    }
}
