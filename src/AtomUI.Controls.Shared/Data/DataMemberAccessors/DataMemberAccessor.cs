using System.Collections;

namespace AtomUI.Controls.Data;

public sealed class DataMemberAccessor<TData, TValue> : IDataMemberAccessor, IDataMemberAccessorMetadataProvider
{
    private readonly Func<TData, TValue> _getter;
    private readonly Action<TData, TValue>? _setter;

    public string Path { get; }

    public Type ValueType => typeof(TValue);

    public IComparer? Comparer { get; }

    public bool CanWrite => _setter is not null;

    public DataMemberAccessorMetadata Metadata { get; }

    public DataMemberAccessor(string path,
                              Func<TData, TValue> getter,
                              Action<TData, TValue>? setter = null,
                              IComparer? comparer = null,
                              DataMemberAccessorMetadata? metadata = null)
    {
        Path     = path;
        _getter  = getter;
        _setter  = setter;
        Comparer = comparer ?? DataMemberValueComparer<TValue>.Default;
        Metadata = metadata ?? DataMemberAccessorMetadata.Default;
    }

    public object? GetValue(object instance)
    {
        return _getter((TData)instance);
    }

    public void SetValue(object instance, object? value)
    {
        if (_setter is null)
        {
            throw new InvalidOperationException($"Data member '{Path}' on type '{typeof(TData).FullName}' cannot be written.");
        }

        _setter((TData)instance, CastValue(value));
    }

    private static TValue CastValue(object? value)
    {
        if (value is null)
        {
            return default!;
        }

        return (TValue)value;
    }
}
