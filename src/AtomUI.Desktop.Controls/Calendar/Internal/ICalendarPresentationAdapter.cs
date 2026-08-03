using System.Globalization;
using System.Windows.Input;

namespace AtomUI.Desktop.Controls.Internal.Calendar;

internal readonly record struct CalendarEffectiveRange(DateTime? Start, DateTime? End, bool IsEmpty)
{
    internal static CalendarEffectiveRange FromValidRange(CalendarDateRange? validRange) =>
        validRange is null
            ? new CalendarEffectiveRange(null, null, false)
            : new CalendarEffectiveRange(validRange.Start, validRange.End, false);

    internal DateTime? BuilderStart => IsEmpty ? DateTime.MaxValue.Date : Start;
    internal DateTime? BuilderEnd => IsEmpty ? DateTime.MinValue.Date : End;
}

internal readonly record struct CalendarPresentationMetrics(
    object MiniContentHeightResourceKey,
    object FullCellMinHeightResourceKey,
    object RangeBarTopOffsetResourceKey);

internal interface ICalendarPresentationAdapter
{
    CalendarPresentationMetrics Metrics { get; }

    CalendarEffectiveRange GetEffectiveRange(CalendarDateRange? validRange);

    CalendarViewCell CreateCell();

    CalendarCellContext CreateCellContext(CalendarView owner, CalendarViewCellModel model);

    void ApplyCellPresentation(CalendarViewCell cell, CalendarView owner, CalendarViewCellModel model);

    void ClearCellPresentation(CalendarViewCell cell);

    string GetAutomationName(CalendarView owner, CalendarViewCellModel model);

    string FormatYearOption(int year, CultureInfo culture);

    string FormatMonthOption(int year, int month, CultureInfo culture);

    CalendarHeaderContext CreateHeaderContext(
        AtomUI.Desktop.Controls.Calendar owner,
        ICommand changeValueCommand,
        ICommand changeModeCommand);
}
