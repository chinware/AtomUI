using System.Globalization;
using AtomUI.Controls.TimePickerLang;
using AtomUI.Data;

namespace AtomUI.Controls.Utils;

internal static class DateTimeUtils
{
    public static string FormatTimeSpan(TimeSpan value, bool is12HourClock = false)
    {
        var dateTime = DateTime.Today.Add(value);
        if (is12HourClock)
        {
            var formatInfo = new DateTimeFormatInfo();
            formatInfo.AMDesignator = LanguageResourceBinder.GetLangResource(TimePickerLangResourceKey.AMText)!;
            formatInfo.PMDesignator = LanguageResourceBinder.GetLangResource(TimePickerLangResourceKey.PMText)!;
            return dateTime.ToString(@"hh:mm:ss tt", formatInfo);
        }

        return dateTime.ToString(@"HH:mm:ss");
    }
}