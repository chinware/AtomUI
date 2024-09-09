namespace AtomUI.Theme.TokenSystem;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class ControlDesignTokenAttribute : Attribute
{
    public const string DefaultResourceCatalog = $"{ResourceCatalogConstants.Root}.{ResourceCatalogConstants.Token}";

    public ControlDesignTokenAttribute(string resourceCatalog = DefaultResourceCatalog)
    {
        ResourceCatalog = resourceCatalog;
    }

    public string ResourceCatalog { get; }
}