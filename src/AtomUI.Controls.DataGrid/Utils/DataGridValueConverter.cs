// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Globalization;
using Avalonia.Data.Converters;

namespace AtomUI.Controls.Utils;

internal class DataGridValueConverter : IValueConverter
{
    public static DataGridValueConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return DefaultValueConverter.Instance.Convert(value, targetType, parameter, culture);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (targetType.IsNullableType())
        {
            var strValue = value as string;

            // This suppresses a warning saying that we should use String.IsNullOrEmpty instead of a string
            // comparison, but in this case we want to explicitly check for Empty and not Null.
            if (strValue == string.Empty)
            {
                return null;
            }
        }

        return DefaultValueConverter.Instance.ConvertBack(value, targetType, parameter, culture);
    }
}