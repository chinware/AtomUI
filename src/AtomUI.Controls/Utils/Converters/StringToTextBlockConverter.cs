using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Layout;

namespace AtomUI.Controls.Utils;

internal class StringToTextBlockConverter : IValueConverter
{
    public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Stretch;
    public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Stretch;
    
    public double Width { get; set; } = double.NaN;
    public double Height { get; set; } = double.NaN;
    public double LineHeight { get; set; } = double.NaN;
    
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string str)
        {
            return new TextBlock
            {
                Text              = str,
                VerticalAlignment = VerticalAlignment,
                HorizontalAlignment = HorizontalAlignment,
                Height = Height,
                Width = Width,
                LineHeight = LineHeight,
            };
        }
        return value;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}