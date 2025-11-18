using System.Globalization;
using Avalonia.Data.Converters;

namespace AtomUI.Desktop.Controls.Converters;

internal class MinimizeButtonVisibleConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count != 2)
        {
            return false;
        }
        var isMinimizeEnabled  = values[0] as bool?;
        var isWindowFullScreen = values[1] as bool?;
        return isMinimizeEnabled == true && isWindowFullScreen != true;
    }
}