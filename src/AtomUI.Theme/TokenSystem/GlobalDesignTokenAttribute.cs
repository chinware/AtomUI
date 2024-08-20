namespace AtomUI.Theme.TokenSystem;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class GlobalDesignTokenAttribute : Attribute
{
   public string Namespace { get; }
   
   public GlobalDesignTokenAttribute(string resourceNamespace = $"{ResourceNamespace.Root}.{ResourceNamespace.Token}")
   {
      Namespace = resourceNamespace;
   }
}