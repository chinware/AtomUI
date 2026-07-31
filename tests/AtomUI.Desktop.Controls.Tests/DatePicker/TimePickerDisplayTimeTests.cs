using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.DatePickers;

public class TimePickerDisplayTimeTests
{
    static TimePickerDisplayTimeTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void TimePicker_Open_With_PickerDisplayTime_Anchors_TimeView_Without_Selecting_Time()
    {
        RunOnUIThread(() =>
        {
            var displayTime = new TimeSpan(14, 25, 30);
            var picker = new TestTimePicker
            {
                ClockIdentifier  = ClockIdentifierType.HourClock24,
                PickerDisplayTime = displayTime
            };

            var presenter = picker.CreatePickerPresenterForTest();
            picker.NotifyPickerOpenedForTest();

            ShowInWindow(presenter, () =>
            {
                var timeView = presenter.GetVisualDescendants()
                                        .OfType<TimeView>()
                                        .Single();

                FindPanel(timeView, "PART_HourSelector").SelectedValue.ShouldBe(14);
                FindPanel(timeView, "PART_MinuteSelector").SelectedValue.ShouldBe(25);
                FindPanel(timeView, "PART_SecondSelector").SelectedValue.ShouldBe(30);
                presenter.SelectedTime.ShouldBeNull();
                picker.SelectedTime.ShouldBeNull();
            });
        });
    }

    [Fact]
    public void TimePicker_Open_Uses_SelectedTime_Before_PickerDisplayTime()
    {
        RunOnUIThread(() =>
        {
            var selectedTime = new TimeSpan(9, 10, 11);
            var picker = new TestTimePicker
            {
                ClockIdentifier  = ClockIdentifierType.HourClock24,
                PickerDisplayTime = new TimeSpan(14, 25, 30),
                SelectedTime      = selectedTime
            };

            var presenter = picker.CreatePickerPresenterForTest();
            picker.NotifyPickerOpenedForTest();

            ShowInWindow(presenter, () =>
            {
                var timeView = presenter.GetVisualDescendants()
                                        .OfType<TimeView>()
                                        .Single();

                timeView.SelectedTime.ShouldBe(selectedTime);
                FindPanel(timeView, "PART_HourSelector").SelectedValue.ShouldBe(9);
                FindPanel(timeView, "PART_MinuteSelector").SelectedValue.ShouldBe(10);
                FindPanel(timeView, "PART_SecondSelector").SelectedValue.ShouldBe(11);
                presenter.SelectedTime.ShouldBe(selectedTime);
                picker.SelectedTime.ShouldBe(selectedTime);
            });
        });
    }

    private static DateTimePickerPanel FindPanel(Control control, string name)
    {
        return control.GetVisualDescendants()
                      .OfType<DateTimePickerPanel>()
                      .Single(panel => panel.Name == name);
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

    private sealed class TestTimePicker : TimePicker
    {
        public TimePickerPresenter CreatePickerPresenterForTest()
        {
            var presenter = CreatePickerPresenter().ShouldBeAssignableTo<TimePickerPresenter>();
            NotifyPickerPresenterCreated(presenter);
            return presenter;
        }

        public void NotifyPickerOpenedForTest()
        {
            NotifyPickerOpened();
        }
    }
}
