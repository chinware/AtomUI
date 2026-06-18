using System.Collections.ObjectModel;

namespace AtomUI.Controls;

internal readonly record struct ResponsiveValueMap<T>
{
    private readonly IReadOnlyDictionary<MediaBreakPoint, T>? _values;

    public ResponsiveValueMap(T scalar)
    {
        IsScalar = true;
        Scalar = scalar;
        _values = null;
    }

    public ResponsiveValueMap(IReadOnlyDictionary<MediaBreakPoint, T> values)
    {
        IsScalar = false;
        Scalar = default;
        _values = new ReadOnlyDictionary<MediaBreakPoint, T>(new Dictionary<MediaBreakPoint, T>(values));
    }

    public bool IsScalar { get; }

    public T? Scalar { get; }

    public T Resolve(MediaBreakPoint current, T fallback)
    {
        if (IsScalar)
        {
            return Scalar!;
        }

        if (_values is null)
        {
            return fallback;
        }

        foreach (var breakPoint in ResponsiveBreakpoints.Descending)
        {
            if (current >= breakPoint && _values.TryGetValue(breakPoint, out var value))
            {
                return value;
            }
        }

        return fallback;
    }

    public bool TryResolve(MediaBreakPoint current, out T value)
    {
        if (IsScalar)
        {
            value = Scalar!;
            return true;
        }

        if (_values is not null)
        {
            foreach (var breakPoint in ResponsiveBreakpoints.Descending)
            {
                if (current >= breakPoint && _values.TryGetValue(breakPoint, out value!))
                {
                    return true;
                }
            }
        }

        value = default!;
        return false;
    }
}
