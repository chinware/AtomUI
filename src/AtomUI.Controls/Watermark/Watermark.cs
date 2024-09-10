using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using AtomUI.Controls.Primitives;
using AtomUI.Controls.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

public sealed class Watermark : Control
{
    public static WatermarkGlyph? GetGlyph(Visual element)
    {
        return element.GetValue(GlyphProperty);
    }

    public static void SetGlyph(Visual element, WatermarkGlyph? value)
    {
        element.SetValue(GlyphProperty, value);
    }

    public static readonly AttachedProperty<WatermarkGlyph?> GlyphProperty = AvaloniaProperty
        .RegisterAttached<Watermark, Visual, WatermarkGlyph?>("Glyph");

    public Visual Target { get; }

    private WatermarkGlyph? Glyph { get; }

    static Watermark()
    {
        IsHitTestVisibleProperty.OverrideMetadata<Watermark>(new StyledPropertyMetadata<bool>(false));
        GlyphProperty.Changed.AddClassHandler<Visual>(OnGlyphChanged);
    }

    private Watermark(Visual target, WatermarkGlyph? glyph)
    {
        Target = target;
        Glyph  = glyph;

        if (glyph != null)
        {
            glyph.PropertyChanged += (sender, args) => { InvalidateVisual(); };
        }
    }

    private static void OnGlyphChanged(Visual target, AvaloniaPropertyChangedEventArgs arg)
    {
        if (target.IsAttachedToVisualTree())
        {
            InstallWatermark(target);
        }
        else
        {
            target.AttachedToVisualTree += TargetOnAttachedToVisualTree;
        }
    }

    private static void TargetOnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (sender is not Visual target)
        {
            return;
        }

        target.AttachedToVisualTree -= TargetOnAttachedToVisualTree;

        InstallWatermark(target);
    }

    private static void InstallWatermark(Visual target)
    {
        if (CheckLayer(target, out var layer) == false)
        {
            return;
        }

        var watermark = layer.GetAdorner<Watermark>(target);
        if (watermark != null)
        {
            return;
        }

        watermark = new Watermark(target, GetGlyph(target));
        layer.AddAdorner(target, watermark);
    }

    private static bool CheckLayer(Visual target, [NotNullWhen(true)] out AtomLayer? layer)
    {
        layer = target.GetLayer();
        if (layer == null)
        {
            Trace.WriteLine($"Can not get AxLayer for {target} to show a watermark.");
        }

        return layer != null;
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        if (Glyph == null)
        {
            return;
        }

        var size = Glyph.GetDesiredSize();
        if (size.Width == 0 || size.Height == 0)
        {
            return;
        }

        using (context.PushClip(new Rect(Target.Bounds.Size)))
        using (context.PushOpacity(Glyph.Opacity))
        {
            var t = Glyph.VerticalOffset;
            var r = 0;
            while (t < Target.Bounds.Height)
            {
                var pushState = new DrawingContext.PushedState();
                if (r % 2 == 1 && Glyph.UseCross)
                {
                    pushState = context.PushTransform(
                        Matrix.CreateTranslation((Glyph.HorizontalSpace - size.Width) / 2 + size.Width, 0));
                }

                using (pushState)
                {
                    var l = Glyph.HorizontalOffset;
                    var c = 0;
                    while (l < Target.Bounds.Width)
                    {
                        var angle = Glyph.Rotate;
                        if (c % 2 == 1 && Glyph.UseMirror)
                        {
                            angle = -angle;
                        }

                        var m = MatrixUtil.CreateRotationRadians(angle * Math.PI / 180, size.Width / 2,
                            size.Height / 2);
                        using (context.PushTransform(Matrix.CreateTranslation(l, t)))
                        using (context.PushTransform(m))
                        {
                            Glyph.Render(context);
                        }

                        l += size.Width + Glyph.HorizontalSpace;
                        c++;
                    }

                    t += size.Height + Glyph.VerticalSpace;
                    r++;
                }
            }
        }
    }
}