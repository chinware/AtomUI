using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace AtomUI.Desktop.Controls.Converters;

internal class NavMenuIsSubMenuOpenConverter : IMultiValueConverter
{
    public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count < 2 || values[0] == AvaloniaProperty.UnsetValue || values[1] == AvaloniaProperty.UnsetValue)
        {
            return false;
        }
        bool isSubMenuOpen = System.Convert.ToBoolean(values[0]);
        bool hasSubMenu    = System.Convert.ToBoolean(values[1]);
        return isSubMenuOpen && hasSubMenu;
    }
}