using Avalonia.Controls;
using Avalonia;
using Avalonia.Threading;
using Avalonia.VisualTree;
using AtomUI.Localization;
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

    [Fact]
    public void TimePicker_Period_Items_Refresh_When_Language_Changes()
    {
        RunOnUIThread(() =>
        {
            var languageManager = Application.Current!.GetLanguageManager().ShouldNotBeNull();
            var originalLanguage = languageManager.Current.CurrentLanguage;
            languageManager.ChangeLanguage(LanguageTags.EnUS);

            var picker = new TestTimePicker
            {
                ClockIdentifier = ClockIdentifierType.HourClock12,
                SelectedTime     = new TimeSpan(14, 25, 30)
            };
            var presenter = picker.CreatePickerPresenterForTest();
            picker.NotifyPickerOpenedForTest();

            try
            {
                ShowInWindow(presenter, () =>
                {
                    var timeView = presenter.GetVisualDescendants()
                                            .OfType<TimeView>()
                                            .Single();
                    var periodPanel = FindPanel(timeView, "PART_PeriodSelector");
                    var periodHost = timeView.GetVisualDescendants()
                                             .OfType<Panel>()
                                             .Single(panel => panel.Name == "PART_PeriodHost");

                    GetPeriodText(periodPanel, 0).ShouldBe("AM");
                    GetPeriodText(periodPanel, 1).ShouldBe("PM");
                    var hostWidth = periodHost.Bounds.Width;

                    languageManager.ChangeLanguage(LanguageTags.ZhCN);
                    Dispatcher.UIThread.RunJobs();

                    GetPeriodText(periodPanel, 0).ShouldBe("上午");
                    GetPeriodText(periodPanel, 1).ShouldBe("下午");
                    periodHost.Bounds.Width.ShouldBe(hostWidth, 0.001);
                    periodHost.Bounds.Width.ShouldBeGreaterThanOrEqualTo(
                        periodPanel.Children.OfType<TimeViewCell>().Max(cell => cell.DesiredSize.Width));
                });
            }
            finally
            {
                languageManager.ChangeLanguage(originalLanguage);
            }
        });
    }

    private static DateTimePickerPanel FindPanel(Control control, string name)
    {
        return control.GetVisualDescendants()
                      .OfType<DateTimePickerPanel>()
                      .Single(panel => panel.Name == name);
    }

    private static string? GetPeriodText(DateTimePickerPanel panel, int value)
    {
        return panel.Children
                    .OfType<TimeViewCell>()
                    .Single(cell => (int)cell.Tag! == value)
                    .Content
                    .ShouldBeOfType<TextBlock>()
                    .Text;
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
