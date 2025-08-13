using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace AtomUI.Controls.Utils;

internal class BoolToThicknessConverter : IValueConverter
{
    // 当绑定值为 true 时，用 Parameter 指定的 Thickness，否则返回 Empty
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // 没传参数则默认为 4 像素左侧间距
        return new Thickness(4, 0, 0, 0);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}