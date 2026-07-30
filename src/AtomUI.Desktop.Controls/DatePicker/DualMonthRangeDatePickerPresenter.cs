namespace AtomUI.Desktop.Controls;

internal class DualMonthRangeDatePickerPresenter : RangeDatePickerPresenter
{
    protected override DateTime ResolveRangeEndDisplayAnchor(DateTime activeEnd)
    {
        return PickerMode switch
        {
            DatePickerMode.Month or DatePickerMode.Quarter =>
                DateTimeHelper.AddYears(activeEnd, -1) ?? activeEnd,
            DatePickerMode.Year => ResolvePreviousDecadeAnchor(activeEnd),
            _                   => DateTimeHelper.AddMonths(activeEnd, -1) ?? activeEnd
        };
    }

    private static DateTime ResolvePreviousDecadeAnchor(DateTime activeEnd)
    {
        var decadeStart = DateTimeHelper.DecadeOfDate(activeEnd) - 10;
        return new DateTime(Math.Max(DateTime.MinValue.Year, decadeStart), 1, 1);
    }
}
