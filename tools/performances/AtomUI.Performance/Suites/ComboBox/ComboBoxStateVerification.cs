using System.Reflection;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.VisualTree;

namespace AtomUI.Performance;

using AtomComboBox = AtomUI.Desktop.Controls.ComboBox;

internal static partial class Program
{
    private static bool RunComboBoxStateVerification()
    {
        var failures = new List<string>();
        VerifyClosedComboBoxCost(failures);
        VerifyComboBoxAccessoryLifecycle(failures);
        VerifyComboBoxPopupLifecycle(failures);
        VerifyComboBoxWindowSubscriptionLifecycle(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("ComboBox state verification passed.");
            return true;
        }

        Console.Error.WriteLine("ComboBox state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifyClosedComboBoxCost(ICollection<string> failures)
    {
        var comboBox = CreateComboBoxWithItems(4);
        using var realized = RealizeControl(comboBox);

        Expect(FindVisualByName<Popup>(comboBox, "PART_Popup") != null,
            "Closed ComboBox should keep the lightweight Popup shell.",
            failures);
        Expect(GetPopupFrame(comboBox) == null,
            "Closed default ComboBox should keep Popup child empty before first open.",
            failures);
        Expect(FindVisualByName<ItemsPresenter>(comboBox, "PART_ItemsPresenter") == null,
            "Closed default ComboBox should not create dropdown ItemsPresenter.",
            failures);
        Expect(FindVisualByTypeName(comboBox, "ComboBoxAccessoryHost") == null,
            "Closed default ComboBox should use lightweight ComboBoxHandle instead of ComboBoxAccessoryHost.",
            failures);
        Expect(FindVisualByTypeName(comboBox, "ComboBoxHandle") != null,
            "Closed default ComboBox should still show a lightweight ComboBoxHandle.",
            failures);
    }

    private static void VerifyComboBoxAccessoryLifecycle(ICollection<string> failures)
    {
        var comboBox = CreateComboBoxWithItems(4);
        using var realized = RealizeControl(comboBox);
        Expect(FindVisualByTypeName(comboBox, "ComboBoxAccessoryHost") == null,
            "Default ComboBox should not create ComboBoxAccessoryHost.",
            failures);

        comboBox.SetCurrentValue(AtomComboBox.ContentRightAddOnProperty, "ms");
        RefreshLayout(realized.Window);
        var accessoryHost = FindVisualByTypeName(comboBox, "ComboBoxAccessoryHost");
        Expect(accessoryHost != null,
            "ComboBox with ContentRightAddOn should create ComboBoxAccessoryHost.",
            failures);
        Expect(FindVisualByName<ContentPresenter>(comboBox, "PART_ContentRightAddOnPresenter") != null,
            "ComboBoxAccessoryHost should create a content-right presenter when ContentRightAddOn is present.",
            failures);

        comboBox.SetCurrentValue(AtomComboBox.ContentRightAddOnProperty, null);
        RefreshLayout(realized.Window);
        Expect(FindVisualByTypeName(comboBox, "ComboBoxAccessoryHost") == null,
            "Removing ContentRightAddOn should detach ComboBoxAccessoryHost.",
            failures);
        Expect(accessoryHost?.GetVisualParent() == null,
            "Detached ComboBoxAccessoryHost should not keep a visual parent.",
            failures);
        Expect(FindVisualByTypeName(comboBox, "ComboBoxHandle") != null,
            "Removing ContentRightAddOn should restore lightweight ComboBoxHandle.",
            failures);
    }

    private static void VerifyComboBoxPopupLifecycle(ICollection<string> failures)
    {
        var comboBox = CreateComboBoxWithItems(4);
        Border? firstPopupFrame;
        ItemsPresenter? firstItemsPresenter;
        using (var realized = RealizeControl(comboBox))
        {
            Expect(GetPopupFrame(comboBox) == null,
                "Closed ComboBox should not create PopupFrame before first open.",
                failures);

            MaterializeLazyComboBoxPopupContentForTest(comboBox);
            RefreshLayout(realized.Window);
            firstPopupFrame = GetPopupFrame(comboBox);
            firstItemsPresenter = (firstPopupFrame?.Child as Avalonia.Controls.ScrollViewer)?.Content as ItemsPresenter;
            Expect(firstPopupFrame != null,
                "Opening ComboBox should lazily create PopupFrame.",
                failures);
            Expect(firstItemsPresenter != null,
                "Opening ComboBox should lazily create dropdown ItemsPresenter.",
                failures);

            Expect(ReferenceEquals(firstPopupFrame, GetPopupFrame(comboBox)),
                "ComboBox should keep lazy popup content after materialization.",
                failures);

            MaterializeLazyComboBoxPopupContentForTest(comboBox);
            RefreshLayout(realized.Window);
            var currentItemsPresenter = (firstPopupFrame?.Child as Avalonia.Controls.ScrollViewer)?.Content as ItemsPresenter;
            Expect(ReferenceEquals(firstItemsPresenter, currentItemsPresenter),
                "Second ComboBox popup materialization should reuse the first ItemsPresenter.",
                failures);
        }

        Expect(firstPopupFrame?.GetVisualParent() == null,
            "Detached ComboBox should clear lazy PopupFrame visual parent.",
            failures);
        Expect(firstItemsPresenter?.GetVisualParent() == null,
            "Detached ComboBox should clear lazy ItemsPresenter visual parent.",
            failures);
    }

    private static void VerifyComboBoxWindowSubscriptionLifecycle(ICollection<string> failures)
    {
        var comboBox = CreateComboBoxWithItems(4);
        using var realized = RealizeControl(comboBox);

        Expect(GetPrivateField(comboBox, "AtomUI.Desktop.Controls.ComboBox", "_attachedWindow") == null,
            "Closed ComboBox should not subscribe to Window.Deactivated.",
            failures);

        SetPrivateField(comboBox, "AtomUI.Desktop.Controls.ComboBox", "_attachedWindow", realized.Window);
        InvokePrivateMethod(comboBox, "AtomUI.Desktop.Controls.ComboBox", "ClearWindowDeactivatedSubscription");
        Expect(GetPrivateField(comboBox, "AtomUI.Desktop.Controls.ComboBox", "_attachedWindow") == null,
            "ComboBox should clear Window.Deactivated subscription state when closed or detached.",
            failures);
    }

    private static void InvokePrivateMethod(object target, string declaringTypeName, string methodName)
    {
        var type = target.GetType();
        while (type is not null)
        {
            if (type.FullName == declaringTypeName)
            {
                type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(target, null);
                return;
            }

            type = type.BaseType;
        }
    }
}
