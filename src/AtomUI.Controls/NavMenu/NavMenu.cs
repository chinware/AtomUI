using Avalonia.Automation;
using Avalonia.Automation.Peers;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;

namespace AtomUI.Controls.NavMenu;

public class NavMenu : NavMenuBase
{
    private static readonly FuncTemplate<Panel?> DefaultPanel =
        new(() => new StackPanel { Orientation = Orientation.Vertical });

    static NavMenu()
    {
        ItemsPanelProperty.OverrideDefaultValue(typeof(NavMenu), DefaultPanel);
        KeyboardNavigation.TabNavigationProperty.OverrideDefaultValue(
            typeof(NavMenu),
            KeyboardNavigationMode.Once);
        AutomationProperties.AccessibilityViewProperty.OverrideDefaultValue<NavMenu>(AccessibilityView.Control);
        AutomationProperties.ControlTypeOverrideProperty.OverrideDefaultValue<NavMenu>(AutomationControlType.Menu);
    }

    public NavMenu()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NavMenu"/> class.
    /// </summary>
    /// <param name="interactionHandler">The menu interaction handler.</param>
    public NavMenu(INavMenuInteractionHandler interactionHandler)
        : base(interactionHandler)
    {
    }

    public override void Close()
    {
        if (!IsOpen)
        {
            return;
        }

        foreach (var i in ((INavMenu)this).SubItems)
        {
            i.Close();
        }

        IsOpen        = false;
        SelectedIndex = -1;

        RaiseEvent(new RoutedEventArgs
        {
            RoutedEvent = ClosedEvent,
            Source      = this,
        });
    }

    public override void Open()
    {
        if (IsOpen)
        {
            return;
        }

        IsOpen = true;

        RaiseEvent(new RoutedEventArgs
        {
            RoutedEvent = OpenedEvent,
            Source      = this,
        });
    }

    protected override void PrepareContainerForItemOverride(Control element, object? item, int index)
    {
        base.PrepareContainerForItemOverride(element, item, index);

        // Child menu items should not inherit the menu's ItemContainerTheme as that is specific
        // for top-level menu items.
        if ((element as NavMenuItem)?.ItemContainerTheme == ItemContainerTheme)
        {
            element.ClearValue(ItemContainerThemeProperty);
        }
    }
}