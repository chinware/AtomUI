using System.Runtime.Versioning;

namespace AtomUI.Desktop.Controls;

[SupportedOSPlatform("linux")]
internal sealed class GenericLinuxWindowChromeManager : AbstractLinuxWindowChromeManager
{
    internal GenericLinuxWindowChromeManager(Window window)
        : base(window)
    {
    }
}
