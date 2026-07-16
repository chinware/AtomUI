namespace AtomUI.Theme.TokenSystem;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ControlDesignTokenAttribute : Attribute
{
    public const string DefaultCatalog = "AtomUI";

    public ControlDesignTokenAttribute(string catalog = DefaultCatalog)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(catalog);
        Catalog = catalog;
    }

    public string Catalog { get; }
}
