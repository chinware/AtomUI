using System.Globalization;
using Avalonia.Animation.Easings;

namespace AtomUI.Theme.Schema;

public static class ThemeTokenValueFormatter
{
    public static string Format<T>(T value)
    {
        return value switch
        {
            null => "null",
            SplineEasing easing => string.Create(
                CultureInfo.InvariantCulture,
                $"Spline({easing.X1:R},{easing.Y1:R},{easing.X2:R},{easing.Y2:R})"),
            SpringEasing easing => string.Create(
                CultureInfo.InvariantCulture,
                $"Spring({easing.Mass:R},{easing.Stiffness:R},{easing.Damping:R},{easing.InitialVelocity:R})"),
            string text => text,
            bool boolean => boolean ? "true" : "false",
            double number => number.ToString("R", CultureInfo.InvariantCulture),
            float number => number.ToString("R", CultureInfo.InvariantCulture),
            TimeSpan timeSpan => timeSpan.ToString("c", CultureInfo.InvariantCulture),
            Enum enumValue => enumValue.ToString(),
            IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture) ?? string.Empty,
            _ => value.ToString() ?? string.Empty
        };
    }
}
