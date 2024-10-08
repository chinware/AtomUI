using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;

namespace AtomUI.Controls;

public enum ArrowPosition
{
    /// <summary>
    /// Preferred location is below the target element.
    /// </summary>
    Bottom,

    /// <summary>
    /// Preferred location is to the right of the target element.
    /// </summary>
    Right,

    /// <summary>
    /// Preferred location is to the left of the target element.
    /// </summary>
    Left,

    /// <summary>
    /// Preferred location is above the target element.
    /// </summary>
    Top,

    /// <summary>
    /// Preferred location is above the target element, with the left edge of the popup
    /// aligned with the left edge of the target element.
    /// </summary>
    TopEdgeAlignedLeft,

    /// <summary>
    /// Preferred location is above the target element, with the right edge of popup aligned with right edge of the target
    /// element.
    /// </summary>
    TopEdgeAlignedRight,

    /// <summary>
    /// Preferred location is below the target element, with the left edge of popup aligned with left edge of the target
    /// element.
    /// </summary>
    BottomEdgeAlignedLeft,

    /// <summary>
    /// Preferred location is below the target element, with the right edge of popup aligned with right edge of the target
    /// element.
    /// </summary>
    BottomEdgeAlignedRight,

    /// <summary>
    /// Preferred location is to the left of the target element, with the top edge of popup aligned with top edge of the
    /// target element.
    /// </summary>
    LeftEdgeAlignedTop,

    /// <summary>
    /// Preferred location is to the left of the target element, with the bottom edge of popup aligned with bottom edge of
    /// the target element.
    /// </summary>
    LeftEdgeAlignedBottom,

    /// <summary>
    /// Preferred location is to the right of the target element, with the top edge of popup aligned with top edge of the
    /// target element.
    /// </summary>
    RightEdgeAlignedTop,

    /// <summary>
    /// Preferred location is to the right of the target element, with the bottom edge of popup aligned with bottom edge of
    /// the target element.
    /// </summary>
    RightEdgeAlignedBottom
}

public class ArrowDecoratedBox : ContentControl, IShadowMaskInfoProvider
{
    #region 公共属性定义

    public static readonly StyledProperty<bool> IsShowArrowProperty =
        AvaloniaProperty.Register<ArrowDecoratedBox, bool>(nameof(IsShowArrow), true);

    public static readonly StyledProperty<ArrowPosition> ArrowPositionProperty =
        AvaloniaProperty.Register<ArrowDecoratedBox, ArrowPosition>(
            nameof(ArrowPosition), ArrowPosition.Bottom);
    
    /// <summary>
    /// 是否显示指示箭头
    /// </summary>
    public bool IsShowArrow
    {
        get => GetValue(IsShowArrowProperty);
        set => SetValue(IsShowArrowProperty, value);
    }

    /// <summary>
    /// 箭头渲染的位置
    /// </summary>
    public ArrowPosition ArrowPosition
    {
        get => GetValue(ArrowPositionProperty);
        set => SetValue(ArrowPositionProperty, value);
    }

    #endregion

    #region 内部属性定义

    internal static readonly StyledProperty<double> ArrowSizeProperty
        = AvaloniaProperty.Register<ArrowDecoratedBox, double>(nameof(ArrowSize));
    
    internal static readonly StyledProperty<Direction> ArrowDirectionProperty
        = AvaloniaProperty.Register<ArrowDecoratedBox, Direction>(nameof(ArrowDirection));
    
    /// <summary>
    /// 箭头的大小
    /// </summary>
    internal double ArrowSize
    {
        get => GetValue(ArrowSizeProperty);
        set => SetValue(ArrowSizeProperty, value);
    }
    
    internal Direction ArrowDirection
    {
        get => GetValue(ArrowDirectionProperty);
        set => SetValue(ArrowDirectionProperty, value);
    }
    
    #endregion

    // 指针最顶点位置
    // 相对坐标
    internal (double, double) ArrowVertexPoint => GetArrowVertexPoint();
    private Border? _contentDecorator;
    private Control? _arrowIndicatorLayout;

    static ArrowDecoratedBox()
    {
        AffectsMeasure<ArrowDecoratedBox>(IsShowArrowProperty);
        AffectsArrange<ArrowDecoratedBox>(ArrowDirectionProperty, ArrowPositionProperty);
    }

    public static Direction GetDirection(ArrowPosition arrowPosition)
    {
        return arrowPosition switch
        {
            ArrowPosition.Left => Direction.Left,
            ArrowPosition.LeftEdgeAlignedBottom => Direction.Left,
            ArrowPosition.LeftEdgeAlignedTop => Direction.Left,

            ArrowPosition.Top => Direction.Top,
            ArrowPosition.TopEdgeAlignedLeft => Direction.Top,
            ArrowPosition.TopEdgeAlignedRight => Direction.Top,

            ArrowPosition.Right => Direction.Right,
            ArrowPosition.RightEdgeAlignedBottom => Direction.Right,
            ArrowPosition.RightEdgeAlignedTop => Direction.Right,

            ArrowPosition.Bottom => Direction.Bottom,
            ArrowPosition.BottomEdgeAlignedLeft => Direction.Bottom,
            ArrowPosition.BottomEdgeAlignedRight => Direction.Bottom,
            _ => throw new ArgumentOutOfRangeException(nameof(arrowPosition), arrowPosition,
                "Invalid value for ArrowPosition")
        };
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.Property == ArrowPositionProperty)
        {
            ArrowDirection = GetDirection(ArrowPosition);
        }
    }

    public CornerRadius GetMaskCornerRadius()
    {
        return CornerRadius;
    }

    public Rect GetMaskBounds()
    {
        Rect targetRect = default;
        if (_contentDecorator is not null)
        {
            targetRect = _contentDecorator.Bounds;
        }
        return targetRect;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _contentDecorator     = e.NameScope.Get<Border>(ArrowDecoratedBoxTheme.ContentDecoratorPart);
        _arrowIndicatorLayout = e.NameScope.Get<Control>(ArrowDecoratedBoxTheme.ArrowIndicatorLayoutPart);
        ArrowDirection        = GetDirection(ArrowPosition);
    }

    private (double, double) GetArrowVertexPoint()
    {
        if (_arrowIndicatorLayout is null)
        {
            return default;
        }

        if (_arrowIndicatorLayout.Bounds == default)
        {
            LayoutHelper.MeasureChild(this, Size.Infinity, Padding);
            Arrange(new Rect(DesiredSize));
        }

        var targetRect  = _arrowIndicatorLayout.Bounds;
        var center      = targetRect.Center;
        var controlSize = Bounds.Size;
        
        // 计算中点
        var direction = GetDirection(ArrowPosition);
        if (direction == Direction.Left || direction == Direction.Right)
        {
            return (center.Y, controlSize.Height - center.Y);
        }
        return (center.X, controlSize.Width - center.X);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var size = base.ArrangeOverride(finalSize);
        if (IsShowArrow)
        {
            ArrangeArrow(finalSize);
        }
        return size;
    }

    private void ArrangeArrow(Size finalSize)
    {
        if (_arrowIndicatorLayout is null)
        {
            return;
        }
        var offsetX  = 0d;
        var offsetY  = 0d;
        var position = ArrowPosition;
        var size     = _arrowIndicatorLayout.DesiredSize;
    
        var minValue = Math.Min(size.Width, size.Height);
        var maxValue = Math.Max(size.Width, size.Height);
        if (position == ArrowPosition.Left ||
            position == ArrowPosition.LeftEdgeAlignedTop ||
            position == ArrowPosition.LeftEdgeAlignedBottom)
        {
            offsetX = 0.5d;
            if (position == ArrowPosition.Left)
            {
                offsetY = (finalSize.Height - maxValue) / 2;
            }
            else if (position == ArrowPosition.LeftEdgeAlignedTop)
            {
                if (maxValue * 2 > finalSize.Height / 2)
                {
                    offsetY = minValue;
                }
                else
                {
                    offsetY = maxValue;
                }
            }
            else
            {
                if (maxValue * 2 > finalSize.Height / 2)
                {
                    offsetY = finalSize.Height - minValue - maxValue;
                }
                else
                {
                    offsetY = finalSize.Height - maxValue * 2;
                }
            }
        }
        else if (position == ArrowPosition.Top ||
                 position == ArrowPosition.TopEdgeAlignedLeft ||
                 position == ArrowPosition.TopEdgeAlignedRight)
        {
            offsetY = 0.5d;
            if (position == ArrowPosition.TopEdgeAlignedLeft)
            {
                offsetX = maxValue;
            }
            else if (position == ArrowPosition.Top)
            {
                offsetX = (finalSize.Width - maxValue) / 2;
            }
            else
            {
                offsetX = finalSize.Width - maxValue * 2;
            }
        }
        else if (position == ArrowPosition.Right ||
                 position == ArrowPosition.RightEdgeAlignedTop ||
                 position == ArrowPosition.RightEdgeAlignedBottom)
        {
            offsetX = -0.5d;
            if (position == ArrowPosition.Right)
            {
                offsetY = (finalSize.Height - maxValue) / 2;
            }
            else if (position == ArrowPosition.RightEdgeAlignedTop)
            {
                if (maxValue * 2 > finalSize.Height / 2)
                {
                    offsetY = minValue;
                }
                else
                {
                    offsetY = maxValue;
                }
            }
            else
            {
                if (maxValue * 2 > finalSize.Height / 2)
                {
                    offsetY = finalSize.Height - minValue - maxValue;
                }
                else
                {
                    offsetY = finalSize.Height - maxValue * 2;
                }
            }
        }
        else
        {
            offsetY = -0.5d;
            if (position == ArrowPosition.BottomEdgeAlignedLeft)
            {
                offsetX = maxValue;
            }
            else if (position == ArrowPosition.Bottom)
            {
                offsetX = (finalSize.Width - maxValue) / 2;
            }
            else
            {
                offsetX = finalSize.Width - maxValue * 2;
            }
        }
        _arrowIndicatorLayout.Arrange(new Rect(new Point(offsetX, offsetY), size));
    }
}