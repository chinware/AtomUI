using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace AtomUI.Toolkits.GalleryBase.Controls;

internal sealed class SemanticPartAdorner : Control
{
    private static readonly ImmutablePen PrimaryPen = new(0xFFFAAD14, 2);
    private static readonly ImmutablePen SecondaryPen = new(0xD9FAAD14, 1);
    private static readonly ImmutablePen PrimaryHaloPen = new(0xFFFFFFFF, 1);

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

        // 与 antd SemanticPreview Marker 保持一致：标记沿目标外沿绘制。
        // 主标记先画 1px 白色外环（对应 boxShadow 0 0 0 1px #fff），再以 2px 金框
        // 紧贴目标外沿描边；副标记为 1px 金框。描边落在目标边界外侧，
        // 细窄目标（如 4px 高的 slider tracks）也能获得清晰可见的金框。
        if (_isPrimary)
        {
            context.DrawRectangle(null, PrimaryHaloPen, GetMarkerRect(Bounds.Size, 2.5));
            context.DrawRectangle(null, PrimaryPen, GetMarkerRect(Bounds.Size, 1));
        }
        else
        {
            context.DrawRectangle(null, SecondaryPen, GetMarkerRect(Bounds.Size, 0.5));
        }
    }

    internal static Rect GetMarkerRect(Size bounds, double outerInset)
    {
        return new Rect(-outerInset,
                        -outerInset,
                        bounds.Width + outerInset * 2,
                        bounds.Height + outerInset * 2);
    }
}
