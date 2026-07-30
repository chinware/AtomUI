using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using AtomUI.Controls.Primitives;
using AtomUI.Desktop.Controls.DesignTokens;
using AtomUI.Theme.Resources;
using AtomUI.Desktop.Controls.CalendarView.Models;
using AtomUI.Desktop.Controls.CalendarView.Rendering;
using Shouldly;
using Xunit;
using AvaloniaGrid = Avalonia.Controls.Grid;
using AvaloniaWindow = Avalonia.Controls.Window;
using PickerCalendar = AtomUI.Desktop.Controls.CalendarView.Calendar;
using PickerCalendarButton = AtomUI.Desktop.Controls.CalendarView.CalendarButton;
using PickerCalendarDayButton = AtomUI.Desktop.Controls.CalendarView.CalendarDayButton;
using PickerDualMonthCalendarItem = AtomUI.Desktop.Controls.CalendarView.DualMonthCalendarItem;
using PickerDualMonthRangeCalendar = AtomUI.Desktop.Controls.CalendarView.DualMonthRangeCalendar;
using PickerCalendarMode = AtomUI.Desktop.Controls.CalendarView.CalendarMode;

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
    public void CalendarItemRenderer_Week_Mode_Renders_Week_Number_And_Selected_Row()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var owner = new PickerCalendar
            {
                PickerMode = DatePickerMode.Week
            };
            var monthView = CreateWeekMonthView();
            var panel = new CalendarMonthPanelModel(
                new DateTime(2026, 7, 1),
                new[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" },
                Enumerable.Range(0, 42)
                          .Select(index =>
                          {
                              var date       = new DateTime(2026, 6, 29).AddDays(index);
                              var isSelected = date >= new DateTime(2026, 7, 6) &&
                                               date <= new DateTime(2026, 7, 12);
                              return new CalendarCellState(
                                  Date: date,
                                  Text: date.Day.ToString(),
                                  IsToday: false,
                                  IsBlackout: false,
                                  IsDisabled: false,
                                  IsInactive: false,
                                  IsSelected: isSelected,
                                  IsRangeStart: false,
                                  IsRangeEnd: false,
                                  IsRangeMiddle: false,
                                  IsFocused: false,
                                  IsHidden: false,
                                  IsWeekSelectionMiddle: isSelected && date.DayOfWeek != DayOfWeek.Sunday,
                                  IsWeekSelectionEnd: isSelected && date.DayOfWeek == DayOfWeek.Sunday);
                          })
                          .ToArray(),
                Enumerable.Range(0, 6)
                          .Select(index => new CalendarCellState(
                              Date: new DateTime(2026, 6, 29).AddDays(index * 7),
                              Text: (27 + index).ToString(),
                              IsToday: false,
                              IsBlackout: false,
                              IsDisabled: false,
                              IsInactive: false,
                              IsSelected: index == 1,
                              IsRangeStart: false,
                              IsRangeEnd: false,
                              IsRangeMiddle: false,
                              IsFocused: false,
                              IsHidden: false,
                              IsWeekNumber: true,
                              IsWeekSelectionStart: index == 1))
                          .ToArray());

            CalendarItemRenderer.RenderMonthPanel(owner, monthView, panel);

            var buttons = monthView.Children.OfType<PickerCalendarDayButton>().ToArray();
            buttons.Length.ShouldBe(48);

            var selectedRow = buttons
                              .Where(button => !button.IsWeekNumber &&
                                               button.DataContext is DateTime date &&
                                               date >= new DateTime(2026, 7, 6) &&
                                               date <= new DateTime(2026, 7, 12))
                              .Prepend(buttons.Single(button => button.IsWeekNumber &&
                                                                button.Content?.Equals("28") == true))
                              .ToArray();

            selectedRow.Length.ShouldBe(8);
            selectedRow[0].IsWeekNumber.ShouldBeTrue();
            selectedRow[0].IsWeekSelectionStart.ShouldBeTrue();
            selectedRow.Skip(1).Take(6).ShouldAllBe(button => button.IsWeekSelectionMiddle);
            selectedRow.Last().IsWeekSelectionEnd.ShouldBeTrue();
        });
    }

    [Fact]
    public void CalendarItemRenderer_Week_Mode_Renders_Hover_Row()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var owner = new PickerCalendar
            {
                PickerMode = DatePickerMode.Week
            };
            var monthView = CreateWeekMonthView();
            var panel = new CalendarMonthPanelModel(
                new DateTime(2026, 7, 1),
                new[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" },
                Enumerable.Range(0, 42)
                          .Select(index =>
                          {
                              var date    = new DateTime(2026, 6, 29).AddDays(index);
                              var isHover = date >= new DateTime(2026, 7, 20) &&
                                            date <= new DateTime(2026, 7, 26);
                              return new CalendarCellState(
                                  Date: date,
                                  Text: date.Day.ToString(),
                                  IsToday: false,
                                  IsBlackout: false,
                                  IsDisabled: false,
                                  IsInactive: false,
                                  IsSelected: false,
                                  IsRangeStart: false,
                                  IsRangeEnd: false,
                                  IsRangeMiddle: false,
                                  IsFocused: false,
                                  IsHidden: false,
                                  IsWeekHoverMiddle: isHover && date.DayOfWeek != DayOfWeek.Sunday,
                                  IsWeekHoverEnd: isHover && date.DayOfWeek == DayOfWeek.Sunday);
                          })
                          .ToArray(),
                Enumerable.Range(0, 6)
                          .Select(index => new CalendarCellState(
                              Date: new DateTime(2026, 6, 29).AddDays(index * 7),
                              Text: (27 + index).ToString(),
                              IsToday: false,
                              IsBlackout: false,
                              IsDisabled: false,
                              IsInactive: false,
                              IsSelected: false,
                              IsRangeStart: false,
                              IsRangeEnd: false,
                              IsRangeMiddle: false,
                              IsFocused: false,
                              IsHidden: false,
                              IsWeekNumber: true,
                              IsWeekHoverStart: index == 3))
                          .ToArray());

            CalendarItemRenderer.RenderMonthPanel(owner, monthView, panel);

            var buttons = monthView.Children.OfType<PickerCalendarDayButton>().ToArray();
            var hoverRow = buttons
                           .Where(button => !button.IsWeekNumber &&
                                            button.DataContext is DateTime date &&
                                            date >= new DateTime(2026, 7, 20) &&
                                            date <= new DateTime(2026, 7, 26))
                           .Prepend(buttons.Single(button => button.IsWeekNumber &&
                                                             button.Content?.Equals("30") == true))
                           .ToArray();

            hoverRow.Length.ShouldBe(8);
            hoverRow[0].IsWeekHoverStart.ShouldBeTrue();
            hoverRow.Skip(1).Take(6).ShouldAllBe(button => button.IsWeekHoverMiddle);
            hoverRow.Last().IsWeekHoverEnd.ShouldBeTrue();
        });
    }

    [Fact]
    public void CalendarItemRenderer_Week_Mode_Renders_Committed_Range_Row()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var owner = new PickerCalendar
            {
                PickerMode = DatePickerMode.Week
            };
            var monthView = CreateWeekMonthView();
            var panel = new CalendarMonthPanelModel(
                new DateTime(2026, 7, 1),
                new[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" },
                Enumerable.Range(0, 42)
                          .Select(index =>
                          {
                              var date    = new DateTime(2026, 6, 29).AddDays(index);
                              var isRange = date >= new DateTime(2026, 7, 20) &&
                                            date <= new DateTime(2026, 7, 26);
                              return new CalendarCellState(
                                  Date: date,
                                  Text: date.Day.ToString(),
                                  IsToday: false,
                                  IsBlackout: false,
                                  IsDisabled: false,
                                  IsInactive: false,
                                  IsSelected: false,
                                  IsRangeStart: false,
                                  IsRangeEnd: false,
                                  IsRangeMiddle: false,
                                  IsFocused: false,
                                  IsHidden: false,
                                  IsWeekRangeMiddle: isRange && date.DayOfWeek != DayOfWeek.Sunday,
                                  IsWeekRangeEnd: isRange && date.DayOfWeek == DayOfWeek.Sunday);
                          })
                          .ToArray(),
                Enumerable.Range(0, 6)
                          .Select(index => new CalendarCellState(
                              Date: new DateTime(2026, 6, 29).AddDays(index * 7),
                              Text: (27 + index).ToString(),
                              IsToday: false,
                              IsBlackout: false,
                              IsDisabled: false,
                              IsInactive: false,
                              IsSelected: false,
                              IsRangeStart: false,
                              IsRangeEnd: false,
                              IsRangeMiddle: false,
                              IsFocused: false,
                              IsHidden: false,
                              IsWeekNumber: true,
                              IsWeekRangeStart: index == 3))
                          .ToArray());

            CalendarItemRenderer.RenderMonthPanel(owner, monthView, panel);

            var rangeRow = GetWeekRowButtons(monthView, "30", new DateTime(2026, 7, 20), new DateTime(2026, 7, 26));
            rangeRow.Length.ShouldBe(8);
            rangeRow[0].IsWeekRangeStart.ShouldBeTrue();
            rangeRow.Skip(1).Take(6).ShouldAllBe(button => button.IsWeekRangeMiddle);
            rangeRow.Last().IsWeekRangeEnd.ShouldBeTrue();
            rangeRow.ShouldAllBe(button => !button.IsRangeStart && !button.IsRangeEnd && !button.IsRangeMiddle);

            CalendarItemRenderer.RenderMonthPanel(
                owner,
                monthView,
                new CalendarMonthPanelModel(
                    new DateTime(2026, 7, 1),
                    new[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" },
                    Enumerable.Range(0, 42)
                              .Select(index => new CalendarCellState(
                                  Date: new DateTime(2026, 6, 29).AddDays(index),
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
                              .ToArray(),
                    Enumerable.Range(0, 6)
                              .Select(index => new CalendarCellState(
                                  Date: new DateTime(2026, 6, 29).AddDays(index * 7),
                                  Text: (27 + index).ToString(),
                                  IsToday: false,
                                  IsBlackout: false,
                                  IsDisabled: false,
                                  IsInactive: false,
                                  IsSelected: false,
                                  IsRangeStart: false,
                                  IsRangeEnd: false,
                                  IsRangeMiddle: false,
                                  IsFocused: false,
                                  IsHidden: false,
                                  IsWeekNumber: true))
                              .ToArray()));

            rangeRow.ShouldAllBe(button => !button.IsWeekRangeStart && !button.IsWeekRangeMiddle && !button.IsWeekRangeEnd);
        });
    }

    [Fact]
    public void CalendarItem_Week_Mode_Cell_Hover_Renders_Row_Hover()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var calendar = new PickerCalendar
            {
                PickerMode   = DatePickerMode.Week,
                DisplayDate  = new DateTime(2026, 7, 1),
                SelectedDate = new DateTime(2026, 7, 13)
            };
            var window = new AvaloniaWindow
            {
                Width   = 420,
                Height  = 360,
                Content = calendar
            };

            try
            {
                window.Show();
                Dispatcher.UIThread.RunJobs();

                var item      = calendar.CalendarItem.ShouldNotBeNull();
                var monthView = item.MonthView.ShouldNotBeNull();
                var hoverButton = monthView.Children
                                           .OfType<PickerCalendarDayButton>()
                                           .Single(button => button.DataContext is DateTime date &&
                                                             date == new DateTime(2026, 7, 22));

                item.HandleCellMouseEntered(hoverButton, null!);
                Dispatcher.UIThread.RunJobs();

                var buttons = monthView.Children.OfType<PickerCalendarDayButton>().ToArray();
                var hoverRow = buttons
                               .Where(button => !button.IsWeekNumber &&
                                                button.DataContext is DateTime date &&
                                                date >= new DateTime(2026, 7, 20) &&
                                                date <= new DateTime(2026, 7, 26))
                               .Prepend(buttons.Single(button => button.IsWeekNumber &&
                                                                 button.Content?.Equals("30") == true))
                               .ToArray();

                hoverRow.Length.ShouldBe(8);
                hoverRow[0].IsWeekHoverStart.ShouldBeTrue();
                hoverRow.Skip(1).Take(6).ShouldAllBe(button => button.IsWeekHoverMiddle);
                hoverRow.Last().IsWeekHoverEnd.ShouldBeTrue();
                hoverButton.Background.ShouldBe(Brushes.Transparent);
            }
            finally
            {
                window.Close();
                Dispatcher.UIThread.RunJobs();
            }
        });
    }

    [Fact]
    public void CalendarItem_Week_Mode_Row_Gap_Hover_Renders_Row_Hover()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var calendar = new PickerCalendar
            {
                PickerMode   = DatePickerMode.Week,
                DisplayDate  = new DateTime(2026, 7, 1),
                SelectedDate = new DateTime(2026, 7, 13)
            };
            var window = new AvaloniaWindow
            {
                Width   = 420,
                Height  = 360,
                Content = calendar
            };

            try
            {
                window.Show();
                Dispatcher.UIThread.RunJobs();

                var item      = calendar.CalendarItem.ShouldNotBeNull();
                var monthView = item.MonthView.ShouldNotBeNull();
                var topLevel  = TopLevel.GetTopLevel(monthView).ShouldNotBeNull();
                var day22 = monthView.Children
                                     .OfType<PickerCalendarDayButton>()
                                     .Single(button => button.DataContext is DateTime date &&
                                                       date == new DateTime(2026, 7, 22));
                var day23 = monthView.Children
                                     .OfType<PickerCalendarDayButton>()
                                     .Single(button => button.DataContext is DateTime date &&
                                                       date == new DateTime(2026, 7, 23));

                var day22Content = FindTemplateContentPresenter(day22);
                var day23Content = FindTemplateContentPresenter(day23);
                var day22Right = day22Content.TranslatePoint(new Point(day22Content.Bounds.Width, day22Content.Bounds.Height / 2), topLevel)
                                             .ShouldNotBeNull();
                var day23Left = day23Content.TranslatePoint(new Point(0, day23Content.Bounds.Height / 2), topLevel)
                                            .ShouldNotBeNull();
                var rowGapPoint = new Point((day22Right.X + day23Left.X) / 2, day22Right.Y);

                item.UpdatePointerMonthViewState(rowGapPoint);
                Dispatcher.UIThread.RunJobs();

                var hoverRow = GetWeekRowButtons(monthView, "30", new DateTime(2026, 7, 20), new DateTime(2026, 7, 26));
                hoverRow[0].IsWeekHoverStart.ShouldBeTrue();
                hoverRow.Skip(1).Take(6).ShouldAllBe(button => button.IsWeekHoverMiddle);
                hoverRow.Last().IsWeekHoverEnd.ShouldBeTrue();
            }
            finally
            {
                window.Close();
                Dispatcher.UIThread.RunJobs();
            }
        });
    }

    [Fact]
    public void DualMonthCalendarItem_Week_Mode_Secondary_Panel_Pointer_Tracking_Keeps_Range_Preview_Row()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var calendar = new PickerDualMonthRangeCalendar
            {
                PickerMode         = DatePickerMode.Week,
                DisplayDate        = new DateTime(2026, 7, 1),
                SelectedDate       = new DateTime(2026, 7, 6),
                IsSelectRangeStart = false
            };
            var window = new AvaloniaWindow
            {
                Width   = 860,
                Height  = 420,
                Content = calendar
            };

            try
            {
                window.Show();
                Dispatcher.UIThread.RunJobs();

                var item = calendar.CalendarItem.ShouldBeOfType<PickerDualMonthCalendarItem>();
                var secondaryMonthView = item.SecondaryMonthView.ShouldNotBeNull();
                var topLevel = TopLevel.GetTopLevel(secondaryMonthView).ShouldNotBeNull();
                var hoverButton = secondaryMonthView.Children
                                                    .OfType<PickerCalendarDayButton>()
                                                    .Single(button => !button.IsWeekNumber &&
                                                                      button.DataContext is DateTime date &&
                                                                      date == new DateTime(2026, 8, 19));

                item.HandleCellMouseEntered(hoverButton, null!);
                Dispatcher.UIThread.RunJobs();

                var hoverButtonContent = FindTemplateContentPresenter(hoverButton);
                var hoverPoint = hoverButtonContent.TranslatePoint(
                                                new Point(hoverButtonContent.Bounds.Width / 2, hoverButtonContent.Bounds.Height / 2),
                                                topLevel)
                                            .ShouldNotBeNull();

                item.UpdatePointerMonthViewState(hoverPoint);
                Dispatcher.UIThread.RunJobs();

                var previewRow = GetWeekRowButtons(
                    secondaryMonthView,
                    "34",
                    new DateTime(2026, 8, 17),
                    new DateTime(2026, 8, 23));

                calendar.HoverDateTime.ShouldBe(new DateTime(2026, 8, 17));
                previewRow[0].IsWeekSelectionStart.ShouldBeTrue();
                previewRow.Skip(1).Take(6).ShouldAllBe(button => button.IsWeekSelectionMiddle);
                previewRow.Last().IsWeekSelectionEnd.ShouldBeTrue();
            }
            finally
            {
                window.Close();
                Dispatcher.UIThread.RunJobs();
            }
        });
    }

    [Fact]
    public void CalendarItem_Quarter_Mode_Uses_Compact_One_Row_Year_View()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var calendar = new PickerCalendar
            {
                PickerMode   = DatePickerMode.Quarter,
                DisplayDate  = new DateTime(2026, 1, 1),
                SelectedDate = new DateTime(2026, 4, 1)
            };
            var window = new AvaloniaWindow
            {
                Width   = 420,
                Height  = 360,
                Content = calendar
            };

            try
            {
                window.Show();
                Dispatcher.UIThread.RunJobs();

                var item     = calendar.CalendarItem.ShouldNotBeNull();
                var yearView = item.YearView.ShouldNotBeNull();

                calendar.DisplayMode.ShouldBe(PickerCalendarMode.Year);
                yearView.IsVisible.ShouldBeTrue();
                yearView.RowDefinitions.Count.ShouldBe(1);
                yearView.ColumnDefinitions.Count.ShouldBe(4);
                item.Bounds.Height.ShouldBeLessThan(200);
                var quarterCellHeight = GetThemeResource<double>(CalendarTokenKind.CellHeight);
                var originalQuarterPanelHeight = GetThemeResource<double>(CalendarTokenKind.WithoutTimeCellHeight);
                var compactQuarterPanelHeight = quarterCellHeight + (originalQuarterPanelHeight - quarterCellHeight) / 2;
                Math.Abs(yearView.Bounds.Height - compactQuarterPanelHeight).ShouldBeLessThan(0.5);

                var quarterButtons = yearView.Children
                                             .OfType<PickerCalendarButton>()
                                             .Where(button => button.Opacity > 0)
                                             .ToArray();

                quarterButtons.Length.ShouldBe(4);
                quarterButtons.ShouldAllBe(button => AvaloniaGrid.GetRow(button) == 0);
                quarterButtons.Select(button => button.Content?.ToString())
                              .ShouldBe(new[] { "Q1", "Q2", "Q3", "Q4" });
            }
            finally
            {
                window.Close();
                Dispatcher.UIThread.RunJobs();
            }
        });
    }

    [Fact]
    public void CalendarItem_Quarter_Mode_Decade_View_Restores_Three_Row_Year_View()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var calendar = new PickerCalendar
            {
                PickerMode   = DatePickerMode.Quarter,
                DisplayDate  = new DateTime(2026, 1, 1),
                SelectedDate = new DateTime(2026, 4, 1)
            };
            var window = new AvaloniaWindow
            {
                Width   = 420,
                Height  = 360,
                Content = calendar
            };

            try
            {
                window.Show();
                Dispatcher.UIThread.RunJobs();

                var item     = calendar.CalendarItem.ShouldNotBeNull();
                var yearView = item.YearView.ShouldNotBeNull();
                yearView.RowDefinitions.Count.ShouldBe(1);

                calendar.DisplayMode = PickerCalendarMode.Decade;
                Dispatcher.UIThread.RunJobs();

                yearView.RowDefinitions.Count.ShouldBe(3);
                yearView.Children
                        .OfType<PickerCalendarButton>()
                        .Where(button => button.Opacity > 0)
                        .Select(button => AvaloniaGrid.GetRow(button))
                        .Distinct()
                        .OrderBy(row => row)
                        .ShouldBe(new[] { 0, 1, 2 });
            }
            finally
            {
                window.Close();
                Dispatcher.UIThread.RunJobs();
            }
        });
    }

    [Fact]
    public void CalendarItem_Month_Mode_Uses_Four_Row_Three_Column_Year_View()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var calendar = new PickerCalendar
            {
                PickerMode   = DatePickerMode.Month,
                DisplayDate  = new DateTime(2026, 1, 1),
                SelectedDate = new DateTime(2026, 2, 1)
            };
            var window = new AvaloniaWindow
            {
                Width   = 420,
                Height  = 360,
                Content = calendar
            };

            try
            {
                window.Show();
                Dispatcher.UIThread.RunJobs();

                var item     = calendar.CalendarItem.ShouldNotBeNull();
                var yearView = item.YearView.ShouldNotBeNull();

                calendar.DisplayMode.ShouldBe(PickerCalendarMode.Year);
                yearView.IsVisible.ShouldBeTrue();
                yearView.RowDefinitions.Count.ShouldBe(4);
                yearView.ColumnDefinitions.Count.ShouldBe(3);

                var monthButtons = yearView.Children
                                           .OfType<PickerCalendarButton>()
                                           .Where(button => button.Opacity > 0)
                                           .ToArray();
                monthButtons.Length.ShouldBe(12);

                monthButtons
                    .GroupBy(AvaloniaGrid.GetRow)
                    .OrderBy(group => group.Key)
                    .Select(group => group.OrderBy(AvaloniaGrid.GetColumn)
                                          .Select(button => ((DateTime)button.DataContext!).Month)
                                          .ToArray())
                    .ShouldBe(new[]
                    {
                        new[] { 1, 2, 3 },
                        new[] { 4, 5, 6 },
                        new[] { 7, 8, 9 },
                        new[] { 10, 11, 12 }
                    });
            }
            finally
            {
                window.Close();
                Dispatcher.UIThread.RunJobs();
            }
        });
    }

    [Fact]
    public void CalendarItem_Header_Uses_Compact_DatePicker_Spacing()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var calendar = new PickerCalendar
            {
                PickerMode  = DatePickerMode.Month,
                DisplayDate = new DateTime(2026, 1, 1)
            };
            var window = new AvaloniaWindow
            {
                Width   = 420,
                Height  = 360,
                Content = calendar
            };

            try
            {
                window.Show();
                Dispatcher.UIThread.RunJobs();

                var item        = calendar.CalendarItem.ShouldNotBeNull();
                var headerFrame = FindTemplatePixelAlignedBorder(item, "PART_HeaderFrame");
                var calendarHeaderMargin = GetThemeResource<Thickness>(CalendarTokenKind.HeaderMargin);

                headerFrame.Padding.ShouldBe(new Thickness(0));
                headerFrame.Margin.ShouldBe(calendarHeaderMargin);
            }
            finally
            {
                window.Close();
                Dispatcher.UIThread.RunJobs();
            }
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

    [Fact]
    public void CalendarDayButton_Range_Edge_Selection_Frame_Does_Not_Include_Cell_Margin()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var button = new PickerCalendarDayButton
            {
                Content             = "18",
                IsRangePreviewStart = true
            };

            ShowInWindow(button, () =>
            {
                var frame   = FindTemplatePixelAlignedBorder(button);
                var content = FindTemplateContentPresenter(button);

                frame.Bounds.Width.ShouldBe(content.Bounds.Width, 0.5);
                frame.Bounds.Height.ShouldBe(content.Bounds.Height, 0.5);
                frame.Margin.Left.ShouldBeGreaterThan(0);
                content.Margin.ShouldBe(new Thickness(0));
            });
        });
    }

    [Fact]
    public void CalendarDayButton_Disabled_Uses_Full_Cell_Background_Layer()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var button = new PickerCalendarDayButton
            {
                Content = "18"
            };

            ShowInWindow(button, () =>
            {
                button.IsEnabled = false;
                Dispatcher.UIThread.RunJobs();

                var disabledBackground = FindTemplateBorder(button, "DisabledBackground");
                var contentFrame      = FindTemplatePixelAlignedBorder(button);

                disabledBackground.IsVisible.ShouldBeTrue();
                disabledBackground.Bounds.Width.ShouldBe(button.Bounds.Width, 0.5);
                disabledBackground.Bounds.Height.ShouldBe(
                    GetThemeResource<double>(CalendarTokenKind.CellHeight),
                    0.5);
                BrushShouldHaveSameColor(
                    disabledBackground.Background,
                    GetThemeResource<IBrush>(CalendarTokenKind.CellBgDisabled));
                BrushShouldHaveSameColor(
                    contentFrame.Background,
                    Brushes.Transparent);
                BrushShouldHaveSameColor(
                    button.Foreground,
                    GetThemeResource<IBrush>(SharedTokenKind.ColorTextDisabled));
            });
        });
    }

    [Fact]
    public void CalendarDayButton_Week_Selection_Uses_Row_Indicator()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var button = new PickerCalendarDayButton
            {
                Content              = "28",
                CornerRadius         = new CornerRadius(4),
                IsWeekNumber         = true,
                IsWeekSelectionStart = true
            };

            ShowInWindow(button, () =>
            {
                button.EffectiveCornerRadius.ShouldBe(new CornerRadius(4, 0, 0, 4));
                BrushShouldHaveSameColor(button.Foreground, GetThemeResource<IBrush>(SharedTokenKind.ColorWhite));
                BrushShouldHaveSameColor(button.Background, Brushes.Transparent);

                var indicator = FindTemplateBorder(button, "WeekSelectionIndicator");
                indicator.IsVisible.ShouldBeTrue();
                BrushShouldHaveSameColor(indicator.Background, GetThemeResource<IBrush>(SharedTokenKind.ColorPrimary));
            });
        });
    }

    [Fact]
    public void CalendarDayButton_Week_Hover_Uses_Row_Indicator()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var button = new PickerCalendarDayButton
            {
                Content          = "22",
                CornerRadius     = new CornerRadius(4),
                IsWeekHoverMiddle = true
            };

            ShowInWindow(button, () =>
            {
                button.EffectiveCornerRadius.ShouldBe(new CornerRadius(0));
                BrushShouldHaveSameColor(button.Background, Brushes.Transparent);

                var indicator = FindTemplateBorder(button, "WeekHoverIndicator");
                indicator.IsVisible.ShouldBeTrue();
                BrushShouldHaveSameColor(indicator.Background, GetThemeResource<IBrush>(CalendarTokenKind.CellHoverBg));
            });
        });
    }

    [Fact]
    public void CalendarDayButton_Week_Range_Uses_Row_Indicator()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var button = new PickerCalendarDayButton
            {
                Content           = "22",
                CornerRadius      = new CornerRadius(4),
                IsWeekRangeMiddle = true
            };

            ShowInWindow(button, () =>
            {
                button.EffectiveCornerRadius.ShouldBe(new CornerRadius(0));
                BrushShouldHaveSameColor(button.Background, Brushes.Transparent);

                var indicator = FindTemplateBorder(button, "WeekRangeIndicator");
                indicator.IsVisible.ShouldBeTrue();
                BrushShouldHaveSameColor(indicator.Background, GetThemeResource<IBrush>(CalendarTokenKind.CellActiveWithRangeBg));
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

    private static AvaloniaGrid CreateWeekMonthView()
    {
        var monthView = new AvaloniaGrid();
        for (var i = 0; i < 8; i++)
        {
            monthView.Children.Add(new Control());
        }

        for (var i = 0; i < 6; i++)
        {
            monthView.Children.Add(new PickerCalendarDayButton());
            for (var j = 0; j < 7; j++)
            {
                monthView.Children.Add(new PickerCalendarDayButton());
            }
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

    private static ContentPresenter FindTemplateContentPresenter(Control control)
    {
        return control.GetVisualDescendants()
                      .OfType<ContentPresenter>()
                      .Single(presenter => presenter.Name == "Content");
    }

    private static PixelAlignedBorder FindTemplatePixelAlignedBorder(Control control)
    {
        return control.GetVisualDescendants()
                      .OfType<PixelAlignedBorder>()
                      .Single();
    }

    private static PixelAlignedBorder FindTemplatePixelAlignedBorder(Control control, string name)
    {
        return control.GetVisualDescendants()
                      .OfType<PixelAlignedBorder>()
                      .Single(border => border.Name == name);
    }

    private static PickerCalendarDayButton[] GetWeekRowButtons(
        AvaloniaGrid monthView,
        string weekNumber,
        DateTime weekStart,
        DateTime weekEnd)
    {
        var buttons = monthView.Children.OfType<PickerCalendarDayButton>().ToArray();
        return buttons
               .Where(button => !button.IsWeekNumber &&
                                button.DataContext is DateTime date &&
                                date >= weekStart &&
                                date <= weekEnd)
               .Prepend(buttons.Single(button => button.IsWeekNumber &&
                                                 button.Content?.Equals(weekNumber) == true))
               .ToArray();
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
