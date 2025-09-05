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

    private Panel? _rootLayout;

    protected override Size MeasureOverride(Size availableSize)
    {
        var actualWidth = availableSize.Width;
        if (LineWidth.IsPixel)
        {
            actualWidth = LineWidth.Value;
        }
        var actualSize = new Size(actualWidth, availableSize.Height);
        return base.MeasureOverride(actualSize);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var size        = base.ArrangeOverride(finalSize);
        var actualWidth = finalSize.Width;
        if (!LineWidth.IsPixel)
        {
            if (!double.IsInfinity(actualWidth))
            {
                actualWidth = Math.Min(actualWidth, actualWidth * (LineWidth.Value / 100.0));
            }
        }
        
        if (_rootLayout != null)
        {
            _rootLayout.Arrange(new Rect(0, 0, actualWidth, DesiredSize.Height));
        }
        return size;
    }
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _rootLayout               = e.NameScope.Find<Panel>(AbstractSkeletonThemeConstants.RootLayoutPart);
        if (!IsFollowMode)
        {
            if (IsActive)
            {
                StartActiveAnimation();
            }
        }
    }
}