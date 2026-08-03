using System.Globalization;
using System.Reflection;
using System.Windows.Input;
using AtomUI.Desktop.Controls.DesignTokens;
using AtomUI.Desktop.Controls.Internal.Calendar;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Shouldly;
using Xunit;
using CalendarViewControl = AtomUI.Desktop.Controls.Internal.Calendar.CalendarView;
using GridControl = Avalonia.Controls.Grid;

namespace AtomUI.Desktop.Controls.Tests.Calendar;

public class CalendarPresentationAdapterTests
{
    static CalendarPresentationAdapterTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void DefaultAdapter_PreservesExistingCellContextAndAutomationText()
    {
        var owner = new CalendarViewControl
        {
            Value = new DateTime(2026, 7, 15),
            Today = new DateTime(2026, 7, 30),
            Culture = CultureInfo.GetCultureInfo("en-US")
        };
        var model = new CalendarViewCellModel(
            new DateTime(2026, 7, 15),
            CalendarViewCellKind.Date,
            "15",
            false,
            true,
            true,
            false,
            true);
        var adapter = DefaultCalendarPresentationAdapter.Instance;

        adapter.Metrics.MiniContentHeightResourceKey.ShouldBe(CalendarTokenKind.MiniContentHeight);
        adapter.CreateCell().GetType().ShouldBe(typeof(CalendarViewCell));
        adapter.CreateCellContext(owner, model).ShouldBe(new CalendarCellContext(
            model.Value,
            owner.Today,
            CalendarCellType.Date,
            model.DisplayText,
            model.IsToday,
            model.IsInView,
            model.IsSelected,
            model.IsDisabled));
        adapter.GetAutomationName(owner, model).ShouldBe(model.Value.ToString("D", owner.Culture));
    }

    [Fact]
    public void ReplacingAdapter_ReplacesPoolAndClearsOldContainers()
    {
        var view = new CalendarViewControl
        {
            Value = new DateTime(2026, 7, 15),
            Today = new DateTime(2026, 7, 30),
            Culture = CultureInfo.InvariantCulture
        };
        ApplyTemplateParts(view);
        var oldCells = GetCellPool(view).ToArray();

        var adapter = new TestPresentationAdapter();
        view.PresentationAdapter = adapter;

        foreach (var cell in oldCells)
        {
            cell.Model.ShouldBeNull();
            cell.Context.ShouldBeNull();
            cell.OwnerView.ShouldBeNull();
        }

        foreach (var cell in GetCellPool(view))
        {
            cell.ShouldBeOfType<TestCalendarViewCell>();
        }
        adapter.AppliedCount.ShouldBe(42);
    }

    [Fact]
    public void CalendarContexts_AreInheritableWithoutChangingExistingMembers()
    {
        var cell = new DerivedCellContext();
        cell.Value.ShouldBe(new DateTime(2026, 7, 15));

        var header = new DerivedHeaderContext();
        header.Value.ShouldBe(new DateTime(2026, 7, 15));
        header.Mode.ShouldBe(CalendarMode.Month);
    }

    private static GridControl ApplyTemplateParts(CalendarViewControl view)
    {
        var scope = new NameScope();
        var weekHeader = new GridControl { Name = "PART_WeekHeader" };
        var host = new GridControl { Name = "PART_CellHost" };
        scope.Register("PART_WeekHeader", weekHeader);
        scope.Register("PART_CellHost", host);
        typeof(CalendarViewControl).GetMethod("OnApplyTemplate", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(view, [new TemplateAppliedEventArgs(scope)]);
        return host;
    }

    private static IReadOnlyList<CalendarViewCell> GetCellPool(CalendarViewControl view) =>
        (IReadOnlyList<CalendarViewCell>)typeof(CalendarViewControl)
            .GetField("_cellPool", BindingFlags.Instance | BindingFlags.NonPublic)!
            .GetValue(view)!;

    private sealed class TestPresentationAdapter : ICalendarPresentationAdapter
    {
        public int AppliedCount { get; private set; }

        public CalendarPresentationMetrics Metrics => DefaultCalendarPresentationAdapter.Instance.Metrics;

        public CalendarEffectiveRange GetEffectiveRange(CalendarDateRange? validRange) =>
            CalendarEffectiveRange.FromValidRange(validRange);

        public CalendarViewCell CreateCell() => new TestCalendarViewCell();

        public CalendarCellContext CreateCellContext(CalendarViewControl owner, CalendarViewCellModel model) =>
            DefaultCalendarPresentationAdapter.Instance.CreateCellContext(owner, model);

        public void ApplyCellPresentation(CalendarViewCell cell, CalendarViewControl owner, CalendarViewCellModel model) =>
            AppliedCount++;

        public void ClearCellPresentation(CalendarViewCell cell)
        {
        }

        public string GetAutomationName(CalendarViewControl owner, CalendarViewCellModel model) =>
            DefaultCalendarPresentationAdapter.Instance.GetAutomationName(owner, model);

        public string FormatYearOption(int year, CultureInfo culture) => year.ToString(culture);

        public string FormatMonthOption(int year, int month, CultureInfo culture) => month.ToString(culture);

        public CalendarHeaderContext CreateHeaderContext(
            AtomUI.Desktop.Controls.Calendar owner,
            ICommand changeValueCommand,
            ICommand changeModeCommand) =>
            new(owner.Value, owner.Mode, changeValueCommand, changeModeCommand);
    }

    private sealed class TestCalendarViewCell : CalendarViewCell
    {
    }

    private sealed record DerivedCellContext() : CalendarCellContext(
        new DateTime(2026, 7, 15),
        new DateTime(2026, 7, 30),
        CalendarCellType.Date,
        "15",
        false,
        true,
        true,
        false);

    private sealed class DerivedHeaderContext : CalendarHeaderContext
    {
        public DerivedHeaderContext()
            : base(
                new DateTime(2026, 7, 15),
                CalendarMode.Month,
                new NoOpCommand(),
                new NoOpCommand())
        {
        }
    }

    private sealed class NoOpCommand : ICommand
    {
        public bool CanExecute(object? parameter) => true;
        public void Execute(object? parameter)
        {
        }

        public event EventHandler? CanExecuteChanged
        {
            add { }
            remove { }
        }
    }
}
