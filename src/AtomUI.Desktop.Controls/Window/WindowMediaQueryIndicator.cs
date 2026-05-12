using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Desktop.Controls;

internal class WindowMediaQueryIndicator : Control
{
    public const string MediaQueryIndicatorName = "PART_MediaQueryIndicator";
    public static readonly StyledProperty<MediaBreakPoint> MediaBreakPointProperty = 
        MediaBreakAwareControlProperty.MediaBreakPointProperty.AddOwner<WindowMediaQueryIndicator>();
    
    public MediaBreakPoint MediaBreakPoint
    {
        get => GetValue(MediaBreakPointProperty);
        set => SetValue(MediaBreakPointProperty, value);
    }
    
    public Window? OwnerWindow { get; set; }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == MediaBreakPointProperty)
        {
            OwnerWindow?.NotifyMediaBreakPointChanged(MediaBreakPoint);
        }
    }
}