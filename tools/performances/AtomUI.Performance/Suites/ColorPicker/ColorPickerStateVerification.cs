using System.Reflection;
using AtomUI.Desktop.Controls;

namespace AtomUI.Performance;

using AtomColorPicker = AtomUI.Desktop.Controls.ColorPicker;

internal static partial class Program
{
    private static bool RunColorPickerStateVerification()
    {
        var failures = new List<string>();
        VerifyColorPickerClickTrigger(failures);
        VerifyColorPickerHoverIgnoresClickHandler(failures);
        VerifyColorPickerFocusTrigger(failures);
        VerifyColorPickerTriggerSubscriptionLifecycle(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("ColorPicker state verification passed.");
            return true;
        }

        Console.Error.WriteLine("ColorPicker state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifyColorPickerClickTrigger(ICollection<string> failures)
    {
        var picker = CreateColorPicker();
        using var realized = RealizeControl(picker);

        Expect(!GetColorPickerIsOpen(picker),
            "Click ColorPicker should start closed.",
            failures);
        Expect(GetColorPickerTriggerSubscriptions(picker) == null,
            "Click ColorPicker should not create per-instance trigger subscriptions.",
            failures);

        Expect(RaisePointerPressedAttemptsColorPickerOpen(picker, realized.Window, failures),
            "Click ColorPicker should attempt to open on anchor pointer press.",
            failures);
    }

    private static void VerifyColorPickerHoverIgnoresClickHandler(ICollection<string> failures)
    {
        var picker = CreateColorPicker();
        picker.TriggerType = FlyoutTriggerType.Hover;

        using var realized = RealizeControl(picker);
        RaisePrimaryPointerPressed(picker, realized.Window);
        RefreshLayout(realized.Window);

        Expect(!GetColorPickerIsOpen(picker),
            "Hover ColorPicker should not be toggled by the click class handler.",
            failures);
    }

    private static void VerifyColorPickerFocusTrigger(ICollection<string> failures)
    {
        var picker = CreateColorPicker();
        picker.TriggerType = FlyoutTriggerType.Focus;

        using var realized = RealizeControl(picker);
        Expect(RaisePointerPressedAttemptsColorPickerOpen(picker, realized.Window, failures),
            "Focus ColorPicker should still open through its focus-mode pointer handler.",
            failures);
    }

    private static void VerifyColorPickerTriggerSubscriptionLifecycle(ICollection<string> failures)
    {
        var picker = CreateColorPicker();
        picker.TriggerType = FlyoutTriggerType.Hover;

        var realized = RealizeControl(picker);
        Expect(GetColorPickerTriggerSubscriptions(picker) != null,
            "Hover ColorPicker should create trigger subscriptions while attached.",
            failures);

        picker.TriggerType = FlyoutTriggerType.Click;
        RefreshLayout(realized.Window);
        Expect(GetColorPickerTriggerSubscriptions(picker) == null,
            "ColorPicker should release trigger subscriptions after switching to Click.",
            failures);

        picker.TriggerType = FlyoutTriggerType.Focus;
        RefreshLayout(realized.Window);
        Expect(GetColorPickerTriggerSubscriptions(picker) != null,
            "Focus ColorPicker should create trigger subscriptions while attached.",
            failures);

        realized.Dispose();
        Expect(GetColorPickerTriggerSubscriptions(picker) == null,
            "Detached ColorPicker should release trigger subscriptions.",
            failures);
    }

    private static bool GetColorPickerIsOpen(AbstractColorPicker picker)
        => GetColorPickerProperty<bool>(picker, "IsPickerOpen");

    private static object? GetColorPickerPresenter(AbstractColorPicker picker)
        => GetColorPickerProperty<object?>(picker, "PickerPresenter");

    private static object? GetColorPickerTriggerSubscriptions(AbstractColorPicker picker)
        => GetPrivateField(picker, "AtomUI.Desktop.Controls.AbstractColorPicker", "_triggerSubscriptions");

    private static bool RaisePointerPressedAttemptsColorPickerOpen(AtomColorPicker picker,
                                                                   Avalonia.Controls.Window window,
                                                                   ICollection<string> failures)
    {
        try
        {
            RaisePrimaryPointerPressed(picker, window);
            RefreshLayout(window);
            return GetColorPickerIsOpen(picker) || GetColorPickerPresenter(picker) != null;
        }
        catch (InvalidOperationException exception) when (IsMissingPopupHostException(exception))
        {
            return true;
        }
        catch (Exception exception)
        {
            failures.Add($"ColorPicker pointer press should not throw {exception.GetType().Name}: {exception.Message}");
            return false;
        }
    }

    private static T GetColorPickerProperty<T>(AbstractColorPicker picker, string propertyName)
    {
        var property = typeof(AbstractColorPicker).GetProperty(
            propertyName,
            BindingFlags.Instance | BindingFlags.NonPublic);
        return property is null ? default! : (T)property.GetValue(picker)!;
    }
}
