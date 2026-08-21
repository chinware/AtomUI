using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Media.Immutable;

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
            context.DrawRectangle(null, PrimaryHaloPen, GetMarkerRect(Bounds.Size, _layoutOutset, 2.5));
            context.DrawRectangle(null, PrimaryPen, GetMarkerRect(Bounds.Size, _layoutOutset, 1));
        }
        else
        {
            context.DrawRectangle(null, SecondaryPen, GetMarkerRect(Bounds.Size, _layoutOutset, 0.5));
        }
    }

    internal static Rect GetMarkerRect(Size adornerBounds, double layoutOutset, double markerOutset)
    {
        var inset = layoutOutset - markerOutset;
        return new Rect(inset,
                        inset,
                        Math.Max(0, adornerBounds.Width - inset * 2),
                        Math.Max(0, adornerBounds.Height - inset * 2));
    }
}
