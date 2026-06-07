using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Controls;

public class MediaBreakPointIndicator : Control
{
    public const string MediaQueryIndicatorName = "PART_MediaQueryIndicator";

    public static readonly StyledProperty<MediaBreakPoint> MediaBreakPointProperty =
        MediaBreakAwareControlProperty.MediaBreakPointProperty.AddOwner<MediaBreakPointIndicator>();

    public event EventHandler<MediaBreakPointChangedEventArgs>? MediaBreakPointChanged;

    public MediaBreakPoint MediaBreakPoint
    {
        get => GetValue(MediaBreakPointProperty);
        set => SetValue(MediaBreakPointProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == MediaBreakPointProperty)
        {
            MediaBreakPointChanged?.Invoke(this, new MediaBreakPointChangedEventArgs(MediaBreakPoint));
        }
    }
}
