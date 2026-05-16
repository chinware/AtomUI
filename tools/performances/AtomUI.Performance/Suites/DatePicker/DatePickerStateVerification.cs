using AtomUI.Desktop.Controls;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.VisualTree;

namespace AtomUI.Performance;

using AvaloniaPopup = Avalonia.Controls.Primitives.Popup;

internal static partial class Program
{
    private static bool RunDatePickerStateVerification()
    {
        var failures = new List<string>();
        VerifyClosedDatePickerCost(failures);
        VerifyDatePickerAccessoryLifecycle(failures);
        VerifyDatePickerAccessoryContentRefresh(failures);
        VerifyDatePickerPopupLifecycle(failures);
        VerifyRangeDatePickerPopupLifecycle(failures);
        VerifyDatePickerWindowSubscriptionLifecycle(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("DatePicker state verification passed.");
            return true;
        }

        Console.Error.WriteLine("DatePicker state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifyClosedDatePickerCost(ICollection<string> failures)
    {
        var datePicker = CreateDatePicker();
        using var realized = RealizeControl(datePicker);

        Expect(FindVisualByName<AvaloniaPopup>(datePicker, "PART_Popup") != null,
            "Closed DatePicker should keep the lightweight Popup shell.",
            failures);
        Expect(GetInfoPickerPopupChild(datePicker) == null,
            "Closed DatePicker should keep Popup child empty before first open.",
            failures);
        Expect(datePicker.PickerPresenter == null,
            "Closed DatePicker should not create PickerPresenter before first open.",
            failures);
        Expect(FindVisualByTypeName(datePicker, "DatePickerPresenter") == null,
            "Closed DatePicker should not create DatePickerPresenter visuals.",
            failures);
        Expect(FindVisualByTypeName(datePicker, "PickerAccessoryHost") == null,
            "Closed default DatePicker should use a lightweight icon presenter instead of PickerAccessoryHost.",
            failures);
        Expect(FindVisualByTypeName(datePicker, "IconPresenter") != null,
            "Closed default DatePicker should still show the calendar icon.",
            failures);
    }

    private static void VerifyDatePickerAccessoryLifecycle(ICollection<string> failures)
    {
        var datePicker = CreateDatePicker(selectedDateTime: new DateTime(2024, 1, 20));
        using var realized = RealizeControl(datePicker);

        Expect(FindVisualByTypeName(datePicker, "PickerAccessoryHost") == null,
            "Selected DatePicker should not create PickerAccessoryHost until clear mode is visible.",
            failures);

        var decoratedBox = FindVisualByName<AddOnDecoratedBox>(
            datePicker,
            AddOnDecoratedBox.AddOnDecoratedBoxPart);
        Expect(decoratedBox != null,
            "DatePicker should materialize AddOnDecoratedBox.",
            failures);

        if (decoratedBox != null)
        {
            decoratedBox.IsInnerBoxHover = true;
            RefreshLayout(realized.Window);
        }

        var accessoryHost = FindVisualByTypeName(datePicker, "PickerAccessoryHost");
        Expect(accessoryHost != null,
            "DatePicker clear mode should create PickerAccessoryHost.",
            failures);
        Expect(FindVisualByTypeName(datePicker, "InputClearIconButton") != null,
            "DatePicker clear mode should create InputClearIconButton.",
            failures);

        if (decoratedBox != null)
        {
            decoratedBox.IsInnerBoxHover = false;
            RefreshLayout(realized.Window);
        }

        Expect(FindVisualByTypeName(datePicker, "PickerAccessoryHost") == null,
            "Leaving clear mode should detach PickerAccessoryHost.",
            failures);
        Expect(accessoryHost?.GetVisualParent() == null,
            "Detached PickerAccessoryHost should not keep a visual parent.",
            failures);
        Expect(FindVisualByTypeName(datePicker, "IconPresenter") != null,
            "Leaving clear mode should restore the lightweight calendar icon presenter.",
            failures);
    }

    private static void VerifyDatePickerAccessoryContentRefresh(ICollection<string> failures)
    {
        var datePicker = CreateDatePicker();
        using var realized = RealizeControl(datePicker);

        datePicker.ContentRightAddOn = "first";
        RefreshLayout(realized.Window);
        var accessoryHost = FindVisualByTypeName(datePicker, "PickerAccessoryHost");
        var contentPresenter = FindVisualByName<ContentPresenter>(datePicker, "PART_ContentRightAddOnPresenter");
        Expect(accessoryHost != null,
            "DatePicker ContentRightAddOn should create PickerAccessoryHost.",
            failures);
        Expect(Equals(contentPresenter?.Content, "first"),
            $"DatePicker ContentRightAddOn presenter should show first content, actual '{contentPresenter?.Content ?? "<null>"}'.",
            failures);

        datePicker.ContentRightAddOn = "second";
        RefreshLayout(realized.Window);
        Expect(Equals(contentPresenter?.Content, "second"),
            $"Existing DatePicker accessory host should refresh ContentRightAddOn, actual '{contentPresenter?.Content ?? "<null>"}'.",
            failures);

        datePicker.ContentRightAddOn = null;
        RefreshLayout(realized.Window);
        Expect(FindVisualByTypeName(datePicker, "PickerAccessoryHost") == null,
            "Clearing DatePicker ContentRightAddOn should remove PickerAccessoryHost.",
            failures);
        Expect(accessoryHost?.GetVisualParent() == null,
            "Removed DatePicker ContentRightAddOn host should not keep a visual parent.",
            failures);
        Expect(contentPresenter?.Content == null,
            "Removed DatePicker ContentRightAddOn presenter should clear Content.",
            failures);
    }

    private static void VerifyDatePickerPopupLifecycle(ICollection<string> failures)
    {
        var datePicker = CreateDatePicker();
        Control? firstPopupContent;
        Control? firstPickerPresenter;
        using (var realized = RealizeControl(datePicker))
        {
            Expect(GetInfoPickerPopupChild(datePicker) == null,
                "Closed DatePicker should not create popup content before first materialization.",
                failures);

            MaterializeInfoPickerPopupContentForTest(datePicker);
            RefreshLayout(realized.Window);
            firstPopupContent = GetInfoPickerPopupChild(datePicker);
            firstPickerPresenter = datePicker.PickerPresenter;

            Expect(firstPopupContent != null && firstPopupContent.GetType().Name == "ArrowDecoratedBox",
                "DatePicker popup materialization should create ArrowDecoratedBox.",
                failures);
            Expect(firstPickerPresenter != null && firstPickerPresenter.GetType().Name == "DatePickerPresenter",
                "DatePicker popup materialization should create DatePickerPresenter.",
                failures);

            MaterializeInfoPickerPopupContentForTest(datePicker);
            RefreshLayout(realized.Window);
            Expect(ReferenceEquals(firstPopupContent, GetInfoPickerPopupChild(datePicker)),
                "Second DatePicker popup materialization should reuse the first popup content.",
                failures);
            Expect(ReferenceEquals(firstPickerPresenter, datePicker.PickerPresenter),
                "Second DatePicker popup materialization should reuse the first PickerPresenter.",
                failures);
        }

        Expect(datePicker.PickerPresenter == null,
            "Detached DatePicker should clear lazy PickerPresenter.",
            failures);
        Expect(GetInfoPickerPopupContentField(datePicker) == null,
            "Detached DatePicker should clear lazy popup content field.",
            failures);
        Expect(firstPopupContent?.TemplatedParent == null,
            "Detached DatePicker popup content should clear TemplatedParent.",
            failures);
        Expect(firstPopupContent is not ContentControl contentControl || contentControl.Content == null,
            "Detached DatePicker popup content should clear Content.",
            failures);
    }

    private static void VerifyRangeDatePickerPopupLifecycle(ICollection<string> failures)
    {
        var rangeDatePicker = CreateRangeDatePicker();
        Control? firstPopupContent;
        using (var realized = RealizeControl(rangeDatePicker))
        {
            MaterializeInfoPickerPopupContentForTest(rangeDatePicker);
            RefreshLayout(realized.Window);
            firstPopupContent = GetInfoPickerPopupChild(rangeDatePicker);

            Expect(firstPopupContent != null && firstPopupContent.GetType().Name == "DualMonthArrowDecoratedBox",
                "RangeDatePicker popup materialization should create DualMonthArrowDecoratedBox.",
                failures);
            Expect(rangeDatePicker.PickerPresenter != null &&
                   rangeDatePicker.PickerPresenter.GetType().Name is "DualMonthRangeDatePickerPresenter",
                "RangeDatePicker popup materialization should create the range presenter.",
                failures);
        }

        Expect(rangeDatePicker.PickerPresenter == null,
            "Detached RangeDatePicker should clear lazy PickerPresenter.",
            failures);
        Expect(GetInfoPickerPopupContentField(rangeDatePicker) == null,
            "Detached RangeDatePicker should clear lazy popup content field.",
            failures);
        Expect(firstPopupContent?.TemplatedParent == null,
            "Detached RangeDatePicker popup content should clear TemplatedParent.",
            failures);
    }

    private static void VerifyDatePickerWindowSubscriptionLifecycle(ICollection<string> failures)
    {
        var datePicker = CreateDatePicker();
        using var realized = RealizeControl(datePicker);

        Expect(GetPrivateField(datePicker, "AtomUI.Desktop.Controls.Primitives.InfoPickerInput", "_attachedWindow") == null,
            "Closed DatePicker should not subscribe to Window.Deactivated.",
            failures);

        SetPrivateField(datePicker, "AtomUI.Desktop.Controls.Primitives.InfoPickerInput", "_attachedWindow", realized.Window);
        InvokePrivateMethod(datePicker,
            "AtomUI.Desktop.Controls.Primitives.InfoPickerInput",
            "ClearWindowDeactivatedSubscription");
        Expect(GetPrivateField(datePicker, "AtomUI.Desktop.Controls.Primitives.InfoPickerInput", "_attachedWindow") == null,
            "DatePicker should clear Window.Deactivated subscription state when closed or detached.",
            failures);
    }

    private static void MaterializeInfoPickerPopupContentForTest(Control picker)
    {
        InvokePrivateMethod(picker,
            "AtomUI.Desktop.Controls.Primitives.InfoPickerInput",
            "EnsurePickerPopupContent");
    }

    private static Control? GetInfoPickerPopupChild(Control picker)
    {
        return FindVisualByName<AvaloniaPopup>(picker, "PART_Popup")?.Child as Control;
    }

    private static object? GetInfoPickerPopupContentField(Control picker)
    {
        return GetPrivateField(picker, "AtomUI.Desktop.Controls.Primitives.InfoPickerInput", "_pickerPopupContent");
    }
}
