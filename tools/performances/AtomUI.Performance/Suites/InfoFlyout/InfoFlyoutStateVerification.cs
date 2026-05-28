using System.Reflection;
using AtomUI.Desktop.Controls;
using Avalonia.Controls;
using Avalonia.Controls.Primitives.PopupPositioning;

namespace AtomUI.Performance;

using AtomFlyout = AtomUI.Desktop.Controls.Flyout;

internal static partial class Program
{
    private const int InfoFlyoutHostBindingCount = 13;

    private static readonly FieldInfo InfoFlyoutHostDisposablesField =
        typeof(FlyoutHost).GetField("_flyoutDisposables", BindingFlags.Instance | BindingFlags.NonPublic)
        ?? throw new InvalidOperationException("FlyoutHost._flyoutDisposables field was not found.");

    private static readonly FieldInfo InfoFlyoutHostRegisteredFlyoutField =
        typeof(FlyoutHost).GetField("_registeredFlyout", BindingFlags.Instance | BindingFlags.NonPublic)
        ?? throw new InvalidOperationException("FlyoutHost._registeredFlyout field was not found.");

    private static bool RunInfoFlyoutStateVerification()
    {
        var failures = new List<string>();
        VerifyInfoFlyoutPropertyBindingLifecycle(failures);
        VerifyInfoFlyoutDetachRelease(failures);
        VerifyInfoFlyoutClosedPopupStaysLazy(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("InfoFlyout state verification passed.");
            return true;
        }

        Console.Error.WriteLine("InfoFlyout state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifyInfoFlyoutPropertyBindingLifecycle(ICollection<string> failures)
    {
        var host = CreateInfoFlyoutHost(FlyoutTriggerType.Click, isArrowVisible: false);
        var firstFlyout = host.Flyout;

        Expect(firstFlyout is not null,
            "InfoFlyout lifecycle verification should create an initial Flyout.",
            failures);
        Expect(GetInfoFlyoutBindingDisposableCount(host) == 0,
            "Detached InfoFlyout should not create host-to-flyout property bindings.",
            failures);
        Expect(GetInfoFlyoutRegisteredFlyout(host) == null,
            "Detached InfoFlyout should not register a flyout for host property propagation.",
            failures);
        if (firstFlyout is null)
        {
            return;
        }

        using var realized = RealizeControl(host);
        Expect(ReferenceEquals(GetInfoFlyoutRegisteredFlyout(host), firstFlyout),
            "Attached InfoFlyout should register the current Flyout.",
            failures);
        Expect(GetInfoFlyoutBindingDisposableCount(host) == InfoFlyoutHostBindingCount,
            $"Attached InfoFlyout should create {InfoFlyoutHostBindingCount} host-to-flyout bindings. Actual: {GetInfoFlyoutBindingDisposableCount(host)}.",
            failures);

        host.Placement = PlacementMode.Bottom;
        host.IsArrowVisible = true;
        RefreshLayout(realized.Window);
        Expect(firstFlyout.RequestedPlacement == PlacementMode.Bottom,
            $"Attached InfoFlyout should propagate Placement to Flyout.RequestedPlacement. Actual: {firstFlyout.RequestedPlacement}.",
            failures);
        Expect(firstFlyout.IsArrowVisible,
            "Attached InfoFlyout should propagate IsArrowVisible to the current Flyout.",
            failures);

        var secondFlyout = CreateInfoTextFlyout();
        host.Flyout = secondFlyout;
        RefreshLayout(realized.Window);
        Expect(ReferenceEquals(GetInfoFlyoutRegisteredFlyout(host), secondFlyout),
            "Replacing InfoFlyout.Flyout should register the new Flyout.",
            failures);
        Expect(GetInfoFlyoutBindingDisposableCount(host) == InfoFlyoutHostBindingCount,
            $"Replacing InfoFlyout.Flyout should keep exactly {InfoFlyoutHostBindingCount} active host-to-flyout bindings. Actual: {GetInfoFlyoutBindingDisposableCount(host)}.",
            failures);

        host.IsArrowVisible = false;
        RefreshLayout(realized.Window);
        Expect(!secondFlyout.IsArrowVisible,
            "Replacement InfoFlyout should receive subsequent host property changes.",
            failures);

        host.Flyout = null;
        RefreshLayout(realized.Window);
        Expect(GetInfoFlyoutBindingDisposableCount(host) == 0,
            $"Setting InfoFlyout.Flyout to null should release all {InfoFlyoutHostBindingCount} host-to-flyout bindings.",
            failures);
        Expect(GetInfoFlyoutRegisteredFlyout(host) == null,
            "Setting InfoFlyout.Flyout to null should clear the registered Flyout reference.",
            failures);

        host.IsArrowVisible = true;
        RefreshLayout(realized.Window);
        Expect(!secondFlyout.IsArrowVisible,
            "Flyout removed from InfoFlyout should not receive host property changes after Flyout=null.",
            failures);
    }

    private static void VerifyInfoFlyoutDetachRelease(ICollection<string> failures)
    {
        var host = CreateInfoFlyoutHost(FlyoutTriggerType.Hover, isArrowVisible: true);
        var flyout = host.Flyout;
        if (flyout is null)
        {
            failures.Add("InfoFlyout detach verification should create a Flyout.");
            return;
        }

        var realized = RealizeControl(host);
        try
        {
            Expect(GetInfoFlyoutBindingDisposableCount(host) == InfoFlyoutHostBindingCount,
                $"Attached InfoFlyout should have {InfoFlyoutHostBindingCount} active host-to-flyout bindings before detach.",
                failures);
        }
        finally
        {
            realized.Dispose();
        }

        Expect(GetInfoFlyoutBindingDisposableCount(host) == 0,
            "Detached InfoFlyout should release host-to-flyout property bindings.",
            failures);
        Expect(GetInfoFlyoutRegisteredFlyout(host) == null,
            "Detached InfoFlyout should clear the registered Flyout reference.",
            failures);

        var replacement = CreateInfoTextFlyout();
        host.Flyout = replacement;
        Expect(GetInfoFlyoutBindingDisposableCount(host) == 0,
            "Changing InfoFlyout.Flyout while detached should not create host-to-flyout property bindings.",
            failures);
        Expect(GetInfoFlyoutRegisteredFlyout(host) == null,
            "Changing InfoFlyout.Flyout while detached should not register the replacement Flyout.",
            failures);
    }

    private static void VerifyInfoFlyoutClosedPopupStaysLazy(ICollection<string> failures)
    {
        var host = CreateInfoFlyoutHost(FlyoutTriggerType.Hover);
        using var _ = RealizeControl(host);
        var flyout = host.Flyout;

        Expect(flyout is not null,
            "InfoFlyout closed popup verification should create a Flyout.",
            failures);
        if (flyout is null)
        {
            return;
        }

        var popupState = GetPopupLazyState(flyout);
        Expect(!popupState.IsCreated,
            "Closed InfoFlyout should not create Popup shell while registering host bindings.",
            failures);
        Expect(!popupState.HasChild,
            "Closed InfoFlyout should not create Popup child while registering host bindings.",
            failures);
    }

    private static int GetInfoFlyoutBindingDisposableCount(FlyoutHost host)
    {
        var disposables = InfoFlyoutHostDisposablesField.GetValue(host);
        if (disposables is null)
        {
            return 0;
        }

        return disposables.GetType().GetProperty("Count")?.GetValue(disposables) is int count
            ? count
            : -1;
    }

    private static AtomFlyout? GetInfoFlyoutRegisteredFlyout(FlyoutHost host)
    {
        return InfoFlyoutHostRegisteredFlyoutField.GetValue(host) as AtomFlyout;
    }
}
