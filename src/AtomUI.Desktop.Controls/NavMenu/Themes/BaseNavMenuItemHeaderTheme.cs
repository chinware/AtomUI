using System.Globalization;
using AtomUI.Controls.Converters;
using Avalonia.Controls.Converters;
using Avalonia.Data.Converters;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Desktop.Controls.Themes;

internal class BaseNavMenuItemHeaderTheme : ControlTheme
{
    public static readonly StringToTextBlockConverter StringToTextBlockConverter = new()
    {
        VerticalAlignment = VerticalAlignment.Center
    };
    public static readonly IValueConverter FirstCharacterConverter = new FirstCharacterValueConverter();
    public static readonly PlatformKeyGestureConverter KeyGestureConverter = new();

    private sealed class FirstCharacterValueConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var text = value?.ToString()?.Trim();
            return string.IsNullOrEmpty(text) ? null : text[..1];
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
