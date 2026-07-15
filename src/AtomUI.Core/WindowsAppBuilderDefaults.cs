using System.Runtime.Versioning;
using Avalonia;

namespace AtomUI;

internal static class WindowsAppBuilderDefaults
{
    [SupportedOSPlatform("windows")]
    internal static AppBuilder Apply(AppBuilder appBuilder)
    {
        return appBuilder.With(CreateOptions());
    }

    internal static Win32PlatformOptions CreateOptions()
    {
        return new Win32PlatformOptions
        {
            RenderingMode =
            [
                Win32RenderingMode.AngleEgl,
                Win32RenderingMode.Software
            ],
            CompositionMode = [Win32CompositionMode.RedirectionSurface]
        };
    }
}
