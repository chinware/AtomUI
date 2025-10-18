using Avalonia.Interactivity;

namespace AtomUI.Controls;

public class NavMenuItemClickEventArgs : RoutedEventArgs
{
    public NavMenuItemClickEventArgs(RoutedEvent routedEvent, INavMenuItem navMenuItem)
        : base(routedEvent)
    {
        NavMenuItem = navMenuItem;
    }

    public INavMenuItem NavMenuItem { get; }
}

public class NavMenuItemSelectedEventArgs : RoutedEventArgs
{
    public NavMenuItemSelectedEventArgs(RoutedEvent routedEvent, INavMenuItem navMenuItem)
        : base(routedEvent)
    {
        NavMenuItem = navMenuItem;
    }

    public INavMenuItem NavMenuItem { get; }
}