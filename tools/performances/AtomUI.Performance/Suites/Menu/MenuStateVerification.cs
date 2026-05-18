using System.Reflection;
using AtomUI.Animations;
using AtomUI.Controls;
using AtomUI.Icons.AntDesign;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace AtomUI.Performance;

using AtomContextMenu = AtomUI.Desktop.Controls.ContextMenu;
using AtomMenuItem = AtomUI.Desktop.Controls.MenuItem;
using AtomPopup = AtomUI.Desktop.Controls.Popup;
using AtomTextBlock = AtomUI.Desktop.Controls.TextBlock;

internal static partial class Program
{
    private static bool RunMenuStateVerification()
    {
        var failures = new List<string>();
        VerifyMenuItemClosedLeafIsLightweight(failures);
        VerifyMenuItemToggleLifecycle(failures);
        VerifyMenuItemIconAndGestureLifecycle(failures);
        VerifyMenuItemSubmenuPopupLifecycle(failures);
        VerifyTopLevelMenuItemHoverTransitions(failures);
        VerifyContextMenuWindowSubscriptionLifecycle(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("Menu state verification passed.");
            return true;
        }

        Console.Error.WriteLine("Menu state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifyMenuItemClosedLeafIsLightweight(ICollection<string> failures)
    {
        var item = CreateMenuLeaf();
        using var _ = RealizeControl(item);

        Expect(FindVisualByName<CheckBox>(item, "PART_ToggleCheckbox") == null,
            "Closed leaf MenuItem with ToggleType=None should not create PART_ToggleCheckbox.",
            failures);
        Expect(FindVisualByName<RadioButton>(item, "PART_ToggleRadio") == null,
            "Closed leaf MenuItem with ToggleType=None should not create PART_ToggleRadio.",
            failures);
        Expect(FindVisualByName<IconPresenter>(item, "ItemIconPresenter") == null,
            "Closed leaf MenuItem with no Icon should not create ItemIconPresenter.",
            failures);
        Expect(FindVisualByName<AtomTextBlock>(item, "InputGestureText") == null,
            "Closed leaf MenuItem with no InputGesture should not create InputGestureText.",
            failures);

        var popup = FindVisualByName<AtomPopup>(item, "PART_Popup");
        Expect(popup?.Child == null,
            "Closed leaf MenuItem should keep PART_Popup child empty.",
            failures);
    }

    private static void VerifyTopLevelMenuItemHoverTransitions(ICollection<string> failures)
    {
        var menu = CreateBasicMenu();
        menu.IsMotionEnabled = true;
        using var _ = RealizeControl(menu);

        var topLevelItem = menu.GetSelfAndVisualDescendants()
                               .OfType<AtomMenuItem>()
                               .FirstOrDefault(item => item.IsTopLevel);
        var colorTransitions = topLevelItem?.Transitions?.OfType<SolidColorBrushTransition>().ToList();
        Expect(colorTransitions?.Any(transition => transition.Property?.Name == "Background") == true,
            "Top-level MenuItem should animate Background for hover/open state.",
            failures);
        Expect(colorTransitions?.Any(transition => transition.Property?.Name == "Foreground") == true,
            "Top-level MenuItem should animate Foreground for hover/open state.",
            failures);
    }

    private static void VerifyMenuItemToggleLifecycle(ICollection<string> failures)
    {
        var item = CreateMenuLeaf(toggleType: MenuItemToggleType.CheckBox);
        using var realized = RealizeControl(item);

        var checkBox = FindVisualByName<CheckBox>(item, "PART_ToggleCheckbox");
        Expect(checkBox != null,
            "MenuItem ToggleType=CheckBox should create PART_ToggleCheckbox.",
            failures);
        Expect(FindVisualByName<RadioButton>(item, "PART_ToggleRadio") == null,
            "MenuItem ToggleType=CheckBox should not create PART_ToggleRadio.",
            failures);

        item.ToggleType = MenuItemToggleType.Radio;
        RefreshLayout(realized.Window);
        var radio = FindVisualByName<RadioButton>(item, "PART_ToggleRadio");
        Expect(radio != null,
            "MenuItem ToggleType=Radio should create PART_ToggleRadio.",
            failures);
        Expect(FindVisualByName<CheckBox>(item, "PART_ToggleCheckbox") == null,
            "MenuItem should remove PART_ToggleCheckbox when ToggleType changes to Radio.",
            failures);
        Expect(checkBox?.GetVisualParent() == null,
            "Removed PART_ToggleCheckbox should not keep a visual parent.",
            failures);

        item.ToggleType = MenuItemToggleType.None;
        RefreshLayout(realized.Window);
        Expect(FindVisualByName<CheckBox>(item, "PART_ToggleCheckbox") == null,
            "MenuItem ToggleType=None should not keep PART_ToggleCheckbox.",
            failures);
        Expect(FindVisualByName<RadioButton>(item, "PART_ToggleRadio") == null,
            "MenuItem ToggleType=None should not keep PART_ToggleRadio.",
            failures);
        Expect(radio?.GetVisualParent() == null,
            "Removed PART_ToggleRadio should not keep a visual parent.",
            failures);
    }

    private static void VerifyMenuItemIconAndGestureLifecycle(ICollection<string> failures)
    {
        var item = CreateMenuLeaf();
        using var realized = RealizeControl(item);

        item.Icon = new CopyOutlined();
        RefreshLayout(realized.Window);
        var iconPresenter = FindVisualByName<IconPresenter>(item, "ItemIconPresenter");
        Expect(iconPresenter != null,
            "MenuItem should create ItemIconPresenter when Icon is assigned.",
            failures);

        item.Icon = null;
        RefreshLayout(realized.Window);
        Expect(FindVisualByName<IconPresenter>(item, "ItemIconPresenter") == null,
            "MenuItem should remove ItemIconPresenter when Icon is cleared.",
            failures);
        Expect(iconPresenter?.GetVisualParent() == null,
            "Removed ItemIconPresenter should not keep a visual parent.",
            failures);

        item.InputGesture = KeyGesture.Parse("Ctrl+C");
        RefreshLayout(realized.Window);
        var gestureText = FindVisualByName<AtomTextBlock>(item, "InputGestureText");
        Expect(gestureText?.Text?.Length > 0,
            "MenuItem should create InputGestureText with platform text when InputGesture is assigned.",
            failures);

        item.InputGesture = null;
        RefreshLayout(realized.Window);
        Expect(FindVisualByName<AtomTextBlock>(item, "InputGestureText") == null,
            "MenuItem should remove InputGestureText when InputGesture is cleared.",
            failures);
        Expect(gestureText?.GetVisualParent() == null,
            "Removed InputGestureText should not keep a visual parent.",
            failures);
    }

    private static void VerifyMenuItemSubmenuPopupLifecycle(ICollection<string> failures)
    {
        var item = CreateSubMenuItem();
        using var realized = RealizeControl(item);
        var popup = FindVisualByName<AtomPopup>(item, "PART_Popup");

        Expect(popup != null,
            "Submenu MenuItem should expose PART_Popup.",
            failures);
        Expect(popup?.Child == null,
            "Closed submenu MenuItem should not create popup content before first open.",
            failures);

        InvokePrivateMethod(item,
            "AtomUI.Desktop.Controls.MenuItem",
            "EnsureSubmenuPopupContent",
            BindingFlags.Instance | BindingFlags.NonPublic);
        RefreshLayout(realized.Window);
        var firstChild = popup?.Child as Control;
        Expect(firstChild != null,
            "Submenu MenuItem should create popup content on first open.",
            failures);
        var itemsPresenter = GetPrivateField(item,
            "AtomUI.Desktop.Controls.MenuItem",
            "_submenuItemsPresenter") as Avalonia.Controls.Presenters.ItemsPresenter;
        Expect(itemsPresenter?.Name == "PART_ItemsPresenter",
            "Submenu popup content should include PART_ItemsPresenter after first open.",
            failures);

        InvokePrivateMethod(item,
            "AtomUI.Desktop.Controls.MenuItem",
            "EnsureSubmenuPopupContent",
            BindingFlags.Instance | BindingFlags.NonPublic);
        RefreshLayout(realized.Window);
        Expect(ReferenceEquals(firstChild, popup?.Child),
            "Submenu MenuItem should reuse popup content on second open.",
            failures);
    }

    private static void VerifyContextMenuWindowSubscriptionLifecycle(ICollection<string> failures)
    {
        var host = CreateContextMenuHost();
        using var realized = RealizeControl(host);
        var contextMenu = host.ContextMenu as AtomContextMenu;

        Expect(contextMenu != null,
            "ContextMenu host should contain AtomUI ContextMenu.",
            failures);
        if (contextMenu == null)
        {
            return;
        }

        Expect(GetPrivateField(contextMenu, "AtomUI.Desktop.Controls.ContextMenu", "_attachedWindow") == null,
            "Closed ContextMenu should not keep a Window.Deactivated subscription.",
            failures);
        SetPrivateField(contextMenu, "AtomUI.Desktop.Controls.ContextMenu", "_attachedWindow", realized.Window);
        InvokePrivateMethod(contextMenu,
            "AtomUI.Desktop.Controls.ContextMenu",
            "ClearWindowDeactivatedSubscription",
            BindingFlags.Instance | BindingFlags.NonPublic);
        Expect(GetPrivateField(contextMenu, "AtomUI.Desktop.Controls.ContextMenu", "_attachedWindow") == null,
            "ContextMenu should clear Window.Deactivated subscription state when closed.",
            failures);
    }

    private static void InvokePrivateMethod(object target,
                                            string declaringTypeName,
                                            string methodName,
                                            BindingFlags bindingFlags)
    {
        var type = target.GetType();
        while (type is not null)
        {
            if (type.FullName == declaringTypeName)
            {
                type.GetMethod(methodName, bindingFlags)?.Invoke(target, null);
                return;
            }

            type = type.BaseType;
        }
    }
}
