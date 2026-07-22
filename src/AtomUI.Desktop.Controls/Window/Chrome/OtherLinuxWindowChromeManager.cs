using System.Runtime.Versioning;

namespace AtomUI.Desktop.Controls;

[SupportedOSPlatform("linux")]
internal sealed class OtherLinuxWindowChromeManager : LinuxWindowChromeManager
{
    internal OtherLinuxWindowChromeManager(Window window)
        : base(window)
    {
    }
}
