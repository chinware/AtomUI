using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Rendering;

namespace AtomUI.Controls;

internal class TabScrollContentPresenter : ScrollContentPresenter, ICustomHitTest
{
    private static readonly MethodInfo SnapOffsetMethodInfo;

    static TabScrollContentPresenter()
    {
        SnapOffsetMethodInfo =
            typeof(ScrollContentPresenter).GetMethod("SnapOffset", BindingFlags.Instance | BindingFlags.NonPublic)!;
    }

    internal Dock TabStripPlacement { get; set; } = Dock.Top;

    public bool HitTest(Point point)
    {
        return true;
    }

    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        if (Extent.Height > Viewport.Height || Extent.Width > Viewport.Width)
        {
            var scrollable = Child as ILogicalScrollable;
            var isLogical  = scrollable?.IsLogicalScrollEnabled == true;

            var x                                                                        = Offset.X;
            var y                                                                        = Offset.Y;
            var delta                                                                    = e.Delta;
            if (TabStripPlacement == Dock.Top || TabStripPlacement == Dock.Bottom) delta = new Vector(delta.Y, delta.X);

            if (Extent.Height > Viewport.Height)
            {
                var height = isLogical ? scrollable!.ScrollSize.Height : 50;
                y += -delta.Y * height;
                y =  Math.Max(y, 0);
                y =  Math.Min(y, Extent.Height - Viewport.Height);
            }

            if (Extent.Width > Viewport.Width)
            {
                var width = isLogical ? scrollable!.ScrollSize.Width : 50;
                x += -delta.X * width;
                x =  Math.Max(x, 0);
                x =  Math.Min(x, Extent.Width - Viewport.Width);
            }

            var newOffset = (Vector)SnapOffsetMethodInfo.Invoke(this, new object?[] { new Vector(x, y), delta, true })!;

            var offsetChanged = newOffset != Offset;
            SetCurrentValue(OffsetProperty, newOffset);

            e.Handled = !IsScrollChainingEnabled || offsetChanged;
        }
    }
}