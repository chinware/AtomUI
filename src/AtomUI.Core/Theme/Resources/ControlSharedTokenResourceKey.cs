using AtomUI.Theme.Styling;

namespace AtomUI.Theme.Resources;

public readonly record struct ControlSharedTokenResourceKey
{
    public ControlSharedTokenResourceKey(string? catalog, string controlId, SharedTokenKind kind)
    {
        Catalog     = string.IsNullOrEmpty(catalog) ? null : catalog;
        ControlId   = controlId;
        Kind        = kind;
    }

    public string? Catalog { get; }
    public string ControlId { get; }
    public SharedTokenKind Kind { get; }
}
