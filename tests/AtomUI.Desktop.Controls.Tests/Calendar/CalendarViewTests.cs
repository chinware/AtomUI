using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using AtomUI.Desktop.Controls.Internal.Calendar;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
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
    public void ComputeFocusTarget_MonthMode_LeftRightMoveOneMonth_UpDownThree()
    {
        var view = NewView(new DateTime(2026, 6, 15), CalendarViewMode.Month, showWeek: false);
        Rebuild(view);
        ComputeFocus(view, new DateTime(2026, 6, 15), "Right").Month.ShouldBe(7);
        ComputeFocus(view, new DateTime(2026, 6, 15), "Left").Month.ShouldBe(5);
        ComputeFocus(view, new DateTime(2026, 6, 15), "Down").Month.ShouldBe(9);
        ComputeFocus(view, new DateTime(2026, 6, 15), "Up").Month.ShouldBe(3);
    }

    [Fact]
    public void ComputeFocusTarget_DateMode_SkipsDisabledCells()
    {
        var view = NewView(
            new DateTime(2026, 7, 15),
            CalendarViewMode.Date,
            showWeek: false,
            disabledDate: d => d.Day is 16 or 17);
        Rebuild(view);

        ComputeFocus(view, new DateTime(2026, 7, 15), "Right").ShouldBe(new DateTime(2026, 7, 18));
    }

    [Fact]
    public void ComputeFocusTarget_MonthMode_SkipsDisabledCells()
    {
        var view = NewView(
            new DateTime(2026, 7, 15),
            CalendarViewMode.Month,
            showWeek: false,
            disabledDate: d => d.Month is 8 or 9);
        Rebuild(view);

        ComputeFocus(view, new DateTime(2026, 7, 15), "Right").Month.ShouldBe(10);
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

    [Fact]
    public void ReportCellActivated_Week_SelectsRowStart()
    {
        var view = NewView(new DateTime(2026, 7, 15), CalendarViewMode.Date, showWeek: true);
        Rebuild(view);
        CalendarCellSelectedEventArgs? selected = null;
        view.CellSelected += (_, e) => selected = e;

        var week = view.CellModels.First(model => model.Kind == CalendarViewCellKind.Week);
        view.ReportCellActivated(week);

        selected.ShouldNotBeNull();
        selected!.Value.ShouldBe(new DateTime(2026, 6, 28));
        selected.Kind.ShouldBe(CalendarViewCellKind.Week);
    }

    [Fact]
    public void ReportCellActivated_DisabledWeek_DoesNotSelect()
    {
        var view = NewView(
            new DateTime(2026, 7, 15),
            CalendarViewMode.Date,
            showWeek: true,
            disabledDate: date => date == new DateTime(2026, 6, 28));
        Rebuild(view);
        var fired = false;
        view.CellSelected += (_, _) => fired = true;

        var week = view.CellModels.First(model => model.Kind == CalendarViewCellKind.Week);
        week.IsDisabled.ShouldBeTrue();
        view.ReportCellActivated(week);

        fired.ShouldBeFalse();
    }

    [Fact]
    public void ComputeFocusTarget_DateTimeBoundary_DoesNotOverflow()
    {
        var minView = NewView(DateTime.MinValue, CalendarViewMode.Date, showWeek: false);
        Rebuild(minView);
        ComputeFocus(minView, DateTime.MinValue, "Left").ShouldBe(DateTime.MinValue);
        ComputeFocus(minView, DateTime.MinValue, "Up").ShouldBe(DateTime.MinValue);

        var maxView = NewView(DateTime.MaxValue, CalendarViewMode.Month, showWeek: false);
        Rebuild(maxView);
        ComputeFocus(maxView, DateTime.MaxValue, "Right").ShouldBe(DateTime.MaxValue.Date);
        ComputeFocus(maxView, DateTime.MaxValue, "Down").ShouldBe(DateTime.MaxValue.Date);
    }

    [Fact]
    public void ReapplyTemplate_ReleasesOldHostBeforeReusingCells()
    {
        var view = NewView(new DateTime(2026, 7, 15), CalendarViewMode.Date, showWeek: false);
        var firstHost = ApplyTemplateParts(view);
        firstHost.Children.Count.ShouldBe(42);

        var secondHost = ApplyTemplateParts(view);

        firstHost.Children.ShouldBeEmpty();
        secondHost.Children.Count.ShouldBe(42);
    }

    [Fact]
    public void SwitchingToMonthMode_UnbindsInactivePooledCells()
    {
        var view = NewView(new DateTime(2026, 7, 15), CalendarViewMode.Date, showWeek: false);
        SetProp(view, "CellTemplate", new FuncDataTemplate<object?>((_, _) => new Border()));
        ApplyTemplateParts(view);

        SetProp(view, "ViewMode", CalendarViewMode.Month);

        var pool = GetCellPool(view);
        pool.Count.ShouldBe(42);
        foreach (var cell in pool.Take(12))
        {
            cell.Model.ShouldNotBeNull();
            cell.CellTemplate.ShouldNotBeNull();
        }

        foreach (var cell in pool.Skip(12))
        {
            cell.Model.ShouldBeNull();
            cell.Context.ShouldBeNull();
            cell.CellTemplate.ShouldBeNull();
            cell.FullCellTemplate.ShouldBeNull();
        }
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

    private static CalendarViewControl NewView(DateTime value, CalendarViewMode mode, bool showWeek, Func<DateTime, bool>? disabledDate = null)
    {
        var view = new CalendarViewControl();
        SetProp(view, "Value", value);
        SetProp(view, "Today", new DateTime(2026, 7, 30));
        SetProp(view, "ViewMode", mode);
        SetProp(view, "ShowWeek", showWeek);
        SetProp(view, "DisabledDate", disabledDate);
        SetProp(view, "Culture", CultureInfo.InvariantCulture);
        return view;
    }

    private static void SetProp(CalendarViewControl view, string name, object? value)
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

    private static Avalonia.Controls.Grid ApplyTemplateParts(CalendarViewControl view)
    {
        var scope = new NameScope();
        var weekHeader = new StackPanel { Name = "PART_WeekHeader" };
        var host = new Avalonia.Controls.Grid { Name = "PART_CellHost" };
        scope.Register("PART_WeekHeader", weekHeader);
        scope.Register("PART_CellHost", host);

        var method = typeof(CalendarViewControl).GetMethod(
            "OnApplyTemplate", BindingFlags.Instance | BindingFlags.NonPublic)!;
        method.Invoke(view, new object[] { new TemplateAppliedEventArgs(scope) });
        return host;
    }

    private static IReadOnlyList<CalendarViewCell> GetCellPool(CalendarViewControl view)
    {
        var field = typeof(CalendarViewControl).GetField(
            "_cellPool", BindingFlags.Instance | BindingFlags.NonPublic)!;
        return (IReadOnlyList<CalendarViewCell>)field.GetValue(view)!;
    }
}
