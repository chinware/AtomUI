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
        var halfThickness = pen.Thickness / 2;
        var width = Math.Max(0, Bounds.Width - pen.Thickness);
        var height = Math.Max(0, Bounds.Height - pen.Thickness);
        var bounds = new Rect(halfThickness, halfThickness, width, height);
        context.DrawRectangle(null, pen, bounds);
    }
}
