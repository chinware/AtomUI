using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace AtomUI.Theme.Schema;

public static class ThemeResourceValue
{
    public static object? Project(SolidColorBrush? value)
    {
        return CloneSolidColorBrush(value);
    }

    public static object? Project<T>(T value)
    {
        return value switch
        {
            Color color  => new ImmutableSolidColorBrush(color),
            IBrush brush => brush.ToImmutable(),
            _            => value
        };
    }

    internal static SolidColorBrush? CloneSolidColorBrush(SolidColorBrush? value)
    {
        if (value is null)
        {
            return null;
        }

        return new SolidColorBrush(value.Color, value.Opacity)
        {
            Transform = value.Transform?.ToImmutable(),
            TransformOrigin = value.TransformOrigin
        };
    }
}
