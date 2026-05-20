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
            Expect(FindVisualByName<ContentPresenter>(container, "PART_Content") == null,
                "PopupConfirmContainer without ConfirmContent should not create PART_Content.",
                failures);
        }

        using (var noCancelRealized = RealizeControl(CreatePopupConfirmContainer(showCancelButton: false)))
        {
            var container = (PopupConfirmContainer)noCancelRealized.RootControls[0];
            Expect(FindVisualByName<AvaloniaButton>(container, "PART_CancelButton") == null,
                "PopupConfirmContainer with IsShowCancelButton=false should not create PART_CancelButton.",
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
        Expect(firstContent != null,
            "PopupConfirmContainer should start with content presenter when ConfirmContent is set.",
            failures);

        container.ConfirmContent = null;
        RefreshLayout(realized.Window);
        Expect(FindVisualByName<ContentPresenter>(container, "PART_Content") == null,
            "PopupConfirmContainer should remove PART_Content after ConfirmContent is cleared.",
            failures);
        Expect(firstContent?.GetVisualParent() == null,
            "Removed PopupConfirm PART_Content should not keep a visual parent.",
            failures);

        container.ConfirmContent = "Restored content";
        RefreshLayout(realized.Window);
        Expect(FindVisualByName<ContentPresenter>(container, "PART_Content") != null,
            "PopupConfirmContainer should recreate PART_Content after ConfirmContent is restored.",
            failures);

        var firstCancelButton = FindVisualByName<AvaloniaButton>(container, "PART_CancelButton");
        Expect(firstCancelButton != null,
            "PopupConfirmContainer should start with cancel button when IsShowCancelButton=true.",
            failures);

        container.IsShowCancelButton = false;
        RefreshLayout(realized.Window);
        Expect(FindVisualByName<AvaloniaButton>(container, "PART_CancelButton") == null,
            "PopupConfirmContainer should remove PART_CancelButton when IsShowCancelButton=false.",
            failures);
        Expect(firstCancelButton?.GetVisualParent() == null,
            "Removed PopupConfirm PART_CancelButton should not keep a visual parent.",
            failures);

        container.IsShowCancelButton = true;
        RefreshLayout(realized.Window);
        Expect(FindVisualByName<AvaloniaButton>(container, "PART_CancelButton") != null,
            "PopupConfirmContainer should recreate PART_CancelButton after IsShowCancelButton=true.",
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

            okButton?.RaiseEvent(new RoutedEventArgs(AvaloniaButton.ClickEvent));
            cancelButton?.RaiseEvent(new RoutedEventArgs(AvaloniaButton.ClickEvent));

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
            cancelButton?.RaiseEvent(new RoutedEventArgs(AvaloniaButton.ClickEvent));

            Expect(cancelledCount == 1,
                "Removed PopupConfirm cancel button should not keep Click handler.",
                failures);
            Expect(popupClickCount == 2,
                "Removed PopupConfirm cancel button should not keep PopupClick handler.",
                failures);

            container.IsShowCancelButton = true;
            RefreshLayout(realized.Window);
            FindVisualByName<AvaloniaButton>(container, "PART_CancelButton")
                ?.RaiseEvent(new RoutedEventArgs(AvaloniaButton.ClickEvent));

            Expect(cancelledCount == 2,
                $"Recreated PopupConfirm cancel button should raise Cancelled once. Actual: {cancelledCount}.",
                failures);
            Expect(popupClickCount == 3,
                $"Recreated PopupConfirm cancel button should raise PopupClick once. Actual: {popupClickCount}.",
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

        okButton?.RaiseEvent(new RoutedEventArgs(AvaloniaButton.ClickEvent));
        cancelButton?.RaiseEvent(new RoutedEventArgs(AvaloniaButton.ClickEvent));

        Expect(confirmedCount == 1,
            $"PopupConfirm OK button should still work after detach and reattach. Actual: {confirmedCount}.",
            failures);
        Expect(cancelledCount == 1,
            $"PopupConfirm cancel button should still work after detach and reattach. Actual: {cancelledCount}.",
            failures);
    }
}
