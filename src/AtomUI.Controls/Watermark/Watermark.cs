using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using AtomUI.Controls.Primitives;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace AtomUI.Controls;

public sealed class Watermark : Control
{
    public static WatermarkGlyph? GetGlyph(Layoutable element)
    {
        return element.GetValue(GlyphProperty);
    }

    public static void SetGlyph(Layoutable element, WatermarkGlyph? value)
    {
        element.SetValue(GlyphProperty, value);
    }

    public static readonly AttachedProperty<WatermarkGlyph?> GlyphProperty = AvaloniaProperty
        .RegisterAttached<Watermark, Layoutable, WatermarkGlyph?>("Glyph");

    public Layoutable Target { get; }

    private WatermarkGlyph? Glyph { get; }

    static Watermark()
    {
        IsHitTestVisibleProperty.OverrideMetadata<Watermark>(new StyledPropertyMetadata<bool>(false));
        GlyphProperty.Changed.AddClassHandler<Layoutable>(OnGlyphChanged);
    }

    private Watermark(Layoutable target, WatermarkGlyph? glyph)
    {
        Target = target;
        Glyph  = glyph;
    }

    #region 渲染缓存

    // 两种旋转矩阵（正向 / 镜像），仅在 glyph 属性或尺寸变化时重建
    private Matrix _normalRotationMatrix;
    private Matrix _mirrorRotationMatrix;
    private Size   _cachedGlyphSize;
    private Size   _cachedTargetSize;
    private bool   _matrixCacheValid;

    private void RebuildMatrixCache(Size glyphSize, Size targetSize)
    {
        var angleRad           = Glyph!.Rotate * Math.PI / 180;
        _normalRotationMatrix  = MatrixUtils.CreateRotationRadians(angleRad, glyphSize.Width / 2, glyphSize.Height / 2);
        _mirrorRotationMatrix  = MatrixUtils.CreateRotationRadians(-angleRad, glyphSize.Width / 2, glyphSize.Height / 2);
        _cachedGlyphSize       = glyphSize;
        _cachedTargetSize      = targetSize;
        _matrixCacheValid      = true;
    }

    #endregion

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (Glyph != null)
        {
            Glyph.PropertyChanged += HandleGlyphPropertyChanged;
        }
    }

    private void HandleGlyphPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs args)
    {
        _matrixCacheValid = false;
        InvalidateVisual();
    }

    private static void OnGlyphChanged(Layoutable target, AvaloniaPropertyChangedEventArgs arg)
    {
        if (target.IsArrangeValid)
        {
            InstallWatermark(target);
        }
        target.LayoutUpdated += HandleTargetLayoutUpdated;
    }

    private static void HandleTargetLayoutUpdated(object? sender, EventArgs e)
    {
        if (sender is not Layoutable target)
        {
            return;
        }
        target.LayoutUpdated -= HandleTargetLayoutUpdated;
        InstallWatermark(target);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        if (Glyph != null)
        {
            Glyph.PropertyChanged -= HandleGlyphPropertyChanged;
        }
        UnInstallWatermark(this);
    }

    private static void InstallWatermark(Layoutable target)
    {
        if (CheckLayer(target, out var layer) == false)
        {
            return;
        }

        var watermark = ScopeAwareAdornerLayer.GetAdorner(target);
        if (watermark != null)
        {
            return;
        }

        watermark = new Watermark(target, GetGlyph(target));
        ScopeAwareAdornerLayer.SetAdornedElement(watermark, target);
        layer.Children.Add(watermark);
    }

    private static void UnInstallWatermark(Visual target)
    {
        if (CheckLayer(target, out var layer) == false)
        {
            return;
        }
        var watermark = ScopeAwareAdornerLayer.GetAdorner(target);
        if (watermark == null)
        {
            return;
        }
        layer.Children.Remove(watermark);
    }

    private static bool CheckLayer(Visual target, [NotNullWhen(true)] out ScopeAwareAdornerLayer? layer)
    {
        layer = ScopeAwareAdornerLayer.GetLayer(target);
        if (layer == null)
        {
            Trace.WriteLine($"Can not get ScopeAwareAdornerLayer for {target} to show a watermark.");
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

        var targetSize = Target.Bounds.Size;

        // Rebuild matrix cache when invalidated or sizes changed
        if (!_matrixCacheValid || _cachedGlyphSize != size || _cachedTargetSize != targetSize)
        {
            RebuildMatrixCache(size, targetSize);
        }

        using (context.PushClip(new Rect(targetSize)))
        using (context.PushOpacity(Glyph.Opacity))
        {
            var t = Glyph.VerticalOffset;
            var r = 0;
            while (t < targetSize.Height)
            {
                var pushState = new DrawingContext.PushedState();
                if (r % 2 == 1 && Glyph.IsCrossUsed)
                {
                    pushState = context.PushTransform(
                        Matrix.CreateTranslation((Glyph.HorizontalSpace - size.Width) / 2 + size.Width, 0));
                }

                using (pushState)
                {
                    var l = Glyph.HorizontalOffset;
                    var c = 0;
                    while (l < targetSize.Width)
                    {
                        // Use pre-cached rotation matrix — no trig/allocation per tile
                        var m = c % 2 == 1 && Glyph.IsMirrorUsed
                            ? _mirrorRotationMatrix
                            : _normalRotationMatrix;

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