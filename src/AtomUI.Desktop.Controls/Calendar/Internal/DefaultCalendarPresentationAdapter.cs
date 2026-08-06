using System.Globalization;
using System.Windows.Input;
using AtomUI.Desktop.Controls.DesignTokens;
using AtomUI.Theme.Resources;
using AtomUI.Desktop.Controls.Localization;
using Avalonia;

namespace AtomUI.Desktop.Controls.Internal.Calendar;

internal sealed class DefaultCalendarPresentationAdapter : ICalendarPresentationAdapter
{
    internal static DefaultCalendarPresentationAdapter Instance { get; } = new();

    public CalendarPresentationMetrics Metrics { get; } = new(
        CalendarTokenKind.MiniContentHeight,
        CalendarTokenKind.FullCellMinHeight,
        SharedTokenKind.ControlHeightSM);

    private DefaultCalendarPresentationAdapter()
    {
    }

    public CalendarEffectiveRange GetEffectiveRange(CalendarDateRange? validRange) =>
        CalendarEffectiveRange.FromValidRange(validRange);

    public CalendarViewCell CreateCell() => new();

    public CalendarCellContext CreateCellContext(CalendarView owner, CalendarViewCellModel model) =>
        new(
            model.Value,
            owner.Today == default ? DateTime.Today : owner.Today.Date,
            model.Kind == CalendarViewCellKind.Month ? CalendarCellType.Month : CalendarCellType.Date,
            model.DisplayText,
            model.IsToday,
            model.IsInView,
            model.IsSelected,
            model.IsDisabled);

    public void ApplyCellPresentation(CalendarViewCell cell, CalendarView owner, CalendarViewCellModel model)
    {
    }

    public void ClearCellPresentation(CalendarViewCell cell)
    {
    }

    public string GetAutomationName(CalendarView owner, CalendarViewCellModel model)
    {
        var culture = owner.Culture ?? CultureInfo.CurrentCulture;
        var localizer = Application.Current is { } application
            ? global::AtomUI.ApplicationExtensions.GetLocalizer(application)
            : null;
        return model.Kind switch
        {
            CalendarViewCellKind.Date => model.Value.ToString("D", culture),
            CalendarViewCellKind.Month => model.Value.ToString("Y", culture),
            _ => $"{localizer?.Get(CalendarControlLangResourceKind.Week) ?? CalendarControlLangResourceKind.Week.ToString()} {model.DisplayText}"
        };
    }

    public string FormatYearOption(int year, CultureInfo culture)
    {
        var localizer = Application.Current is { } application
            ? global::AtomUI.ApplicationExtensions.GetLocalizer(application)
            : null;
        var suffix = localizer?.Get(CalendarControlLangResourceKind.YearSuffix) ?? string.Empty;
        return year.ToString(CultureInfo.InvariantCulture) + suffix;
    }

    public string FormatMonthOption(int year, int month, CultureInfo culture) =>
        culture.DateTimeFormat.AbbreviatedMonthNames[month - 1];

    public CalendarHeaderContext CreateHeaderContext(
        AtomUI.Desktop.Controls.Calendar owner,
        ICommand changeValueCommand,
        ICommand changeModeCommand) =>
        new(owner.Value.Date, owner.Mode, changeValueCommand, changeModeCommand);
}
