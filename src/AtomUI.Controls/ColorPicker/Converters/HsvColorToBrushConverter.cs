using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace AtomUI.Controls.Converters;

internal class HsvColorToBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is HsvColor color)
        {
            return new SolidColorBrush(color.ToRgb());
        }
        return value;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}