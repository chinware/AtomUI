using System.Globalization;
using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.Desktop.Controls.Localization;
using AtomUI.Media;
using Avalonia;
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

    internal static bool IsFormattedTextAffectingProperty(
        AvaloniaProperty property,
        AvaloniaProperty isShowTimeProperty,
        AvaloniaProperty formatProperty,
        AvaloniaProperty clockIdentifierProperty,
        AvaloniaProperty amTextProperty,
        AvaloniaProperty pmTextProperty)
    {
        return property == isShowTimeProperty ||
               property == formatProperty ||
               property == clockIdentifierProperty ||
               property == amTextProperty ||
               property == pmTextProperty;
    }

    internal static bool IsPreferredWidthAffectingProperty(
        AvaloniaProperty property,
        AvaloniaProperty fontSizeProperty,
        AvaloniaProperty fontFamilyProperty,
        AvaloniaProperty fontStyleProperty,
        AvaloniaProperty fontWeightProperty,
        AvaloniaProperty placeholderTextProperty,
        AvaloniaProperty sizeTypeProperty,
        AvaloniaProperty minWidthProperty,
        AvaloniaProperty widthProperty,
        AvaloniaProperty maxWidthProperty,
        AvaloniaProperty horizontalAlignmentProperty,
        AvaloniaProperty? secondaryPlaceholderTextProperty = null)
    {
        return property == fontSizeProperty ||
               property == fontFamilyProperty ||
               property == fontStyleProperty ||
               property == fontWeightProperty ||
               property == placeholderTextProperty ||
               property == secondaryPlaceholderTextProperty ||
               property == sizeTypeProperty ||
               property == minWidthProperty ||
               property == widthProperty ||
               property == maxWidthProperty ||
               property == horizontalAlignmentProperty;
    }

    internal static double CalculatePreferredInputWidth(
        string? placeholderText,
        string format,
        double fontSize,
        FontFamily fontFamily,
        FontStyle fontStyle,
        FontWeight fontWeight,
        DateTimeFormatInfo? formatInfo)
    {
        var preferredWidth = DateTimeUtils.CalculateWidestFormattedDateTimeSize(
            format, fontSize, fontFamily, fontStyle, fontWeight, formatInfo).Width;

        if (!string.IsNullOrEmpty(placeholderText))
        {
            var placeholderWidth = TextUtils.CalculateTextSize(placeholderText, fontSize, fontFamily, fontStyle, fontWeight).Width;
            preferredWidth = Math.Max(preferredWidth, placeholderWidth);
        }

        return preferredWidth;
    }

    internal static double CalculateBoundedPreferredInputWidth(
        string? placeholderText,
        string format,
        double fontSize,
        FontFamily fontFamily,
        FontStyle fontStyle,
        FontWeight fontWeight,
        double minWidth,
        double maxWidth,
        DateTimeFormatInfo? formatInfo)
    {
        var preferredWidth = CalculatePreferredInputWidth(
            placeholderText,
            format,
            fontSize,
            fontFamily,
            fontStyle,
            fontWeight,
            formatInfo);

        return ApplyWidthBounds(preferredWidth, minWidth, maxWidth);
    }

    internal static double CalculateBoundedRangePreferredInputWidth(
        string? placeholderText,
        string? secondaryPlaceholderText,
        string format,
        double fontSize,
        FontFamily fontFamily,
        FontStyle fontStyle,
        FontWeight fontWeight,
        double minWidth,
        double maxWidth,
        DateTimeFormatInfo? formatInfo)
    {
        var preferredWidth = Math.Max(
            CalculatePreferredInputWidth(
                placeholderText,
                format,
                fontSize,
                fontFamily,
                fontStyle,
                fontWeight,
                formatInfo),
            CalculatePreferredInputWidth(
                secondaryPlaceholderText,
                format,
                fontSize,
                fontFamily,
                fontStyle,
                fontWeight,
                formatInfo));

        return ApplyWidthBounds(preferredWidth, minWidth, maxWidth);
    }

    private static double ApplyWidthBounds(double preferredWidth, double minWidth, double maxWidth)
    {
        if (!double.IsNaN(minWidth))
        {
            preferredWidth = Math.Max(minWidth, preferredWidth);
        }

        if (!double.IsNaN(maxWidth))
        {
            preferredWidth = Math.Min(maxWidth, preferredWidth);
        }

        return preferredWidth;
    }
}
