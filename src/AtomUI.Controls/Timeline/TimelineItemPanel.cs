using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml.Templates;

namespace AtomUI.Controls;

internal class TimelineItemPanel : Panel
{
    #region 公共属性定义

    public static readonly StyledProperty<bool> IsOddProperty =
        AvaloniaProperty.Register<TimelineItemPanel, bool>(nameof(IsOdd));

    public static readonly StyledProperty<TimeLineMode> ModeProperty =
        AvaloniaProperty.Register<TimelineItemPanel, TimeLineMode>(nameof(Mode), TimeLineMode.Left);

    public static readonly StyledProperty<bool> IsLabelLayoutProperty =
        AvaloniaProperty.Register<TimelineItemPanel, bool>(nameof(IsLabelLayout), false);

    public bool IsOdd
    {
        get => GetValue(IsOddProperty);
        set => SetValue(IsOddProperty, value);
    }

    public TimeLineMode Mode
    {
        get => GetValue(ModeProperty);
        set => SetValue(ModeProperty, value);
    }

    public bool IsLabelLayout
    {
        get => GetValue(IsLabelLayoutProperty);
        set => SetValue(IsLabelLayoutProperty, value);
    }

    #endregion

    static TimelineItemPanel()
    {
        AffectsMeasure<TimelineItemPanel>(IsOddProperty, ModeProperty, IsLabelLayoutProperty);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var indicator = GetTimelineIndicator();
        indicator.Measure(availableSize);
        var labelTextBlock   = GetLabelTextBlock();
        var contentPresenter = GetContentPresenter();
        var indicatorWidth   = indicator.DesiredSize.Width;
        var height           = 0d;
        if (IsLabelLayout || Mode == TimeLineMode.Alternate)
        {
            // 有标签就是平分
            var labelOrContentWidth = (availableSize.Width - indicatorWidth) / 2;
            var labelOrContentSize  = new Size(labelOrContentWidth, availableSize.Height);
            labelTextBlock.Measure(labelOrContentSize);
            height = Math.Max(height, labelTextBlock.DesiredSize.Height);
            contentPresenter.Measure(labelOrContentSize);
            height = Math.Max(height, contentPresenter.DesiredSize.Height);
        }
        else if (Mode == TimeLineMode.Left || Mode == TimeLineMode.Right)
        {
            var contentWidth = availableSize.Width - indicatorWidth;
            var contentSize  = new Size(contentWidth, availableSize.Height);
            contentPresenter.Measure(contentSize);
            height = Math.Max(height, contentPresenter.DesiredSize.Height);
        }

        return new Size(availableSize.Width, height);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var indicator        = GetTimelineIndicator();
        var labelTextBlock   = GetLabelTextBlock();
        var contentPresenter = GetContentPresenter();
        var indicatorWidth   = indicator.DesiredSize.Width;
        if (IsLabelLayout || Mode == TimeLineMode.Alternate)
        {
            var labelOrContentWidth = (finalSize.Width - indicatorWidth) / 2;
            var leftRect = new Rect(0, 0, labelOrContentWidth, finalSize.Height);
            var rightRect = new Rect(labelOrContentWidth + indicatorWidth, 0, labelOrContentWidth, finalSize.Height);
            if (Mode == TimeLineMode.Left)
            {
                labelTextBlock.Arrange(leftRect);
                indicator.Arrange(new Rect(labelOrContentWidth, 0, indicatorWidth, finalSize.Height));
                contentPresenter.Arrange(rightRect);
            }
            else if (Mode == TimeLineMode.Right)
            {
                labelTextBlock.Arrange(rightRect);
                indicator.Arrange(new Rect(labelOrContentWidth, 0, indicatorWidth, finalSize.Height));
                contentPresenter.Arrange(leftRect);
            }
            else
            {
                if (IsOdd)
                {
                    labelTextBlock.Arrange(leftRect);
                    indicator.Arrange(new Rect(labelOrContentWidth, 0, indicatorWidth, finalSize.Height));
                    contentPresenter.Arrange(rightRect);
                }
                else
                {
                    labelTextBlock.Arrange(rightRect);
                    indicator.Arrange(new Rect(labelOrContentWidth, 0, indicatorWidth, finalSize.Height));
                    contentPresenter.Arrange(leftRect);
                }
            }
        }
        else
        {
            var contentWidth = finalSize.Width - indicatorWidth;
            if (Mode == TimeLineMode.Left)
            {
                indicator.Arrange(new Rect(0, 0, indicatorWidth, finalSize.Height));
                contentPresenter.Arrange(new Rect(indicatorWidth, 0, contentWidth, finalSize.Height));
            }
            else if (Mode == TimeLineMode.Right)
            {
                indicator.Arrange(new Rect(contentWidth, 0, indicatorWidth, finalSize.Height));
                contentPresenter.Arrange(new Rect(0, 0, contentWidth, finalSize.Height));
            }
        }

        return finalSize;
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        SetupItemsHorizontalAlignment();
    }

    private void SetupItemsHorizontalAlignment()
    {
        var labelTextBlock   = GetLabelTextBlock();
        var contentPresenter = GetContentPresenter();
        if (IsLabelLayout || Mode == TimeLineMode.Alternate)
        {
            if (Mode == TimeLineMode.Left)
            {
                labelTextBlock.HorizontalAlignment   = HorizontalAlignment.Right;
                contentPresenter.HorizontalAlignment = HorizontalAlignment.Left;
            }
            else if (Mode == TimeLineMode.Right)
            {
                labelTextBlock.HorizontalAlignment   = HorizontalAlignment.Left;
                contentPresenter.HorizontalAlignment = HorizontalAlignment.Right;
            }
            else
            {
                if (IsOdd)
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
        }
        else
        {
            if (Mode == TimeLineMode.Left)
            {
                contentPresenter.HorizontalAlignment = HorizontalAlignment.Left;
            }
            else if (Mode == TimeLineMode.Right)
            {
                contentPresenter.HorizontalAlignment = HorizontalAlignment.Right;
            }
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsOddProperty || change.Property == ModeProperty)
        {
            SetupItemsHorizontalAlignment();
        }
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