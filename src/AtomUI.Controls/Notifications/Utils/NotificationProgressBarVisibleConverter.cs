using System.Globalization;
using Avalonia.Data.Converters;

namespace AtomUI.Controls.Utils;

internal class NotificationProgressBarVisibleConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        var items      = values.ToList();
        if (items[0] is bool isShowProgress && items[1] is TimeSpan expiration)
        {
            return isShowProgress && expiration > TimeSpan.Zero;
        }
        return false;
    }
}