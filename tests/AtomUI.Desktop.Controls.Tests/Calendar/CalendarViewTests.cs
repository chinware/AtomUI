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

    [Fact]
    public void CultureChange_RebuildsCellModels()
    {
        var view = NewView(new DateTime(2026, 7, 15), CalendarViewMode.Month, showWeek: false);
        Rebuild(view);
        var enJan = view.CellModels[0].DisplayText;

        SetProp(view, "Culture", new System.Globalization.CultureInfo("zh-CN"));
        // 属性变更会触发 RebuildCells
        var zhJan = view.CellModels[0].DisplayText;

        // 中文与英文的一月短名不同(Jan vs 1月),验证重建生效
        zhJan.ShouldNotBe(enJan);
    }

    [Fact]
    public void ComputeFocusTarget_DateMode_LeftRightMoveOneDay()
    {
        var view = NewView(new DateTime(2026, 7, 15), CalendarViewMode.Date, showWeek: false);
        Rebuild(view);
        ComputeFocus(view, new DateTime(2026, 7, 15), "Right").ShouldBe(new DateTime(2026, 7, 16));
        ComputeFocus(view, new DateTime(2026, 7, 15), "Left").ShouldBe(new DateTime(2026, 7, 14));
    }

    [Fact]
    public void ComputeFocusTarget_DateMode_UpDownMoveOneWeek()
    {
        var view = NewView(new DateTime(2026, 7, 15), CalendarViewMode.Date, showWeek: false);
        Rebuild(view);
        ComputeFocus(view, new DateTime(2026, 7, 15), "Down").ShouldBe(new DateTime(2026, 7, 22));
        ComputeFocus(view, new DateTime(2026, 7, 15), "Up").ShouldBe(new DateTime(2026, 7, 8));
    }

    [Fact]
    public void ComputeFocusTarget_MonthMode_LeftRightMoveOneMonth_UpDownFour()
    {
        var view = NewView(new DateTime(2026, 6, 15), CalendarViewMode.Month, showWeek: false);
        Rebuild(view);
        ComputeFocus(view, new DateTime(2026, 6, 15), "Right").Month.ShouldBe(7);
        ComputeFocus(view, new DateTime(2026, 6, 15), "Left").Month.ShouldBe(5);
        ComputeFocus(view, new DateTime(2026, 6, 15), "Down").Month.ShouldBe(10);
        ComputeFocus(view, new DateTime(2026, 6, 15), "Up").Month.ShouldBe(2);
    }

    [Fact]
    public void ComputeFocusTarget_StopsOutsideRealizedGrid()
    {
        var view = NewView(new DateTime(2026, 7, 15), CalendarViewMode.Date, showWeek: false);
        Rebuild(view);
        // 网格首日 2026-06-28,再往上一周越界 → 保持原值
        var gridStart = new DateTime(2026, 6, 28);
        ComputeFocus(view, gridStart, "Up").ShouldBe(gridStart);
    }

    private static DateTime ComputeFocus(CalendarViewControl view, DateTime current, string dir)
    {
        var dirType = typeof(CalendarViewControl).GetNestedType("FocusDirection",
            System.Reflection.BindingFlags.NonPublic)!;
        var dirVal = Enum.Parse(dirType, dir);
        var m = typeof(CalendarViewControl).GetMethod("ComputeFocusTarget",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!;
        return (DateTime)m.Invoke(view, new object[] { current, dirVal })!;
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
