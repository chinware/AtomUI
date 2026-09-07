using System.Runtime.Versioning;
using NWayland.Protocols.Wayland;

namespace AtomUI.Native;

[SupportedOSPlatform("linux")]
internal static class WaylandWindowUtils
{
    public static bool SetInputRectangle(
        object surface,
        object compositor,
        int x,
        int y,
        int width,
        int height)
    {
        if (!OperatingSystem.IsLinux() ||
            surface is not WlSurface wlSurface ||
            compositor is not WlCompositor wlCompositor ||
            width <= 0 ||
            height <= 0)
        {
            return false;
        }

        try
        {
            var region = wlCompositor.CreateRegion();
            try
            {
                region.Add(x, y, width, height);
                wlSurface.SetInputRegion(region);
            }
            finally
            {
                try
                {
                    region.Destroy();
                }
                finally
                {
                    region.Dispose();
                }
            }

            return true;
        }
        catch (ObjectDisposedException)
        {
            return false;
        }
    }
}
