using System.Globalization;
using AtomUI.Media;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace AtomUI.Controls.Converters;

internal class WindowShadowsToPaddingConverter : IValueConverter
{
    
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is BoxShadows boxShadows)
        {
            return boxShadows.Thickness();
        }
        return value;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}