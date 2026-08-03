using System.Globalization;
using System.Reflection;
using AtomUI.Desktop.Controls.Internal.Calendar;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using CalendarViewControl = AtomUI.Desktop.Controls.Internal.Calendar.CalendarView;
using GridControl = Avalonia.Controls.Grid;
using LunarCalendarControl = AtomUI.Desktop.Controls.LunarCalendar;

namespace AtomUI.Desktop.Controls.Tests.Calendar;

public class LunarCalendarProviderTests
{
    static LunarCalendarProviderTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void MonthPanelData_QueriesProviderOnceAndReusesItForSamePanelSelection()
    {
        var provider = new RecordingProvider([
            new LunarCalendarHoliday(new DateTime(2026, 7, 15), "Holiday", LunarCalendarHolidayKind.Holiday)
        ]);
        var calendar = new LunarCalendarControl
        {
            Value = new DateTime(2026, 7, 15),
            HolidayProvider = provider
        };
        var view = CreateView(calendar);
        ApplyTemplateParts(view);

        provider.CallCount.ShouldBe(1);
        provider.LastRange!.Start.ShouldBe(new DateTime(2026, 6, 28));
        provider.LastRange.End.ShouldBe(new DateTime(2026, 8, 8));

        view.Value = new DateTime(2026, 7, 16);
        provider.CallCount.ShouldBe(1);

        view.Fullscreen = false;
        provider.CallCount.ShouldBe(1);

        view.Value = new DateTime(2026, 8, 16);
        provider.CallCount.ShouldBe(2);
    }

    [Fact]
    public void YearModeAndDisabledHolidayDisplay_DoNotQueryProvider()
    {
        var provider = new RecordingProvider([]);
        var calendar = new LunarCalendarControl
        {
            Value = new DateTime(2026, 7, 15),
            HolidayProvider = provider,
            ShowHolidays = false
        };
        var view = CreateView(calendar);
        ApplyTemplateParts(view);
        provider.CallCount.ShouldBe(0);

        calendar.ShowHolidays = true;
        view.ViewMode = CalendarViewMode.Month;
        provider.CallCount.ShouldBe(0);
    }

    [Fact]
    public void ProviderRows_AreCopiedFilteredAndLastDuplicateWins()
    {
        var source = new List<LunarCalendarHoliday>
        {
            new(new DateTime(2026, 7, 15), "First", LunarCalendarHolidayKind.Holiday),
            new(new DateTime(2026, 7, 15, 18, 0, 0), "Work", LunarCalendarHolidayKind.Workday),
            new(new DateTime(2027, 1, 1), "Outside", LunarCalendarHolidayKind.Holiday)
        };
        var provider = new RecordingProvider(source);
        var calendar = new LunarCalendarControl
        {
            Value = new DateTime(2026, 7, 15),
            HolidayProvider = provider
        };
        var view = CreateView(calendar);
        ApplyTemplateParts(view);
        source.Clear();

        var cell = GetLunarCells(view).Single(item => item.Model?.Value == new DateTime(2026, 7, 15));
        cell.LunarContext!.Holiday!.Name.ShouldBe("Work");
        cell.LunarContext.IsAdjustedWorkday.ShouldBeTrue();
        cell.LunarContext.IsWeekend.ShouldBeFalse();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void UnavailableOrSuccessfulEmptyProviderResult_ProducesNoHolidayAnnotations(bool succeeds)
    {
        var provider = new ConfigurableProvider(succeeds, []);
        var calendar = new LunarCalendarControl
        {
            Value = new DateTime(2026, 7, 15),
            HolidayProvider = provider
        };
        var view = CreateView(calendar);

        ApplyTemplateParts(view);

        provider.CallCount.ShouldBe(1);
        foreach (var cell in GetLunarCells(view).Where(cell => cell.Model?.Kind == CalendarViewCellKind.Date))
        {
            cell.LunarContext!.Holiday.ShouldBeNull();
        }
    }

    [Fact]
    public void SuccessfulProviderResult_WithNullCollection_IsAContractViolation()
    {
        var calendar = new LunarCalendarControl
        {
            Value = new DateTime(2026, 7, 15),
            HolidayProvider = new ConfigurableProvider(true, null)
        };

        Should.Throw<TargetInvocationException>(() => ApplyTemplateParts(CreateView(calendar)))
            .InnerException.ShouldBeOfType<InvalidOperationException>();
    }

    [Fact]
    public void EmptyHolidayName_PreservesHolidayStateAndFallsBackToBuiltInSecondaryText()
    {
        var calendar = new LunarCalendarControl
        {
            Value = new DateTime(2024, 2, 10),
            HolidayProvider = new RecordingProvider([
                new LunarCalendarHoliday(new DateTime(2024, 2, 10), string.Empty, LunarCalendarHolidayKind.Holiday)
            ])
        };
        var view = CreateView(calendar);

        ApplyTemplateParts(view);

        var cell = GetLunarCells(view).Single(item => item.Model?.Value == new DateTime(2024, 2, 10));
        cell.LunarContext!.IsHoliday.ShouldBeTrue();
        cell.LunarContext.Holiday.ShouldNotBeNull();
        cell.LunarContext.SecondaryContentKind.ShouldBe(LunarCalendarSecondaryContentKind.TraditionalFestival);
        cell.LunarContext.SecondaryText.ShouldNotBeEmpty();
    }

    [Fact]
    public void RefreshHolidayData_InvalidatesCurrentPanelData()
    {
        var provider = new RecordingProvider([]);
        var calendar = new LunarCalendarControl
        {
            Value = new DateTime(2026, 7, 15),
            HolidayProvider = provider
        };
        var view = CreateView(calendar);
        ApplyTemplateParts(view);
        provider.CallCount.ShouldBe(1);

        calendar.RefreshHolidayData();
        view.RefreshPresentation();

        provider.CallCount.ShouldBe(2);
    }

    [Fact]
    public void RefreshHolidayData_RefreshesRealizedLunarCellThroughCalendarRoot()
    {
        var provider = new MutableProvider();
        var calendar = new LunarCalendarControl
        {
            Value = new DateTime(2024, 2, 10),
            HolidayProvider = provider
        };
        var window = Show(calendar);
        try
        {
            var cell = calendar.GetVisualDescendants()
                .OfType<LunarCalendarViewCell>()
                .Single(item => item.Model?.Value == calendar.Value);
            cell.LunarContext!.Holiday.ShouldBeNull();

            provider.Holidays = [
                new LunarCalendarHoliday(calendar.Value, "春节假期", LunarCalendarHolidayKind.Holiday)
            ];
            calendar.RefreshHolidayData();
            Avalonia.Threading.Dispatcher.UIThread.RunJobs();

            cell.LunarContext!.Holiday!.Name.ShouldBe("春节假期");
            provider.CallCount.ShouldBe(2);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void InvalidProviderKindAndProviderException_AreNotSwallowed()
    {
        var invalid = new LunarCalendarControl
        {
            Value = new DateTime(2026, 7, 15),
            HolidayProvider = new RecordingProvider([
                new LunarCalendarHoliday(new DateTime(2026, 7, 15), "Bad", (LunarCalendarHolidayKind)99)
            ])
        };
        Should.Throw<TargetInvocationException>(() => ApplyTemplateParts(CreateView(invalid)))
            .InnerException.ShouldBeOfType<ArgumentOutOfRangeException>();

        var throwing = new LunarCalendarControl
        {
            Value = new DateTime(2026, 7, 15),
            HolidayProvider = new ThrowingProvider()
        };
        Should.Throw<TargetInvocationException>(() => ApplyTemplateParts(CreateView(throwing)))
            .InnerException.ShouldBeOfType<InvalidOperationException>();
    }

    private static CalendarViewControl CreateView(LunarCalendarControl calendar) => new()
    {
        Value = calendar.Value,
        Today = calendar.Value,
        Culture = CultureInfo.GetCultureInfo("en-US"),
        PresentationAdapter = (LunarCalendarPresentationAdapter)typeof(AtomUI.Desktop.Controls.Calendar)
            .GetField("_presentationAdapter", BindingFlags.Instance | BindingFlags.NonPublic)!
            .GetValue(calendar)!
    };

    private static void ApplyTemplateParts(CalendarViewControl view)
    {
        var scope = new NameScope();
        scope.Register("PART_WeekHeader", new GridControl { Name = "PART_WeekHeader" });
        scope.Register("PART_CellHost", new GridControl { Name = "PART_CellHost" });
        typeof(CalendarViewControl).GetMethod("OnApplyTemplate", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(view, [new TemplateAppliedEventArgs(scope)]);
    }

    private static IReadOnlyList<LunarCalendarViewCell> GetLunarCells(CalendarViewControl view) =>
        ((IReadOnlyList<CalendarViewCell>)typeof(CalendarViewControl)
            .GetField("_cellPool", BindingFlags.Instance | BindingFlags.NonPublic)!
            .GetValue(view)!).Cast<LunarCalendarViewCell>().ToArray();

    private sealed class RecordingProvider(IReadOnlyList<LunarCalendarHoliday> holidays)
        : ILunarCalendarHolidayProvider
    {
        public int CallCount { get; private set; }
        public CalendarDateRange? LastRange { get; private set; }

        public bool TryGetHolidays(
            CalendarDateRange visibleRange,
            CultureInfo culture,
            out IReadOnlyList<LunarCalendarHoliday> result)
        {
            CallCount++;
            LastRange = visibleRange;
            result = holidays;
            return true;
        }
    }

    private sealed class ThrowingProvider : ILunarCalendarHolidayProvider
    {
        public bool TryGetHolidays(
            CalendarDateRange visibleRange,
            CultureInfo culture,
            out IReadOnlyList<LunarCalendarHoliday> holidays)
        {
            holidays = [];
            throw new InvalidOperationException("provider failure");
        }
    }

    private sealed class ConfigurableProvider(
        bool succeeds,
        IReadOnlyList<LunarCalendarHoliday>? holidays) : ILunarCalendarHolidayProvider
    {
        public int CallCount { get; private set; }

        public bool TryGetHolidays(
            CalendarDateRange visibleRange,
            CultureInfo culture,
            out IReadOnlyList<LunarCalendarHoliday> result)
        {
            CallCount++;
            result = holidays!;
            return succeeds;
        }
    }

    private sealed class MutableProvider : ILunarCalendarHolidayProvider
    {
        public IReadOnlyList<LunarCalendarHoliday> Holidays { get; set; } = [];
        public int CallCount { get; private set; }

        public bool TryGetHolidays(
            CalendarDateRange visibleRange,
            CultureInfo culture,
            out IReadOnlyList<LunarCalendarHoliday> result)
        {
            CallCount++;
            result = Holidays;
            return true;
        }
    }

    private static Avalonia.Controls.Window Show(LunarCalendarControl calendar)
    {
        var window = new Avalonia.Controls.Window
        {
            Width = 420,
            Height = 420,
            Content = calendar
        };
        window.Show();
        Avalonia.Threading.Dispatcher.UIThread.RunJobs();
        return window;
    }
}
