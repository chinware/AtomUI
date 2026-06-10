// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Globalization;
using AtomUI.Utils;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace AtomUI.Desktop.Controls.Utils;

internal class DataGridValueConverter : IValueConverter
{
    public static DataGridValueConverter Instance = new DataGridValueConverter();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return ConvertValue(value, targetType, culture);
    }


    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (targetType.IsNullableType())
        {
            var strValue = value as string;

            // This suppresses a warning saying that we should use String.IsNullOrEmpty instead of a string
            // comparison, but in this case we want to explicitly check for Empty and not Null.
#pragma warning disable CA1820
            if (strValue == string.Empty)
#pragma warning restore CA1820
            {
                return null;
            }
        }
        return ConvertValue(value, targetType, culture);
    }

    private static object? ConvertValue(object? value, Type targetType, CultureInfo culture)
    {
        if (value is null)
        {
            return null;
        }

        var nonNullableTargetType = targetType.GetNonNullableType();
        if (nonNullableTargetType == typeof(object) ||
            nonNullableTargetType.IsInstanceOfType(value))
        {
            return value;
        }

        try
        {
            if (nonNullableTargetType == typeof(string))
            {
                return value.ToString();
            }

            if (nonNullableTargetType.IsEnum)
            {
                if (value is string enumText)
                {
                    return Enum.Parse(nonNullableTargetType, enumText, ignoreCase: true);
                }

                var enumValue = System.Convert.ChangeType(value, Enum.GetUnderlyingType(nonNullableTargetType), culture);
                return Enum.ToObject(nonNullableTargetType, enumValue!);
            }

            if (value is IConvertible)
            {
                return System.Convert.ChangeType(value, nonNullableTargetType, culture);
            }
        }
        catch (Exception exception)
        {
            return new BindingNotification(exception, BindingErrorType.Error);
        }

        return new BindingNotification(
            new InvalidCastException($"Could not convert '{value}' to '{targetType.Name}'."),
            BindingErrorType.Error);
    }
}
