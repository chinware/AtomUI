using System.Globalization;
using Avalonia.Data.Converters;

namespace AtomUI.Controls.Converters;

internal class TreeViewItemIndicatorEnabledConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count != 2)
        {
            return false;
        }
        var isHeaderEnabled = values[0] as bool?;
        var isIndicatorEnabled = values[1] as bool?;
        return isHeaderEnabled == true && isIndicatorEnabled == true;
    }
}