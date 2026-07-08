using Avalonia;
using Avalonia.Layout;

namespace AtomUI.Utils;

internal static class BorderUtils
{
    public static Thickness BuildLayoutRoundedThickness(Layoutable owner, Thickness borderThickness)
    {
        if (!owner.UseLayoutRounding)
        {
            return borderThickness;
        }

        var scale = LayoutHelper.GetLayoutScale(owner);
        return LayoutHelper.RoundLayoutThickness(borderThickness, scale);
    }
}
