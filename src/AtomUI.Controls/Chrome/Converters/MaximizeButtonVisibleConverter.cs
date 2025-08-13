using System.Globalization;
using Avalonia.Data.Converters;

namespace AtomUI.Controls.Converters;

internal class MaximizeButtonVisibleConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count != 2)
        {
            return false;
        }
        var isMaximizeEnabled  = values[0] as bool?;
        var isWindowFullScreen = values[1] as bool?;
        return isMaximizeEnabled == true && isWindowFullScreen != true;
    }
}