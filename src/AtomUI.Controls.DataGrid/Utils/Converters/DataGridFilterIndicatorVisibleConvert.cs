using System.Globalization;
using Avalonia.Data.Converters;

namespace AtomUI.Controls.Converters;

internal class DataGridFilterIndicatorVisibleConvert : IMultiValueConverter
{
    public static readonly DataGridFilterIndicatorVisibleConvert Default = new();
    
    public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        return values.All(v => v is bool && (bool)v);
    }
}