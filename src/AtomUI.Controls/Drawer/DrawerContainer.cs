using Avalonia.Controls;

namespace AtomUI.Controls;

public class DrawerContainer : ContentControl
{
    protected override Type StyleKeyOverride { get; } = typeof(ContentControl);
}