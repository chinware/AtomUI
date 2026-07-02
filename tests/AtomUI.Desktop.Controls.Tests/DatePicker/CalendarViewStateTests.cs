using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AtomUI.Controls;
using AtomUI.Desktop.Controls.CalendarView;
using AtomUI.Desktop.Controls.CalendarView.State;
using AtomUI.Desktop.Controls.Primitives;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
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
    public void DualMonthRangeCalendar_Theme_BasedOn_CalendarTheme()
    {
        var source = ReadRepoFile("src/AtomUI.Desktop.Controls/DatePicker/Themes/CalendarView/DualMonthRangeCalendarTheme.axaml");

        source.ShouldContain("<atom:CalendarTheme TargetType=\"calendarView:DualMonthRangeCalendar\" />");
        source.ShouldNotContain("<atom:CalendarItemTheme TargetType=\"calendarView:DualMonthRangeCalendar\" />");
    }

    [Fact]
    public void RangeDatePickerPresenter_Now_With_Confirm_Confirms_Active_Range_Part()
    {
        var presenter = new TestRangeDatePickerPresenter
        {
            IsNeedConfirm      = true,
            IsRangeStartActive = true
        };
        var rangePartConfirmedCount = 0;
        presenter.RangePartConfirmed += (_, _) => rangePartConfirmedCount++;

        presenter.NotifyNowButtonClickedForTest();

        presenter.SelectedDateTime.ShouldNotBeNull();
        rangePartConfirmedCount.ShouldBe(1);
    }

    [Fact]
    public void TimedRangeDatePickerPresenter_Now_With_Confirm_Only_Selects_Active_Date()
    {
        var presenter = new TestTimedRangeDatePickerPresenter
        {
            IsNeedConfirm      = true,
            IsRangeStartActive = true
        };
        var rangePartConfirmedCount = 0;
        presenter.RangePartConfirmed += (_, _) => rangePartConfirmedCount++;

        presenter.NotifyNowButtonClickedForTest();

        presenter.SelectedDateTime.ShouldNotBeNull();
        rangePartConfirmedCount.ShouldBe(0);
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

    [Theory]
    [InlineData(DatePickerMode.Date, CalendarMode.Month)]
    [InlineData(DatePickerMode.Week, CalendarMode.Month)]
    [InlineData(DatePickerMode.Month, CalendarMode.Year)]
    [InlineData(DatePickerMode.Quarter, CalendarMode.Year)]
    [InlineData(DatePickerMode.Year, CalendarMode.Decade)]
    public void Calendar_PickerMode_Change_Uses_Target_DisplayMode(DatePickerMode pickerMode, CalendarMode expectedDisplayMode)
    {
        RunOnUIThread(() =>
        {
            var calendar = new PickerCalendar
            {
                PickerMode = pickerMode
            };

            calendar.DisplayMode.ShouldBe(expectedDisplayMode);
        });
    }

    [Theory]
    [InlineData(DatePickerMode.Month, 7, 2026, 7, 1)]
    [InlineData(DatePickerMode.Quarter, 8, 2026, 7, 1)]
    public void CalendarItem_Year_Mode_Target_Picker_Selects_Unit_Without_Drilling_Down(
        DatePickerMode pickerMode,
        int sourceMonth,
        int expectedYear,
        int expectedMonth,
        int expectedDay)
    {
        RunOnUIThread(() =>
        {
            var calendar = new PickerCalendar
            {
                PickerMode  = pickerMode,
                DisplayMode = CalendarMode.Year
            };
            var item = new TestCalendarItem
            {
                Owner = calendar
            };
            var button = new PickerCalendarButton
            {
                DataContext = new DateTime(2026, sourceMonth, 1)
            };
            DateTime? emittedDate = null;
            calendar.DateSelected += (_, args) => emittedDate = args.Date;

            item.HandleMonthCalendarButtonMouseUpForTest(button);

            var expectedDate = new DateTime(expectedYear, expectedMonth, expectedDay);
            calendar.SelectedDate.ShouldBe(expectedDate);
            emittedDate.ShouldBe(expectedDate);
            calendar.DisplayMode.ShouldBe(CalendarMode.Year);
        });
    }

    [Fact]
    public void CalendarItem_Year_Picker_Selects_Year_From_Decade_Mode()
    {
        RunOnUIThread(() =>
        {
            var calendar = new PickerCalendar
            {
                PickerMode  = DatePickerMode.Year,
                DisplayMode = CalendarMode.Decade
            };
            var item = new TestCalendarItem
            {
                Owner = calendar
            };
            var button = new PickerCalendarButton
            {
                DataContext = new DateTime(2028, 1, 1)
            };
            DateTime? emittedDate = null;
            calendar.DateSelected += (_, args) => emittedDate = args.Date;

            item.HandleMonthCalendarButtonMouseUpForTest(button);

            var expectedDate = new DateTime(2028, 1, 1);
            calendar.SelectedDate.ShouldBe(expectedDate);
            emittedDate.ShouldBe(expectedDate);
            calendar.DisplayMode.ShouldBe(CalendarMode.Decade);
        });
    }

    [Theory]
    [InlineData(DatePickerMode.Month, CalendarMode.Year, 2026, 8, 1, 2026, 8, 1)]
    [InlineData(DatePickerMode.Quarter, CalendarMode.Year, 2026, 8, 1, 2026, 7, 1)]
    [InlineData(DatePickerMode.Year, CalendarMode.Decade, 2028, 1, 1, 2028, 1, 1)]
    public void Calendar_Enter_Key_Selects_Target_Picker_Unit_Without_Drilling_Down(
        DatePickerMode pickerMode,
        CalendarMode displayMode,
        int sourceYear,
        int sourceMonth,
        int sourceDay,
        int expectedYear,
        int expectedMonth,
        int expectedDay)
    {
        RunOnUIThread(() =>
        {
            var sourceDate = new DateTime(sourceYear, sourceMonth, sourceDay);
            var calendar = new PickerCalendar
            {
                PickerMode  = pickerMode,
                DisplayMode = displayMode
            };
            if (displayMode == CalendarMode.Year)
            {
                calendar.SelectedMonth = sourceDate;
            }
            else
            {
                calendar.SelectedYear = sourceDate;
            }

            DateTime? emittedDate = null;
            calendar.DateSelected += (_, args) => emittedDate = args.Date;

            var handled = calendar.ProcessCalendarKey(new KeyEventArgs
            {
                Key          = Key.Enter,
                PhysicalKey  = PhysicalKey.Enter,
                KeyModifiers = KeyModifiers.None
            });

            var expectedDate = new DateTime(expectedYear, expectedMonth, expectedDay);
            handled.ShouldBeTrue();
            calendar.SelectedDate.ShouldBe(expectedDate);
            emittedDate.ShouldBe(expectedDate);
            calendar.DisplayMode.ShouldBe(displayMode);
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

            calendar.SelectedDate.ShouldBe(new DateTime(2026, 6, 10));

            ShowInWindow(calendar, () =>
            {
                var firstButton = FindDayButton(calendar, new DateTime(2026, 6, 10));
                var state       = calendar.SyncAndGetCurrentViewState();
                IsSelected(firstButton).ShouldBeTrue(
                    $"SelectedDate={calendar.SelectedDate:yyyy-MM-dd}; StateSelected={state.SelectedDate:yyyy-MM-dd}; " +
                    $"StatePickerMode={state.PickerMode}; ButtonIsSelected={firstButton.IsSelected}");

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

                IsSelected(FindDayButton(calendar, new DateTime(2026, 6, 15))).ShouldBeFalse();
                IsRangeEnd(FindDayButton(calendar, new DateTime(2026, 6, 15))).ShouldBeFalse();
                IsRangeMiddle(FindDayButton(calendar, new DateTime(2026, 6, 15))).ShouldBeTrue();
                IsSelected(FindDayButton(calendar, new DateTime(2026, 6, 18))).ShouldBeTrue();
                IsRangeEnd(FindDayButton(calendar, new DateTime(2026, 6, 18))).ShouldBeTrue();
            });
        });
    }

    [Fact]
    public void RangeCalendar_HoverDate_Does_Not_Apply_Committed_Range_End_To_Hover_Button()
    {
        RunOnUIThread(() =>
        {
            var calendar = new RangeCalendar
            {
                DisplayDate        = new DateTime(2026, 7, 1),
                SelectedDate       = new DateTime(2026, 7, 15),
                HoverDateTime      = new DateTime(2026, 7, 16),
                IsSelectRangeStart = false
            };

            ShowInWindow(calendar, () =>
            {
                var hoverButton = FindDayButton(calendar, new DateTime(2026, 7, 16));

                IsSelected(hoverButton).ShouldBeFalse();
                IsRangeEnd(hoverButton).ShouldBeFalse();
                IsRangePreviewEnd(hoverButton).ShouldBeTrue();
            });
        });
    }

    [Fact]
    public void RangeCalendar_Active_Start_HoverDate_Does_Not_Apply_Committed_Range_Start_To_Hover_Button()
    {
        RunOnUIThread(() =>
        {
            var calendar = new RangeCalendar
            {
                DisplayDate           = new DateTime(2026, 7, 1),
                SecondarySelectedDate = new DateTime(2026, 7, 16),
                HoverDateTime         = new DateTime(2026, 7, 8),
                IsSelectRangeStart    = true
            };

            ShowInWindow(calendar, () =>
            {
                var hoverButton = FindDayButton(calendar, new DateTime(2026, 7, 8));
                var endButton   = FindDayButton(calendar, new DateTime(2026, 7, 16));

                IsSelected(hoverButton).ShouldBeFalse();
                IsRangeStart(hoverButton).ShouldBeFalse();
                IsRangePreviewStart(hoverButton).ShouldBeTrue();
                IsSelected(endButton).ShouldBeTrue();
                IsRangeEnd(endButton).ShouldBeFalse();
                IsRangePreviewEnd(endButton).ShouldBeTrue();
            });
        });
    }

    [Fact]
    public void Calendar_DisplayDateStart_After_DisplayDateEnd_Clamps_End_To_Start()
    {
        RunOnUIThread(() =>
        {
            var calendar = new PickerCalendar
            {
                DisplayDate      = new DateTime(2026, 6, 1),
                DisplayDateEnd   = new DateTime(2026, 6, 10),
                DisplayDateStart = new DateTime(2026, 6, 20)
            };

            calendar.DisplayDateEnd.ShouldBe(new DateTime(2026, 6, 20));
        });
    }

    [Fact]
    public void Calendar_SelectedDate_Before_DisplayDateStart_Throws_And_Does_Not_Expand_Range()
    {
        RunOnUIThread(() =>
        {
            var calendar = new PickerCalendar
            {
                DisplayDateStart = new DateTime(2026, 6, 10),
                DisplayDateEnd   = new DateTime(2026, 6, 20)
            };

            Should.Throw<ArgumentOutOfRangeException>(() =>
            {
                calendar.SelectedDate = new DateTime(2026, 6, 5);
            });

            calendar.DisplayDateStart.ShouldBe(new DateTime(2026, 6, 10));
            calendar.DisplayDateEnd.ShouldBe(new DateTime(2026, 6, 20));
            calendar.SelectedDate.ShouldBeNull();
        });
    }

    [Fact]
    public void Calendar_Type_Initialization_Does_Not_Read_ThemeManager_Static_State()
    {
        var property = PickerCalendar.FirstDayOfWeekProperty;

        property.Name.ShouldBe(nameof(PickerCalendar.FirstDayOfWeek));
    }

    [Fact]
    public void CalendarButton_Default_Content_Is_Culture_Free()
    {
        RunOnUIThread(() =>
        {
            var button = new PickerCalendarButton();

            button.Content.ShouldBe(string.Empty);
        });
    }

    [Fact]
    public void Calendar_CurrentViewState_Tracks_Normalized_Display_Range()
    {
        RunOnUIThread(() =>
        {
            var calendar = new PickerCalendar
            {
                DisplayDate      = new DateTime(2026, 6, 1),
                DisplayDateEnd   = new DateTime(2026, 6, 10),
                DisplayDateStart = new DateTime(2026, 6, 20)
            };

            var state = calendar.SyncAndGetCurrentViewState();

            state.DisplayDateStart.ShouldBe(new DateTime(2026, 6, 20));
            state.DisplayDateEnd.ShouldBe(new DateTime(2026, 6, 20));
            state.DisplayDate.ShouldBe(new DateTime(2026, 6, 1));
        });
    }

    [Fact]
    public void Calendar_CurrentViewState_Tracks_Week_Start_And_CrossMonth_SelectedDate()
    {
        RunOnUIThread(() =>
        {
            var calendar = new PickerCalendar
            {
                DisplayDate    = new DateTime(2026, 6, 1),
                FirstDayOfWeek = DayOfWeek.Monday
            };

            calendar.SyncAndGetCurrentViewState().FirstDayOfWeek.ShouldBe(DayOfWeek.Monday);

            calendar.FirstDayOfWeek = DayOfWeek.Sunday;
            calendar.SelectedDate   = new DateTime(2026, 8, 12);

            var state = calendar.SyncAndGetCurrentViewState();

            state.FirstDayOfWeek.ShouldBe(DayOfWeek.Sunday);
            state.SelectedDate.ShouldBe(new DateTime(2026, 8, 12));
            state.DisplayDate.ShouldBe(new DateTime(2026, 8, 1));
        });
    }

    [Fact]
    public void RangeCalendar_CurrentViewState_Tracks_Range_Selection()
    {
        RunOnUIThread(() =>
        {
            var calendar = new RangeCalendar
            {
                DisplayDate           = new DateTime(2026, 6, 1),
                SelectedDate          = new DateTime(2026, 6, 10),
                SecondarySelectedDate = new DateTime(2026, 6, 15),
                HoverDateTime         = new DateTime(2026, 6, 18),
                IsSelectRangeStart    = false
            };

            var state = calendar.SyncAndGetCurrentViewState();

            state.SelectedDate.ShouldBe(new DateTime(2026, 6, 10));
            state.SecondarySelectedDate.ShouldBe(new DateTime(2026, 6, 15));
            state.RangeSelection.HoverDate.ShouldBe(new DateTime(2026, 6, 18));
            state.RangeSelection.ActivePart.ShouldBe(CalendarRangeActivePart.End);
        });
    }

    [Fact]
    public void RangeCalendar_End_Selection_Does_Not_Let_Start_Date_Rewind_DisplayDate()
    {
        RunOnUIThread(() =>
        {
            var calendar = new RangeCalendar
            {
                DisplayDate        = new DateTime(2026, 8, 1),
                IsSelectRangeStart = false
            };

            calendar.SelectedDate = new DateTime(2026, 7, 12);

            calendar.DisplayDate.ShouldBe(new DateTime(2026, 8, 1));
            calendar.SyncAndGetCurrentViewState().DisplayDate.ShouldBe(new DateTime(2026, 8, 1));
        });
    }

    [Fact]
    public void RangeCalendar_TemplateApply_Does_Not_Let_Start_Date_Rewind_DisplayDate_When_End_Is_Active()
    {
        RunOnUIThread(() =>
        {
            var calendar = new RangeCalendar
            {
                SelectedDate       = new DateTime(2026, 7, 12),
                DisplayDate        = new DateTime(2026, 8, 1),
                IsSelectRangeStart = false
            };

            ShowInWindow(calendar, () =>
            {
                calendar.DisplayDate.ShouldBe(new DateTime(2026, 8, 1));
                calendar.SyncAndGetCurrentViewState().DisplayDate.ShouldBe(new DateTime(2026, 8, 1));
            });
        });
    }

    [Fact]
    public void RangeDatePicker_Open_End_Part_Does_Not_Let_Start_Date_Rewind_Calendar_DisplayDate()
    {
        RunOnUIThread(() =>
        {
            var picker = new TestRangeDatePicker
            {
                RangeStartSelectedDate = new DateTime(2024, 1, 12)
            };
            picker.RangeActivatedPart = RangeActivatedPart.End;

            var presenter = picker.CreatePickerPresenterForTest();
            picker.NotifyPickerOpenedForTest();

            ShowInWindow(presenter, () =>
            {
                var calendar = presenter.GetVisualDescendants()
                                        .OfType<DualMonthRangeCalendar>()
                                        .Single();

                calendar.IsSelectRangeStart.ShouldBeFalse();
                DateTimeHelper.CompareYearMonth(calendar.DisplayDate, picker.RangeStartSelectedDate.Value)
                              .ShouldNotBe(0);
            });
        });
    }

    [Fact]
    public void TimedRangeDatePicker_Open_End_Part_Does_Not_Let_Start_Date_Rewind_Calendar_DisplayDate()
    {
        RunOnUIThread(() =>
        {
            var picker = new TestRangeDatePicker
            {
                IsShowTime             = true,
                RangeStartSelectedDate = new DateTime(2024, 1, 12, 10, 30, 0)
            };
            picker.RangeActivatedPart = RangeActivatedPart.End;

            var presenter = picker.CreatePickerPresenterForTest();
            picker.NotifyPickerOpenedForTest();

            ShowInWindow(presenter, () =>
            {
                var calendar = presenter.GetVisualDescendants()
                                        .OfType<RangeCalendar>()
                                        .Single();

                calendar.IsSelectRangeStart.ShouldBeFalse();
                DateTimeHelper.CompareYearMonth(calendar.DisplayDate, picker.RangeStartSelectedDate.Value)
                              .ShouldNotBe(0);
            });
        });
    }

    [Fact]
    public void Calendar_FirstDayOfWeek_Change_Rebuilds_Week_Title_Order()
    {
        RunOnUIThread(() =>
        {
            var calendar = new PickerCalendar
            {
                DisplayDate    = new DateTime(2026, 6, 1),
                FirstDayOfWeek = DayOfWeek.Monday
            };

            ShowInWindow(calendar, () =>
            {
                var titles = calendar.CalendarItem.ShouldNotBeNull()
                                     .MonthView.ShouldNotBeNull()
                                     .Children
                                     .Take(7)
                                     .Select(child => child.DataContext?.ToString())
                                     .ToArray();

                titles[0].ShouldNotBeNullOrWhiteSpace();
                calendar.FirstDayOfWeek = DayOfWeek.Sunday;
                Dispatcher.UIThread.RunJobs();

                var updatedTitles = calendar.CalendarItem.ShouldNotBeNull()
                                            .MonthView.ShouldNotBeNull()
                                            .Children
                                            .Take(7)
                                            .Select(child => child.DataContext?.ToString())
                                            .ToArray();

                updatedTitles.ShouldNotBe(titles);
            });
        });
    }

    [Fact]
    public void DualMonthRangeCalendar_Secondary_Panel_Uses_Range_Highlight()
    {
        RunOnUIThread(() =>
        {
            var calendar = new DualMonthRangeCalendar
            {
                DisplayDate           = new DateTime(2026, 6, 1),
                SelectedDate          = new DateTime(2026, 6, 28),
                SecondarySelectedDate = new DateTime(2026, 7, 3)
            };

            ShowInWindow(calendar, () =>
            {
                var item = calendar.CalendarItem.ShouldBeOfType<PickerDualMonthCalendarItem>();
                var secondaryButton = item.SecondaryMonthView.ShouldNotBeNull()
                                          .Children
                                          .OfType<PickerCalendarDayButton>()
                                          .Single(button => button.DataContext is DateTime day &&
                                                            DateTimeHelper.CompareDays(day, new DateTime(2026, 7, 3)) == 0);

                IsRangeEnd(secondaryButton).ShouldBeTrue();
            });
        });
    }

    [Fact]
    public void DualMonthRangeCalendar_Hover_End_Renders_Continuous_Preview_Range_In_Secondary_Panel()
    {
        RunOnUIThread(() =>
        {
            var calendar = new DualMonthRangeCalendar
            {
                DisplayDate        = new DateTime(2026, 7, 1),
                SelectedDate       = new DateTime(2026, 7, 17),
                HoverDateTime      = new DateTime(2026, 8, 19),
                IsSelectRangeStart = false
            };

            ShowInWindow(calendar, () =>
            {
                var middleButton = FindSecondaryDayButton(calendar, new DateTime(2026, 8, 18));
                var endButton    = FindSecondaryDayButton(calendar, new DateTime(2026, 8, 19));

                IsRangePreviewMiddle(middleButton).ShouldBeTrue();
                IsSelected(endButton).ShouldBeFalse();
                IsRangeEnd(endButton).ShouldBeFalse();
                IsRangePreviewEnd(endButton).ShouldBeTrue();

                var endIndicator = FindTemplateBorder(endButton, "RangeEndIndicator");
                endIndicator.Bounds.Width.ShouldBe(endButton.Bounds.Width / 2, 0.5);
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

    private static bool IsRangeStart(PickerCalendarDayButton button)
    {
        return button.Classes.Contains(":range-start");
    }

    private static bool IsRangeMiddle(PickerCalendarDayButton button)
    {
        return button.Classes.Contains(":range-middle");
    }

    private static bool IsRangePreviewEnd(PickerCalendarDayButton button)
    {
        return button.Classes.Contains(":range-preview-end");
    }

    private static bool IsRangePreviewStart(PickerCalendarDayButton button)
    {
        return button.Classes.Contains(":range-preview-start");
    }

    private static bool IsRangePreviewMiddle(PickerCalendarDayButton button)
    {
        return button.Classes.Contains(":range-preview-middle");
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

    private static PickerCalendarDayButton FindSecondaryDayButton(DualMonthRangeCalendar calendar, DateTime date)
    {
        var item    = calendar.CalendarItem.ShouldBeOfType<PickerDualMonthCalendarItem>();
        var buttons = item.SecondaryMonthView.ShouldNotBeNull()
                          .Children
                          .OfType<PickerCalendarDayButton>()
                          .ToArray();
        return buttons.Single(button =>
            button.DataContext is DateTime buttonDate &&
            DateTimeHelper.CompareDays(buttonDate, date) == 0);
    }

    private static Border FindTemplateBorder(Control control, string name)
    {
        control.ApplyTemplate();
        Dispatcher.UIThread.RunJobs();
        return control.GetVisualDescendants()
                      .OfType<Border>()
                      .Single(border => border.Name == name);
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

    private static string ReadRepoFile(string relativePath)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, relativePath);
            if (File.Exists(candidate))
            {
                return File.ReadAllText(candidate);
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException($"Could not find repository file: {relativePath}");
    }

    private sealed class TestCalendarItem : PickerCalendarItem
    {
        public void ClearGeneratedMonthViewForTest(AvaloniaGrid monthView)
        {
            ClearGeneratedMonthView(monthView);
        }

        public void HandleMonthCalendarButtonMouseUpForTest(PickerCalendarButton button)
        {
            HandleMonthCalendarButtonMouseUp(button, null!);
        }
    }

    private sealed class TestRangeDatePicker : RangeDatePicker
    {
        public RangeDatePickerPresenter CreatePickerPresenterForTest()
        {
            var presenter = CreatePickerPresenter().ShouldBeAssignableTo<RangeDatePickerPresenter>();
            NotifyPickerPresenterCreated(presenter);
            return presenter;
        }

        public void NotifyPickerOpenedForTest()
        {
            NotifyPickerOpened();
        }
    }

    private sealed class TestRangeDatePickerPresenter : RangeDatePickerPresenter
    {
        public void NotifyNowButtonClickedForTest()
        {
            NotifyNowButtonClicked();
        }
    }

    private sealed class TestTimedRangeDatePickerPresenter : TimedRangeDatePickerPresenter
    {
        public void NotifyNowButtonClickedForTest()
        {
            NotifyNowButtonClicked();
        }
    }
}
