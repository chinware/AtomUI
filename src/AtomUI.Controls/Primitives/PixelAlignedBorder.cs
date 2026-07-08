using Avalonia.Controls;

namespace AtomUI.Controls.Primitives;

public class PixelAlignedBorder : DashedBorder
{
    protected override Type StyleKeyOverride => typeof(Border);
}
