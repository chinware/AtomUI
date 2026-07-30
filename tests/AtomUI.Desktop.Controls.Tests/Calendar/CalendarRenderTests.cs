using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AtomUICalendar = AtomUI.Desktop.Controls.Calendar;
using CalendarViewControl = AtomUI.Desktop.Controls.Internal.Calendar.CalendarView;
using CalendarCellControl = AtomUI.Desktop.Controls.Internal.Calendar.CalendarViewCell;

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
    public void Calendar_CellActivation_CommitsSelection()
    {
        var calendar = new AtomUICalendar { Value = new DateTime(2026, 7, 15) };
        DateTime? selected = null;
        calendar.Selected += (_, e) => selected = e.Value;

        var window = Show(calendar);
        try
        {
            var cell = calendar.GetVisualDescendants()
                .OfType<CalendarCellControl>()
                .First(c => c.Model is { IsInView: true, IsDisabled: false, Value.Day: 20 });
            cell.Activate();
            Dispatcher.UIThread.RunJobs();

            selected.ShouldBe(new DateTime(2026, 7, 20));
            calendar.Value.ShouldBe(new DateTime(2026, 7, 20));
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

            // 运行时切到 Year:应换成 12 个月份容器
            calendar.Mode = CalendarMode.Year;
            Dispatcher.UIThread.RunJobs();
            calendar.GetVisualDescendants().OfType<CalendarCellControl>().Count().ShouldBe(12);

            // 切回 Month:回到 42
            calendar.Mode = CalendarMode.Month;
            Dispatcher.UIThread.RunJobs();
            calendar.GetVisualDescendants().OfType<CalendarCellControl>().Count().ShouldBe(42);
        }
        finally
        {
            window.Close();
        }
    }

    private static Avalonia.Controls.Window Show(Control content)
    {
        var window = new Avalonia.Controls.Window
        {
            Width   = 400,
            Height  = 400,
            Content = content
        };
        window.Show();
        Dispatcher.UIThread.RunJobs();
        return window;
    }
}
