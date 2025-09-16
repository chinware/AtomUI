using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace AtomUI.Controls.Primitives;

internal class MotionBitmapGhostControl : Control
{
    protected RenderTargetBitmap _contentBitmap;

    public MotionBitmapGhostControl(RenderTargetBitmap motionTargetBitmap)
    {
        _contentBitmap = motionTargetBitmap;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        return _contentBitmap.Size;
    }

    public override void Render(DrawingContext context)
    {
        var scaling = TopLevel.GetTopLevel(this)?.RenderScaling ?? 1.0;
        context.DrawImage(_contentBitmap, new Rect(new Point(0, 0), DesiredSize * scaling),
            new Rect(new Point(0, 0), DesiredSize));
    }
}