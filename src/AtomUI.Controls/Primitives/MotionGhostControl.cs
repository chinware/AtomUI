﻿using AtomUI.Media;
using AtomUI.MotionScene;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace AtomUI.Controls.Primitives;

// TODO 这个类的实例只在动画过程中存在，所以是否需要处理属性变化对重绘的需求需要再评估，暂时先不处理
internal class MotionGhostControl : Control, INotifyCaptureGhostBitmap
{
    public static readonly StyledProperty<BoxShadows> ShadowsProperty =
        Border.BoxShadowProperty.AddOwner<MotionGhostControl>();

    public static readonly StyledProperty<CornerRadius> MaskCornerRadiusProperty =
        Border.CornerRadiusProperty.AddOwner<MotionGhostControl>();

    /// <summary>
    /// 渲染的阴影值，一般在探测失败的时候使用
    /// </summary>
    public BoxShadows Shadows
    {
        get => GetValue(ShadowsProperty);
        set => SetValue(ShadowsProperty, value);
    }

    /// <summary>
    /// mask 的圆角大小，一般在探测失败的时候使用
    /// </summary>
    public CornerRadius MaskCornerRadius
    {
        get => GetValue(MaskCornerRadiusProperty);
        set => SetValue(MaskCornerRadiusProperty, value);
    }

    protected bool _initialized;
    protected Canvas? _layout;
    protected Control _motionTarget;
    protected RenderTargetBitmap? _ghostBitmap;
    protected RenderTargetBitmap? _contentBitmap;
    protected Size _motionTargetSize;
    protected Point _maskOffset;
    protected Size _maskSize;

    static MotionGhostControl()
    {
        AffectsMeasure<ShadowRenderer>(ShadowsProperty);
        AffectsRender<ShadowRenderer>(MaskCornerRadiusProperty);
    }

    public MotionGhostControl(Control motionTarget, BoxShadows fallbackShadows = default)
    {
        _motionTarget = motionTarget;
        if (_motionTarget.DesiredSize == default)
        {
            _motionTargetSize = LayoutHelper.MeasureChild(_motionTarget, Size.Infinity, new Thickness());
        }
        else
        {
            _motionTargetSize = _motionTarget.DesiredSize;
        }

        _maskOffset = default;
        _maskSize   = _motionTargetSize;

        Shadows = fallbackShadows;
        DetectMotionTargetInfo();
        var shadowThickness = Shadows.Thickness();
        Width  = _motionTargetSize.Width + shadowThickness.Left + shadowThickness.Right;
        Height = _motionTargetSize.Height + shadowThickness.Top + shadowThickness.Bottom;
    }

    private void DetectMotionTargetInfo()
    {
        CornerRadius cornerRadius = default;
        if (_motionTarget is IShadowMaskInfoProvider shadowMaskInfoProvider)
        {
            cornerRadius = shadowMaskInfoProvider.GetMaskCornerRadius();
            var maskBounds = shadowMaskInfoProvider.GetMaskBounds();
            _maskOffset = maskBounds.Position;
            _maskSize   = maskBounds.Size;
        }
        else if (_motionTarget is Border bordered)
        {
            cornerRadius = bordered.CornerRadius;
        }
        else if (_motionTarget is TemplatedControl templatedControl)
        {
            cornerRadius = templatedControl.CornerRadius;
        }

        // 探测出来的是最准确的，优先级高
        if (cornerRadius != default)
        {
            MaskCornerRadius = cornerRadius;
        }
    }

    public sealed override void ApplyTemplate()
    {
        base.ApplyTemplate();
        if (!_initialized)
        {
            IsHitTestVisible = false;

            _layout = new Canvas();
            VisualChildren.Add(_layout);
            ((ISetLogicalParent)_layout).SetParent(this);

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
                Width  = _motionTarget.DesiredSize.Width,
                Height = _motionTarget.DesiredSize.Height
            };

            _layout.Children.Add(border);

            Canvas.SetLeft(border, offsetX);
            Canvas.SetTop(border, offsetY);

            _initialized = true;
        }
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

        offsetX += _maskOffset.X;
        offsetY += _maskOffset.Y;

        var renderers = new List<Control>();
        for (var i = 0; i < shadows.Count; ++i)
        {
            var renderer = new Border
            {
                BorderThickness = new Thickness(0),
                BoxShadow       = new BoxShadows(shadows[i]),
                CornerRadius    = MaskCornerRadius,
                Width           = _maskSize.Width,
                Height          = _maskSize.Height
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
        var  shadowThickness = Shadows.Thickness();
        Size motionTargetSize;
        if (_motionTarget.DesiredSize == default)
        {
            motionTargetSize = LayoutHelper.MeasureChild(_motionTarget, Size.Infinity, new Thickness());
        }
        else
        {
            motionTargetSize = _motionTarget.DesiredSize;
        }

        return motionTargetSize.Inflate(shadowThickness);
    }

    public override void Render(DrawingContext context)
    {
        var scaling = TopLevel.GetTopLevel(this)?.RenderScaling ?? 1.0;
        if (_ghostBitmap is not null && _contentBitmap is not null)
        {
            context.DrawImage(_ghostBitmap, new Rect(new Point(0, 0), DesiredSize * scaling),
                new Rect(new Point(0, 0), DesiredSize));
            var shadowThickness = Shadows.Thickness();
            var offsetX         = shadowThickness.Left;
            var offsetY         = shadowThickness.Top;
            context.DrawImage(_contentBitmap, new Rect(new Point(0, 0), _motionTargetSize * scaling),
                new Rect(new Point(offsetX, offsetY), _motionTargetSize));
        }
    }

    public void NotifyCaptureGhostBitmap()
    {
        if (_ghostBitmap is null)
        {
            var scaling = TopLevel.GetTopLevel(this)?.RenderScaling ?? 1.0;
            _ghostBitmap = new RenderTargetBitmap(
                new PixelSize((int)(DesiredSize.Width * scaling), (int)(DesiredSize.Height * scaling)),
                new Vector(96 * scaling, 96 * scaling));
            _contentBitmap = new RenderTargetBitmap(
                new PixelSize((int)(_motionTargetSize.Width * scaling), (int)(_motionTargetSize.Height * scaling)),
                new Vector(96 * scaling, 96 * scaling));
            _ghostBitmap.Render(this);
            _contentBitmap.Render(_motionTarget);
            _layout!.Children.Clear();
        }
    }
}