namespace AtomUI.Theme.TokenSystem;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class ControlDesignTokenAttribute : Attribute
{
   public const string DefaultResourceCatalog = $"{ResourceCatalogConstants.Root}.{ResourceCatalogConstants.Token}";
   public string ResourceCatalog { get; }
   
   public ControlDesignTokenAttribute(string resourceCatalog = DefaultResourceCatalog)
   {
      ResourceCatalog = resourceCatalog;
   }
}