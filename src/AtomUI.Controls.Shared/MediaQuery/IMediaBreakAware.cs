using Avalonia;

namespace AtomUI.Controls;

public interface IMediaBreakAwareControl
{
    public const string GlobalQueryContainerName = "PART_GlobalQueryContainer";
    MediaBreakPoint MediaBreakPoint { get; }
    event EventHandler<MediaBreakPointChangedEventArgs>? MediaBreakPointChanged;
}

public abstract class MediaBreakAwareControlProperty
{
    public const string MediaBreakPointPropertyName = "MediaBreakPoint";
    
    public static readonly StyledProperty<MediaBreakPoint> MediaBreakPointProperty = 
        AvaloniaProperty.Register<StyledElement, MediaBreakPoint>(MediaBreakPointPropertyName, MediaBreakPoint.None);
}