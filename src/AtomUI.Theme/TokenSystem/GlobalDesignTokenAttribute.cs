namespace AtomUI.Theme.TokenSystem;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class GlobalDesignTokenAttribute : Attribute
{
    public const string DefaultResourceCatalog = $"{ResourceCatalogConstants.Root}.{ResourceCatalogConstants.Token}";

    public GlobalDesignTokenAttribute(string resourceCatalog = DefaultResourceCatalog)
    {
        ResourceCatalog = resourceCatalog;
    }

    public string ResourceCatalog { get; }
}