using System.Globalization;

namespace AtomUI.Theme.Schema;

public static class ThemeTokenValueFormatter
{
    public static string Format<T>(T value)
    {
        return value switch
        {
            null => "null",
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
