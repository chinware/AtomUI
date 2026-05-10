using System.Collections.Specialized;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;

namespace AtomUI.Controls.Commons;

public abstract class AbstractTimeline : ItemsControl
{
    #region 公共属性定义

    public static readonly StyledProperty<TimelineMode> ModeProperty =
        AvaloniaProperty.Register<AbstractTimeline, TimelineMode>(nameof(Mode), TimelineMode.Left);

    public static readonly StyledProperty<object?> PendingProperty =
        AvaloniaProperty.Register<AbstractTimeline, object?>(nameof(Pending));

    public static readonly StyledProperty<bool> IsReverseProperty =
        AvaloniaProperty.Register<AbstractTimeline, bool>(nameof(IsReverse), false);

    public static readonly StyledProperty<PathIcon?> PendingIconProperty =
        AvaloniaProperty.Register<AbstractTimeline, PathIcon?>(nameof(PendingIcon));

    public TimelineMode Mode
    {
        get => GetValue(ModeProperty);
        set => SetValue(ModeProperty, value);
    }

    public object? Pending
    {
        get => GetValue(PendingProperty);
        set => SetValue(PendingProperty, value);
    }

    public bool IsReverse
    {
        get => GetValue(IsReverseProperty);
        set => SetValue(IsReverseProperty, value);
    }

    public PathIcon? PendingIcon
    {
        get => GetValue(PendingIconProperty);
        set => SetValue(PendingIconProperty, value);
    }

    #endregion

    #region 内部属性定义

    internal WeakReference<AbstractTimelineItem>? PendingItemReference => _pendingItemReference;
    private WeakReference<AbstractTimelineItem>? _pendingItemReference;

    #endregion

    static AbstractTimeline()
    {
        AffectsMeasure<AbstractTimeline>(ModeProperty);
        AffectsArrange<AbstractTimeline>(IsReverseProperty);
    }
    
    public AbstractTimeline()
    {
        LogicalChildren.CollectionChanged += HandleItemsChanged;
    }

    private void HandleItemsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        var isLabelLayout = false;
        foreach (var item in LogicalChildren)
        {
            if (item is AbstractTimelineItem timelineItem)
            {
                if (timelineItem.Label is not null)
                {
                    isLabelLayout = true;
                }
            }
        }

        foreach (var item in LogicalChildren)
        {
            if (item is AbstractTimelineItem timelineItem)
            {
                timelineItem.IsLabelLayout = isLabelLayout;
            }
        }
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        SetupPendingItem();
    }

    private void SetupPendingItem()
    {
        if (Pending != null)
        {
            if (_pendingItemReference != null)
            {
                if (_pendingItemReference.TryGetTarget(out var item))
                {
                    Items.Remove(item);
                }
            }
            
            var pendingTimelineItem = CreatePendingItem();
            _pendingItemReference = new WeakReference<AbstractTimelineItem>(pendingTimelineItem);
            Items.Add(pendingTimelineItem);
        }
    }

    protected abstract AbstractTimelineItem CreatePendingItem();

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        return NeedsContainer<AbstractTimelineItem>(item, out recycleKey);
    }

    protected override void PrepareContainerForItemOverride(Control element, object? item, int index)
    {
        base.PrepareContainerForItemOverride(element, item, index);
        if (element is AbstractTimelineItem timelineItem)
        {
            timelineItem[!AbstractTimelineItem.ModeProperty]      = this[!ModeProperty];
            timelineItem[!AbstractTimelineItem.IsReverseProperty] = this[!IsReverseProperty];
        }
    }

    protected override void ContainerForItemPreparedOverride(Control container, object? item, int index)
    {
        base.ContainerForItemPreparedOverride(container, item, index);
        CalculateItemsPositionInfo();
    }

    protected override void ContainerIndexChangedOverride(Control container, int oldIndex, int newIndex)
    {
        base.ContainerIndexChangedOverride(container, oldIndex, newIndex);
        CalculateItemsPositionInfo();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == PendingProperty || change.Property == PendingIconProperty)
        {
            SetupPendingItem();
        }

        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == IsReverseProperty)
            {
                CalculateItemsPositionInfo();
            }
        }
    }

    private void CalculateItemsPositionInfo()
    {
        for (int i = 0, logicalIndex = 0; i < ItemCount; i++)
        {
            if (ContainerFromIndex(i) is AbstractTimelineItem timelineItem && timelineItem.IsVisible)
            {
                var idx = IsReverse ? ItemCount - 1 - logicalIndex : logicalIndex;
                CalculateItemPositionInfo(timelineItem, idx);
                logicalIndex++;
            }
        }
    }
    
    internal void CalculateItemPositionInfo(AbstractTimelineItem timelineItem, int index)
    {
        timelineItem.IsOdd         = index % 2 != 0;
        timelineItem.IsFirst       = index == 0;
        timelineItem.IsLast        = index == GetVisibleItemsCount() - 1;
        timelineItem.NextIsPending = false;
        if (PendingItemReference != null && PendingItemReference.TryGetTarget(out var pendingItem))
        {
            if (timelineItem == pendingItem)
            {
                if (!IsReverse)
                {
                    var previousItemIndex = index - 1;
                    while (previousItemIndex >= 0)
                    {
                        if (ContainerFromIndex(previousItemIndex) is AbstractTimelineItem previousItem && previousItem.IsVisible)
                        {
                            previousItem.NextIsPending = true;
                            break;
                        }

                        previousItemIndex--;
                    }
                }
                else
                {
                    var previousItemIndex = index + 1;
                    while (previousItemIndex < ItemCount)
                    {
                        if (ContainerFromIndex(previousItemIndex) is AbstractTimelineItem previousItem && previousItem.IsVisible)
                        {
                            previousItem.NextIsPending = true;
                            break;
                        }

                        previousItemIndex++;
                    }
                }
            }
        }
    }

    private int GetVisibleItemsCount()
    {
        var count = 0;
        for (var i = 0; i < ItemCount; i++)
        {
            if (ContainerFromIndex(i) is AbstractTimelineItem previousItem && previousItem.IsVisible)
            {
                ++count;
            }
        }
        return count;
    }
}