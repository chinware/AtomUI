namespace AtomUI.Controls.Data;

public interface IDataMemberAccessorDescriptor
{
    Type DataType { get; }

    Func<object>? NewItemFactory { get; }

    IReadOnlyList<IDataMemberAccessor> Accessors { get; }

    bool TryGetAccessor(string path, out IDataMemberAccessor accessor);
}
