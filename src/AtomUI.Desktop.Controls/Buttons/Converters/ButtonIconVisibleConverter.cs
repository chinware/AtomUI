using System.Globalization;
using AtomUI.IconPkg;
using Avalonia.Data.Converters;

namespace AtomUI.Controls.Converters;

internal class ButtonIconVisibleConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count != 3)
        {
            return false;
        }
        var icon        = values[0] as Icon;
        var iconVisible = values[1] as bool?;
        var isLoading = values[2] as bool?;
        return icon != null && iconVisible == true && isLoading != true;
    }
}