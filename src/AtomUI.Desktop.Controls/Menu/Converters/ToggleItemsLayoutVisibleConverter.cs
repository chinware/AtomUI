using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data.Converters;

namespace AtomUI.Desktop.Controls.Converters;

internal class ToggleItemsLayoutVisibleConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count != 3)
        {
            return false;
        }
        if (values[0] is bool isTopLevel && values[1] is MenuItemToggleType toggleType && values[2] is int itemCount)
        {
            return !isTopLevel && toggleType != MenuItemToggleType.None && itemCount == 0;
        }

        return false;
    }
}
