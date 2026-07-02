using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using AtomUI.Theme.Styling;
using AtomUI.Desktop.Controls.CalendarView.Models;
using AtomUI.Desktop.Controls.CalendarView.Rendering;
using Shouldly;
using Xunit;
using AvaloniaGrid = Avalonia.Controls.Grid;
using AvaloniaWindow = Avalonia.Controls.Window;
using PickerCalendar = AtomUI.Desktop.Controls.CalendarView.Calendar;
using PickerCalendarButton = AtomUI.Desktop.Controls.CalendarView.CalendarButton;
using PickerCalendarDayButton = AtomUI.Desktop.Controls.CalendarView.CalendarDayButton;

namespace AtomUI.Desktop.Controls.Tests.DatePickers;

public class CalendarViewLifecycleTests
{
    static CalendarViewLifecycleTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void CalendarItem_Pointer_Bounds_Check_Tolerates_Empty_MonthView()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var item      = new TestableCalendarItem();
            var monthView = new AvaloniaGrid();

            item.GetMonthViewRectForTest(monthView).ShouldBe(default(Rect));
        });
    }

    [Fact]
    public void CalendarItem_Pointer_Bounds_Check_Tolerates_Missing_MonthView()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var item = new TestableCalendarItem();

            item.GetMonthViewRectForTest(null).ShouldBe(default(Rect));
        });
    }

    [Fact]
    public void CalendarItem_Detach_Releases_Generated_Day_Button_Owners()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var calendar = new PickerCalendar
            {
                DisplayDate = new DateTime(2026, 6, 1)
            };
            var window = new AvaloniaWindow
            {
                Width   = 420,
                Height  = 360,
                Content = calendar
            };

            window.Show();
            Dispatcher.UIThread.RunJobs();

            var buttons = calendar.CalendarItem.ShouldNotBeNull()
                                  .MonthView.ShouldNotBeNull()
                                  .Children
                                  .OfType<PickerCalendarDayButton>()
                                  .ToArray();
            buttons.ShouldNotBeEmpty();
            buttons.ShouldAllBe(button => button.Owner == calendar);

            window.Close();
            Dispatcher.UIThread.RunJobs();

            buttons.ShouldAllBe(button => button.Owner == null);
        });
    }

    [Fact]
    public void CalendarItemRenderer_Applies_Cell_State_And_Clears_Stale_State()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var owner    = new PickerCalendar();
            var monthView = CreateMonthView();
            var button   = monthView.Children.OfType<PickerCalendarDayButton>().First();

            CalendarItemRenderer.RenderMonthPanel(
                owner,
                monthView,
                new CalendarMonthPanelModel(
                    new DateTime(2026, 6, 1),
                    new[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" },
                    Enumerable.Range(1, 42)
                              .Select(index => new CalendarCellState(
                                  Date: new DateTime(2026, 6, 1).AddDays(index - 1),
                                  Text: index.ToString(),
                                  IsToday: index == 1,
                                  IsBlackout: index == 1,
                                  IsDisabled: false,
                                  IsInactive: false,
                                  IsSelected: true,
                                  IsRangeStart: true,
                                  IsRangeEnd: false,
                                  IsRangeMiddle: false,
                                  IsFocused: index == 1,
                                  IsHidden: false))
                              .ToArray()));

            button.IsSelected.ShouldBeTrue();
            button.IsRangeStart.ShouldBeTrue();
            button.IsBlackout.ShouldBeTrue();
            owner.FocusButton.ShouldBe(button);

            CalendarItemRenderer.RenderMonthPanel(
                owner,
                monthView,
                new CalendarMonthPanelModel(
                    new DateTime(2026, 6, 1),
                    new[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" },
                    Enumerable.Range(1, 42)
                              .Select(index => new CalendarCellState(
                                  Date: new DateTime(2026, 6, 1).AddDays(index - 1),
                                  Text: index.ToString(),
                                  IsToday: false,
                                  IsBlackout: false,
                                  IsDisabled: false,
                                  IsInactive: false,
                                  IsSelected: false,
                                  IsRangeStart: false,
                                  IsRangeEnd: false,
                                  IsRangeMiddle: false,
                                  IsFocused: false,
                                  IsHidden: false))
                              .ToArray()));

            button.IsSelected.ShouldBeFalse();
            button.IsRangeStart.ShouldBeFalse();
            button.IsRangeEnd.ShouldBeFalse();
            button.IsRangeMiddle.ShouldBeFalse();
            button.IsBlackout.ShouldBeFalse();
            owner.FocusButton.ShouldBeNull();
        });
    }

    [Fact]
    public void CalendarItemRenderer_Applies_CalendarButton_State_And_Clears_Stale_Focus()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var owner    = new PickerCalendar();
            var yearView = CreateYearView();
            var button   = yearView.Children.OfType<PickerCalendarButton>().First();

            CalendarItemRenderer.RenderYearPanel(
                owner,
                yearView,
                new CalendarYearPanelModel(
                    new DateTime(2026, 1, 1),
                    Enumerable.Range(1, 12)
                              .Select(index => new CalendarCellState(
                                  Date: new DateTime(2026, index, 1),
                                  Text: index.ToString(),
                                  IsToday: false,
                                  IsBlackout: false,
                                  IsDisabled: false,
                                  IsInactive: index == 1,
                                  IsSelected: true,
                                  IsRangeStart: false,
                                  IsRangeEnd: false,
                                  IsRangeMiddle: false,
                                  IsFocused: index == 1,
                                  IsHidden: false))
                              .ToArray()));

            button.IsSelected.ShouldBeTrue();
            button.IsInactive.ShouldBeTrue();
            owner.FocusCalendarButton.ShouldBe(button);

            CalendarItemRenderer.RenderYearPanel(
                owner,
                yearView,
                new CalendarYearPanelModel(
                    new DateTime(2026, 1, 1),
                    Enumerable.Range(1, 12)
                              .Select(index => new CalendarCellState(
                                  Date: new DateTime(2026, index, 1),
                                  Text: index.ToString(),
                                  IsToday: false,
                                  IsBlackout: false,
                                  IsDisabled: false,
                                  IsInactive: false,
                                  IsSelected: false,
                                  IsRangeStart: false,
                                  IsRangeEnd: false,
                                  IsRangeMiddle: false,
                                  IsFocused: false,
                                  IsHidden: false))
                              .ToArray()));

            button.IsSelected.ShouldBeFalse();
            button.IsInactive.ShouldBeFalse();
            owner.FocusCalendarButton.ShouldBeNull();
        });
    }

    [Fact]
    public void CalendarDayButton_Selected_Preview_Start_Uses_Range_Start_Visuals()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var button = new PickerCalendarDayButton
            {
                Content             = "17",
                CornerRadius        = new CornerRadius(4),
                IsSelected          = true,
                IsRangePreviewStart = true
            };

            ShowInWindow(button, () =>
            {
                button.EffectiveCornerRadius.ShouldBe(new CornerRadius(4, 0, 0, 4));
                FindTemplateBorder(button, "RangeStartIndicator").IsVisible.ShouldBeTrue();
                FindTemplateBorder(button, "RangeEndIndicator").IsVisible.ShouldBeFalse();
            });
        });
    }

    [Fact]
    public void CalendarDayButton_Preview_End_Shows_Range_End_Indicator_Without_Selected_State()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var button = new PickerCalendarDayButton
            {
                Content           = "18",
                CornerRadius      = new CornerRadius(4),
                IsRangePreviewEnd = true
            };

            ShowInWindow(button, () =>
            {
                button.IsSelected.ShouldBeFalse();
                button.Classes.Contains(":selected").ShouldBeFalse();
                button.EffectiveCornerRadius.ShouldBe(new CornerRadius(0, 4, 4, 0));
                BrushShouldHaveSameColor(button.Background, GetThemeResource<IBrush>(SharedTokenKind.ColorPrimary));
                BrushShouldHaveSameColor(button.Foreground, GetThemeResource<IBrush>(SharedTokenKind.ColorWhite));
                FindTemplateBorder(button, "RangeStartIndicator").IsVisible.ShouldBeFalse();
                FindTemplateBorder(button, "RangeEndIndicator").IsVisible.ShouldBeTrue();
            });
        });
    }

    [Fact]
    public void CalendarDayButton_Preview_Start_Uses_Range_Start_Visuals_Without_Selected_State()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var button = new PickerCalendarDayButton
            {
                Content             = "17",
                CornerRadius        = new CornerRadius(4),
                IsRangePreviewStart = true
            };

            ShowInWindow(button, () =>
            {
                button.IsSelected.ShouldBeFalse();
                button.Classes.Contains(":selected").ShouldBeFalse();
                button.EffectiveCornerRadius.ShouldBe(new CornerRadius(4, 0, 0, 4));
                BrushShouldHaveSameColor(button.Background, GetThemeResource<IBrush>(SharedTokenKind.ColorPrimary));
                BrushShouldHaveSameColor(button.Foreground, GetThemeResource<IBrush>(SharedTokenKind.ColorWhite));
                FindTemplateBorder(button, "RangeStartIndicator").IsVisible.ShouldBeTrue();
                FindTemplateBorder(button, "RangeEndIndicator").IsVisible.ShouldBeFalse();
            });
        });
    }

    [Fact]
    public void CalendarDayButton_Range_Edge_Indicator_Uses_Half_Of_Actual_Cell_Width()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var startButton = new PickerCalendarDayButton
            {
                Width               = 100,
                Height              = 40,
                Content             = "17",
                IsRangePreviewStart = true
            };
            var endButton = new PickerCalendarDayButton
            {
                Width             = 100,
                Height            = 40,
                Content           = "19",
                IsRangePreviewEnd = true
            };
            var layout = new StackPanel
            {
                Children =
                {
                    startButton,
                    endButton
                }
            };

            ShowInWindow(layout, () =>
            {
                var startIndicator = FindTemplateBorder(startButton, "RangeStartIndicator");
                var endIndicator   = FindTemplateBorder(endButton, "RangeEndIndicator");

                startIndicator.Bounds.Width.ShouldBe(startButton.Bounds.Width / 2, 0.5);
                endIndicator.Bounds.Width.ShouldBe(endButton.Bounds.Width / 2, 0.5);
            });
        });
    }

    private static AvaloniaGrid CreateMonthView()
    {
        var monthView = new AvaloniaGrid();
        for (var i = 0; i < 7; i++)
        {
            monthView.Children.Add(new Control());
        }

        for (var i = 0; i < 42; i++)
        {
            monthView.Children.Add(new PickerCalendarDayButton());
        }

        return monthView;
    }

    private static AvaloniaGrid CreateYearView()
    {
        var yearView = new AvaloniaGrid();
        for (var i = 0; i < 12; i++)
        {
            yearView.Children.Add(new PickerCalendarButton());
        }

        return yearView;
    }

    private static void ShowInWindow(Control content, Action assertion)
    {
        var window = new AvaloniaWindow
        {
            Width   = 120,
            Height  = 120,
            Content = content
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            content.ApplyTemplate();
            Dispatcher.UIThread.RunJobs();
            assertion();
        }
        finally
        {
            window.Close();
            Dispatcher.UIThread.RunJobs();
        }
    }

    private static Border FindTemplateBorder(Control control, string name)
    {
        return control.GetVisualDescendants()
                      .OfType<Border>()
                      .Single(border => border.Name == name);
    }

    private static T GetThemeResource<T>(object key)
    {
        var application = Application.Current;
        application.ShouldNotBeNull();
        application!.TryGetResource(key, application.ActualThemeVariant, out var value).ShouldBeTrue();
        value.ShouldBeAssignableTo<T>();
        return (T)value!;
    }

    private static void BrushShouldHaveSameColor(IBrush? actual, IBrush? expected)
    {
        GetSolidBrushColor(actual).ShouldBe(GetSolidBrushColor(expected));
    }

    private static Color GetSolidBrushColor(IBrush? brush)
    {
        brush.ShouldNotBeNull();
        brush.ShouldBeAssignableTo<ISolidColorBrush>();
        return ((ISolidColorBrush)brush!).Color;
    }

    private sealed class TestableCalendarItem : CalendarView.CalendarItem
    {
        public Rect GetMonthViewRectForTest(AvaloniaGrid? monthView)
        {
            return GetMonthViewRect(monthView);
        }
    }
}
