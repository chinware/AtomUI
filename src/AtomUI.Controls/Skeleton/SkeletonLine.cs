using AtomUI.Controls.Themes;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AtomUI.Controls;

public class SkeletonLine : AbstractSkeleton
{
    #region 公共属性定义

    public static readonly StyledProperty<SkeletonWidth> LineWidthProperty =
        AvaloniaProperty.Register<SkeletonLine, SkeletonWidth>(nameof(LineWidth), new SkeletonWidth(100.0, SkeletonUnitType.Percentage));
    
    public static readonly StyledProperty<bool> IsRoundProperty =
        AvaloniaProperty.Register<SkeletonLine, bool>(nameof(IsRound));
    
    public SkeletonWidth LineWidth
    {
        get => GetValue(LineWidthProperty);
        set => SetValue(LineWidthProperty, value);
    }
    
    public bool IsRound
    {
        get => GetValue(IsRoundProperty);
        set => SetValue(IsRoundProperty, value);
    }

    #endregion

    protected override Type StyleKeyOverride { get; } = typeof(SkeletonLine);

    private Border? _frame;

    protected override Size MeasureOverride(Size availableSize)
    {
        var actualWidth = availableSize.Width;
        if (LineWidth.IsPixel)
        {
            actualWidth = LineWidth.Value;
        }
        else if (!double.IsInfinity(actualWidth))
        {
            actualWidth = Math.Min(actualWidth, actualWidth * (LineWidth.Value / 100.0));
        }

        var actualSize = new Size(actualWidth, availableSize.Height);
        var size = base.MeasureOverride(actualSize);
        return actualSize.WithHeight(size.Height);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var size = base.ArrangeOverride(finalSize);
        if (_frame != null)
        {
            _frame.Arrange(new Rect(0, 0, DesiredSize.Width, DesiredSize.Height));
        }
        return size;
    }
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _frame               = e.NameScope.Find<Border>(SkeletonLineThemeConstants.LineFramePart);
        if (!IsFollowMode)
        {
            if (IsActive)
            {
                StartActiveAnimation();
            }
        }
    }
}