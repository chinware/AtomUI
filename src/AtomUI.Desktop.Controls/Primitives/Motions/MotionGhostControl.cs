using System.Diagnostics;
using AtomUI.Controls.Utils;
using AtomUI.Media;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls.Primitives;

/// <summary>
/// 只在动画过程中使用且不能改变属性，所以在这里使用了自动属性
/// </summary>
internal class MotionGhostControl : Control
{
    #region 公共属性定义
    
    public static readonly StyledProperty<Control?> ContentProperty =
        AvaloniaProperty.Register<MotionGhostControl, Control?>(nameof(Content));
    
    public static readonly StyledProperty<BoxShadows> MaskShadowsProperty =
        Border.BoxShadowProperty.AddOwner<MotionGhostControl>();
    
    public static readonly StyledProperty<CornerRadius> MaskCornerRadiusProperty =
        AvaloniaProperty.Register<MotionGhostControl, CornerRadius>(nameof(MaskCornerRadius));
    
    public static readonly StyledProperty<Point> MaskOffsetProperty =
        AvaloniaProperty.Register<MotionGhostControl, Point>(nameof(MaskOffset));
    
    public static readonly StyledProperty<Size> MaskSizeProperty =
        AvaloniaProperty.Register<MotionGhostControl, Size>(nameof(MaskSize));
    
    public Control? Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }
    
    public BoxShadows MaskShadows
    {
        get => GetValue(MaskShadowsProperty);
        set => SetValue(MaskShadowsProperty, value);
    }
    
    public CornerRadius MaskCornerRadius
    {
        get => GetValue(MaskCornerRadiusProperty);
        set => SetValue(MaskCornerRadiusProperty, value);
    }
    
    public Point MaskOffset
    {
        get => GetValue(MaskOffsetProperty);
        set => SetValue(MaskOffsetProperty, value);
    }
    
    public Size MaskSize
    {
        get => GetValue(MaskSizeProperty);
        set => SetValue(MaskSizeProperty, value);
    }

    #endregion

    protected Canvas? _layout;
    protected Border? _maskCenterFrame;

    static MotionGhostControl()
    {
        AffectsMeasure<MotionGhostControl>(ContentProperty, MaskShadowsProperty, MaskOffsetProperty, MaskSizeProperty);
        AffectsRender<MotionGhostControl>(MaskCornerRadiusProperty);
    }

    public MotionGhostControl()
    {
        IsHitTestVisible = false;
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (_layout != null)
        {
            return;
        }
        
        _layout = new Canvas();
        LogicalChildren.Add(_layout);
        VisualChildren.Add(_layout);
        var shadowThickness = MaskShadows.Thickness();
        var offsetX         = shadowThickness.Left;
        var offsetY         = shadowThickness.Top;

        SetupShadowRenderers(MaskShadows);

        _maskCenterFrame = new Border();
        Canvas.SetLeft(_maskCenterFrame, offsetX);
        Canvas.SetTop(_maskCenterFrame, offsetY);
        _layout.Children.Add(_maskCenterFrame);
        
        SetupContent(Content);
    }

    private void SetupShadowRenderers(in BoxShadows boxShadows)
    {
        Debug.Assert(_layout != null);
        
        for (int i = _layout.Children.Count - 1; i >= 0; i--)
        {
            var child = _layout.Children[i];
            if (child != _maskCenterFrame && child != Content)
            {
                _layout.Children.RemoveAt(i);
                (child as IDisposable)?.Dispose();
            }
        }
        if (boxShadows.Count > 0)
        {
            var shadowRenderers = BuildShadowRenderers(boxShadows);
            _layout.Children.InsertRange(0, shadowRenderers);
        }
    }

    private void SetupContent(Control? content)
    {
        Debug.Assert(_layout != null);
        if (content != null)
        {
            var shadowThickness = MaskShadows.Thickness();
            var offsetX         = shadowThickness.Left;
            var offsetY         = shadowThickness.Top;
            _layout.Children.Add(content);
            Canvas.SetLeft(content, offsetX);
            Canvas.SetTop(content, offsetY);
            content.SizeChanged += HandleContentSizeChanged;
            Debug.Assert(_maskCenterFrame != null);
            _maskCenterFrame.Width = content.Width;
            _maskCenterFrame.Height = content.Height;
        }
    }

    /// <summary>
    /// 目前的 Avalonia 版本中，当控件渲染到 RenderTargetBitmap 的时候，如果 BoxShadows 的 Count > 1 的时候，如果不是主阴影，后面的阴影如果
    /// 指定 offset，再 RenderScaling > 1 的情况下是错的。
    /// </summary>
    /// <returns></returns>
    private List<Control> BuildShadowRenderers(in BoxShadows shadows)
    {
        var thickness = shadows.Thickness();
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
        var  maskThickness = MaskShadows.Thickness();
        Size size          = default;
        if (Content != null)
        {
            size = Content.DesiredSize;
        } 
        return size.Inflate(maskThickness);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
      
        if (this.IsAttachedToVisualTree())
        {
            Debug.Assert(_layout != null);
            if (change.Property == ContentProperty)
            {
                if (change.OldValue is Control oldContent)
                {
                    oldContent.SizeChanged -= HandleContentSizeChanged;
                    _layout.Children.Remove(oldContent);
                }

                if (change.NewValue is Control newContent)
                {
                    SetupContent(newContent);
                }
            }
            else if (change.Property == MaskShadowsProperty)
            {
                SetupShadowRenderers(MaskShadows);
            }
            else if (change.Property == MaskCornerRadiusProperty || change.Property == MaskSizeProperty)
            {
                HandleMaskCornerRadiusOrSizeChanged();
            }
        }
    }

    private void HandleMaskCornerRadiusOrSizeChanged()
    {
        Debug.Assert(_layout != null);
        for (int i = 0; i < MaskShadows.Count; ++i)
        {
            if (_layout.Children[i] is Border border)
            {
                border.CornerRadius = MaskCornerRadius;
                border.Width        = MaskSize.Width;
                border.Height       = MaskSize.Height;
            }
        }
    }

    private void HandleContentSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        Debug.Assert(_maskCenterFrame != null);
        _maskCenterFrame.Width  = e.NewSize.Width;
        _maskCenterFrame.Height = e.NewSize.Height;
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
        
        if (motionTarget.DesiredSize == default)
        {
            LayoutHelper.MeasureChild(motionTarget, Size.Infinity, new Thickness());
        }

        var motionTargetBitmap = motionTarget.CaptureCurrentBitmap();

        return new MotionGhostControl
        {
            Content = new MotionTargetBitmapControl(motionTargetBitmap),
            MaskShadows = maskShadows,
            MaskCornerRadius = cornerRadius,
            MaskOffset = maskOffset,
            MaskSize = maskSize,
        };
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