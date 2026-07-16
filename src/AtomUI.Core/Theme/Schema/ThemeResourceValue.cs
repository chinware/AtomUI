using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace AtomUI.Theme.Schema;

public static class ThemeResourceValue
{
    public static object? Project<T>(T value)
    {
        return value is Color color
            ? new ImmutableSolidColorBrush(color)
            : value;
    }
}
