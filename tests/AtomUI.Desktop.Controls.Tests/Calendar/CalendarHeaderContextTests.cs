using System.Reflection;
using Shouldly;
using Xunit;
using AtomUICalendar = AtomUI.Desktop.Controls.Calendar;

namespace AtomUI.Desktop.Controls.Tests.Calendar;

public class CalendarHeaderContextTests
{
    static CalendarHeaderContextTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void HeaderContext_ExposesValueAndMode()
    {
        var c = new AtomUICalendar { Value = new DateTime(2026, 7, 1), Mode = CalendarMode.Year };
        var ctx = BuildHeaderContext(c);
        ctx.Value.ShouldBe(new DateTime(2026, 7, 1));
        ctx.Mode.ShouldBe(CalendarMode.Year);
    }

    [Fact]
    public void ChangeValueCommand_RejectsNonDateParameter()
    {
        var c = new AtomUICalendar { Value = new DateTime(2026, 7, 1) };
        var ctx = BuildHeaderContext(c);
        ctx.ChangeValueCommand.CanExecute("not-a-date").ShouldBeFalse();
        ctx.ChangeValueCommand.CanExecute(null).ShouldBeFalse();
        ctx.ChangeValueCommand.CanExecute(new DateTime(2026, 8, 1)).ShouldBeTrue();
    }

    [Fact]
    public void ChangeValueCommand_CommitsAsCustomizeSource()
    {
        var c = new AtomUICalendar { Value = new DateTime(2026, 7, 1) };
        CalendarSelectSource? seen = null;
        c.Selected += (_, e) => seen = e.Source;

        var ctx = BuildHeaderContext(c);
        ctx.ChangeValueCommand.Execute(new DateTime(2026, 9, 3));

        seen.ShouldBe(CalendarSelectSource.Customize);
        c.Value.ShouldBe(new DateTime(2026, 9, 3));
    }

    [Fact]
    public void ChangeModeCommand_RejectsInvalidParameter()
    {
        var c = new AtomUICalendar();
        var ctx = BuildHeaderContext(c);
        ctx.ChangeModeCommand.CanExecute("nope").ShouldBeFalse();
        ctx.ChangeModeCommand.CanExecute((CalendarMode)999).ShouldBeFalse();
        ctx.ChangeModeCommand.CanExecute(CalendarMode.Year).ShouldBeTrue();
    }

    [Fact]
    public void ChangeModeCommand_SwitchesModeAndRaisesPanelChanged()
    {
        var c = new AtomUICalendar { Value = new DateTime(2026, 7, 1) };
        var panelRaised = 0;
        c.PanelChanged += (_, _) => panelRaised++;

        var ctx = BuildHeaderContext(c);
        ctx.ChangeModeCommand.Execute(CalendarMode.Year);

        c.Mode.ShouldBe(CalendarMode.Year);
        panelRaised.ShouldBe(1);
    }

    private static CalendarHeaderContext BuildHeaderContext(AtomUICalendar c)
    {
        var m = typeof(AtomUICalendar).GetMethod(
            "BuildHeaderContext", BindingFlags.Instance | BindingFlags.NonPublic)!;
        return (CalendarHeaderContext)m.Invoke(c, Array.Empty<object>())!;
    }
}
