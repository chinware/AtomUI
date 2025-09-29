using System.Globalization;
using Avalonia.Data.Converters;

namespace AtomUI.Controls.Converters;

internal class DataGridPaginationVisibilityConvertor : IValueConverter
{
    public bool IsTop { get; set; }
    
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DataGridGridPaginationVisibility paginationVisibility)
        {
            if (paginationVisibility == DataGridGridPaginationVisibility.None)
            {
                return false;
            }

            if (IsTop)
            {
                return paginationVisibility.HasFlag(DataGridGridPaginationVisibility.Top);
            }
            
            return paginationVisibility.HasFlag(DataGridGridPaginationVisibility.Bottom);
        }
        return false;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}