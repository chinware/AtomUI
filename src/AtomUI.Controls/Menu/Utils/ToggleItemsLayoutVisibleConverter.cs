using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data.Converters;

namespace AtomUI.Controls.Utils;

internal class ToggleItemsLayoutVisibleConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> objects, Type targetType, object? parameter, CultureInfo culture)
    {
        var items      = objects.ToList();
        if (items[0] is bool isTopLevel && items[1] is MenuItemToggleType toggleType)
        {
            return !isTopLevel && toggleType != MenuItemToggleType.None;
        }

        return false;
    }
}
