using System.Globalization;
using AtomUI.Desktop.Controls.CalendarView.State;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.DatePickers;

public class CalendarViewStateControllerTests
{
    [Fact]
    public void SetDisplayRange_Normalizes_End_When_Start_Is_After_End()
    {
        var controller = new CalendarViewStateController(
            CalendarViewState.CreateDefault(new DateTime(2026, 6, 1),
                CultureInfo.InvariantCulture.DateTimeFormat));

        controller.Apply(CalendarViewAction.SetDisplayRange(new DateTime(2026, 7, 10),
            new DateTime(2026, 7, 1)));

        controller.State.DisplayDateStart.ShouldBe(new DateTime(2026, 7, 10));
        controller.State.DisplayDateEnd.ShouldBe(new DateTime(2026, 7, 10));
    }

    [Fact]
    public void SelectDate_Updates_Selected_And_DisplayMonth_When_Date_Is_In_Different_Month()
    {
        var controller = new CalendarViewStateController(
            CalendarViewState.CreateDefault(new DateTime(2026, 6, 1),
                CultureInfo.InvariantCulture.DateTimeFormat));

        controller.Apply(CalendarViewAction.SelectDate(new DateTime(2026, 8, 12)));

        controller.State.SelectedDate.ShouldBe(new DateTime(2026, 8, 12));
        controller.State.DisplayDate.ShouldBe(new DateTime(2026, 8, 1));
    }
}
