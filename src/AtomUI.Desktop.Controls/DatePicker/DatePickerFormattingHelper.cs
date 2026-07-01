using System.Globalization;
using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.Desktop.Controls.Localization;
using AtomUI.Media;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

internal static class DatePickerFormattingHelper
{
    internal static string GetEffectiveFormat(string? format, bool isShowTime, ClockIdentifierType clockIdentifier)
    {
        if (format is not null)
        {
            return format;
        }

        var effectiveFormat = "yyyy-MM-dd";
        if (!isShowTime)
        {
            return effectiveFormat;
        }

        return clockIdentifier == ClockIdentifierType.HourClock12
            ? $"{effectiveFormat} hh:mm:ss tt"
            : $"{effectiveFormat} HH:mm:ss";
    }

    internal static DateTimeFormatInfo? CreateFormatInfo(
        ClockIdentifierType clockIdentifier,
        string? amText,
        string? pmText)
    {
        if (clockIdentifier != ClockIdentifierType.HourClock12)
        {
            return null;
        }

        var amDesignator = amText ?? LanguageResourceBinder.GetLangResource(TimePickerLangResourceKind.AMText);
        var pmDesignator = pmText ?? LanguageResourceBinder.GetLangResource(TimePickerLangResourceKind.PMText);
        if (amDesignator is null || pmDesignator is null)
        {
            return null;
        }

        return new DateTimeFormatInfo
        {
            AMDesignator = amDesignator,
            PMDesignator = pmDesignator
        };
    }

    internal static string FormatDateTime(DateTime dateTime, string format, DateTimeFormatInfo? formatInfo)
    {
        return formatInfo is null
            ? dateTime.ToString(format)
            : dateTime.ToString(format, formatInfo);
    }

    internal static double CalculateContentPreferredWidth(
        string? text,
        string? placeholderText,
        string format,
        double fontSize,
        FontFamily fontFamily,
        FontStyle fontStyle,
        FontWeight fontWeight,
        DateTimeFormatInfo? formatInfo)
    {
        if (!string.IsNullOrEmpty(text))
        {
            return TextUtils.CalculateTextSize(text, fontSize, fontFamily, fontStyle, fontWeight).Width;
        }

        if (!string.IsNullOrEmpty(placeholderText))
        {
            return TextUtils.CalculateTextSize(placeholderText, fontSize, fontFamily, fontStyle, fontWeight).Width;
        }

        return DateTimeUtils.CalculateWidestFormattedDateTimeSize(
            format, fontSize, fontFamily, fontStyle, fontWeight, formatInfo).Width;
    }
}
