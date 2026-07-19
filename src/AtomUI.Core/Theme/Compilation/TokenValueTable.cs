using AtomUI.Theme.Schema;
using AtomUI.Theme.TokenSystem;
using Avalonia.Animation.Easings;
using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace AtomUI.Theme.Compilation;

internal sealed class TokenValueTable
{
    internal static readonly TokenValueTable Empty = new(Array.Empty<object?>());

    private readonly object?[] _values;

    private TokenValueTable(object?[] values)
    {
        _values                = values;
        EstimatedRetainedBytes = ThemeRetainedBytesEstimator.EstimateDenseValues(values);
    }

    internal int Count => _values.Length;
    internal long EstimatedRetainedBytes { get; }

    internal T Get<T>(int slot)
    {
        var value = GetValue(slot);
        if (value is T typed)
        {
            return typed;
        }

        if (value is null && default(T) is null)
        {
            return default!;
        }

        throw new InvalidCastException(
            $"Token slot {slot} contains '{value?.GetType().FullName ?? "null"}', not '{typeof(T).FullName}'.");
    }

    internal object? GetValue(int slot)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(slot);
        if ((uint)slot >= (uint)_values.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(slot));
        }

        return _values[slot];
    }

    internal bool ValueEquals(int slot, object? value)
    {
        return Equals(GetValue(slot), value);
    }

    internal static TokenValueTable Freeze(
        AbstractDesignToken builder,
        IReadOnlyList<TokenDescriptor> descriptors)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(descriptors);

        var values = new object?[GetRequiredLength(descriptors)];
        foreach (var descriptor in descriptors)
        {
            values[descriptor.Slot] = FreezeValue(descriptor, descriptor.GetValue(builder));
        }

        return new TokenValueTable(values);
    }

    private static int GetRequiredLength(IReadOnlyList<TokenDescriptor> descriptors)
    {
        var length = 0;
        foreach (var descriptor in descriptors)
        {
            length = Math.Max(length, descriptor.Slot + 1);
        }

        return length;
    }

    private static object? FreezeValue(TokenDescriptor descriptor, object? value)
    {
        if (value is SolidColorBrush solidColorBrush)
        {
            return ThemeResourceValue.CloneSolidColorBrush(solidColorBrush);
        }

        if (value is IBrush brush)
        {
            return brush.ToImmutable();
        }

        if (value is SplineEasing splineEasing)
        {
            return new SplineEasing(
                splineEasing.X1,
                splineEasing.Y1,
                splineEasing.X2,
                splineEasing.Y2);
        }

        if (value is SpringEasing springEasing)
        {
            return new SpringEasing(
                springEasing.Mass,
                springEasing.Stiffness,
                springEasing.Damping,
                springEasing.InitialVelocity);
        }

        if (value is Easing easing)
        {
            return descriptor.Parse(easing.GetType().Name);
        }

        if (value is null || value.GetType().IsValueType || value is string ||
            value is FontFamily or ImmutableTransform)
        {
            return value;
        }

        // Descriptor conversion is the schema-owned copy boundary for other reference values.
        return descriptor.Parse(descriptor.Format(value));
    }
}
