using Avalonia;
using Avalonia.Controls;
using Avalonia.Metadata;

namespace AtomUI.Controls;

// todo 參考ListBox的实现
public class TimelineItem : ContentControl
{

    public static readonly StyledProperty<string?> LabelProperty = AvaloniaProperty.Register<TimelineItem, string?>(nameof(String));
    public static readonly StyledProperty<int> IndexProperty = AvaloniaProperty.Register<TimelineItem, int>(nameof(Index), 0);
    public static readonly StyledProperty<string> ModeProperty = AvaloniaProperty.Register<TimelineItem, string>(nameof(Mode), "left");
    public static readonly StyledProperty<string> ColorProperty = AvaloniaProperty.Register<TimelineItem, string>(nameof(Color), "blue");
    public static readonly StyledProperty<bool> HasLabelProperty = AvaloniaProperty.Register<TimelineItem, bool>(nameof(HasLabel), false);
    public static readonly StyledProperty<bool> IsLastProperty = AvaloniaProperty.Register<TimelineItem, bool>(nameof(IsLast), false);
    public static readonly StyledProperty<bool> ReverseProperty = AvaloniaProperty.Register<TimelineItem, bool>(nameof(Reverse), false);
    
    public int Index
    {
        get => GetValue(IndexProperty);
        set => SetValue(IndexProperty, value);
    }

    public string? Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }
    
    public string Mode
    {
        get => GetValue(ModeProperty);
        set => SetValue(ModeProperty, value);
    }
    
    public string Color
    {
        get => GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }
    
    public bool HasLabel
    {
        get => GetValue(HasLabelProperty);
        set => SetValue(HasLabelProperty, value);
    }
    
    public bool IsLast
    {
        get => GetValue(IsLastProperty);
        set => SetValue(IsLastProperty, value);
    }
    
    public bool Reverse
    {
        get => GetValue(ReverseProperty);
        set => SetValue(ReverseProperty, value);
    }
}