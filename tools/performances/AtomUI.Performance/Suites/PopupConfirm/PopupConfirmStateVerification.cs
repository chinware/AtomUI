using AtomUI.Desktop.Controls;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace AtomUI.Performance;

using AvaloniaButton = Avalonia.Controls.Button;

internal static partial class Program
{
    private static bool RunPopupConfirmStateVerification()
    {
        var failures = new List<string>();
        VerifyPopupConfirmClosedKeepsPopupLazy(failures);
        VerifyPopupConfirmContainerOptionalSlots(failures);
        VerifyPopupConfirmContainerSlotToggles(failures);
        VerifyPopupConfirmContainerDetachReattach(failures);
        VerifyPopupConfirmContainerButtonLifecycle(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("PopupConfirm state verification passed.");
            return true;
        }

        Console.Error.WriteLine("PopupConfirm state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }

        return false;
    }

    private static void VerifyPopupConfirmClosedKeepsPopupLazy(ICollection<string> failures)
    {
        var popupConfirm = CreatePopupConfirm();
        using var _ = RealizeControl(popupConfirm);

        Expect(popupConfirm.Flyout != null,
            "PopupConfirm should create its Flyout after attach.",
            failures);
        if (popupConfirm.Flyout is not PopupFlyoutBase flyout)
        {
            return;
        }

        var popupState = GetPopupLazyState(flyout);
        Expect(!popupState.IsCreated,
            "Closed PopupConfirm should not create Popup shell before first open.",
            failures);
        Expect(!popupState.HasChild,
            "Closed PopupConfirm should not create popup child before first open.",
            failures);
    }

    private static void VerifyPopupConfirmContainerOptionalSlots(ICollection<string> failures)
    {
        using (var basicRealized = RealizeControl(CreatePopupConfirmContainer()))
        {
            var container = (PopupConfirmContainer)basicRealized.RootControls[0];
            Expect(FindVisualByName<ContentPresenter>(container, "PART_Content") != null,
                "PopupConfirmContainer with ConfirmContent should create PART_Content.",
                failures);
            Expect(FindVisualByName<AvaloniaButton>(container, "PART_CancelButton") != null,
                "PopupConfirmContainer with IsShowCancelButton=true should create PART_CancelButton.",
                failures);
            Expect(FindVisualByName<AvaloniaButton>(container, "PART_OkButton") != null,
                "PopupConfirmContainer should always create PART_OkButton.",
                failures);
        }

        using (var titleOnlyRealized = RealizeControl(CreatePopupConfirmContainer(confirmContent: null)))
        {
            var container = (PopupConfirmContainer)titleOnlyRealized.RootControls[0];
            var contentPresenter = FindVisualByName<ContentPresenter>(container, "PART_Content");
            Expect(contentPresenter != null,
                "PopupConfirmContainer without ConfirmContent should keep the static PART_Content slot.",
                failures);
            Expect(contentPresenter is { IsVisible: false, Content: null },
                "PopupConfirmContainer without ConfirmContent should keep PART_Content hidden and empty.",
                failures);
        }

        using (var noCancelRealized = RealizeControl(CreatePopupConfirmContainer(showCancelButton: false)))
        {
            var container = (PopupConfirmContainer)noCancelRealized.RootControls[0];
            var cancelButton = FindVisualByName<AvaloniaButton>(container, "PART_CancelButton");
            Expect(cancelButton != null,
                "PopupConfirmContainer with IsShowCancelButton=false should keep the static PART_CancelButton slot.",
                failures);
            Expect(cancelButton is { IsVisible: false },
                "PopupConfirmContainer with IsShowCancelButton=false should keep PART_CancelButton hidden.",
                failures);
            Expect(FindVisualByName<AvaloniaButton>(container, "PART_OkButton") != null,
                "PopupConfirmContainer without cancel button should keep PART_OkButton.",
                failures);
        }
    }

    private static void VerifyPopupConfirmContainerSlotToggles(ICollection<string> failures)
    {
        var container = CreatePopupConfirmContainer();
        using var realized = RealizeControl(container);

        var firstContent = FindVisualByName<ContentPresenter>(container, "PART_Content");
        Expect(firstContent is { IsVisible: true },
            "PopupConfirmContainer should start with a visible content presenter when ConfirmContent is set.",
            failures);

        container.ConfirmContent = null;
        RefreshLayout(realized.Window);
        var hiddenContent = FindVisualByName<ContentPresenter>(container, "PART_Content");
        Expect(ReferenceEquals(firstContent, hiddenContent),
            "PopupConfirmContainer should keep the static PART_Content slot after ConfirmContent is cleared.",
            failures);
        Expect(hiddenContent is { IsVisible: false, Content: null },
            "PopupConfirmContainer should hide and clear PART_Content after ConfirmContent is cleared.",
            failures);

        container.ConfirmContent = "Restored content";
        RefreshLayout(realized.Window);
        var restoredContent = FindVisualByName<ContentPresenter>(container, "PART_Content");
        Expect(ReferenceEquals(firstContent, restoredContent),
            "PopupConfirmContainer should reuse the static PART_Content slot after ConfirmContent is restored.",
            failures);
        Expect(restoredContent is { IsVisible: true, Content: "Restored content" },
            "PopupConfirmContainer should show restored ConfirmContent in PART_Content.",
            failures);

        var firstCancelButton = FindVisualByName<AvaloniaButton>(container, "PART_CancelButton");
        Expect(firstCancelButton is { IsVisible: true },
            "PopupConfirmContainer should start with a visible cancel button when IsShowCancelButton=true.",
            failures);

        container.IsShowCancelButton = false;
        RefreshLayout(realized.Window);
        var hiddenCancelButton = FindVisualByName<AvaloniaButton>(container, "PART_CancelButton");
        Expect(ReferenceEquals(firstCancelButton, hiddenCancelButton),
            "PopupConfirmContainer should keep the static PART_CancelButton slot when IsShowCancelButton=false.",
            failures);
        Expect(hiddenCancelButton is { IsVisible: false },
            "PopupConfirmContainer should hide PART_CancelButton when IsShowCancelButton=false.",
            failures);

        container.IsShowCancelButton = true;
        RefreshLayout(realized.Window);
        var restoredCancelButton = FindVisualByName<AvaloniaButton>(container, "PART_CancelButton");
        Expect(ReferenceEquals(firstCancelButton, restoredCancelButton),
            "PopupConfirmContainer should reuse the static PART_CancelButton slot after IsShowCancelButton=true.",
            failures);
        Expect(restoredCancelButton is { IsVisible: true },
            "PopupConfirmContainer should show PART_CancelButton after IsShowCancelButton=true.",
            failures);
    }

    private static void VerifyPopupConfirmContainerButtonLifecycle(ICollection<string> failures)
    {
        var popupConfirm = CreatePopupConfirm();
        var container = CreatePopupConfirmContainer();
        container.PopupConfirmRef = new WeakReference<PopupConfirm>(popupConfirm);

        var confirmedCount = 0;
        var cancelledCount = 0;
        var popupClickCount = 0;
        popupConfirm.Confirmed += (_, _) => confirmedCount++;
        popupConfirm.Cancelled += (_, _) => cancelledCount++;
        popupConfirm.PopupClick += (_, _) => popupClickCount++;

        AvaloniaButton? okButton;
        AvaloniaButton? cancelButton;
        using (var realized = RealizeControl(container))
        {
            okButton = FindVisualByName<AvaloniaButton>(container, "PART_OkButton");
            cancelButton = FindVisualByName<AvaloniaButton>(container, "PART_CancelButton");

            Expect(okButton != null,
                "PopupConfirmContainer should create PART_OkButton for button lifecycle verification.",
                failures);
            Expect(cancelButton != null,
                "PopupConfirmContainer should create PART_CancelButton for button lifecycle verification.",
                failures);

            if (okButton != null && cancelButton != null)
            {
                Expect(!GetLocalRoutedHandlerNames(okButton).Any(name => name.Contains("Click")),
                    "PopupConfirm OK button should use the container class handler instead of a local Click handler.",
                    failures);
                Expect(!GetLocalRoutedHandlerNames(cancelButton).Any(name => name.Contains("Click")),
                    "PopupConfirm cancel button should use the container class handler instead of a local Click handler.",
                    failures);
            }

            okButton?.RaiseEvent(new RoutedEventArgs(AvaloniaButton.ClickEvent)
            {
                Source = okButton
            });
            cancelButton?.RaiseEvent(new RoutedEventArgs(AvaloniaButton.ClickEvent)
            {
                Source = cancelButton
            });

            Expect(confirmedCount == 1,
                $"PopupConfirm OK click should raise Confirmed once. Actual: {confirmedCount}.",
                failures);
            Expect(cancelledCount == 1,
                $"PopupConfirm Cancel click should raise Cancelled once. Actual: {cancelledCount}.",
                failures);
            Expect(popupClickCount == 2,
                $"PopupConfirm button clicks should raise PopupClick twice. Actual: {popupClickCount}.",
                failures);

            container.IsShowCancelButton = false;
            RefreshLayout(realized.Window);
            cancelButton?.RaiseEvent(new RoutedEventArgs(AvaloniaButton.ClickEvent)
            {
                Source = cancelButton
            });

            Expect(cancelledCount == 1,
                "Hidden PopupConfirm cancel button should not raise Cancelled.",
                failures);
            Expect(popupClickCount == 2,
                "Hidden PopupConfirm cancel button should not raise PopupClick.",
                failures);

            container.IsShowCancelButton = true;
            RefreshLayout(realized.Window);
            var restoredCancelButton = FindVisualByName<AvaloniaButton>(container, "PART_CancelButton");
            restoredCancelButton?.RaiseEvent(new RoutedEventArgs(AvaloniaButton.ClickEvent)
            {
                Source = restoredCancelButton
            });

            Expect(cancelledCount == 2,
                $"Re-enabled PopupConfirm cancel button should raise Cancelled once. Actual: {cancelledCount}.",
                failures);
            Expect(popupClickCount == 3,
                $"Re-enabled PopupConfirm cancel button should raise PopupClick once. Actual: {popupClickCount}.",
                failures);
        }
    }

    private static void VerifyPopupConfirmContainerDetachReattach(ICollection<string> failures)
    {
        var popupConfirm = CreatePopupConfirm();
        var container = CreatePopupConfirmContainer();
        container.PopupConfirmRef = new WeakReference<PopupConfirm>(popupConfirm);

        var confirmedCount = 0;
        var cancelledCount = 0;
        popupConfirm.Confirmed += (_, _) => confirmedCount++;
        popupConfirm.Cancelled += (_, _) => cancelledCount++;

        using var realized = RealizeControl(container);
        if (realized.Window.Content is not StackPanel host)
        {
            failures.Add("PopupConfirm detach/reattach verification could not find host panel.");
            return;
        }

        host.Children.Remove(container);
        RefreshLayout(realized.Window);
        host.Children.Add(container);
        RefreshLayout(realized.Window);

        var contentPresenter = FindVisualByName<ContentPresenter>(container, "PART_Content");
        var okButton = FindVisualByName<AvaloniaButton>(container, "PART_OkButton");
        var cancelButton = FindVisualByName<AvaloniaButton>(container, "PART_CancelButton");

        Expect(contentPresenter != null,
            "PopupConfirmContainer should keep PART_Content after detach and reattach.",
            failures);
        Expect(okButton != null,
            "PopupConfirmContainer should keep PART_OkButton after detach and reattach.",
            failures);
        Expect(cancelButton != null,
            "PopupConfirmContainer should keep PART_CancelButton after detach and reattach.",
            failures);

        okButton?.RaiseEvent(new RoutedEventArgs(AvaloniaButton.ClickEvent)
        {
            Source = okButton
        });
        cancelButton?.RaiseEvent(new RoutedEventArgs(AvaloniaButton.ClickEvent)
        {
            Source = cancelButton
        });

        Expect(confirmedCount == 1,
            $"PopupConfirm OK button should still work after detach and reattach. Actual: {confirmedCount}.",
            failures);
        Expect(cancelledCount == 1,
            $"PopupConfirm cancel button should still work after detach and reattach. Actual: {cancelledCount}.",
            failures);
    }
}
