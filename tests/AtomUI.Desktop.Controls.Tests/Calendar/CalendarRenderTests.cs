using AtomUI.Controls;
using AtomUI.Desktop.Controls.DesignTokens;
using AtomUI.Desktop.Controls.Internal.Calendar;
using AtomUI.Theme.Resources;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AtomUICalendar = AtomUI.Desktop.Controls.Calendar;
using CalendarHeaderControl = AtomUI.Desktop.Controls.Internal.Calendar.CalendarHeader;
using CalendarHeaderItem = AtomUI.Desktop.Controls.Internal.Calendar.CalendarHeaderItem;
using CalendarViewControl = AtomUI.Desktop.Controls.Internal.Calendar.CalendarView;
using CalendarCellControl = AtomUI.Desktop.Controls.Internal.Calendar.CalendarViewCell;
using DesktopComboBox = AtomUI.Desktop.Controls.ComboBox;
using DesktopOptionButton = AtomUI.Desktop.Controls.OptionButton;
using DesktopOptionButtonGroup = AtomUI.Desktop.Controls.OptionButtonGroup;
using DesktopTextBlock = AtomUI.Desktop.Controls.TextBlock;
using AtomUI.Theme.Language;

namespace AtomUI.Desktop.Controls.Tests.Calendar;

public class CalendarRenderTests
{
    static CalendarRenderTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Calendar_AppliesTemplate_And_ResolvesCalendarView()
    {
        var calendar = new AtomUICalendar { Value = new DateTime(2026, 7, 15) };
        var window = Show(calendar);
        try
        {
            var view = calendar.GetVisualDescendants().OfType<CalendarViewControl>().FirstOrDefault();
            view.ShouldNotBeNull();
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Calendar_MonthMode_Generates42CellContainers()
    {
        var calendar = new AtomUICalendar { Value = new DateTime(2026, 7, 15) };
        var window = Show(calendar);
        try
        {
            var cells = calendar.GetVisualDescendants().OfType<CalendarCellControl>().ToList();
            cells.Count.ShouldBe(42);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Calendar_YearMode_Generates12CellContainers()
    {
        var calendar = new AtomUICalendar { Value = new DateTime(2026, 7, 15), Mode = CalendarMode.Year };
        var window = Show(calendar);
        try
        {
            var cells = calendar.GetVisualDescendants().OfType<CalendarCellControl>().ToList();
            cells.Count.ShouldBe(12);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Calendar_YearMode_UsesThreeColumnsAndFourRows()
    {
        var calendar = new AtomUICalendar { Value = new DateTime(2026, 7, 15), Mode = CalendarMode.Year };
        var window = Show(calendar);
        try
        {
            var host = calendar.GetVisualDescendants()
                .OfType<Avalonia.Controls.Grid>()
                .Single(grid => grid.Name == "PART_CellHost");
            host.ColumnDefinitions.Count.ShouldBe(3);
            host.RowDefinitions.Count.ShouldBe(4);

            var december = host.Children.OfType<CalendarCellControl>()
                .Single(cell => cell.Model?.Value.Month == 12);
            Avalonia.Controls.Grid.GetRow(december).ShouldBe(3);
            Avalonia.Controls.Grid.GetColumn(december).ShouldBe(2);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Calendar_WeekCellActivation_SelectsRowStartWithDateSource()
    {
        var calendar = new AtomUICalendar
        {
            Value = new DateTime(2026, 7, 15),
            ShowWeek = true
        };
        CalendarSelectedEventArgs? selected = null;
        calendar.Selected += (_, e) => selected = e;
        var window = Show(calendar);
        try
        {
            var week = calendar.GetVisualDescendants()
                .OfType<CalendarCellControl>()
                .First(cell => cell.Model?.Kind == CalendarViewCellKind.Week);
            week.Activate();
            Dispatcher.UIThread.RunJobs();

            selected.ShouldNotBeNull();
            selected!.Source.ShouldBe(CalendarSelectSource.Date);
            selected.Value.ShouldBe(new DateTime(2026, 6, 28));
            calendar.Value.ShouldBe(selected.Value);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Calendar_ShowWeek_Generates48CellContainers()
    {
        var calendar = new AtomUICalendar { Value = new DateTime(2026, 7, 15), ShowWeek = true };
        var window = Show(calendar);
        try
        {
            var cells = calendar.GetVisualDescendants().OfType<CalendarCellControl>().ToList();
            cells.Count.ShouldBe(48);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Calendar_RendersDefaultHeader_WhenNoHeaderTemplate()
    {
        var calendar = new AtomUICalendar { Value = new DateTime(2026, 7, 15) };
        var window = Show(calendar);
        try
        {
            var header = calendar.GetVisualDescendants()
                .OfType<AtomUI.Desktop.Controls.Internal.Calendar.CalendarHeader>()
                .FirstOrDefault();
            header.ShouldNotBeNull();
            header!.IsVisible.ShouldBeTrue();
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Calendar_Header_UsesAntVerticalSpacing()
    {
        var calendar = new AtomUICalendar
        {
            Value = new DateTime(2026, 7, 15),
            Fullscreen = false
        };
        var window = Show(calendar, 400, 400);
        try
        {
            var header = calendar.GetVisualDescendants().OfType<CalendarHeaderControl>().Single();
            var year = header.GetVisualDescendants().OfType<DesktopComboBox>().Single(control => control.Name == "PART_YearSelect");
            var month = header.GetVisualDescendants().OfType<DesktopComboBox>().Single(control => control.Name == "PART_MonthSelect");
            var mode = header.GetVisualDescendants().OfType<DesktopOptionButtonGroup>().Single(control => control.Name == "PART_ModeSwitch");
            var padding = GetThemeResource<double>(SharedTokenKind.UniformlyPaddingSM);
            var contentHeight = year.Bounds.Height;
            year.DropDownDisplayPageSize.ShouldBe(8);
            month.DropDownDisplayPageSize.ShouldBe(8);
            header.Bounds.Height.ShouldBe(contentHeight + padding * 2, 0.5);
            year.TranslatePoint(new Point(), header)!.Value.Y.ShouldBe(padding, 0.5);
            month.TranslatePoint(new Point(), header)!.Value.Y.ShouldBe(padding, 0.5);
            mode.TranslatePoint(new Point(), header)!.Value.Y.ShouldBe(padding, 0.5);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Calendar_CellTemplate_PreservesValue_And_IgnoresWeekCells()
    {
        var calendar = new AtomUICalendar
        {
            Value = new DateTime(2026, 7, 15),
            ShowWeek = true,
            CellTemplate = CreateMarkerTemplate("cell")
        };
        var window = Show(calendar);
        try
        {
            var dateCell = calendar.GetVisualDescendants()
                .OfType<CalendarCellControl>()
                .First(c => c.Model is { Kind: CalendarViewCellKind.Date, Value.Day: 15 });

            dateCell.GetVisualDescendants()
                .OfType<Avalonia.Controls.Control>()
                .Any(c => Equals(c.Tag, "cell"))
                .ShouldBeTrue();

            var valueText = dateCell.GetVisualDescendants()
                .OfType<DesktopTextBlock>()
                .Single(tb => tb.Name == "PART_Value");
            valueText.IsVisible.ShouldBeTrue();
            valueText.Text.ShouldBe("15");

            var weekCell = calendar.GetVisualDescendants()
                .OfType<CalendarCellControl>()
                .First(c => c.Model is { Kind: CalendarViewCellKind.Week });

            weekCell.GetVisualDescendants()
                .OfType<Avalonia.Controls.Control>()
                .Any(c => Equals(c.Tag, "cell"))
                .ShouldBeFalse();

            var weekValue = weekCell.GetVisualDescendants()
                .OfType<DesktopTextBlock>()
                .Single(tb => tb.Name == "PART_Value");
            weekValue.IsVisible.ShouldBeTrue();
            weekValue.Text.ShouldNotBeEmpty();
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Calendar_FullCellTemplate_TakesPriorityOverCellTemplate()
    {
        var calendar = new AtomUICalendar
        {
            Value = new DateTime(2026, 7, 15),
            CellTemplate = CreateMarkerTemplate("cell"),
            FullCellTemplate = CreateMarkerTemplate("full")
        };
        var window = Show(calendar);
        try
        {
            var cell = calendar.GetVisualDescendants()
                .OfType<CalendarCellControl>()
                .First(c => c.Model is { Kind: CalendarViewCellKind.Date, Value.Day: 15 });

            cell.GetVisualDescendants()
                .OfType<Avalonia.Controls.Control>()
                .Any(c => Equals(c.Tag, "full"))
                .ShouldBeTrue();
            cell.GetVisualDescendants()
                .OfType<Avalonia.Controls.Control>()
                .Any(c => Equals(c.Tag, "cell"))
                .ShouldBeFalse();

            var valueText = cell.GetVisualDescendants()
                .OfType<DesktopTextBlock>()
                .Single(tb => tb.Name == "PART_Value");
            valueText.IsVisible.ShouldBeFalse();
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Calendar_HeaderLanguageAndMiniSizing_RefreshWithLanguageVariant()
    {
        var originalVariant = Application.Current!.GetLanguageVariant() ?? LanguageVariant.en_US;
        var calendar = new AtomUICalendar
        {
            Value = new DateTime(2026, 7, 15),
            Fullscreen = false,
            ShowWeek = true
        };
        var window = Show(calendar);
        try
        {
            Application.Current!.SetLanguageVariant(LanguageVariant.zh_CN);
            Dispatcher.UIThread.RunJobs();

            AssertHeaderLanguage(calendar, "月", "年", "周", "年");

            Application.Current!.SetLanguageVariant(LanguageVariant.en_US);
            Dispatcher.UIThread.RunJobs();

            AssertHeaderLanguage(calendar, "Month", "Year", "Week", string.Empty);
        }
        finally
        {
            window.Close();
            Application.Current!.SetLanguageVariant(originalVariant);
            Dispatcher.UIThread.RunJobs();
        }
    }

    [Fact]
    public void Calendar_LanguageVariant_DrivesCultureAndLanguageResources()
    {
        var originalVariant = Application.Current!.GetLanguageVariant() ?? LanguageVariant.en_US;
        Application.Current!.SetLanguageVariant(LanguageVariant.en_US);
        var calendar = new AtomUICalendar
        {
            Value = new DateTime(2026, 7, 15),
            Fullscreen = false,
            ShowWeek = true
        };
        var window = Show(calendar);
        try
        {
            Application.Current!.SetLanguageVariant(LanguageVariant.zh_TW);
            Dispatcher.UIThread.RunJobs();
            AssertHeaderLanguage(calendar, "月", "年", "週", "年");

            Application.Current!.SetLanguageVariant(LanguageVariant.zh_CN);
            Dispatcher.UIThread.RunJobs();
            AssertHeaderLanguage(calendar, "月", "年", "周", "年");

            var view = calendar.GetVisualDescendants().OfType<CalendarViewControl>().Single();
            view.Culture!.Name.ShouldBe("zh-CN");
        }
        finally
        {
            window.Close();
            Application.Current!.SetLanguageVariant(originalVariant);
            Dispatcher.UIThread.RunJobs();
        }
    }

    [Fact]
    public void Calendar_ReapplyTemplate_DoesNotDoubleFireSelection()
    {
        var calendar = new AtomUICalendar { Value = new DateTime(2026, 7, 15) };
        var window = Show(calendar);
        try
        {
            // 强制重新应用模板：旧 CalendarView/Header 订阅应被解绑
            calendar.ApplyTemplate();
            Dispatcher.UIThread.RunJobs();

            var fired = 0;
            calendar.Selected += (_, _) => fired++;

            var cell = calendar.GetVisualDescendants()
                .OfType<CalendarCellControl>()
                .First(c => c.Model is { IsInView: true, IsDisabled: false, Value.Day: 10 });
            cell.Activate();
            Dispatcher.UIThread.RunJobs();

            fired.ShouldBe(1); // 一次激活只触发一次,证明旧订阅未泄漏
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Calendar_DetachReleasesCells_AndReattachRestoresGrid()
    {
        var calendar = new AtomUICalendar { Value = new DateTime(2026, 7, 15) };
        var firstWindow = Show(calendar);
        var view = calendar.GetVisualDescendants().OfType<CalendarViewControl>().Single();

        firstWindow.Content = null;
        Dispatcher.UIThread.RunJobs();
        view.GetRealizedCells().ShouldBeEmpty();

        var secondWindow = Show(calendar);
        try
        {
            calendar.GetVisualDescendants().OfType<CalendarCellControl>().Count().ShouldBe(42);
        }
        finally
        {
            secondWindow.Close();
            firstWindow.Close();
        }
    }

    [Fact]
    public void Calendar_CellActivation_CommitsSelection()
    {
        var calendar = new AtomUICalendar { Value = new DateTime(2026, 7, 15) };
        DateTime? selected = null;
        calendar.Selected += (_, e) => selected = e.Value;

        var window = Show(calendar);
        try
        {
            var realizedCellsBeforeActivation = calendar.GetVisualDescendants()
                .OfType<CalendarCellControl>()
                .ToList();
            var cell = calendar.GetVisualDescendants()
                .OfType<CalendarCellControl>()
                .First(c => c.Model is { IsInView: true, IsDisabled: false, Value.Day: 20 });
            cell.Activate();
            Dispatcher.UIThread.RunJobs();

            selected.ShouldBe(new DateTime(2026, 7, 20));
            calendar.Value.ShouldBe(new DateTime(2026, 7, 20));
            calendar.GetVisualDescendants()
                .OfType<CalendarCellControl>()
                .ToList()
                .ShouldBe(realizedCellsBeforeActivation);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Calendar_FocusView_FocusesSelectedCell()
    {
        var calendar = new AtomUICalendar { Value = new DateTime(2026, 7, 15) };
        var window = Show(calendar);
        try
        {
            var view = calendar.GetVisualDescendants()
                .OfType<AtomUI.Desktop.Controls.Internal.Calendar.CalendarView>()
                .First();
            view.Focus();
            Dispatcher.UIThread.RunJobs();

            // 选中日期 7/15 的 cell 应带 :focused 伪类
            var focusedCell = calendar.GetVisualDescendants()
                .OfType<CalendarCellControl>()
                .FirstOrDefault(c => c.Classes.Contains(":focused"));
            focusedCell.ShouldNotBeNull();
            focusedCell!.Model!.Value.ShouldBe(new DateTime(2026, 7, 15));
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Calendar_ArrowKey_MovesFocusedValue()
    {
        var calendar = new AtomUICalendar { Value = new DateTime(2026, 7, 15) };
        var window = Show(calendar);
        try
        {
            var view = calendar.GetVisualDescendants()
                .OfType<AtomUI.Desktop.Controls.Internal.Calendar.CalendarView>()
                .First();
            view.Focus();
            Dispatcher.UIThread.RunJobs();

            view.RaiseEvent(new Avalonia.Input.KeyEventArgs
            {
                RoutedEvent = Avalonia.Input.InputElement.KeyDownEvent,
                Key = Avalonia.Input.Key.Right
            });
            Dispatcher.UIThread.RunJobs();

            view.FocusedValue.ShouldBe(new DateTime(2026, 7, 16));
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Calendar_FullscreenAndMini_TogglePseudoClassesOnView()
    {
        var calendar = new AtomUICalendar { Value = new DateTime(2026, 7, 15), Fullscreen = true };
        var window = Show(calendar);
        try
        {
            var view = calendar.GetVisualDescendants()
                .OfType<AtomUI.Desktop.Controls.Internal.Calendar.CalendarView>()
                .First();
            view.Classes.Contains(":fullscreen").ShouldBeTrue();
            view.Classes.Contains(":mini").ShouldBeFalse();

            calendar.Fullscreen = false;
            Dispatcher.UIThread.RunJobs();
            view.Classes.Contains(":mini").ShouldBeTrue();
            view.Classes.Contains(":fullscreen").ShouldBeFalse();
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Calendar_WeekHeader_UsesSameGridColumnsAsCellHost()
    {
        var calendar = new AtomUICalendar
        {
            Value = new DateTime(2026, 7, 15),
            ShowWeek = true
        };
        var window = Show(calendar);
        try
        {
            var header = calendar.GetVisualDescendants()
                .OfType<Panel>()
                .Single(panel => panel.Name == "PART_WeekHeader")
                .ShouldBeOfType<Avalonia.Controls.Grid>();
            var host = calendar.GetVisualDescendants()
                .OfType<Avalonia.Controls.Grid>()
                .Single(grid => grid.Name == "PART_CellHost");

            header.ColumnDefinitions.Count.ShouldBe(host.ColumnDefinitions.Count);
            header.Children.Count.ShouldBe(8);
            for (var column = 0; column < header.Children.Count; column++)
            {
                Avalonia.Controls.Grid.GetColumn(header.Children[column]).ShouldBe(column);
            }

            Math.Abs(header.Children[1].Bounds.Width - host.Children[1].Bounds.Width)
                .ShouldBeLessThan(1.0);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Calendar_CellTemplate_IsPlacedBelowValueRow()
    {
        var calendar = new AtomUICalendar
        {
            Value = new DateTime(2026, 7, 15),
            CellTemplate = CreateMarkerTemplate("cell")
        };
        var window = Show(calendar);
        try
        {
            var cell = calendar.GetVisualDescendants().OfType<CalendarCellControl>()
                .Single(item => item.Model is { IsSelected: true });
            var value = cell.GetVisualDescendants().OfType<DesktopTextBlock>()
                .Single(control => control.Name == "PART_Value");
            var content = cell.GetVisualDescendants().OfType<ContentControl>()
                .Single(control => control.Name == "PART_ItemContent");

            Avalonia.Controls.Grid.GetRow(value).ShouldBe(0);
            Avalonia.Controls.Grid.GetRow(content).ShouldBe(1);
            content.Bounds.Top.ShouldBeGreaterThanOrEqualTo(value.Bounds.Bottom);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Calendar_Fullscreen_StretchesAndUsesUnframedLayout()
    {
        var calendar = new AtomUICalendar { Value = new DateTime(2026, 7, 15) };
        var window = Show(calendar, 720, 900);
        try
        {
            calendar.Bounds.Width.ShouldBeGreaterThan(650);
            var root = calendar.GetVisualDescendants().OfType<Border>()
                .Single(border => border.Name == "PART_Root");
            root.BorderThickness.ShouldBe(default);

            var host = calendar.GetVisualDescendants().OfType<Avalonia.Controls.Grid>()
                .Single(grid => grid.Name == "PART_CellHost");
            host.Bounds.Height.ShouldBeGreaterThan(690);

            var firstCell = calendar.GetVisualDescendants()
                .OfType<CalendarCellControl>()
                .First(cell => cell.Model is { Kind: CalendarViewCellKind.Date });
            firstCell.Cursor.ShouldNotBeNull();
            firstCell.Cursor.ToString().ShouldContain("Hand");

            var firstCellInner = firstCell
                .GetVisualDescendants()
                .OfType<Border>()
                .Single(border => border.Name == "PART_CellInner");
            firstCellInner.Bounds.Height.ShouldBeGreaterThan(100);
            Math.Abs(firstCellInner.Bounds.Height - firstCell.Bounds.Height).ShouldBeLessThan(0.5);
            firstCellInner.CornerRadius.ShouldBe(default);

            var selectedCell = calendar.GetVisualDescendants()
                .OfType<CalendarCellControl>()
                .Single(cell => cell.Model is { IsSelected: true });
            var selectedItem = selectedCell
                .GetVisualDescendants()
                .OfType<Border>()
                .Single(border => border.Name == "PART_Item");
            selectedItem.Background.ShouldNotBeNull()
                .ShouldBeAssignableTo<ISolidColorBrush>()
                .Color.A.ShouldBe((byte)0);

            var selectedCellInner = selectedCell.GetVisualDescendants()
                .OfType<Border>()
                .Single(border => border.Name == "PART_CellInner");
            selectedCellInner.Background.ShouldNotBeNull()
                .ShouldBeAssignableTo<ISolidColorBrush>()
                .Color.A.ShouldBeGreaterThan((byte)0);
            selectedCellInner.BorderBrush.ShouldNotBeNull()
                .ShouldBeAssignableTo<ISolidColorBrush>()
                .Color.A.ShouldBeGreaterThan((byte)0);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Calendar_Mini_UsesUnframedCardContentLayout()
    {
        var calendar = new AtomUICalendar
        {
            Value = new DateTime(2026, 7, 15),
            Fullscreen = false,
            Width = 300
        };
        var window = Show(calendar, 720, 600);
        try
        {
            var root = calendar.GetVisualDescendants().OfType<Border>()
                .Single(border => border.Name == "PART_Root");
            root.BorderThickness.ShouldBe(default);
            root.Padding.ShouldBe(default);

            var view = calendar.GetVisualDescendants()
                .OfType<CalendarViewControl>()
                .Single();
            view.Bounds.Height.ShouldBe(256, 0.5);

            var host = calendar.GetVisualDescendants().OfType<Avalonia.Controls.Grid>()
                .Single(grid => grid.Name == "PART_CellHost");
            host.Bounds.Height.ShouldBeLessThan(255.5);

            var selectedCell = calendar.GetVisualDescendants()
                .OfType<CalendarCellControl>()
                .Single(cell => cell.Model is { IsSelected: true });
            var selectedCellInner = selectedCell
                .GetVisualDescendants()
                .OfType<Border>()
                .Single(border => border.Name == "PART_CellInner");
            selectedCellInner.CornerRadius.TopLeft.ShouldBeGreaterThan(0);
            BrushShouldHaveSameColor(
                selectedCellInner.Background,
                GetThemeResource<IBrush>(SharedTokenKind.ColorPrimary));

            var selectedValue = selectedCell.GetVisualDescendants()
                .OfType<DesktopTextBlock>()
                .Single(text => text.Name == "PART_Value");
            BrushShouldHaveSameColor(
                selectedValue.Foreground,
                GetThemeResource<IBrush>(SharedTokenKind.ColorTextLightSolid));
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Calendar_Mini_UsesCardDateStatesAndWeekHeaderStyle()
    {
        var today = DateTime.Today;
        var selectedDay = today.Day == 1 ? 2 : 1;
        var disabledDay = selectedDay is 10 ? 11 : 10;
        var calendar = new AtomUICalendar
        {
            Value = new DateTime(today.Year, today.Month, selectedDay),
            Fullscreen = false,
            Width = 300,
            DisabledDate = date => date.Year == today.Year &&
                                   date.Month == today.Month &&
                                   date.Day == disabledDay
        };
        var window = Show(calendar, 720, 600);
        try
        {
            var cells = calendar.GetVisualDescendants()
                .OfType<CalendarCellControl>()
                .ToList();

            var todayCell = cells.Single(cell => cell.Model is { IsToday: true });
            var todayInner = GetCellBorder(todayCell, "PART_CellInner");
            BrushShouldHaveSameColor(
                todayInner.BorderBrush,
                GetThemeResource<IBrush>(SharedTokenKind.ColorPrimary));
            todayInner.BorderThickness.ShouldBe(
                GetThemeResource<Thickness>(SharedTokenKind.BorderThickness));

            var outsideCell = cells.First(cell => cell.Model is
                { Kind: CalendarViewCellKind.Date, IsInView: false });
            var outsideValue = GetCellValue(outsideCell);
            BrushShouldHaveSameColor(
                outsideValue.Foreground,
                GetThemeResource<IBrush>(SharedTokenKind.ColorTextDisabled));

            var disabledCell = cells.Single(cell => cell.Model is
                { Kind: CalendarViewCellKind.Date, IsInView: true, IsDisabled: true });
            BrushShouldHaveSameColor(
                GetCellValue(disabledCell).Foreground,
                GetThemeResource<IBrush>(SharedTokenKind.ColorTextDisabled));
            BrushShouldHaveSameColor(
                GetCellBorder(disabledCell, "PART_Item").Background,
                GetThemeResource<IBrush>(SharedTokenKind.ColorBgContainerDisabled));

            var weekHeader = calendar.GetVisualDescendants()
                .OfType<Panel>()
                .Single(panel => panel.Name == "PART_WeekHeader");
            var weekText = weekHeader.Children.OfType<DesktopTextBlock>().First();
            BrushShouldHaveSameColor(
                weekText.Foreground,
                GetThemeResource<IBrush>(SharedTokenKind.ColorText));
            weekText.Bounds.Height.ShouldBeLessThan(
                GetThemeResource<double>(SharedTokenKind.ControlHeightSM));
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Calendar_FocusedCell_HasVisibleFullscreenFocusBorder()
    {
        var calendar = new AtomUICalendar { Value = new DateTime(2026, 7, 15) };
        var window = Show(calendar);
        try
        {
            var view = calendar.GetVisualDescendants().OfType<CalendarViewControl>().Single();
            view.Focus();
            Dispatcher.UIThread.RunJobs();

            var focused = calendar.GetVisualDescendants().OfType<CalendarCellControl>()
                .Single(cell => cell.Classes.Contains(":focused"));
            var inner = focused.GetVisualDescendants().OfType<Border>()
                .Single(border => border.Name == "PART_CellInner");
            inner.BorderBrush.ShouldNotBeNull();
            inner.BorderThickness.Top.ShouldBeGreaterThan(0);
            inner.BorderThickness.Left.ShouldBe(0);
            inner.BorderThickness.Right.ShouldBe(0);
            inner.BorderThickness.Bottom.ShouldBe(0);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Calendar_RuntimeModeSwitch_SwapsGrid()
    {
        var calendar = new AtomUICalendar { Value = new DateTime(2026, 7, 15) };
        var window = Show(calendar);
        try
        {
            // 初始 Month 模式:42 个日期容器
            calendar.GetVisualDescendants().OfType<CalendarCellControl>().Count().ShouldBe(42);
            var modeSwitch = calendar.GetVisualDescendants()
                .OfType<DesktopOptionButtonGroup>()
                .Single(control => control.Name == "PART_ModeSwitch");
            modeSwitch.ButtonStyle.ShouldBe(OptionButtonStyle.Outline);
            modeSwitch.SelectedIndex.ShouldBe(0);

            // Header 切到 Year:应更新公开 Mode 并换成 12 个月份容器
            modeSwitch.SelectedIndex = 1;
            Dispatcher.UIThread.RunJobs();
            calendar.Mode.ShouldBe(CalendarMode.Year);
            calendar.GetVisualDescendants().OfType<CalendarCellControl>().Count().ShouldBe(12);

            // Header 切回 Month:公开 Mode 和日期网格一起恢复
            modeSwitch.SelectedIndex = 0;
            Dispatcher.UIThread.RunJobs();
            calendar.Mode.ShouldBe(CalendarMode.Month);
            calendar.GetVisualDescendants().OfType<CalendarCellControl>().Count().ShouldBe(42);
        }
        finally
        {
            window.Close();
        }
    }

    private static Avalonia.Controls.Window Show(Control content, double width = 400, double height = 400)
    {
        var window = new Avalonia.Controls.Window
        {
            Width = width,
            Height = height,
            Content = content
        };
        window.Show();
        Dispatcher.UIThread.RunJobs();
        return window;
    }

    private static void AssertHeaderLanguage(
        AtomUICalendar calendar,
        string expectedMonthLabel,
        string expectedYearLabel,
        string expectedWeekLabel,
        string expectedYearSuffix)
    {
        var header = calendar.GetVisualDescendants()
            .OfType<CalendarHeaderControl>()
            .Single();

        var yearSelect = header.GetVisualDescendants()
            .OfType<DesktopComboBox>()
            .Single(control => control.Name == "PART_YearSelect");
        yearSelect.SizeType.ShouldBe(CustomizableSizeType.Small);

        var monthSelect = header.GetVisualDescendants()
            .OfType<DesktopComboBox>()
            .Single(control => control.Name == "PART_MonthSelect");
        monthSelect.SizeType.ShouldBe(CustomizableSizeType.Small);

        var modeSwitch = header.GetVisualDescendants()
            .OfType<DesktopOptionButtonGroup>()
            .Single(control => control.Name == "PART_ModeSwitch");
        modeSwitch.SizeType.ShouldBe(CustomizableSizeType.Small);

        var items = ((System.Collections.IEnumerable)yearSelect.ItemsSource!)
            .Cast<CalendarHeaderItem>()
            .ToList();
        items.ShouldNotBeEmpty();
        if (string.IsNullOrEmpty(expectedYearSuffix))
        {
            items[0].Display.ShouldNotContain("年");
        }
        else
        {
            items[0].Display.EndsWith(expectedYearSuffix).ShouldBeTrue();
        }

        var modeButtons = modeSwitch.GetVisualDescendants()
            .OfType<DesktopOptionButton>()
            .ToList();
        modeButtons.Any(item => Equals(item.Content, expectedMonthLabel)).ShouldBeTrue();
        modeButtons.Any(item => Equals(item.Content, expectedYearLabel)).ShouldBeTrue();

        var weekHeader = calendar.GetVisualDescendants()
            .OfType<Panel>()
            .Single(panel => panel.Name == "PART_WeekHeader");
        var weekText = weekHeader.Children.OfType<DesktopTextBlock>().First();
        weekText.Text.ShouldBe(expectedWeekLabel);
    }

    private static IDataTemplate CreateMarkerTemplate(string marker)
    {
        return new FuncDataTemplate<object?>((_, _) =>
        {
            return new Border
            {
                Tag = marker,
                Width = 8,
                Height = 8
            };
        });
    }

    private static Border GetCellBorder(CalendarCellControl cell, string name)
    {
        return cell.GetVisualDescendants()
            .OfType<Border>()
            .Single(border => border.Name == name);
    }

    private static DesktopTextBlock GetCellValue(CalendarCellControl cell)
    {
        return cell.GetVisualDescendants()
            .OfType<DesktopTextBlock>()
            .Single(text => text.Name == "PART_Value");
    }

    private static T GetThemeResource<T>(object key)
    {
        var application = Application.Current.ShouldNotBeNull();
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
}
