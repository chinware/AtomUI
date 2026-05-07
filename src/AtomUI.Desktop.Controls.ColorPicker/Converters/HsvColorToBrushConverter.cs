using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls.Converters;

internal class HsvColorToBrushConverter : IValueConverter
{
    public static readonly HsvColorToBrushConverter Default = new();
    public static readonly HsvColorToBrushConverter WithoutAlpha = new()
    {
        EnableAlpha = false
    };
    
    public bool EnableAlpha { get; set; } = true;
    
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is HsvColor color)
        {
            if (EnableAlpha)
            {
                return new SolidColorBrush(color.ToRgb());
            }
            return new SolidColorBrush(HsvColor.ToRgb(color.H, color.S, color.V));
        }
        return value;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}