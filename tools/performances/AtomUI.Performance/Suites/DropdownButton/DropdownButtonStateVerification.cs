using System.Reflection;
using Avalonia.Interactivity;

namespace AtomUI.Performance;

using AtomDropdownButton = AtomUI.Desktop.Controls.DropdownButton;
using AtomMenuFlyout = AtomUI.Desktop.Controls.MenuFlyout;
using AtomMenuItem = AtomUI.Desktop.Controls.MenuItem;
using FlyoutMenuItemClickedEventArgs = AtomUI.Desktop.Controls.FlyoutMenuItemClickedEventArgs;

internal static partial class Program
{
    private static readonly FieldInfo DropdownButtonFlyoutBindingDisposablesField =
        typeof(AtomDropdownButton).GetField("_flyoutBindingDisposables", BindingFlags.Instance | BindingFlags.NonPublic)
        ?? throw new InvalidOperationException("DropdownButton._flyoutBindingDisposables field was not found.");

    private static readonly FieldInfo MenuFlyoutMenuItemClickedEventField =
        typeof(AtomMenuFlyout).GetField("MenuItemClicked", BindingFlags.Instance | BindingFlags.NonPublic)
        ?? throw new InvalidOperationException("MenuFlyout.MenuItemClicked event field was not found.");

    private static bool RunDropdownButtonStateVerification()
    {
        var failures = new List<string>();
        VerifyDropdownButtonFlyoutLifecycle(failures);
        VerifyDropdownButtonMenuItemForwarding(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("DropdownButton state verification passed.");
            return true;
        }

        Console.Error.WriteLine("DropdownButton state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifyDropdownButtonFlyoutLifecycle(ICollection<string> failures)
    {
        var firstFlyout = CreateDropdownButtonMenuFlyout();
        var dropdownButton = new AtomDropdownButton
        {
            Content        = "Actions",
            DropdownFlyout = firstFlyout
        };

        Expect(GetMenuFlyoutItemClickedHandlerCount(firstFlyout) == 0,
            $"Detached DropdownButton should not subscribe MenuFlyout.MenuItemClicked. Actual: {GetMenuFlyoutItemClickedHandlerCount(firstFlyout)}.",
            failures);
        Expect(GetDropdownButtonFlyoutBindingDisposables(dropdownButton) == null,
            "Detached DropdownButton should not create flyout property bindings.",
            failures);

        var realized = RealizeControl(dropdownButton);
        var secondFlyout = CreateDropdownButtonMenuFlyout();
        try
        {
            Expect(GetMenuFlyoutItemClickedHandlerCount(firstFlyout) == 1,
                $"Attached DropdownButton should register one MenuItemClicked handler. Actual: {GetMenuFlyoutItemClickedHandlerCount(firstFlyout)}.",
                failures);
            Expect(GetDropdownButtonFlyoutBindingDisposables(dropdownButton) != null,
                "Attached DropdownButton should create flyout property bindings.",
                failures);

            dropdownButton.DropdownFlyout = secondFlyout;
            RefreshLayout(realized.Window);

            Expect(GetMenuFlyoutItemClickedHandlerCount(firstFlyout) == 0,
                $"Replacing DropdownFlyout should remove MenuItemClicked handler from old flyout. Actual: {GetMenuFlyoutItemClickedHandlerCount(firstFlyout)}.",
                failures);
            Expect(GetMenuFlyoutItemClickedHandlerCount(secondFlyout) == 1,
                $"Replacement DropdownFlyout should register one MenuItemClicked handler. Actual: {GetMenuFlyoutItemClickedHandlerCount(secondFlyout)}.",
                failures);
        }
        finally
        {
            realized.Dispose();
        }

        Expect(GetMenuFlyoutItemClickedHandlerCount(secondFlyout) == 0,
            $"Detached DropdownButton should release MenuItemClicked handler. Actual: {GetMenuFlyoutItemClickedHandlerCount(secondFlyout)}.",
            failures);
        Expect(GetDropdownButtonFlyoutBindingDisposables(dropdownButton) == null,
            "Detached DropdownButton should release flyout property bindings.",
            failures);
    }

    private static void VerifyDropdownButtonMenuItemForwarding(ICollection<string> failures)
    {
        var flyout = CreateDropdownButtonMenuFlyout();
        var dropdownButton = new AtomDropdownButton
        {
            Content        = "Actions",
            DropdownFlyout = flyout
        };
        var forwardedCount = 0;
        dropdownButton.MenuItemClicked += (_, _) => forwardedCount++;

        using var _ = RealizeControl(dropdownButton);
        RaiseMenuFlyoutItemClicked(flyout);
        Expect(forwardedCount == 1,
            $"DropdownButton should forward MenuFlyout.MenuItemClicked once. Actual: {forwardedCount}.",
            failures);
    }

    private static int GetMenuFlyoutItemClickedHandlerCount(AtomMenuFlyout flyout)
    {
        return MenuFlyoutMenuItemClickedEventField.GetValue(flyout) is Delegate handler
            ? handler.GetInvocationList().Length
            : 0;
    }

    private static object? GetDropdownButtonFlyoutBindingDisposables(AtomDropdownButton dropdownButton)
    {
        return DropdownButtonFlyoutBindingDisposablesField.GetValue(dropdownButton);
    }

    private static void RaiseMenuFlyoutItemClicked(AtomMenuFlyout flyout)
    {
        if (MenuFlyoutMenuItemClickedEventField.GetValue(flyout) is not EventHandler<FlyoutMenuItemClickedEventArgs> handler)
        {
            return;
        }

        handler.Invoke(flyout, new FlyoutMenuItemClickedEventArgs(
            AtomDropdownButton.MenuItemClickedEvent,
            new AtomMenuItem
            {
                Header = "Action"
            }));
    }
}
