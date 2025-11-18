using System.Globalization;
using Avalonia.Data.Converters;

namespace AtomUI.Desktop.Controls.Converters;

internal class FullScreenButtonVisibleConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count != 2)
        {
            return false;
        }
        var isFullScreenEnabled = values[0] as bool?;
        var isMaximized         = values[1] as bool?;
        return isFullScreenEnabled == true && isMaximized != true;
    }
}