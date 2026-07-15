using AtomUI.Theme.Styling;

namespace AtomUI.Theme.Resources;

public readonly record struct ComponentSharedTokenResourceKey
{
    public ComponentSharedTokenResourceKey(string? catalog, string componentId, SharedTokenKind kind)
    {
        Catalog     = string.IsNullOrEmpty(catalog) ? null : catalog;
        ComponentId = componentId;
        Kind        = kind;
    }

    public string? Catalog { get; }
    public string ComponentId { get; }
    public SharedTokenKind Kind { get; }
}
