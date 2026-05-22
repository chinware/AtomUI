using System.Reflection;
using AtomUI.Desktop.Controls;
using Avalonia.Controls;

namespace AtomUI.Performance;

using AtomRangeTimePicker = AtomUI.Desktop.Controls.RangeTimePicker;
using AtomTimePicker = AtomUI.Desktop.Controls.TimePicker;

internal static partial class Program
{
    private static bool RunTimePickerStateVerification()
    {
        var failures = new List<string>();
        VerifyTimePickerLazyPresenter(failures);
        VerifyRangeTimePickerLazyPresenter(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("TimePicker state verification passed.");
            return true;
        }

        Console.Error.WriteLine("TimePicker state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifyTimePickerLazyPresenter(ICollection<string> failures)
    {
        var timePicker = new AtomTimePicker
        {
            Width           = 260,
            PlaceholderText = "Select time"
        };

        Control? firstPresenter;
        using (var realized = RealizeControl(timePicker))
        {
            Expect(timePicker.PickerPresenter == null,
                "Closed TimePicker should not create PickerPresenter before first open.",
                failures);
            Expect(FindVisualByTypeName(timePicker, "TimePickerPresenter") == null,
                "Closed TimePicker should not create TimePickerPresenter visuals.",
                failures);
            Expect(FindVisualByTypeName(timePicker, "IconPresenter") != null,
                "Closed TimePicker should still show the clock icon.",
                failures);

            MaterializePickerPresenterForTest(timePicker);
            RefreshLayout(realized.Window);
            firstPresenter = timePicker.PickerPresenter;
            Expect(firstPresenter?.GetType().Name == "TimePickerPresenter",
                "TimePicker presenter materialization should create TimePickerPresenter.",
                failures);

            MaterializePickerPresenterForTest(timePicker);
            RefreshLayout(realized.Window);
            Expect(ReferenceEquals(firstPresenter, timePicker.PickerPresenter),
                "Second TimePicker presenter materialization should reuse the first PickerPresenter.",
                failures);
        }

        Expect(timePicker.PickerPresenter == null,
            "Detached TimePicker should clear owned PickerPresenter.",
            failures);
    }

    private static void VerifyRangeTimePickerLazyPresenter(ICollection<string> failures)
    {
        var rangeTimePicker = new AtomRangeTimePicker
        {
            Width                    = 320,
            PlaceholderText          = "Start time",
            SecondaryPlaceholderText = "End time"
        };

        Control? firstPresenter;
        using (var realized = RealizeControl(rangeTimePicker))
        {
            Expect(rangeTimePicker.PickerPresenter == null,
                "Closed RangeTimePicker should not create PickerPresenter before first open.",
                failures);
            Expect(FindVisualByTypeName(rangeTimePicker, "TimePickerPresenter") == null,
                "Closed RangeTimePicker should not create TimePickerPresenter visuals.",
                failures);

            MaterializePickerPresenterForTest(rangeTimePicker);
            RefreshLayout(realized.Window);
            firstPresenter = rangeTimePicker.PickerPresenter;
            Expect(firstPresenter?.GetType().Name == "TimePickerPresenter",
                "RangeTimePicker presenter materialization should create TimePickerPresenter.",
                failures);

            MaterializePickerPresenterForTest(rangeTimePicker);
            RefreshLayout(realized.Window);
            Expect(ReferenceEquals(firstPresenter, rangeTimePicker.PickerPresenter),
                "Second RangeTimePicker presenter materialization should reuse the first PickerPresenter.",
                failures);
        }

        Expect(rangeTimePicker.PickerPresenter == null,
            "Detached RangeTimePicker should clear owned PickerPresenter.",
            failures);
    }

    private static void MaterializePickerPresenterForTest(Control picker)
    {
        var type = picker.GetType();
        while (type is not null)
        {
            if (type.FullName == "AtomUI.Desktop.Controls.Primitives.InfoPickerInput")
            {
                type.GetMethod("EnsurePickerPresenter", BindingFlags.Instance | BindingFlags.NonPublic)!
                    .Invoke(picker, null);
                return;
            }
            type = type.BaseType;
        }

        throw new InvalidOperationException("InfoPickerInput.EnsurePickerPresenter not found.");
    }
}
