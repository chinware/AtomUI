using AtomUI.Media;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;

namespace AtomUI.Controls;

public enum ArrowPosition
{
   /// <summary>
   ///     Preferred location is below the target element.
   /// </summary>
   Bottom,

   /// <summary>
   ///     Preferred location is to the right of the target element.
   /// </summary>
   Right,

   /// <summary>
   ///     Preferred location is to the left of the target element.
   /// </summary>
   Left,

   /// <summary>
   ///     Preferred location is above the target element.
   /// </summary>
   Top,

   /// <summary>
   ///     Preferred location is above the target element, with the left edge of the popup
   ///     aligned with the left edge of the target element.
   /// </summary>
   TopEdgeAlignedLeft,

   /// <summary>
   ///     Preferred location is above the target element, with the right edge of popup aligned with right edge of the target
   ///     element.
   /// </summary>
   TopEdgeAlignedRight,

   /// <summary>
   ///     Preferred location is below the target element, with the left edge of popup aligned with left edge of the target
   ///     element.
   /// </summary>
   BottomEdgeAlignedLeft,

   /// <summary>
   ///     Preferred location is below the target element, with the right edge of popup aligned with right edge of the target
   ///     element.
   /// </summary>
   BottomEdgeAlignedRight,

   /// <summary>
   ///     Preferred location is to the left of the target element, with the top edge of popup aligned with top edge of the
   ///     target element.
   /// </summary>
   LeftEdgeAlignedTop,

   /// <summary>
   ///     Preferred location is to the left of the target element, with the bottom edge of popup aligned with bottom edge of
   ///     the target element.
   /// </summary>
   LeftEdgeAlignedBottom,

   /// <summary>
   ///     Preferred location is to the right of the target element, with the top edge of popup aligned with top edge of the
   ///     target element.
   /// </summary>
   RightEdgeAlignedTop,

   /// <summary>
   ///     Preferred location is to the right of the target element, with the bottom edge of popup aligned with bottom edge of
   ///     the target element.
   /// </summary>
   RightEdgeAlignedBottom
}


public class ArrowDecoratedBox : ContentControl,
    IShadowMaskInfoProvider,
    IControlCustomStyle
{
    public static readonly StyledProperty<bool> IsShowArrowProperty =
        AvaloniaProperty.Register<ArrowDecoratedBox, bool>(nameof(IsShowArrow), true);

    public static readonly StyledProperty<ArrowPosition> ArrowPositionProperty =
        AvaloniaProperty.Register<ArrowDecoratedBox, ArrowPosition>(
            nameof(ArrowPosition), ArrowPosition.Bottom);

    internal static readonly StyledProperty<double> ArrowSizeProperty
        = AvaloniaProperty.Register<ArrowDecoratedBox, double>(nameof(ArrowSize));

    private Geometry? _arrowGeometry;
    private Rect _arrowRect;

    // 指针最顶点位置
    // 相对坐标
    private (double, double) _arrowVertexPoint;
    private Rect _contentRect;

    private readonly IControlCustomStyle _customStyle;
    private bool _needGenerateArrowVertexPoint = true;

    static ArrowDecoratedBox()
    {
        AffectsMeasure<ArrowDecoratedBox>(ArrowPositionProperty, IsShowArrowProperty);
    }

    public ArrowDecoratedBox()
    {
        _customStyle = this;
    }

    internal (double, double) ArrowVertexPoint => GetArrowVertexPoint();

    /// <summary>
    ///     是否显示指示箭头
    /// </summary>
    public bool IsShowArrow
    {
        get => GetValue(IsShowArrowProperty);
        set => SetValue(IsShowArrowProperty, value);
    }

    /// <summary>
    ///     箭头渲染的位置
    /// </summary>
    public ArrowPosition ArrowPosition
    {
        get => GetValue(ArrowPositionProperty);
        set => SetValue(ArrowPositionProperty, value);
    }

    /// <summary>
    ///     箭头的大小
    /// </summary>
    internal double ArrowSize
    {
        get => GetValue(ArrowSizeProperty);
        set => SetValue(ArrowSizeProperty, value);
    }

    void IControlCustomStyle.HandleTemplateApplied(INameScope scope)
    {
        if (IsShowArrow) BuildGeometry(true);
    }

    public CornerRadius GetMaskCornerRadius()
    {
        return CornerRadius;
    }

    public Rect GetMaskBounds()
    {
        return GetContentRect(DesiredSize).Deflate(0.5);
    }

    public static Direction GetDirection(ArrowPosition arrowPosition)
    {
        return arrowPosition switch
        {
            ArrowPosition.Left                  => Direction.Left,
            ArrowPosition.LeftEdgeAlignedBottom => Direction.Left,
            ArrowPosition.LeftEdgeAlignedTop    => Direction.Left,

            ArrowPosition.Top                 => Direction.Top,
            ArrowPosition.TopEdgeAlignedLeft  => Direction.Top,
            ArrowPosition.TopEdgeAlignedRight => Direction.Top,

            ArrowPosition.Right                  => Direction.Right,
            ArrowPosition.RightEdgeAlignedBottom => Direction.Right,
            ArrowPosition.RightEdgeAlignedTop    => Direction.Right,

            ArrowPosition.Bottom                 => Direction.Bottom,
            ArrowPosition.BottomEdgeAlignedLeft  => Direction.Bottom,
            ArrowPosition.BottomEdgeAlignedRight => Direction.Bottom,
            _ => throw new ArgumentOutOfRangeException(nameof(arrowPosition), arrowPosition,
                "Invalid value for ArrowPosition")
        };
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        _customStyle.HandlePropertyChangedForStyle(e);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _customStyle.HandleTemplateApplied(e.NameScope);
    }



    #region IControlCustomStyle 实现

    private (double, double) GetArrowVertexPoint()
    {
        if (_needGenerateArrowVertexPoint)
        {
            BuildGeometry(true);
            _arrowRect                    = GetArrowRect(DesiredSize);
            _needGenerateArrowVertexPoint = false;
        }

        return _arrowVertexPoint;
    }

    void IControlCustomStyle.HandlePropertyChangedForStyle(AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == IsShowArrowProperty   ||
            e.Property == ArrowPositionProperty ||
            e.Property == ArrowSizeProperty     ||
            e.Property == VisualParentProperty)
        {
            if (e.Property == IsShowArrowProperty && VisualRoot is null)

                // 当开启的时候，但是还没有加入的渲染树，这个时候我们取不到 Token 需要在取值的时候重新生成一下
                _needGenerateArrowVertexPoint = true;

            if (VisualRoot is not null)
            {
                BuildGeometry(true);
                _arrowRect = GetArrowRect(DesiredSize);
            }
        }
    }

    private void BuildGeometry(bool force = false)
    {
        if (_arrowGeometry is null || force) _arrowGeometry = CommonShapeBuilder.BuildArrow(ArrowSize, 1.5);
    }

    public sealed override void Render(DrawingContext context)
    {
        if (IsShowArrow)
        {
            var direction = GetDirection(ArrowPosition);
            var matrix    = Matrix.CreateTranslation(-ArrowSize / 2, -ArrowSize / 2);

            if (direction == Direction.Right)
            {
                matrix *= Matrix.CreateRotation(MathUtils.Deg2Rad(90));
                matrix *= Matrix.CreateTranslation(ArrowSize / 2, ArrowSize / 2);
            }
            else if (direction == Direction.Top)
            {
                matrix *= Matrix.CreateTranslation(ArrowSize / 2, 0);
            }
            else if (direction == Direction.Left)
            {
                matrix *= Matrix.CreateRotation(MathUtils.Deg2Rad(-90));
                matrix *= Matrix.CreateTranslation(0, ArrowSize / 2);
            }
            else
            {
                matrix *= Matrix.CreateRotation(MathUtils.Deg2Rad(180));
                matrix *= Matrix.CreateTranslation(ArrowSize / 2, ArrowSize / 2);
            }

            matrix                    *= Matrix.CreateTranslation(_arrowRect.X, _arrowRect.Y);
            _arrowGeometry!.Transform =  new MatrixTransform(matrix);
            context.DrawGeometry(Background, null, _arrowGeometry);
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var size         = base.MeasureOverride(availableSize);
        var targetWidth  = size.Width;
        var targetHeight = size.Height;
        targetHeight = Math.Max(MinHeight, targetHeight);

        if (IsShowArrow)
        {
            BuildGeometry();
            var realArrowSize = Math.Min(_arrowGeometry!.Bounds.Size.Height, _arrowGeometry!.Bounds.Size.Width);
            var direction     = GetDirection(ArrowPosition);
            if (direction == Direction.Left || direction == Direction.Right)
                targetWidth += realArrowSize;
            else
                targetHeight += realArrowSize;
        }

        var targetSize = new Size(targetWidth, targetHeight);
        _arrowRect = GetArrowRect(targetSize);
        return targetSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var visualChildren = VisualChildren;
        var visualCount    = visualChildren.Count;
        _contentRect = GetContentRect(finalSize);
        for (var i = 0; i < visualCount; ++i)
        {
            var child = visualChildren[i];
            if (child is Layoutable layoutable) layoutable.Arrange(_contentRect);
        }

        return finalSize;
    }

    internal Rect GetContentRect(Size finalSize)
    {
        var offsetX      = 0d;
        var offsetY      = 0d;
        var targetWidth  = finalSize.Width;
        var targetHeight = finalSize.Height;
        if (IsShowArrow)
        {
            var arrowSize = Math.Min(_arrowGeometry!.Bounds.Size.Height, _arrowGeometry!.Bounds.Size.Width) + 0.5;
            var direction = GetDirection(ArrowPosition);
            if (direction == Direction.Left || direction == Direction.Right)
                targetWidth -= arrowSize;
            else
                targetHeight -= arrowSize;

            if (direction == Direction.Right)
                offsetX = 0.5;
            else if (direction == Direction.Bottom)
                offsetY = 0.5;
            else if (direction == Direction.Top)
                offsetY = arrowSize - 0.5;
            else
                offsetX = arrowSize - 0.5;
        }

        return new Rect(offsetX, offsetY, targetWidth, targetHeight);
    }

    private Rect GetArrowRect(Size finalSize)
    {
        var offsetX      = 0d;
        var offsetY      = 0d;
        var targetWidth  = 0d;
        var targetHeight = 0d;
        var position     = ArrowPosition;
        if (IsShowArrow)
        {
            var size = _arrowGeometry!.Bounds.Size;

            var minValue = Math.Min(size.Width, size.Height);
            var maxValue = Math.Max(size.Width, size.Height);
            if (position == ArrowPosition.Left               ||
                position == ArrowPosition.LeftEdgeAlignedTop ||
                position == ArrowPosition.LeftEdgeAlignedBottom)
            {
                targetWidth  = minValue;
                targetHeight = maxValue;
                if (position == ArrowPosition.Left)
                {
                    offsetY = (finalSize.Height - maxValue) / 2;
                }
                else if (position == ArrowPosition.LeftEdgeAlignedTop)
                {
                    if (maxValue * 2 > finalSize.Height / 2)
                        offsetY = minValue;
                    else
                        offsetY = maxValue;
                }
                else
                {
                    if (maxValue * 2 > finalSize.Height / 2)
                        offsetY = finalSize.Height - minValue - maxValue;
                    else
                        offsetY = finalSize.Height - maxValue * 2;
                }
            }
            else if (position == ArrowPosition.Top                ||
                     position == ArrowPosition.TopEdgeAlignedLeft ||
                     position == ArrowPosition.TopEdgeAlignedRight)
            {
                if (position == ArrowPosition.TopEdgeAlignedLeft)
                    offsetX = maxValue;
                else if (position == ArrowPosition.Top)
                    offsetX = (finalSize.Width - maxValue) / 2;
                else
                    offsetX = finalSize.Width - maxValue * 2;

                targetWidth  = maxValue;
                targetHeight = minValue;
            }
            else if (position == ArrowPosition.Right               ||
                     position == ArrowPosition.RightEdgeAlignedTop ||
                     position == ArrowPosition.RightEdgeAlignedBottom)
            {
                offsetX = finalSize.Width - minValue;
                if (position == ArrowPosition.Right)
                {
                    offsetY = (finalSize.Height - maxValue) / 2;
                }
                else if (position == ArrowPosition.RightEdgeAlignedTop)
                {
                    if (maxValue * 2 > finalSize.Height / 2)
                        offsetY = minValue;
                    else
                        offsetY = maxValue;
                }
                else
                {
                    if (maxValue * 2 > finalSize.Height / 2)
                        offsetY = finalSize.Height - minValue - maxValue;
                    else
                        offsetY = finalSize.Height - maxValue * 2;
                }

                targetWidth  = minValue;
                targetHeight = maxValue;
            }
            else
            {
                offsetY      = finalSize.Height - minValue;
                targetWidth  = maxValue;
                targetHeight = minValue;
                if (position == ArrowPosition.BottomEdgeAlignedLeft)
                    offsetX = maxValue;
                else if (position == ArrowPosition.Bottom)
                    offsetX = (finalSize.Width - maxValue) / 2;
                else
                    offsetX = finalSize.Width - maxValue * 2;
            }
        }

        var targetRect = new Rect(offsetX, offsetY, targetWidth, targetHeight);
        var center     = targetRect.Center;

        // 计算中点
        var direction = GetDirection(position);
        if (direction == Direction.Left || direction == Direction.Right)
            _arrowVertexPoint = (center.Y, finalSize.Height - center.Y);
        else if (direction == Direction.Top || direction == Direction.Bottom)
            _arrowVertexPoint = (center.X, finalSize.Width - center.X);

        return targetRect;
    }

    #endregion
}