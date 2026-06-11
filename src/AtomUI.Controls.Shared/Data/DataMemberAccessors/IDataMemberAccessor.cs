using System.Collections;

namespace AtomUI.Controls.Data;

public interface IDataMemberAccessor
{
    string Path { get; }

    Type ValueType { get; }

    IComparer? Comparer { get; }

    bool CanWrite { get; }

    object? GetValue(object instance);

    void SetValue(object instance, object? value);
}
