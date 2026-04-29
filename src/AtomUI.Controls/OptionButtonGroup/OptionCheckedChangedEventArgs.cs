using AtomUI.Controls.Commons;
using Avalonia.Interactivity;

namespace AtomUI.Controls;

public class OptionCheckedChangedEventArgs : RoutedEventArgs
{
    public OptionCheckedChangedEventArgs(RoutedEvent routedEvent, AbstractOptionButton option, int index)
        : base(routedEvent)
    {
        CheckedOption = option;
        Index         = index;
    }

    public AbstractOptionButton CheckedOption { get; }
    public int Index { get; }
}