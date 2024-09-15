using System.Collections;
using AtomUI.Data;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;

namespace AtomUI.Controls;

public class Timeline : ItemsControl
{
    public static readonly StyledProperty<string> ModeProperty =
        AvaloniaProperty.Register<Timeline, string>(nameof(Mode), "left");
    
    public static readonly StyledProperty<string> PendingProperty =
        AvaloniaProperty.Register<Timeline, string>(nameof(Pending), "");
    
    public static readonly StyledProperty<bool> ReverseProperty =
        AvaloniaProperty.Register<Timeline, bool>(nameof(Reverse), false);

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
        set
        {
            SetValue(ReverseProperty, value);
            // Items.Reverse();
        }
    }
    
    public Timeline()
    {
        OnReversePropertyChanged();
        System.Console.WriteLine("Timelien Reverse {0}", Reverse);
        if (Reverse)
        {
            System.Console.WriteLine("Timelien Reverse 222");
            OnReversePropertyChanged();
        }
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new TimelineItem();
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        return NeedsContainer<TimelineItem>(item, out recycleKey);
    }

    // 像TimelineItem传递Timeline的Mode属性
    protected override void PrepareContainerForItemOverride(Control element, object? item, int index)
    {
        base.PrepareContainerForItemOverride(element, item, index);
        if (element is TimelineItem timelineItem)
        { 
            timelineItem.Index = index;
            timelineItem.Mode = Mode;
            timelineItem.IsLast = Items.Count - 1 == index;
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
            var bb = change.GetNewValue<bool>();
            System.Console.WriteLine("22 Timelien onpropertychanged {0} {1}", change.Property.Name, bb);

            OnReversePropertyChanged();
        }
    }
    
    private void OnReversePropertyChanged()
    {
        System.Console.WriteLine("OnReversePropertyChanged onpropertychanged, Items {0}", Items.Count);

        var items = Items.Cast<object>().ToList();
        items.Reverse();
        Items.Clear();
        foreach (var item in items)
        {
            Items.Add(item);
        }
    }
}