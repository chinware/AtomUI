using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Layout;

namespace AtomUI.Controls;

internal class StringToTextBlockConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string str)
        {
            return new TextBlock()
            {
                Text              = str,
                VerticalAlignment = VerticalAlignment.Center
            };
        }
        return value;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}