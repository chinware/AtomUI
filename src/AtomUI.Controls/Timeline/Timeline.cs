using AtomUI.Data;
using AtomUI.IconPkg;
using AtomUI.IconPkg.AntDesign;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Layout;

namespace AtomUI.Controls;

public enum TimeLineMode
{
    Left,
    Right,
    Alternate
}

public class Timeline : ItemsControl
{
    #region 公共属性定义

    public static readonly StyledProperty<TimeLineMode> ModeProperty =
        AvaloniaProperty.Register<Timeline, TimeLineMode>(nameof(Mode), TimeLineMode.Left);

    public static readonly StyledProperty<string> PendingProperty =
        AvaloniaProperty.Register<Timeline, string>(nameof(Pending), "");

    public static readonly StyledProperty<bool> ReverseProperty =
        AvaloniaProperty.Register<Timeline, bool>(nameof(Reverse), false);

    public static readonly StyledProperty<Icon?> PendingIconProperty =
        AvaloniaProperty.Register<Alert, Icon?>(nameof(PendingIcon));

    public TimeLineMode Mode
    {
        get => GetValue(ModeProperty);
        set => SetValue(ModeProperty, value);
    }

    public string Pending
    {
        get => GetValue(PendingProperty);
        set => SetValue(PendingProperty, value);
    }

    public bool Reverse
    {
        get => GetValue(ReverseProperty);
        set { SetValue(ReverseProperty, value); }
    }

    public Icon? PendingIcon
    {
        get => GetValue(PendingIconProperty);
        set => SetValue(PendingIconProperty, value);
    }

    #endregion

    private TimelineItem? _pendingItem;

    static Timeline()
    {
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new TimelineItem();
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
            BindUtils.RelayBind(this, ModeProperty, timelineItem, TimelineItem.ModeProperty);
            BindUtils.RelayBind(this, ReverseProperty, timelineItem, TimelineItem.ReverseProperty);
            BindUtils.RelayBind(this, ItemCountProperty, timelineItem, TimelineItem.CountProperty);
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        OnReversePropertyChanged();
        addPendingItem();

        TokenResourceBinder.CreateTokenBinding(this, BorderThicknessProperty,
            SharedTokenKey.BorderThickness,
            BindingPriority.Template,
            new RenderScaleAwareThicknessConfigure(this));
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ReverseProperty && VisualRoot is not null)
        {
            OnReversePropertyChanged();
        }

        if (change.Property == ItemCountProperty && VisualRoot is not null)
        {
            OnItemCountPropertyChanged();
        }

        if (change.Property == PendingProperty && VisualRoot is not null)
        {
            OnPendingPropertyChanged();
        }
    }

    private void OnPendingPropertyChanged()
    {
        foreach (var item in Items)
        {
            if (item is TimelineItem timelineItem && timelineItem.IsPending)
            {
                Items.Remove(item);
                break;
            }
        }

        addPendingItem();
    }

    private void addPendingItem()
    {
        if (!String.IsNullOrEmpty(Pending))
        {
            if (_pendingItem is null)
            {
                _pendingItem = new TimelineItem();
                var textBlock = new TextBlock();

                if (PendingIcon is null)
                {
                    PendingIcon                     = AntDesignIconPackage.LoadingOutlined();
                    PendingIcon.Width               = 10;
                    PendingIcon.Height              = 10;
                    PendingIcon.LoadingAnimation    = IconAnimation.Spin;
                    PendingIcon.VerticalAlignment   = VerticalAlignment.Top;
                    PendingIcon.HorizontalAlignment = HorizontalAlignment.Center;
                }

                _pendingItem.DotIcon   = PendingIcon;
                _pendingItem.IsPending = true;
                _pendingItem.Content   = textBlock;
                BindUtils.RelayBind(this, PendingProperty, textBlock, TextBlock.TextProperty);
            }

            if (Reverse)
            {
                Items.Insert(0, _pendingItem);
            }
            else
            {
                Items.Add(_pendingItem);
            }
        }
    }

    private void OnItemCountPropertyChanged()
    {
    }

    private void OnReversePropertyChanged()
    {
        var items = Items.Cast<object>().ToList();
        items.Reverse();
        Items.Clear();
        foreach (var item in items)
        {
            Items.Add(item);
            if (item is TimelineItem timelineItem)
            {
                timelineItem.Index = Items.IndexOf(item);
            }
        }

        foreach (var item in items)
        {
            if (item is TimelineItem timelineItem)
            {
                timelineItem.Index = Items.IndexOf(item);
            }
        }
    }
}