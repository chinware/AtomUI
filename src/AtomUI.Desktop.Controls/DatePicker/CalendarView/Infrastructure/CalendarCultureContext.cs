using System.Globalization;

namespace AtomUI.Desktop.Controls.CalendarView.Infrastructure;

internal sealed class CalendarCultureContext
{
    public DateTimeFormatInfo CurrentFormat { get; private set; } = CreateInvariantGregorianFormat();

    public void Refresh()
    {
        CurrentFormat = DateTimeHelper.GetCurrentDateFormat();
    }

    private static DateTimeFormatInfo CreateInvariantGregorianFormat()
    {
        var format = new CultureInfo(CultureInfo.InvariantCulture.Name).DateTimeFormat;
        format.Calendar = new GregorianCalendar();
        return format;
    }
}
