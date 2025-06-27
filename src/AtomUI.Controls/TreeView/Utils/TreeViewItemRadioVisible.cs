using System.Globalization;
using Avalonia.Data.Converters;

namespace AtomUI.Controls.Utils;

public class TreeViewItemRadioVisible : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count != 2)
        {
            return false;
        }
        var itemToggleType = values[0] as ItemToggleType?;
        var itemCount = values[1] as int?;
        return itemToggleType == ItemToggleType.Radio && itemCount == 0;
    }
}