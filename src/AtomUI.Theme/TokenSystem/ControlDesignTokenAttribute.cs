namespace AtomUI.Theme.TokenSystem;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class ControlDesignTokenAttribute : Attribute
{
    public string? ResourceCatalog { get; }

    public ControlDesignTokenAttribute(string? resourceCatalog = null)
    {
        ResourceCatalog = resourceCatalog;
    }
}