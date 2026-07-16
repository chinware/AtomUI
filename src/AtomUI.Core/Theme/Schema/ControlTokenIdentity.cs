namespace AtomUI.Theme.Schema;

public readonly record struct ControlTokenIdentity
{
    public ControlTokenIdentity(string catalog, string id)
    {
        SchemaIdentifier.Validate(catalog, nameof(catalog));
        SchemaIdentifier.Validate(id, nameof(id));
        Catalog = catalog;
        Id      = id;
    }

    public string Catalog { get; }
    public string Id { get; }

    public override string ToString() => $"{Catalog}:{Id}";
}
