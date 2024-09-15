﻿using System.Diagnostics;
using System.Globalization;
using AtomUI.Theme;

namespace AtomUI.Controls;

internal static class DateTimeHelper
{
    public static DateTime? AddDays(DateTime time, int days)
    {
        System.Globalization.Calendar cal = new GregorianCalendar();
        try
        {
            return cal.AddDays(time, days);
        }
        catch (ArgumentException)
        {
            return null;
        }
    }

    public static DateTime? AddMonths(DateTime time, int months)
    {
        System.Globalization.Calendar cal = new GregorianCalendar();
        try
        {
            return cal.AddMonths(time, months);
        }
        catch (ArgumentException)
        {
            return null;
        }
    }

    public static DateTime? AddYears(DateTime time, int years)
    {
        System.Globalization.Calendar cal = new GregorianCalendar();
        try
        {
            return cal.AddYears(time, years);
        }
        catch (ArgumentException)
        {
            return null;
        }
    }

    public static int CompareDays(DateTime dt1, DateTime dt2)
    {
        return DateTime.Compare(DiscardTime(dt1), DiscardTime(dt2));
    }

    public static int CompareYearMonth(DateTime dt1, DateTime dt2)
    {
        return (dt1.Year - dt2.Year) * 12 + (dt1.Month - dt2.Month);
    }

    public static int DecadeOfDate(DateTime date)
    {
        return date.Year - date.Year % 10;
    }

    public static DateTime DiscardDayTime(DateTime d)
    {
        return new DateTime(d.Year, d.Month, 1, 0, 0, 0);
    }

    public static DateTime DiscardTime(DateTime d)
    {
        return d.Date;
    }

    public static int EndOfDecade(DateTime date)
    {
        return DecadeOfDate(date) + 9;
    }

    public static DateTimeFormatInfo GetCurrentDateFormat()
    {
        var culture = ThemeManager.Current.CultureInfo ?? CultureInfo.CurrentCulture;
        if (culture.Calendar is GregorianCalendar)
        {
            return culture.DateTimeFormat;
        }

        foreach (var cal in culture.OptionalCalendars)
        {
            if (cal is GregorianCalendar)
            {
                // if the default calendar is not Gregorian, return the
                // first supported GregorianCalendar dtfi
                var dtfi = new CultureInfo(culture.Name).DateTimeFormat;
                dtfi.Calendar = cal;
                return dtfi;
            }
        }

        // if there are no GregorianCalendars in the OptionalCalendars
        // list, use the invariant dtfi
        var dt = new CultureInfo(CultureInfo.InvariantCulture.Name).DateTimeFormat;
        dt.Calendar = new GregorianCalendar();
        return dt;
    }

    public static bool InRange(DateTime date, CalendarDateRange range)
    {
        Debug.Assert(DateTime.Compare(range.Start, range.End) < 1, "The range should start before it ends!");

        if (CompareDays(date, range.Start) > -1 && CompareDays(date, range.End) < 1)
        {
            return true;
        }

        return false;
    }

    public static bool InRange(DateTime date, in DateTime rangeStart, in DateTime rangeEnd)
    {
        Debug.Assert(DateTime.Compare(rangeStart, rangeEnd) < 1, "The range should start before it ends!");

        if (CompareDays(date, rangeStart) > -1 && CompareDays(date, rangeEnd) < 1)
        {
            return true;
        }

        return false;
    }

    public static string ToYearMonthPatternString(DateTime date)
    {
        var format = GetCurrentDateFormat();
        return date.ToString(format.YearMonthPattern, format);
    }

    public static string ToYearString(DateTime date)
    {
        var format = GetCurrentDateFormat();
        return date.Year.ToString(format);
    }
}