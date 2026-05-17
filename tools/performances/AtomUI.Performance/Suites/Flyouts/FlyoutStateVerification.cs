using System.Reflection;
using AtomUI.Desktop.Controls;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace AtomUI.Performance;

using AvaloniaButton = Avalonia.Controls.Button;

internal static partial class Program
{
    private static readonly FieldInfo PopupLazyField =
        typeof(PopupFlyoutBase).GetField("_popupLazy", BindingFlags.Instance | BindingFlags.NonPublic)
        ?? throw new InvalidOperationException("PopupFlyoutBase._popupLazy field was not found.");

    private static readonly PropertyInfo LazyIsValueCreatedProperty =
        typeof(Lazy<Avalonia.Controls.Primitives.Popup>).GetProperty(
            nameof(Lazy<Avalonia.Controls.Primitives.Popup>.IsValueCreated))
        ?? throw new InvalidOperationException("Lazy<Popup>.IsValueCreated property was not found.");

    private static readonly PropertyInfo LazyValueProperty =
        typeof(Lazy<Avalonia.Controls.Primitives.Popup>).GetProperty(
            nameof(Lazy<Avalonia.Controls.Primitives.Popup>.Value))
        ?? throw new InvalidOperationException("Lazy<Popup>.Value property was not found.");

    private static bool RunFlyoutStateVerification()
    {
        var failures = new List<string>();
        VerifyClosedFlyoutsDoNotCreatePopup(failures);
        VerifyFlyoutTriggerLightDismissWithoutPopupCreation(failures);
        VerifyFlyoutPresenterMaterializationLifecycle(failures);
        VerifyPopupConfirmClickLifecycle(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("Flyouts state verification passed.");
            return true;
        }

        Console.Error.WriteLine("Flyouts state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifyClosedFlyoutsDoNotCreatePopup(ICollection<string> failures)
    {
        var scenarios = new (string Name, Func<Control> Create)[]
        {
            ("FlyoutHost.Hover.Closed", () => CreateFlyoutHost(FlyoutTriggerType.Hover)),
            ("FlyoutHost.Click.Closed", () => CreateFlyoutHost(FlyoutTriggerType.Click)),
            ("FlyoutHost.Focus.Closed", () => CreateFlyoutHost(FlyoutTriggerType.Focus)),
            ("PopupConfirm.Closed", CreatePopupConfirm),
            ("DropdownButton.MenuFlyout.Closed", CreateFlyoutDropdownButton),
            ("SplitButton.MenuFlyout.Closed", CreateFlyoutSplitButton),
            ("Flyouts.GalleryShape", CreateFlyoutGalleryShape)
        };

        foreach (var scenario in scenarios)
        {
            var control = scenario.Create();
            using var _ = RealizeControl(control);
            var flyouts = GetReferencedFlyouts(control);
            var createdCount = flyouts.Count(flyout => GetPopupLazyState(flyout).IsCreated);
            var childCount = flyouts.Count(flyout => GetPopupLazyState(flyout).HasChild);

            Expect(createdCount == 0,
                $"{scenario.Name} should not create Popup shell while closed. Created: {createdCount}/{flyouts.Count}.",
                failures);
            Expect(childCount == 0,
                $"{scenario.Name} should not create Popup child while closed. Created children: {childCount}/{flyouts.Count}.",
                failures);
        }
    }

    private static void VerifyFlyoutTriggerLightDismissWithoutPopupCreation(ICollection<string> failures)
    {
        var host = CreateFlyoutHost(FlyoutTriggerType.Hover);
        using var realized = RealizeControl(host);
        var flyout = host.Flyout;
        var helper = GetPrivateField(host, "AtomUI.Desktop.Controls.FlyoutHost", "_flyoutStateHelper");

        Expect(helper is not null,
            "FlyoutHost should keep a FlyoutStateHelper for light-dismiss verification.",
            failures);
        if (helper is null)
        {
            return;
        }

        Expect(flyout is null || !GetPopupLazyState(flyout).IsCreated,
            "Closed Hover trigger setup should not create Popup shell.",
            failures);
        InvokePrivateMethod(helper, "AtomUI.Desktop.Controls.FlyoutStateHelper", "ConfigureFlyoutLightDismiss");
        Expect(flyout is { IsLightDismissEnabled: false },
            "Hover FlyoutHost should disable light dismiss immediately before show.",
            failures);

        host.Trigger = FlyoutTriggerType.Click;
        RefreshLayout(realized.Window);
        Expect(flyout is null || !GetPopupLazyState(flyout).IsCreated,
            "Closed Click trigger setup should not create Popup shell.",
            failures);
        InvokePrivateMethod(helper, "AtomUI.Desktop.Controls.FlyoutStateHelper", "ConfigureFlyoutLightDismiss");
        Expect(flyout is { IsLightDismissEnabled: true },
            "Click FlyoutHost should enable light dismiss immediately before show.",
            failures);

        host.Trigger = FlyoutTriggerType.Focus;
        RefreshLayout(realized.Window);
        Expect(flyout is null || !GetPopupLazyState(flyout).IsCreated,
            "Closed Focus trigger setup should not create Popup shell.",
            failures);
        InvokePrivateMethod(helper, "AtomUI.Desktop.Controls.FlyoutStateHelper", "ConfigureFlyoutLightDismiss");
        Expect(flyout is { IsLightDismissEnabled: false },
            "Focus FlyoutHost should disable light dismiss immediately before show.",
            failures);
    }

    private static void VerifyFlyoutPresenterMaterializationLifecycle(ICollection<string> failures)
    {
        VerifyPresenterMaterializationLifecycle(
            "FlyoutHost.Click",
            CreateFlyoutHost(FlyoutTriggerType.Click),
            host => host.Flyout,
            failures);

        VerifyPresenterMaterializationLifecycle(
            "PopupConfirm",
            CreatePopupConfirm(),
            popupConfirm => popupConfirm.Flyout,
            failures);

        VerifyPresenterMaterializationLifecycle(
            "DropdownButton.MenuFlyout",
            CreateFlyoutDropdownButton(),
            dropdownButton => dropdownButton.DropdownFlyout,
            failures);

        VerifyPresenterMaterializationLifecycle(
            "SplitButton.MenuFlyout",
            CreateFlyoutSplitButton(),
            splitButton => splitButton.Flyout,
            failures);
    }

    private static void VerifyPresenterMaterializationLifecycle<TControl>(string name,
                                                                          TControl control,
                                                                          Func<TControl, PopupFlyoutBase?> flyoutSelector,
                                                                          ICollection<string> failures)
        where TControl : Control
    {
        using var _ = RealizeControl(control);
        var flyout = flyoutSelector(control);

        Expect(flyout is not null, $"{name} should expose a flyout after attach.", failures);
        if (flyout is null)
        {
            return;
        }

        Expect(!GetPopupLazyState(flyout).IsCreated,
            $"{name} should keep Popup shell lazy before first open.",
            failures);

        var firstPresenter = EnsureFlyoutPresenterForTest(flyout);
        var firstMaterializedState = GetPopupLazyState(flyout);
        Expect(firstPresenter is not null && firstMaterializedState is { IsCreated: true, HasChild: true },
            $"{name} should create Popup shell and child on first presenter materialization.",
            failures);

        var secondPresenter = EnsureFlyoutPresenterForTest(flyout);
        Expect(ReferenceEquals(firstPresenter, secondPresenter),
            $"{name} should reuse first materialized Popup child.",
            failures);
    }

    private static Control? EnsureFlyoutPresenterForTest(PopupFlyoutBase flyout)
    {
        if (flyout.Popup.Child is Control existing)
        {
            return existing;
        }

        var createPresenter = flyout.GetType().GetMethod("CreatePresenter",
            BindingFlags.Instance | BindingFlags.NonPublic);
        if (createPresenter?.Invoke(flyout, null) is Control presenter)
        {
            flyout.Popup.Child = presenter;
            return presenter;
        }

        return null;
    }

    private static void VerifyPopupConfirmClickLifecycle(ICollection<string> failures)
    {
        var popupConfirm = CreatePopupConfirm();
        var containerType = typeof(PopupConfirm).Assembly.GetType("AtomUI.Desktop.Controls.PopupConfirmContainer");
        Expect(containerType is not null,
            "PopupConfirmContainer type should exist for lifecycle verification.",
            failures);
        if (containerType is null)
        {
            return;
        }

        var confirmedCount = 0;
        var popupClickCount = 0;
        popupConfirm.Confirmed += (_, _) => confirmedCount++;
        popupConfirm.PopupClick += (_, _) => popupClickCount++;

        var container = (Control)Activator.CreateInstance(containerType, popupConfirm)!;
        using var _ = RealizeControl(container);
        var okButton = container.GetSelfAndVisualDescendants()
                                .OfType<AvaloniaButton>()
                                .FirstOrDefault(button => button.Name == "PART_OkButton");
        Expect(okButton is not null,
            "PopupConfirm should create PART_OkButton on first open.",
            failures);
        if (okButton is null)
        {
            return;
        }

        okButton.RaiseEvent(new RoutedEventArgs(AvaloniaButton.ClickEvent));

        Expect(confirmedCount == 1,
            $"PopupConfirm OK click should raise Confirmed once. Actual: {confirmedCount}.",
            failures);
        Expect(popupClickCount == 1,
            $"PopupConfirm OK click should raise PopupClick once. Actual: {popupClickCount}.",
            failures);
    }

    private static IReadOnlyList<PopupFlyoutBase> GetReferencedFlyouts(Control root)
    {
        var flyouts = new List<PopupFlyoutBase>();
        foreach (var control in root.GetSelfAndVisualDescendants().OfType<Control>())
        {
            if (control is FlyoutHost { Flyout: { } hostFlyout })
            {
                flyouts.Add(hostFlyout);
            }

            if (control is DropdownButton { DropdownFlyout: { } dropdownFlyout })
            {
                flyouts.Add(dropdownFlyout);
            }

            if (control is AtomUI.Desktop.Controls.SplitButton { Flyout: { } splitFlyout })
            {
                flyouts.Add(splitFlyout);
            }
        }

        return flyouts;
    }

    private static (bool IsCreated, bool HasChild) GetPopupLazyState(PopupFlyoutBase flyout)
    {
        var lazy = PopupLazyField.GetValue(flyout);
        if (lazy is null)
        {
            return (false, false);
        }

        var isCreated = LazyIsValueCreatedProperty.GetValue(lazy) is true;
        if (!isCreated)
        {
            return (false, false);
        }

        var popup = LazyValueProperty.GetValue(lazy) as Avalonia.Controls.Primitives.Popup;
        return (true, popup?.Child is not null);
    }
}
