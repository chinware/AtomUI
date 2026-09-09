using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.VisualTree;

namespace AtomUI.Toolkits.GalleryBase.Controls;

internal sealed class SemanticPartAdorner : Control
{
    private const double PrimaryLayoutOutset = 3;
    private const double SecondaryLayoutOutset = 1;

    private static readonly ImmutablePen PrimaryPen = new(0xFFFAAD14, 2);
    private static readonly ImmutablePen SecondaryPen = new(0xD9FAAD14, 1);
    private static readonly ImmutablePen PrimaryHaloPen = new(0xFFFFFFFF, 1);

    private readonly bool _isPrimary;
    private readonly double _layoutOutset;

    private SemanticPartAdorner(bool isPrimary)
    {
        _isPrimary       = isPrimary;
        _layoutOutset    = isPrimary ? PrimaryLayoutOutset : SecondaryLayoutOutset;
        Focusable        = false;
        IsHitTestVisible = false;
        Margin           = new Thickness(-_layoutOutset);

        // Semantic markers are an inspection overlay, not content constrained by the
        // target's ancestor layout. Keep this invariant on the adorner itself so every
        // creation path remains safe when the target sits under ClipToBounds ancestors.
        AdornerLayer.SetIsClipEnabled(this, false);
    }

    internal static SemanticPartAdorner Create(Visual target, bool isPrimary)
    {
        ArgumentNullException.ThrowIfNull(target);
        var adorner = new SemanticPartAdorner(isPrimary);
        AdornerLayer.SetAdornedElement(adorner, target);
        return adorner;
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        // AdornerLayer 按目标尺寸排列 child。负 Margin 让 adorner 自身覆盖外扩区域，
        // 避免把描边画到 Bounds 之外后在目标左侧或上侧被裁掉。
        if (_isPrimary)
        {
            DrawMarker(context, PrimaryHaloPen, 2.5);
            DrawMarker(context, PrimaryPen, 1);
        }
        else
        {
            DrawMarker(context, SecondaryPen, 0.5);
        }
    }

    private void DrawMarker(DrawingContext context, IPen pen, double markerOutset)
    {
        var markerRect = GetMarkerRect(Bounds.Size, _layoutOutset, markerOutset);

        // 贴边满区目标（如 native 预览对话框的 popup.root/body）外扩描边会越出窗口
        // 表面被 OS 裁剪（只剩贴窗的一条边）。钳制到 adorner 所在层（窗口客户区）内，
        // 并预留笔宽与外扩余量；层不可达或余量不足时按原矩形绘制。
        var layer = _layer ??= this.GetVisualAncestors().OfType<AdornerLayer>().FirstOrDefault();
        if (layer is not null)
        {
            var transform = (RenderTransform as MatrixTransform)?.Matrix ?? Matrix.Identity;
            var positionInLayer = new Point(
                Bounds.Position.X + transform.M31,
                Bounds.Position.Y + transform.M32);
            markerRect = ClampMarkerRect(markerRect, layer.Bounds, positionInLayer, _layoutOutset);
        }

        context.DrawRectangle(null, pen, markerRect);
    }

    /// <summary>
    /// 把描边矩形钳制到层（窗口客户区）在 adorner 本地坐标空间的范围内。钳制量保留
    /// 外扩与笔宽余量，保证四条边完整落在窗口表面内；交集退化（层过小）时返回原矩形。
    /// </summary>
    internal static Rect ClampMarkerRect(
        Rect markerRect,
        Rect layerBounds,
        Point adornerPositionInLayer,
        double layoutOutset)
    {
        var layerLocal = new Rect(
            -adornerPositionInLayer.X,
            -adornerPositionInLayer.Y,
            layerBounds.Width,
            layerBounds.Height);
        var clamped = markerRect.Intersect(layerLocal.Deflate(layoutOutset + 2));
        return clamped.Width >= 1 && clamped.Height >= 1 ? clamped : markerRect;
    }

    internal static Rect GetMarkerRect(Size adornerBounds, double layoutOutset, double markerOutset)
    {
        var inset = layoutOutset - markerOutset;
        return new Rect(inset,
                        inset,
                        Math.Max(0, adornerBounds.Width - inset * 2),
                        Math.Max(0, adornerBounds.Height - inset * 2));
    }

    private AdornerLayer? _layer;
}
