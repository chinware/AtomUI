namespace AtomUI.Controls.Data;

public sealed class DataMemberAccessorDescriptor<TData> : IDataMemberAccessorDescriptor,
                                                          IDataMemberAccessorDescriptorMetadataProvider
{
    private readonly Dictionary<string, IDataMemberAccessor> _accessorsByPath;

    public Type DataType => typeof(TData);

    public Func<object>? NewItemFactory { get; }

    public IReadOnlyList<IDataMemberAccessor> Accessors { get; }

    public bool IsDataTypeReadOnly { get; }

    public DataMemberAccessorDescriptor(Func<TData>? newItemFactory,
                                        IReadOnlyList<IDataMemberAccessor> accessors,
                                        bool isDataTypeReadOnly = false)
    {
        NewItemFactory     = newItemFactory is null ? null : () => newItemFactory()!;
        Accessors          = accessors;
        IsDataTypeReadOnly = isDataTypeReadOnly;
        _accessorsByPath   = new Dictionary<string, IDataMemberAccessor>(accessors.Count, StringComparer.Ordinal);
        foreach (var accessor in accessors)
        {
            _accessorsByPath[accessor.Path] = accessor;
        }
    }

    public bool TryGetAccessor(string path, out IDataMemberAccessor accessor)
    {
        return _accessorsByPath.TryGetValue(path, out accessor!);
    }
}
