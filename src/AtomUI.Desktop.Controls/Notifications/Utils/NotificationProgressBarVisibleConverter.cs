using System.Globalization;
using Avalonia.Data.Converters;

namespace AtomUI.Desktop.Controls.Utils;

internal class NotificationProgressBarVisibleConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count >= 2 &&
            values[0] is bool isShowProgress &&
            values[1] is TimeSpan expiration)
        {
            return isShowProgress && expiration > TimeSpan.Zero;
        }
        return false;
    }
}
