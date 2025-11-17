using Avalonia.Interactivity;

namespace AtomUI.Controls;

internal class GradientActiveStopChangedEventArgs : RoutedEventArgs
{
    public GradientActiveStopChangedEventArgs(RoutedEvent? routedEvent)
        : base(routedEvent)
    {}

    public GradientActiveStopChangedEventArgs(RoutedEvent? routedEvent, object? source)
        : base(routedEvent, source)
    {
        
    }
    
    public int? OldIndex { get; set; }
    public int? NewIndex { get; set; }
}