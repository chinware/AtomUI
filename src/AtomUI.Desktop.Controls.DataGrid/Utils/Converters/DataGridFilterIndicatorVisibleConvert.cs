using System.Globalization;
using Avalonia.Data.Converters;

namespace AtomUI.Controls.Converters;

internal class DataGridFilterIndicatorVisibleConvert : IMultiValueConverter
{
    public static readonly DataGridFilterIndicatorVisibleConvert Default = new();
    
    public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count != 3)
        {
            return false;
        }
        bool areSeparatorsVisible = values[0] is bool b0 && b0;
        bool canUserFilter        = values[1] is bool b1 && b1;
        int  filterCount          = values[2] is int count ? count : 0;
        return areSeparatorsVisible && canUserFilter && filterCount > 0;
    }
}