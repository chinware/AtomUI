using System.Globalization;
using Avalonia.Data.Converters;

namespace AtomUI.Desktop.Controls.Converters;

internal class CheckBoxIndicatorStateConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool bvalue)
        {
            if (bvalue)
            {
                return CheckBoxIndicatorState.Checked;
            }
            return CheckBoxIndicatorState.Unchecked;
        }
        return CheckBoxIndicatorState.Indeterminate;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}