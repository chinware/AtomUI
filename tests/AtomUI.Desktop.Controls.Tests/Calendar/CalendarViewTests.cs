using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using AtomUI.Desktop.Controls.Internal.Calendar;
using Shouldly;
using Xunit;
using CalendarViewControl = AtomUI.Desktop.Controls.Internal.Calendar.CalendarView;

namespace AtomUI.Desktop.Controls.Tests.Calendar;

public class CalendarViewTests
{
    static CalendarViewTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void DateMode_Produces42CellModels()
    {
        var view = NewView(new DateTime(2026, 7, 15), CalendarViewMode.Date, showWeek: false);
        Rebuild(view);
        CellModelCount(view).ShouldBe(42);
    }

    [Fact]
    public void DateMode_ShowWeek_Produces48CellModels()
    {
        var view = NewView(new DateTime(2026, 7, 15), CalendarViewMode.Date, showWeek: true);
        Rebuild(view);
        CellModelCount(view).ShouldBe(48); // 42 date + 6 week
    }

    [Fact]
    public void MonthMode_Produces12CellModels()
    {
        var view = NewView(new DateTime(2026, 7, 15), CalendarViewMode.Month, showWeek: false);
        Rebuild(view);
        CellModelCount(view).ShouldBe(12);
    }

    [Fact]
    public void MonthMode_IgnoresShowWeek()
    {
        var view = NewView(new DateTime(2026, 7, 15), CalendarViewMode.Month, showWeek: true);
        Rebuild(view);
        CellModelCount(view).ShouldBe(12);
    }

    [Fact]
    public void RepeatedRebuild_KeepsCellModelCountBounded()
    {
        var view = NewView(new DateTime(2026, 7, 15), CalendarViewMode.Date, showWeek: false);
        for (var i = 0; i < 20; i++)
        {
            SetProp(view, "Value", new DateTime(2026, 7, 15).AddMonths(i));
            Rebuild(view);
        }

        CellModelCount(view).ShouldBe(42);
    }

    private static CalendarViewControl NewView(DateTime value, CalendarViewMode mode, bool showWeek)
    {
        var view = new CalendarViewControl();
        SetProp(view, "Value", value);
        SetProp(view, "Today", new DateTime(2026, 7, 30));
        SetProp(view, "ViewMode", mode);
        SetProp(view, "ShowWeek", showWeek);
        SetProp(view, "Culture", CultureInfo.InvariantCulture);
        return view;
    }

    private static void SetProp(CalendarViewControl view, string name, object value)
    {
        var p = typeof(CalendarViewControl).GetProperty(name,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!;
        p.SetValue(view, value);
    }

    private static void Rebuild(CalendarViewControl view)
    {
        var m = typeof(CalendarViewControl).GetMethod("RebuildCells",
            BindingFlags.Instance | BindingFlags.NonPublic)!;
        m.Invoke(view, Array.Empty<object>());
    }

    private static int CellModelCount(CalendarViewControl view)
    {
        var f = typeof(CalendarViewControl).GetField("_cellModels",
            BindingFlags.Instance | BindingFlags.NonPublic)!;
        var list = (IReadOnlyList<CalendarViewCellModel>)f.GetValue(view)!;
        return list.Count;
    }
}
