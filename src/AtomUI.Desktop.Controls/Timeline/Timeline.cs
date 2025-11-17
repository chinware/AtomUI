using System.Collections.Specialized;
using System.Reactive.Disposables;
using AtomUI.Data;
using AtomUI.IconPkg;
using AtomUI.IconPkg.AntDesign;
using AtomUI.Theme;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

public enum TimeLineMode
{
    Left,
    Right,
    Alternate
}

public class Timeline : ItemsControl,
                        IControlSharedTokenResourcesHost
{
    #region 公共属性定义

    public static readonly StyledProperty<TimeLineMode> ModeProperty =
        AvaloniaProperty.Register<Timeline, TimeLineMode>(nameof(Mode), TimeLineMode.Left);

    public static readonly StyledProperty<object?> PendingProperty =
        AvaloniaProperty.Register<Timeline, object?>(nameof(Pending));

    public static readonly StyledProperty<bool> IsReverseProperty =
        AvaloniaProperty.Register<Timeline, bool>(nameof(IsReverse), false);

    public static readonly StyledProperty<Icon?> PendingIconProperty =
        AvaloniaProperty.Register<Timeline, Icon?>(nameof(PendingIcon));

    public TimeLineMode Mode
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

    public Icon? PendingIcon
    {
        get => GetValue(PendingIconProperty);
        set => SetValue(PendingIconProperty, value);
    }

    #endregion

    #region 内部属性定义

    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => TimelineToken.ID;

    internal WeakReference<TimelineItem>? PendingItemReference => _pendingItemReference;
    private WeakReference<TimelineItem>? _pendingItemReference;

    #endregion
    
    private readonly Dictionary<TimelineItem, CompositeDisposable> _itemsBindingDisposables = new();

    static Timeline()
    {
        AffectsMeasure<Timeline>(ModeProperty);
        AffectsArrange<Timeline>(IsReverseProperty);
    }
    
    public Timeline()
    {
        this.RegisterResources();
        LogicalChildren.CollectionChanged += HandleItemsChanged;
    }

    private void HandleItemsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        var isLabelLayout = false;
        foreach (var item in LogicalChildren)
        {
            if (item is TimelineItem timelineItem)
            {
                if (timelineItem.Label is not null)
                {
                    isLabelLayout = true;
                }
            }
        }

        foreach (var item in LogicalChildren)
        {
            if (item is TimelineItem timelineItem)
            {
                timelineItem.IsLabelLayout = isLabelLayout;
            }
        }
        
        if (e.OldItems != null)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems.Count > 0)
            {
                foreach (var item in e.OldItems)
                {
                    if (item is TimelineItem timelineItem)
                    {
                        if (_itemsBindingDisposables.TryGetValue(timelineItem, out var disposable))
                        {
                            disposable.Dispose();
                            _itemsBindingDisposables.Remove(timelineItem);
                        }
                    }
                }
            }
        }
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new TimelineItem()
        {
            IsPending = false
        };
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

            var icon = PendingIcon ?? AntDesignIconPackage.LoadingOutlined();
            icon.LoadingAnimation = IconAnimation.Spin;
            var pendingTimelineItem = new TimelineItem()
            {
                Content       = Pending,
                IndicatorIcon = icon,
                IsPending     = true
            };
            _pendingItemReference = new WeakReference<TimelineItem>(pendingTimelineItem);
            Items.Add(pendingTimelineItem);
        }
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        return NeedsContainer<TimelineItem>(item, out recycleKey);
    }

    protected override void PrepareContainerForItemOverride(Control element, object? item, int index)
    {
        base.PrepareContainerForItemOverride(element, item, index);
        if (element is TimelineItem timelineItem)
        {
            var disposables = new CompositeDisposable(2);
            disposables.Add(BindUtils.RelayBind(this, ModeProperty, timelineItem, TimelineItem.ModeProperty));
            disposables.Add(BindUtils.RelayBind(this, IsReverseProperty, timelineItem, TimelineItem.IsReverseProperty));
            if (_itemsBindingDisposables.TryGetValue(timelineItem, out var oldDisposables))
            {
                oldDisposables.Dispose();
                _itemsBindingDisposables.Remove(timelineItem);
            }
            _itemsBindingDisposables.Add(timelineItem, disposables);
        }
    }

    protected override void ContainerForItemPreparedOverride(Control container, object? item, int index)
    {
        base.ContainerForItemPreparedOverride(container, item, index);
        if (container is TimelineItem timelineItem)
        {
            var idx = IsReverse ? ItemCount - 1 - index : index;
            timelineItem.NotifyVisualIndexChanged(this, idx);
        }
    }

    protected override void ContainerIndexChangedOverride(Control container, int oldIndex, int newIndex)
    {
        base.ContainerIndexChangedOverride(container, oldIndex, newIndex);
        if (container is TimelineItem timelineItem)
        {
            var idx = IsReverse ? ItemCount - 1 - newIndex : newIndex;
            timelineItem.NotifyVisualIndexChanged(this, idx);
        }
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
                NotifyItemsVisualIndex();
            }
        }
    }

    private void NotifyItemsVisualIndex()
    {
        for (int i = 0; i < ItemCount; i++)
        {
            if (ContainerFromIndex(i) is TimelineItem timelineItem)
            {
                var idx = IsReverse ? ItemCount - 1 - i : i;
                timelineItem.NotifyVisualIndexChanged(this, idx);
            }
        }
    }
}