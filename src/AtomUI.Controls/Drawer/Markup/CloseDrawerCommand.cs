using System.Diagnostics;
using AtomUI.Controls.Utils;
using Avalonia.Controls;

namespace AtomUI.Controls;

internal class CloseDrawerCommand : InternalCommand
{
    public CloseDrawerCommand(Control anchor) : base(p => CloseDrawer(anchor))
    {

    }

    private static void CloseDrawer(Control anchor)
    {
        var drawer = Drawer.GetDrawer(anchor);
        if (drawer == null)
        {
            Trace.WriteLine($"Can not find the drawer that contains the control {anchor} (#{anchor.GetHashCode()}).");
            return;
        }

        drawer.SetCurrentValue(Drawer.IsOpenProperty, false);
    }
}