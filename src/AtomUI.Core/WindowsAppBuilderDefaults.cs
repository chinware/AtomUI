using System.Runtime.Versioning;
using Avalonia;

namespace AtomUI;

internal static class WindowsAppBuilderDefaults
{
    private const int Windows11InitialBuild = 22000;

    [SupportedOSPlatform("windows")]
    internal static AppBuilder Apply(AppBuilder appBuilder)
    {
        var isWindows11OrLater = OperatingSystem.IsWindowsVersionAtLeast(
            10,
            0,
            Windows11InitialBuild);
        return appBuilder.With(CreateOptions(isWindows11OrLater));
    }

    internal static Win32PlatformOptions CreateOptions(bool isWindows11OrLater)
    {
        return new Win32PlatformOptions
        {
            RenderingMode =
            [
                Win32RenderingMode.AngleEgl,
                Win32RenderingMode.Software
            ],
            CompositionMode = isWindows11OrLater
                ? [
                    Win32CompositionMode.WinUIComposition,
                    Win32CompositionMode.DirectComposition,
                    Win32CompositionMode.RedirectionSurface
                ]
                : [Win32CompositionMode.RedirectionSurface]
        };
    }
}
