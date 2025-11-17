using System.Globalization;
using Avalonia.Data.Converters;

namespace AtomUI.Controls.Converters;

internal class OverlayDialogResizerVisibleConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count != 2)
        {
            return false;
        }
        var isResizable = values[0] as bool?;
        var windowState = values[1] as OverlayDialogState?;
        return isResizable == true && windowState == OverlayDialogState.Normal;
    }
}