using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace AtomUI.Toolkits.GalleryBase.Controls;

internal sealed class SemanticPartAdorner : Control
{
    private static readonly ImmutablePen PrimaryPen = new(0xFFFAAD14, 2);
    private static readonly ImmutablePen SecondaryPen = new(0xD9FAAD14, 1);

    private readonly bool _isPrimary;

    public SemanticPartAdorner(bool isPrimary)
    {
        _isPrimary       = isPrimary;
        Focusable        = false;
        IsHitTestVisible = false;
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var pen = _isPrimary ? PrimaryPen : SecondaryPen;
        context.DrawRectangle(null, pen, GetStrokeRect(Bounds.Size, pen.Thickness));
    }

    internal static Rect GetStrokeRect(Size bounds, double penThickness)
    {
        // 常规目标按画笔厚度内缩描边；目标比画笔还细（如 2px 宽的 rail）时，
        // 以画笔厚度为下限居中描边，避免矩形退化为零尺寸导致描边不可见
        //（首个目标是 2px 主标记，恰好命中该退化场景）。
        var width  = Math.Max(bounds.Width - penThickness, penThickness);
        var height = Math.Max(bounds.Height - penThickness, penThickness);
        var x      = Math.Max(0, (bounds.Width - width) / 2);
        var y      = Math.Max(0, (bounds.Height - height) / 2);
        return new Rect(x, y, width, height);
    }
}
