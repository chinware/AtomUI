using AtomUI.Data;
using AtomUI.Icon;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Layout;

namespace AtomUI.Controls;

public class Timeline : ItemsControl
{
    #region 公共属性定义

    public static readonly StyledProperty<string> ModeProperty =
        AvaloniaProperty.Register<Timeline, string>(nameof(Mode), "left");

    public static readonly StyledProperty<string> PendingProperty =
        AvaloniaProperty.Register<Timeline, string>(nameof(Pending), "");

    public static readonly StyledProperty<bool> ReverseProperty =
        AvaloniaProperty.Register<Timeline, bool>(nameof(Reverse), false);

    public static readonly StyledProperty<PathIcon?> PendingIconProperty =
        AvaloniaProperty.Register<Alert, PathIcon?>(nameof(PendingIcon));

    public string Mode
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

    public PathIcon? PendingIcon
    {
        get => GetValue(PendingIconProperty);
        set => SetValue(PendingIconProperty, value);
    }

    #endregion

    public Timeline()
    {
        if (Reverse)
        {
            OnReversePropertyChanged();
        }
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (!String.IsNullOrEmpty(Pending))
        {
            var item      = new TimelineItem();
            var textBlock = new TextBlock();

            if (PendingIcon is null)
            {
                PendingIcon = new PathIcon
                {
                    Kind                = "LoadingOutlined",
                    Width               = 10,
                    Height              = 10,
                    LoadingAnimation    = IconAnimation.Spin,
                    VerticalAlignment   = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Center,
                };
            }

            item.DotIcon   = PendingIcon;
            item.IsPending = true;
            item.Content   = textBlock;
            BindUtils.RelayBind(this, PendingProperty, textBlock, TextBlock.TextProperty);

            Items.Add(item);
        }

        if (Reverse)
        {
            OnReversePropertyChanged();
        }
    }

    static Timeline()
    {
        ReverseProperty.Changed.AddClassHandler<Timeline>((x, e) => x.OnReversePropertyChanged());
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
            timelineItem.Index   = index;
            timelineItem.Mode    = Mode;
            timelineItem.IsLast  = Items.Count - 1 == index;
            timelineItem.IsFirst = index == 0;
            BindUtils.RelayBind(this, ModeProperty, timelineItem, TimelineItem.ModeProperty);
            BindUtils.RelayBind(this, ReverseProperty, timelineItem, TimelineItem.ReverseProperty);
            foreach (var child in Items)
            {
                if (child is TimelineItem otherItem)
                {
                    if (!string.IsNullOrEmpty(otherItem.Label))
                    {
                        timelineItem.HasLabel = true;
                        break;
                    }
                }
            }
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        TokenResourceBinder.CreateGlobalResourceBinding(this, BorderThicknessProperty,
            GlobalTokenResourceKey.BorderThickness,
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
    }

    private void OnReversePropertyChanged()
    {
        var items = Items.Cast<object>().ToList();
        items.Reverse();
        Items.Clear();
        foreach (var item in items)
        {
            Items.Add(item);
        }
    }
}