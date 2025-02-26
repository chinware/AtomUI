using AtomUI.Controls.Utils;
using AtomUI.Media;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace AtomUI.Controls.Primitives;

/// <summary>
/// 只在动画过程中使用且不能改变属性，所以在这里使用了自动属性
/// </summary>
internal class MotionGhostControl : Control
{
    #region 公共属性定义

    /// <summary>
    /// 渲染的阴影值，一般在探测失败的时候使用
    /// </summary>
    public BoxShadows Shadows { get; }

    /// <summary>
    /// mask 的圆角大小，一般在探测失败的时候使用
    /// </summary>
    public CornerRadius MaskCornerRadius { get; }

    public Point MaskOffset { get; }

    public Size MaskSize { get; }

    public Size MotionTargetSize { get; }

    #endregion

    protected Canvas? _layout;
    protected MotionTargetBitmapControl _motionTargetBitmapControl;

    public MotionGhostControl(RenderTargetBitmap motionTargetBitmap,
                              Size motionTargetSize,
                              Size maskSize,
                              Point maskOffset,
                              CornerRadius maskCornerRadius,
                              BoxShadows shadows)
    {
        MotionTargetSize = motionTargetSize;

        MaskOffset       = maskOffset;
        MaskSize         = maskSize;
        MaskCornerRadius = maskCornerRadius;

        Shadows = shadows;

        _motionTargetBitmapControl = new MotionTargetBitmapControl(motionTargetBitmap)
        {
            Width  = MotionTargetSize.Width,
            Height = MotionTargetSize.Height
        };
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        IsHitTestVisible = false;

        _layout = new Canvas();
        LogicalChildren.Add(_layout);
        VisualChildren.Add(_layout);

        var shadowThickness = Shadows.Thickness();
        var offsetX         = shadowThickness.Left;
        var offsetY         = shadowThickness.Top;

        var shadowRenderers = BuildShadowRenderers(Shadows);

        foreach (var renderer in shadowRenderers)
        {
            _layout.Children.Add(renderer);
        }

        var border = new Border
        {
            Width      = MotionTargetSize.Width,
            Height     = MotionTargetSize.Height,
        };

        _layout.Children.Add(border);
        _layout.Children.Add(_motionTargetBitmapControl);

        Canvas.SetLeft(_motionTargetBitmapControl, offsetX);
        Canvas.SetTop(_motionTargetBitmapControl, offsetY);

        Canvas.SetLeft(border, offsetX);
        Canvas.SetTop(border, offsetY);
    }

    /// <summary>
    /// 目前的 Avalonia 版本中，当控件渲染到 RenderTargetBitmap 的时候，如果 BoxShadows 的 Count > 1 的时候，如果不是主阴影，后面的阴影如果
    /// 指定 offset，再 RenderScaling > 1 的情况下是错的。
    /// </summary>
    /// <returns></returns>
    private List<Control> BuildShadowRenderers(in BoxShadows shadows)
    {
        var thickness = Shadows.Thickness();
        var offsetX   = thickness.Left;
        var offsetY   = thickness.Top;

        offsetX += MaskOffset.X;
        offsetY += MaskOffset.Y;
        // 不知道这里为啥不行
        var renderers = new List<Control>();
        for (var i = 0; i < shadows.Count; ++i)
        {
            var renderer = new Border
            {
                BorderThickness = new Thickness(0),
                BoxShadow       = new BoxShadows(shadows[i]),
                CornerRadius    = MaskCornerRadius,
                Width           = MaskSize.Width,
                Height          = MaskSize.Height
            };
            Canvas.SetLeft(renderer, offsetX);
            Canvas.SetTop(renderer, offsetY);
            renderers.Add(renderer);
        }

        return renderers;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        base.MeasureOverride(availableSize);
        return MotionTargetSize.Inflate(Shadows.Thickness());
    }
}

internal static class MotionGhostControlUtils
{
    public static MotionGhostControl BuildMotionGhost(Control motionTarget, BoxShadows maskShadows)
    {
        CornerRadius cornerRadius = default;
        Point        maskOffset   = default;
        Size         maskSize     = default;
        if (motionTarget is IShadowMaskInfoProvider shadowMaskInfoProvider)
        {
            cornerRadius = shadowMaskInfoProvider.GetMaskCornerRadius();
            var maskBounds = shadowMaskInfoProvider.GetMaskBounds();
            maskOffset = maskBounds.Position;
            maskSize   = maskBounds.Size;
        }
        else if (motionTarget is Border bordered)
        {
            cornerRadius = bordered.CornerRadius;
        }
        else if (motionTarget is TemplatedControl templatedControl)
        {
            cornerRadius = templatedControl.CornerRadius;
        }

        Size targetSize = default;
        if (motionTarget.DesiredSize == default)
        {
            targetSize = LayoutHelper.MeasureChild(motionTarget, Size.Infinity, new Thickness());
        }
        else
        {
            targetSize = motionTarget.DesiredSize;
        }

        var motionTargetBitmap = motionTarget.CaptureCurrentBitmap();

        return new MotionGhostControl(motionTargetBitmap, targetSize, maskSize, maskOffset, cornerRadius, maskShadows);
    }
}

internal class MotionTargetBitmapControl : Control
{
    protected RenderTargetBitmap _contentBitmap;

    public MotionTargetBitmapControl(RenderTargetBitmap motionTargetBitmap)
    {
        _contentBitmap = motionTargetBitmap;
    }

    public override void Render(DrawingContext context)
    {
        var scaling = TopLevel.GetTopLevel(this)?.RenderScaling ?? 1.0;
        context.DrawImage(_contentBitmap, new Rect(new Point(0, 0), DesiredSize * scaling),
            new Rect(new Point(0, 0), DesiredSize));
    }
}