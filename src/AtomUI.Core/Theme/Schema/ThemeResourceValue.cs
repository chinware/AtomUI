using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Animation.Easings;

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

    internal static object? CloneForConsumer(object? value)
    {
        return value switch
        {
            SolidColorBrush brush => CloneSolidColorBrush(brush),
            IBrush brush => brush.ToImmutable(),
            SplineEasing easing => new SplineEasing(
                easing.X1,
                easing.Y1,
                easing.X2,
                easing.Y2),
            SpringEasing easing => new SpringEasing(
                easing.Mass,
                easing.Stiffness,
                easing.Damping,
                easing.InitialVelocity),
            _ => value
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
