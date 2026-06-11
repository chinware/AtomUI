namespace AtomUI.Controls.Data;

public sealed class DataMemberAccessorMetadata
{
    public static DataMemberAccessorMetadata Default { get; } = new();

    public string? DisplayName { get; }

    public bool? AutoGenerateField { get; }

    public int? Order { get; }

    public bool IsReadOnly { get; }

    public bool IsEditable { get; }

    public DataMemberAccessorMetadata(string? displayName = null,
                                      bool? autoGenerateField = null,
                                      int? order = null,
                                      bool isReadOnly = false,
                                      bool isEditable = true)
    {
        DisplayName       = displayName;
        AutoGenerateField = autoGenerateField;
        Order             = order;
        IsReadOnly        = isReadOnly;
        IsEditable        = isEditable;
    }
}

public interface IDataMemberAccessorMetadataProvider
{
    DataMemberAccessorMetadata Metadata { get; }
}

public interface IDataMemberAccessorDescriptorMetadataProvider
{
    bool IsDataTypeReadOnly { get; }
}
