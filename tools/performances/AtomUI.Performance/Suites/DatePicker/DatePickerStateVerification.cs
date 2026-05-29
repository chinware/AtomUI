using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Interactivity;
using Avalonia.Layout;
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
        VerifyDatePickerClearButtonLifecycle(failures);
        VerifyDatePickerAccessoryContentRefresh(failures);
        VerifyDatePickerPopupLifecycle(failures);
        VerifyRangeDatePickerPopupLifecycle(failures);
        VerifyRangeDatePickerIndicatorOffsetStaysStable(failures);
        VerifyInfoPickerRepeatedStateNotificationsAreStable(failures);
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
        var popupChild = GetInfoPickerPopupChild(datePicker);
        Expect(popupChild != null && popupChild.GetType().Name == "ArrowDecoratedBox",
            "Closed DatePicker should keep only the static lightweight ArrowDecoratedBox popup shell.",
            failures);
        Expect(popupChild is not ContentControl contentControl || contentControl.Content == null,
            "Closed DatePicker popup shell should not contain a PickerPresenter before first open.",
            failures);
        Expect(datePicker.PickerPresenter == null,
            "Closed DatePicker should not create PickerPresenter before first open.",
            failures);
        Expect(FindVisualByTypeName(datePicker, "DatePickerPresenter") == null,
            "Closed DatePicker should not create DatePickerPresenter visuals.",
            failures);
        Expect(FindVisualByName<Control>(datePicker, "PART_ClearButton") is { IsVisible: false },
            "Closed default DatePicker should keep the static clear button hidden.",
            failures);
        Expect(FindVisualByTypeName(datePicker, "IconPresenter") != null,
            "Closed default DatePicker should still show the calendar icon.",
            failures);
    }

    private static void VerifyDatePickerAccessoryLifecycle(ICollection<string> failures)
    {
        var datePicker = CreateDatePicker(selectedDateTime: new DateTime(2024, 1, 20));
        using var realized = RealizeControl(datePicker);

        var clearButton = FindVisualByName<Control>(datePicker, "PART_ClearButton");
        Expect(clearButton is { IsVisible: false },
            "Selected DatePicker should keep PART_ClearButton hidden until clear mode is visible.",
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

        Expect(clearButton is { IsVisible: true },
            "DatePicker clear mode should show the static InputClearIconButton.",
            failures);

        if (decoratedBox != null)
        {
            decoratedBox.IsInnerBoxHover = false;
            RefreshLayout(realized.Window);
        }

        Expect(clearButton is { IsVisible: false },
            "Leaving clear mode should hide the static InputClearIconButton.",
            failures);
        Expect(FindVisualByTypeName(datePicker, "IconPresenter") != null,
            "Leaving clear mode should restore the lightweight calendar icon presenter.",
            failures);
    }

    private static void VerifyDatePickerClearButtonLifecycle(ICollection<string> failures)
    {
        var selectedDate = new DateTime(2024, 1, 20);
        var datePicker = CreateDatePicker(selectedDateTime: selectedDate);
        using var realized = RealizeControl(datePicker);

        var decoratedBox = FindVisualByName<AddOnDecoratedBox>(
            datePicker,
            AddOnDecoratedBox.AddOnDecoratedBoxPart);
        var clearButton = FindVisualByName<Control>(datePicker, "PART_ClearButton");

        Expect(clearButton != null,
            "DatePicker should realize PickerClearUpButton PART_ClearButton.",
            failures);
        if (clearButton == null)
        {
            return;
        }

        Expect(!GetLocalRoutedHandlerNames(clearButton).Any(name => name.Contains("Click")),
            "DatePicker PickerClearUpButton should use class handler instead of a local Click handler.",
            failures);

        clearButton.RaiseEvent(new RoutedEventArgs(Avalonia.Controls.Button.ClickEvent)
        {
            Source = clearButton
        });
        Expect(datePicker.SelectedDateTime == selectedDate,
            "Hidden DatePicker clear button should be inert when clear mode is not active.",
            failures);

        if (decoratedBox != null)
        {
            decoratedBox.IsInnerBoxHover = true;
            RefreshLayout(realized.Window);
        }

        Expect(clearButton is { IsVisible: true },
            "DatePicker clear mode should show PART_ClearButton.",
            failures);

        clearButton.RaiseEvent(new RoutedEventArgs(Avalonia.Controls.Button.ClickEvent)
        {
            Source = clearButton
        });
        Expect(datePicker.SelectedDateTime == null,
            "Visible DatePicker clear button should still clear SelectedDateTime.",
            failures);
    }

    private static void VerifyDatePickerAccessoryContentRefresh(ICollection<string> failures)
    {
        var datePicker = CreateDatePicker();
        using var realized = RealizeControl(datePicker);

        datePicker.ContentRightAddOn = "first";
        RefreshLayout(realized.Window);
        var contentPresenter = FindVisualByName<ContentPresenter>(datePicker, "PART_ContentRightAddOnPresenter");
        Expect(contentPresenter != null,
            "DatePicker should keep the static ContentRightAddOn presenter.",
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
            firstPopupContent = GetInfoPickerPopupChild(datePicker);
            Expect(firstPopupContent != null && firstPopupContent.GetType().Name == "ArrowDecoratedBox",
                "Closed DatePicker should keep the static ArrowDecoratedBox popup shell before presenter materialization.",
                failures);
            Expect(firstPopupContent is not ContentControl closedContent || closedContent.Content == null,
                "Closed DatePicker popup shell should not contain presenter content before materialization.",
                failures);

            MaterializeInfoPickerPresenterForTest(datePicker);
            RefreshLayout(realized.Window);
            firstPickerPresenter = datePicker.PickerPresenter;

            Expect(ReferenceEquals(firstPopupContent, GetInfoPickerPopupChild(datePicker)),
                "DatePicker popup materialization should reuse the static ArrowDecoratedBox shell.",
                failures);
            Expect(firstPickerPresenter != null && firstPickerPresenter.GetType().Name == "DatePickerPresenter",
                "DatePicker popup materialization should create DatePickerPresenter.",
                failures);
            Expect(firstPopupContent is not ContentControl materializedContent ||
                   ReferenceEquals(materializedContent.Content, firstPickerPresenter),
                "DatePicker popup shell should host the materialized DatePickerPresenter.",
                failures);

            MaterializeInfoPickerPresenterForTest(datePicker);
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
        Expect(firstPopupContent?.GetVisualParent() == null,
            "Detached DatePicker popup shell should leave the visual tree.",
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
            MaterializeInfoPickerPresenterForTest(rangeDatePicker);
            RefreshLayout(realized.Window);
            firstPopupContent = GetInfoPickerPopupChild(rangeDatePicker);

            Expect(firstPopupContent != null && firstPopupContent.GetType().Name == "DualMonthArrowDecoratedBox",
                "RangeDatePicker should keep the static DualMonthArrowDecoratedBox popup shell.",
            failures);
            Expect(rangeDatePicker.PickerPresenter != null &&
                   rangeDatePicker.PickerPresenter.GetType().Name is "DualMonthRangeDatePickerPresenter",
                "RangeDatePicker popup materialization should create the range presenter.",
                failures);
            Expect(firstPopupContent is not ContentControl contentControl ||
                   ReferenceEquals(contentControl.Content, rangeDatePicker.PickerPresenter),
                "RangeDatePicker popup shell should host the materialized range presenter.",
                failures);
        }

        Expect(rangeDatePicker.PickerPresenter == null,
            "Detached RangeDatePicker should clear lazy PickerPresenter.",
            failures);
        Expect(GetInfoPickerPopupContentField(rangeDatePicker) == null,
            "Detached RangeDatePicker should clear lazy popup content field.",
            failures);
        Expect(firstPopupContent?.GetVisualParent() == null,
            "Detached RangeDatePicker popup shell should leave the visual tree.",
            failures);
    }

    private static void VerifyRangeDatePickerIndicatorOffsetStaysStable(ICollection<string> failures)
    {
        var rangeDatePicker = CreateRangeDatePicker();
        using var realized = RealizeControl(rangeDatePicker);

        var indicator = FindVisualByName<Control>(rangeDatePicker, "PART_RangePickerIndicator");
        Expect(indicator != null,
            "RangeDatePicker should realize PART_RangePickerIndicator.",
            failures);
        if (indicator == null)
        {
            return;
        }

        RefreshLayout(realized.Window);
        var firstLeft = Canvas.GetLeft(indicator);
        var firstTop  = Canvas.GetTop(indicator);
        RefreshLayout(realized.Window);

        Expect(Canvas.GetLeft(indicator).Equals(firstLeft),
            "RangeDatePicker repeated layout should keep indicator Canvas.Left stable.",
            failures);
        Expect(Canvas.GetTop(indicator).Equals(firstTop),
            "RangeDatePicker repeated layout should keep indicator Canvas.Top stable.",
            failures);

        SetRangeActivatedPartForTest(rangeDatePicker, "Start");
        InvokePrivateMethod(
            rangeDatePicker,
            "AtomUI.Desktop.Controls.Primitives.RangeInfoPickerInput",
            "SetupPickerIndicatorPosition");
        var startWidth = indicator.Width;
        var startLeft  = Canvas.GetLeft(indicator);
        InvokePrivateMethod(
            rangeDatePicker,
            "AtomUI.Desktop.Controls.Primitives.RangeInfoPickerInput",
            "SetupPickerIndicatorPosition");
        Expect(indicator.Width.Equals(startWidth),
            "RangeDatePicker repeated start indicator setup should keep Width stable.",
            failures);
        Expect(Canvas.GetLeft(indicator).Equals(startLeft),
            "RangeDatePicker repeated start indicator setup should keep Canvas.Left stable.",
            failures);

        SetRangeActivatedPartForTest(rangeDatePicker, "End");
        InvokePrivateMethod(
            rangeDatePicker,
            "AtomUI.Desktop.Controls.Primitives.RangeInfoPickerInput",
            "SetupPickerIndicatorPosition");
        var endWidth = indicator.Width;
        var endLeft  = Canvas.GetLeft(indicator);
        InvokePrivateMethod(
            rangeDatePicker,
            "AtomUI.Desktop.Controls.Primitives.RangeInfoPickerInput",
            "SetupPickerIndicatorPosition");
        Expect(indicator.Width.Equals(endWidth),
            "RangeDatePicker repeated end indicator setup should keep Width stable.",
            failures);
        Expect(Canvas.GetLeft(indicator).Equals(endLeft),
            "RangeDatePicker repeated end indicator setup should keep Canvas.Left stable.",
            failures);
    }

    private static void VerifyDatePickerWindowSubscriptionLifecycle(ICollection<string> failures)
    {
        var datePicker = CreateDatePicker();
        using var realized = RealizeControl(datePicker);

        Expect(GetPrivateField(datePicker, "AtomUI.Desktop.Controls.Primitives.InfoPickerInput", "_attachedWindow") == null,
            "Closed DatePicker should not subscribe to Window.Deactivated.",
            failures);

        SetPrivateField(
            datePicker,
            "AtomUI.Desktop.Controls.Primitives.InfoPickerInput",
            "_attachedWindow",
            new AtomUI.Desktop.Controls.Window());
        if (realized.Window.Content is StackPanel host)
        {
            host.Children.Remove(datePicker);
            RefreshLayout(realized.Window);
        }
        Expect(GetPrivateField(datePicker, "AtomUI.Desktop.Controls.Primitives.InfoPickerInput", "_attachedWindow") == null,
            "DatePicker should clear Window.Deactivated subscription state when detached.",
            failures);
    }

    private static void VerifyInfoPickerRepeatedStateNotificationsAreStable(ICollection<string> failures)
    {
        var datePicker = CreateDatePicker();
        using var realized = RealizeControl(datePicker);

        var changedProperties = new List<string>();
        datePicker.PropertyChanged += (_, e) =>
        {
            if (e.Property.Name is "IsUsedInCompactSpace" or
                                   "CompactSpaceItemPosition" or
                                   "CompactSpaceOrientation" or
                                   "FormFeedback")
            {
                changedProperties.Add(e.Property.Name);
            }
        };

        var firstLastPosition = CreateSpaceItemPosition("First", "Last");
        NotifyCompactSpacePosition(datePicker, firstLastPosition);
        NotifyCompactSpaceOrientation(datePicker, Orientation.Vertical);
        var feedback = new FormValidateFeedback();
        InvokePrivateMethod(
            datePicker,
            "AtomUI.Desktop.Controls.Primitives.InfoPickerInput",
            "NotifySetFeedBackControl",
            feedback);

        changedProperties.Clear();
        NotifyCompactSpacePosition(datePicker, firstLastPosition);
        NotifyCompactSpaceOrientation(datePicker, Orientation.Vertical);
        InvokePrivateMethod(
            datePicker,
            "AtomUI.Desktop.Controls.Primitives.InfoPickerInput",
            "NotifySetFeedBackControl",
            feedback);

        Expect(changedProperties.Count == 0,
            $"DatePicker repeated compact-space/form-feedback notifications should not rewrite state. Actual: {string.Join(", ", changedProperties)}.",
            failures);
    }

    private static void MaterializeInfoPickerPresenterForTest(Control picker)
    {
        InvokePrivateMethod(picker,
            "AtomUI.Desktop.Controls.Primitives.InfoPickerInput",
            "EnsurePickerPresenter");
    }

    private static void SetRangeActivatedPartForTest(Control picker, string value)
    {
        var enumType = typeof(AtomUI.Desktop.Controls.DatePicker)
            .Assembly
            .GetType("AtomUI.Desktop.Controls.Primitives.RangeActivatedPart");
        if (enumType == null)
        {
            return;
        }

        SetPrivateField(
            picker,
            "AtomUI.Desktop.Controls.Primitives.RangeInfoPickerInput",
            "_rangeActivatedPart",
            Enum.Parse(enumType, value));
    }

    private static object CreateSpaceItemPosition(params string[] names)
    {
        var enumType = typeof(AtomUI.Desktop.Controls.DatePicker)
            .Assembly
            .GetType("AtomUI.Desktop.Controls.SpaceItemPosition");
        if (enumType == null)
        {
            throw new InvalidOperationException("SpaceItemPosition type should exist.");
        }

        var value = 0;
        foreach (var name in names)
        {
            value |= (int)Enum.Parse(enumType, name);
        }
        return Enum.ToObject(enumType, value);
    }

    private static void NotifyCompactSpacePosition(Control target, object? position)
    {
        InvokeCompactSpaceAwareMethod(target, "NotifyPositionChange", position);
    }

    private static void NotifyCompactSpaceOrientation(Control target, Orientation orientation)
    {
        InvokeCompactSpaceAwareMethod(target, "NotifyOrientationChange", orientation);
    }

    private static void InvokeCompactSpaceAwareMethod(Control target, string methodName, params object?[] args)
    {
        var interfaceType = typeof(AtomUI.Desktop.Controls.DatePicker)
            .Assembly
            .GetType("AtomUI.Desktop.Controls.ICompactSpaceAware");
        var method = interfaceType?.GetMethod(methodName);
        method?.Invoke(target, args);
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
