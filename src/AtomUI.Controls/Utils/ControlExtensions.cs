using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media.Imaging;

namespace AtomUI.Controls.Utils;

internal static class ControlExtensions
{
    public static RenderTargetBitmap CaptureCurrentBitmap(this Control control)
    {
        var scaling    = TopLevel.GetTopLevel(control)?.RenderScaling ?? 1.0;
        Size targetSize = default;
        if (control.DesiredSize == default)
        {
            targetSize = LayoutHelper.MeasureChild(control, Size.Infinity, new Thickness());
        }
        else
        {
            targetSize = control.DesiredSize;
        }
        var bitmap = new RenderTargetBitmap(
            new PixelSize((int)(targetSize.Width * scaling), (int)(targetSize.Height * scaling)),
            new Vector(96 * scaling, 96 * scaling));
        bitmap.Render(control);
        return bitmap;
    }
}