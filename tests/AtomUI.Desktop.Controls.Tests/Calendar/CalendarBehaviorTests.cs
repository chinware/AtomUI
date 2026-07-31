using System;
using System.Collections.Generic;
using System.Reflection;
using Avalonia.Data;
using Shouldly;
using Xunit;
using AtomUICalendar = AtomUI.Desktop.Controls.Calendar;

namespace AtomUI.Desktop.Controls.Tests.Calendar;

public class CalendarBehaviorTests
{
    static CalendarBehaviorTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Defaults_MatchSpec()
    {
        var c = new AtomUICalendar();
        c.Mode.ShouldBe(CalendarMode.Month);
        c.Fullscreen.ShouldBeTrue();
        c.ShowWeek.ShouldBeFalse();
        c.ValidRange.ShouldBeNull();
        c.DisabledDate.ShouldBeNull();
    }

    [Fact]
    public void ValueAndMode_DefaultBindingMode_IsTwoWay()
    {
        AtomUICalendar.ValueProperty.GetMetadata(typeof(AtomUICalendar))
            .DefaultBindingMode.ShouldBe(BindingMode.TwoWay);
        AtomUICalendar.ModeProperty.GetMetadata(typeof(AtomUICalendar))
            .DefaultBindingMode.ShouldBe(BindingMode.TwoWay);
    }

    [Fact]
    public void SettingValueProgrammatically_NormalizesTimeToDate_AndDoesNotRaiseUserEvents()
    {
        var c = new AtomUICalendar { Value = new DateTime(2026, 7, 1) };
        var fired = new List<string>();
        c.ValueChanged += (_, _) => fired.Add("value");
        c.Selected += (_, _) => fired.Add("selected");
        c.PanelChanged += (_, _) => fired.Add("panel");

        c.Value = new DateTime(2026, 8, 15, 9, 30, 0);

        c.Value.ShouldBe(new DateTime(2026, 8, 15));
        fired.ShouldBeEmpty();
    }

    [Fact]
    public void SettingModeProgrammatically_DoesNotRaiseUserEvents()
    {
        var c = new AtomUICalendar { Value = new DateTime(2026, 7, 1) };
        var fired = new List<string>();
        c.PanelChanged += (_, _) => fired.Add("panel");
        c.ValueChanged += (_, _) => fired.Add("value");
        c.Selected += (_, _) => fired.Add("selected");

        c.Mode = CalendarMode.Year;

        fired.ShouldBeEmpty();
    }

    [Fact]
    public void CommitUserSelection_SameMonthDifferentDay_RaisesValueThenSelected_NoPanel()
    {
        var c = new AtomUICalendar { Value = new DateTime(2026, 7, 1) };
        var order = new List<string>();
        c.PanelChanged += (_, _) => order.Add("panel");
        c.ValueChanged += (_, _) => order.Add("value");
        c.Selected += (_, _) => order.Add("selected");

        Commit(c, new DateTime(2026, 7, 20), CalendarSelectSource.Date);

        order.ShouldBe(new[] { "value", "selected" });
        c.Value.ShouldBe(new DateTime(2026, 7, 20));
    }

    [Fact]
    public void CommitUserSelection_AdjacentMonth_RaisesPanelThenValueThenSelected()
    {
        var c = new AtomUICalendar { Value = new DateTime(2026, 7, 1) };
        var order = new List<string>();
        c.PanelChanged += (_, _) => order.Add("panel");
        c.ValueChanged += (_, _) => order.Add("value");
        c.Selected += (_, _) => order.Add("selected");

        Commit(c, new DateTime(2026, 8, 2), CalendarSelectSource.Date);

        order.ShouldBe(new[] { "panel", "value", "selected" });
    }

    [Fact]
    public void CommitUserSelection_SameDate_RaisesOnlySelected()
    {
        var c = new AtomUICalendar { Value = new DateTime(2026, 7, 20) };
        var order = new List<string>();
        c.PanelChanged += (_, _) => order.Add("panel");
        c.ValueChanged += (_, _) => order.Add("value");
        c.Selected += (_, _) => order.Add("selected");

        Commit(c, new DateTime(2026, 7, 20), CalendarSelectSource.Date);

        order.ShouldBe(new[] { "selected" });
    }

    [Fact]
    public void CommitUserSelection_YearMode_SameYearDifferentMonth_NoPanel_RaisesValueThenSelected()
    {
        var c = new AtomUICalendar { Value = new DateTime(2026, 3, 15), Mode = CalendarMode.Year };
        var order = new List<string>();
        c.PanelChanged += (_, _) => order.Add("panel");
        c.ValueChanged += (_, _) => order.Add("value");
        c.Selected += (_, _) => order.Add("selected");

        // Year 模式下同年跨月不触发 PanelChanged
        Commit(c, new DateTime(2026, 9, 15), CalendarSelectSource.Month);

        order.ShouldBe(new[] { "value", "selected" });
    }

    [Fact]
    public void CommitUserSelection_YearMode_CrossYear_RaisesPanelThenValueThenSelected()
    {
        var c = new AtomUICalendar { Value = new DateTime(2026, 3, 15), Mode = CalendarMode.Year };
        var order = new List<string>();
        c.PanelChanged += (_, _) => order.Add("panel");
        c.ValueChanged += (_, _) => order.Add("value");
        c.Selected += (_, _) => order.Add("selected");

        Commit(c, new DateTime(2027, 3, 15), CalendarSelectSource.Year);

        order.ShouldBe(new[] { "panel", "value", "selected" });
    }

    [Fact]
    public void CommitModeChange_RaisesSinglePanelChanged_NoValueOrSelected()
    {
        var c = new AtomUICalendar { Value = new DateTime(2026, 7, 1) };
        var order = new List<string>();
        c.PanelChanged += (_, _) => order.Add("panel");
        c.ValueChanged += (_, _) => order.Add("value");
        c.Selected += (_, _) => order.Add("selected");

        CommitMode(c, CalendarMode.Year);

        order.ShouldBe(new[] { "panel" });
        c.Mode.ShouldBe(CalendarMode.Year);
    }

    [Fact]
    public void CommitUserSelection_NormalizesTimeToDate()
    {
        var c = new AtomUICalendar { Value = new DateTime(2026, 7, 1) };
        Commit(c, new DateTime(2026, 7, 20, 9, 30, 0), CalendarSelectSource.Date);
        c.Value.ShouldBe(new DateTime(2026, 7, 20));
    }

    [Fact]
    public void PanelChanged_CarriesNewValueAndMode()
    {
        var c = new AtomUICalendar { Value = new DateTime(2026, 7, 1) };
        CalendarPanelChangedEventArgs? seen = null;
        c.PanelChanged += (_, e) => seen = e;

        Commit(c, new DateTime(2026, 8, 2), CalendarSelectSource.Date);

        seen.ShouldNotBeNull();
        seen!.Value.ShouldBe(new DateTime(2026, 8, 2));
        seen.Mode.ShouldBe(CalendarMode.Month);
    }

    private static void Commit(AtomUICalendar c, DateTime target, CalendarSelectSource src)
    {
        var m = typeof(AtomUICalendar).GetMethod(
            "CommitUserSelection", BindingFlags.Instance | BindingFlags.NonPublic)!;
        m.Invoke(c, new object[] { target, src });
    }

    private static void CommitMode(AtomUICalendar c, CalendarMode mode)
    {
        var m = typeof(AtomUICalendar).GetMethod(
            "CommitModeChange", BindingFlags.Instance | BindingFlags.NonPublic)!;
        m.Invoke(c, new object[] { mode });
    }
}
