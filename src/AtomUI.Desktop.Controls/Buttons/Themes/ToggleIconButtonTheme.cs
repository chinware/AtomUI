using System.Globalization;
using AtomUI.Controls;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Styling;

namespace AtomUI.Desktop.Controls.Themes;

internal class ToggleIconButtonTheme : ControlTheme
{
    public static readonly ToggleIconButtonCurrentIconConverter CurrentIconConverter = new();
}

internal sealed class ToggleIconButtonCurrentIconConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count != 3)
        {
            return null;
        }

        var isChecked     = values[0] as bool?;
        var checkedIcon   = values[1] as PathIcon;
        var unCheckedIcon = values[2] as PathIcon;
        return isChecked == true ? checkedIcon : unCheckedIcon;
    }
}
