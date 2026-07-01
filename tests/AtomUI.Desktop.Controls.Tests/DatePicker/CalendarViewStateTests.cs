using System;
using System.Collections.Generic;
using System.Linq;
using AtomUI.Controls;
using AtomUI.Desktop.Controls.CalendarView;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using Shouldly;
using Xunit;
using AvaloniaGrid = Avalonia.Controls.Grid;
using AvaloniaWindow = Avalonia.Controls.Window;
using PickerCalendar = AtomUI.Desktop.Controls.CalendarView.Calendar;
using PickerCalendarButton = AtomUI.Desktop.Controls.CalendarView.CalendarButton;
using PickerCalendarItem = AtomUI.Desktop.Controls.CalendarView.CalendarItem;
using PickerCalendarDayButton = AtomUI.Desktop.Controls.CalendarView.CalendarDayButton;
using PickerDualMonthCalendarItem = AtomUI.Desktop.Controls.CalendarView.DualMonthCalendarItem;

namespace AtomUI.Desktop.Controls.Tests.DatePickers;

public class CalendarViewStateTests
{
    static CalendarViewStateTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void CalendarItem_Declares_TemplateParts_Used_By_Default_Themes()
    {
        RunOnUIThread(() =>
        {
            var expectedParts = new Dictionary<string, Type>
            {
                ["PART_HeaderButton"]        = typeof(HeadTextButton),
                ["PART_HeaderLayout"]        = typeof(Panel),
                ["PART_MonthViewLayout"]     = typeof(UniformGrid),
                ["PART_MonthView"]           = typeof(AvaloniaGrid),
                ["PART_PreviousButton"]      = typeof(IconButton),
                ["PART_PreviousMonthButton"] = typeof(IconButton),
                ["PART_NextButton"]          = typeof(IconButton),
                ["PART_NextMonthButton"]     = typeof(IconButton),
                ["PART_YearView"]            = typeof(AvaloniaGrid)
            };

            AssertTemplateParts(typeof(PickerCalendarItem), expectedParts);
        });
    }

    [Fact]
    public void DualMonthCalendarItem_Declares_Secondary_TemplateParts()
    {
        RunOnUIThread(() =>
        {
            var expectedParts = new Dictionary<string, Type>
            {
                ["PART_SecondaryMonthView"]           = typeof(AvaloniaGrid),
                ["PART_SecondaryHeaderButton"]        = typeof(HeadTextButton),
                ["PART_SecondaryPreviousButton"]      = typeof(IconButton),
                ["PART_SecondaryPreviousMonthButton"] = typeof(IconButton),
                ["PART_SecondaryNextButton"]          = typeof(IconButton),
                ["PART_SecondaryNextMonthButton"]     = typeof(IconButton)
            };

            AssertTemplateParts(typeof(PickerDualMonthCalendarItem), expectedParts);
        });
    }

    [Fact]
    public void CalendarItem_ClearGeneratedMonthView_Releases_Owner_FocusButton()
    {
        RunOnUIThread(() =>
        {
            var calendar  = new PickerCalendar();
            var item      = new TestCalendarItem();
            var monthView = new AvaloniaGrid();
            var dayButton = new PickerCalendarDayButton
            {
                Owner     = calendar,
                IsCurrent = true
            };
            calendar.FocusButton = dayButton;
            monthView.Children.Add(dayButton);

            item.ClearGeneratedMonthViewForTest(monthView);

            calendar.FocusButton.ShouldBeNull();
            dayButton.Owner.ShouldBeNull();
            dayButton.IsCurrent.ShouldBeFalse();
            monthView.Children.Count.ShouldBe(0);
        });
    }

    [Fact]
    public void CalendarItem_UpdateYearViewSelection_Tolerates_Missing_Previous_FocusButton()
    {
        RunOnUIThread(() =>
        {
            var selectedMonth = new DateTime(2026, 8, 1);
            var calendar = new PickerCalendar
            {
                DisplayMode = CalendarMode.Year
            };
            var item = new TestCalendarItem
            {
                Owner = calendar
            };
            var monthButton = new PickerCalendarButton
            {
                DataContext = selectedMonth
            };

            item.UpdateYearViewSelection(monthButton);

            calendar.FocusCalendarButton.ShouldBe(monthButton);
            calendar.SelectedMonth.ShouldBe(selectedMonth);
        });
    }

    [Fact]
    public void Calendar_SelectedDate_Change_Refreshes_Selected_Day_Button()
    {
        RunOnUIThread(() =>
        {
            var calendar = new PickerCalendar
            {
                DisplayDate  = new DateTime(2026, 6, 1),
                SelectedDate = new DateTime(2026, 6, 10)
            };

            ShowInWindow(calendar, () =>
            {
                IsSelected(FindDayButton(calendar, new DateTime(2026, 6, 10))).ShouldBeTrue();

                calendar.SelectedDate = new DateTime(2026, 6, 12);
                Dispatcher.UIThread.RunJobs();

                IsSelected(FindDayButton(calendar, new DateTime(2026, 6, 10))).ShouldBeFalse();
                IsSelected(FindDayButton(calendar, new DateTime(2026, 6, 12))).ShouldBeTrue();
            });
        });
    }

    [Fact]
    public void RangeCalendar_SecondarySelectedDate_Change_Refreshes_Range_Highlight()
    {
        RunOnUIThread(() =>
        {
            var calendar = new RangeCalendar
            {
                DisplayDate           = new DateTime(2026, 6, 1),
                SelectedDate          = new DateTime(2026, 6, 10),
                SecondarySelectedDate = new DateTime(2026, 6, 15)
            };

            ShowInWindow(calendar, () =>
            {
                IsRangeEnd(FindDayButton(calendar, new DateTime(2026, 6, 15))).ShouldBeTrue();

                calendar.SecondarySelectedDate = new DateTime(2026, 6, 18);
                Dispatcher.UIThread.RunJobs();

                IsSelected(FindDayButton(calendar, new DateTime(2026, 6, 15))).ShouldBeTrue();
                IsRangeEnd(FindDayButton(calendar, new DateTime(2026, 6, 15))).ShouldBeFalse();
                IsSelected(FindDayButton(calendar, new DateTime(2026, 6, 18))).ShouldBeTrue();
                IsRangeEnd(FindDayButton(calendar, new DateTime(2026, 6, 18))).ShouldBeTrue();
            });
        });
    }

    private static bool IsSelected(PickerCalendarDayButton button)
    {
        return button.Classes.Contains(":selected");
    }

    private static bool IsRangeEnd(PickerCalendarDayButton button)
    {
        return button.Classes.Contains(":range-end");
    }

    private static void AssertTemplateParts(Type controlType, IReadOnlyDictionary<string, Type> expectedParts)
    {
        var parts = controlType.GetCustomAttributes(typeof(TemplatePartAttribute), false)
                               .Cast<TemplatePartAttribute>()
                               .ToDictionary(part => part.Name, part => part.Type);

        foreach (var (name, expectedType) in expectedParts)
        {
            parts.TryGetValue(name, out var actualType).ShouldBeTrue(name);
            actualType.ShouldBe(expectedType);
        }
    }

    private static PickerCalendarDayButton FindDayButton(PickerCalendar calendar, DateTime date)
    {
        var buttons = calendar.CalendarItem.ShouldNotBeNull()
                              .MonthView.ShouldNotBeNull()
                              .Children
                              .OfType<PickerCalendarDayButton>()
                              .ToArray();
        var matchedButton = buttons.SingleOrDefault(button =>
            button.DataContext is DateTime buttonDate &&
            DateTimeHelper.CompareDays(buttonDate, date) == 0);
        if (matchedButton is not null)
        {
            return matchedButton;
        }

        var actualDates = string.Join(
            ", ",
            buttons.Select(button => button.DataContext)
                   .OfType<DateTime>()
                   .Select(buttonDate => buttonDate.ToString("yyyy-MM-dd")));
        var monthView = calendar.CalendarItem.ShouldNotBeNull().MonthView.ShouldNotBeNull();
        var childTypes = string.Join(
            ", ",
            monthView.Children.Select(child => child.GetType().FullName));
        throw new InvalidOperationException(
            $"Date {date:yyyy-MM-dd} was not generated. ChildCount: {monthView.Children.Count}; ButtonCount: {buttons.Length}; Actual dates: {actualDates}; ChildTypes: {childTypes}");
    }

    private static void ShowInWindow(Control content, Action assertion)
    {
        var window = new AvaloniaWindow
        {
            Width   = 420,
            Height  = 360,
            Content = content
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            assertion();
        }
        finally
        {
            window.Close();
        }
    }

    private static void RunOnUIThread(Action action)
    {
        Dispatcher.UIThread.Invoke(action);
    }

    private sealed class TestCalendarItem : PickerCalendarItem
    {
        public void ClearGeneratedMonthViewForTest(AvaloniaGrid monthView)
        {
            ClearGeneratedMonthView(monthView);
        }
    }
}
