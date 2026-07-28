using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Layout;

namespace AtomUI.Controls.Commons;

internal class TimelineItemPanel : Panel
{
    #region 公共属性定义

    public static readonly StyledProperty<bool> IsOddProperty =
        AvaloniaProperty.Register<TimelineItemPanel, bool>(nameof(IsOdd));

    public static readonly StyledProperty<TimelineMode> ModeProperty =
        AvaloniaProperty.Register<TimelineItemPanel, TimelineMode>(nameof(Mode), TimelineMode.Start);

    public static readonly StyledProperty<Orientation> OrientationProperty =
        StackPanel.OrientationProperty.AddOwner<TimelineItemPanel>();

    public static readonly StyledProperty<bool> IsLabelLayoutProperty =
        AvaloniaProperty.Register<TimelineItemPanel, bool>(nameof(IsLabelLayout), false);

    public static readonly StyledProperty<double> IndicatorSpacingProperty =
        AvaloniaProperty.Register<TimelineItemPanel, double>(nameof(IndicatorSpacing));

    public bool IsOdd
    {
        get => GetValue(IsOddProperty);
        set => SetValue(IsOddProperty, value);
    }

    public TimelineMode Mode
    {
        get => GetValue(ModeProperty);
        set => SetValue(ModeProperty, value);
    }

    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public bool IsLabelLayout
    {
        get => GetValue(IsLabelLayoutProperty);
        set => SetValue(IsLabelLayoutProperty, value);
    }

    public double IndicatorSpacing
    {
        get => GetValue(IndicatorSpacingProperty);
        set => SetValue(IndicatorSpacingProperty, value);
    }

    #endregion

    static TimelineItemPanel()
    {
        OrientationProperty.OverrideDefaultValue<TimelineItemPanel>(Orientation.Vertical);
        AffectsMeasure<TimelineItemPanel>(
            IsOddProperty,
            ModeProperty,
            OrientationProperty,
            IsLabelLayoutProperty,
            IndicatorSpacingProperty);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        return Orientation == Orientation.Horizontal
            ? MeasureHorizontal(availableSize)
            : MeasureVertical(availableSize);
    }

    private Size MeasureVertical(Size availableSize)
    {
        var indicator        = GetTimelineIndicator();
        var labelTextBlock   = GetLabelTextBlock();
        var contentPresenter = GetContentPresenter();
        indicator.Measure(availableSize);
        var indicatorWidth   = indicator.DesiredSize.Width;
        var desiredWidth     = 0d;
        var desiredHeight    = indicator.DesiredSize.Height;
        if (IsLabelLayout || Mode == TimelineMode.Alternate)
        {
            var labelOrContentWidth = double.IsPositiveInfinity(availableSize.Width)
                ? double.PositiveInfinity
                : Math.Max(0, availableSize.Width - indicatorWidth) / 2;
            var labelOrContentSize  = new Size(labelOrContentWidth, availableSize.Height);
            labelTextBlock.Measure(labelOrContentSize);
            contentPresenter.Measure(labelOrContentSize);
            var sideWidth = Math.Max(labelTextBlock.DesiredSize.Width, contentPresenter.DesiredSize.Width);
            desiredWidth  = sideWidth * 2 + indicatorWidth;
            desiredHeight = Math.Max(
                desiredHeight,
                Math.Max(labelTextBlock.DesiredSize.Height, contentPresenter.DesiredSize.Height));
        }
        else
        {
            var contentWidth = double.IsPositiveInfinity(availableSize.Width)
                ? double.PositiveInfinity
                : Math.Max(0, availableSize.Width - indicatorWidth);
            var contentSize  = new Size(contentWidth, availableSize.Height);
            contentPresenter.Measure(contentSize);
            desiredWidth  = indicatorWidth + contentPresenter.DesiredSize.Width;
            desiredHeight = Math.Max(desiredHeight, contentPresenter.DesiredSize.Height);
        }

        if (!double.IsPositiveInfinity(availableSize.Width))
        {
            desiredWidth = availableSize.Width;
        }

        return new Size(desiredWidth, desiredHeight);
    }

    private Size MeasureHorizontal(Size availableSize)
    {
        var indicator        = GetTimelineIndicator();
        var labelTextBlock   = GetLabelTextBlock();
        var contentPresenter = GetContentPresenter();
        indicator.Measure(availableSize);

        var childSize = new Size(availableSize.Width, availableSize.Height);
        var desiredWidth = indicator.DesiredSize.Width;
        double desiredHeight;
        if (IsLabelLayout || Mode == TimelineMode.Alternate)
        {
            labelTextBlock.Measure(childSize);
            contentPresenter.Measure(childSize);
            var sideExtent = Math.Max(labelTextBlock.DesiredSize.Height, contentPresenter.DesiredSize.Height);
            desiredWidth = Math.Max(
                desiredWidth,
                Math.Max(labelTextBlock.DesiredSize.Width, contentPresenter.DesiredSize.Width));
            desiredHeight = sideExtent * 2 + indicator.DesiredSize.Height + IndicatorSpacing * 2;
        }
        else
        {
            contentPresenter.Measure(childSize);
            desiredWidth = Math.Max(desiredWidth, contentPresenter.DesiredSize.Width);
            desiredHeight = indicator.DesiredSize.Height + contentPresenter.DesiredSize.Height;
            if (contentPresenter.DesiredSize.Height > 0)
            {
                desiredHeight += IndicatorSpacing;
            }
        }

        if (!double.IsPositiveInfinity(availableSize.Width))
        {
            desiredWidth = availableSize.Width;
        }

        return new Size(desiredWidth, desiredHeight);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (Orientation == Orientation.Horizontal)
        {
            ArrangeHorizontal(finalSize);
        }
        else
        {
            ArrangeVertical(finalSize);
        }

        return finalSize;
    }

    private void ArrangeVertical(Size finalSize)
    {
        var indicator        = GetTimelineIndicator();
        var labelTextBlock   = GetLabelTextBlock();
        var contentPresenter = GetContentPresenter();
        var indicatorWidth   = indicator.DesiredSize.Width;
        var effectiveMode    = GetEffectiveMode();
        if (IsLabelLayout || Mode == TimelineMode.Alternate)
        {
            var labelOrContentWidth = Math.Max(0, finalSize.Width - indicatorWidth) / 2;
            var leftRect = new Rect(0, 0, labelOrContentWidth, finalSize.Height);
            var rightRect = new Rect(labelOrContentWidth + indicatorWidth, 0, labelOrContentWidth, finalSize.Height);
            indicator.Arrange(new Rect(labelOrContentWidth, 0, indicatorWidth, finalSize.Height));
            if (effectiveMode == TimelineMode.Start)
            {
                labelTextBlock.Arrange(leftRect);
                contentPresenter.Arrange(rightRect);
            }
            else
            {
                labelTextBlock.Arrange(rightRect);
                contentPresenter.Arrange(leftRect);
            }
        }
        else
        {
            var contentWidth = Math.Max(0, finalSize.Width - indicatorWidth);
            if (effectiveMode == TimelineMode.Start)
            {
                indicator.Arrange(new Rect(0, 0, indicatorWidth, finalSize.Height));
                contentPresenter.Arrange(new Rect(indicatorWidth, 0, contentWidth, finalSize.Height));
            }
            else
            {
                indicator.Arrange(new Rect(contentWidth, 0, indicatorWidth, finalSize.Height));
                contentPresenter.Arrange(new Rect(0, 0, contentWidth, finalSize.Height));
            }
        }
    }

    private void ArrangeHorizontal(Size finalSize)
    {
        var indicator        = GetTimelineIndicator();
        var labelTextBlock   = GetLabelTextBlock();
        var contentPresenter = GetContentPresenter();
        var indicatorHeight  = indicator.DesiredSize.Height;
        var effectiveMode    = GetEffectiveMode();
        if (IsLabelLayout || Mode == TimelineMode.Alternate)
        {
            var sideExtent = Math.Max(0, finalSize.Height - indicatorHeight - IndicatorSpacing * 2) / 2;
            var topRect = new Rect(0, 0, finalSize.Width, sideExtent);
            var indicatorRect = new Rect(
                0,
                sideExtent + IndicatorSpacing,
                finalSize.Width,
                indicatorHeight);
            var bottomRect = new Rect(
                0,
                sideExtent + IndicatorSpacing * 2 + indicatorHeight,
                finalSize.Width,
                sideExtent);
            indicator.Arrange(indicatorRect);
            if (effectiveMode == TimelineMode.Start)
            {
                labelTextBlock.Arrange(topRect);
                contentPresenter.Arrange(bottomRect);
            }
            else
            {
                contentPresenter.Arrange(topRect);
                labelTextBlock.Arrange(bottomRect);
            }
        }
        else
        {
            var spacing = contentPresenter.DesiredSize.Height > 0 ? IndicatorSpacing : 0;
            var contentHeight = Math.Max(0, finalSize.Height - indicatorHeight - spacing);
            if (effectiveMode == TimelineMode.Start)
            {
                indicator.Arrange(new Rect(0, 0, finalSize.Width, indicatorHeight));
                contentPresenter.Arrange(new Rect(
                    0,
                    indicatorHeight + spacing,
                    finalSize.Width,
                    contentHeight));
            }
            else
            {
                contentPresenter.Arrange(new Rect(0, 0, finalSize.Width, contentHeight));
                indicator.Arrange(new Rect(
                    0,
                    contentHeight + spacing,
                    finalSize.Width,
                    indicatorHeight));
            }
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        SetupItemsHorizontalAlignment();
    }

    private void SetupItemsHorizontalAlignment()
    {
        if (LogicalChildren.Count == 0)
        {
            return;
        }

        var labelTextBlock   = GetLabelTextBlock();
        var contentPresenter = GetContentPresenter();
        if (Orientation == Orientation.Horizontal)
        {
            labelTextBlock.HorizontalAlignment   = HorizontalAlignment.Stretch;
            contentPresenter.HorizontalAlignment = HorizontalAlignment.Stretch;
            return;
        }

        var effectiveMode = GetEffectiveMode();
        if (IsLabelLayout || Mode == TimelineMode.Alternate)
        {
            if (effectiveMode == TimelineMode.Start)
            {
                labelTextBlock.HorizontalAlignment   = HorizontalAlignment.Right;
                contentPresenter.HorizontalAlignment = HorizontalAlignment.Left;
            }
            else
            {
                labelTextBlock.HorizontalAlignment   = HorizontalAlignment.Left;
                contentPresenter.HorizontalAlignment = HorizontalAlignment.Right;
            }
        }
        else
        {
            if (effectiveMode == TimelineMode.Start)
            {
                contentPresenter.HorizontalAlignment = HorizontalAlignment.Left;
            }
            else
            {
                contentPresenter.HorizontalAlignment = HorizontalAlignment.Right;
            }
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsOddProperty ||
            change.Property == ModeProperty ||
            change.Property == OrientationProperty ||
            change.Property == IsLabelLayoutProperty)
        {
            SetupItemsHorizontalAlignment();
        }
    }

    private TimelineMode GetEffectiveMode()
    {
        if (Mode != TimelineMode.Alternate)
        {
            return Mode;
        }

        return IsOdd ? TimelineMode.End : TimelineMode.Start;
    }

    private TimelineIndicator GetTimelineIndicator()
    {
        TimelineIndicator? target = null;
        foreach (var child in LogicalChildren)
        {
            if (child is TimelineIndicator indicator)
            {
                target = indicator;
            }
        }

        Debug.Assert(target != null, "TimelineItemPanel: TimelineIndicator is null");
        return target;
    }

    private TextBlock GetLabelTextBlock()
    {
        TextBlock? target = null;
        foreach (var child in LogicalChildren)
        {
            if (child is TextBlock textBlock)
            {
                target = textBlock;
            }
        }

        Debug.Assert(target != null, "TimelineItemPanel: TextBlock is null");
        return target;
    }

    private ContentPresenter GetContentPresenter()
    {
        ContentPresenter? target = null;
        foreach (var child in LogicalChildren)
        {
            if (child is ContentPresenter contentPresenter)
            {
                target = contentPresenter;
            }
        }

        Debug.Assert(target != null, "TimelineItemPanel: ContentPresenter is null");
        return target;
    }
}
