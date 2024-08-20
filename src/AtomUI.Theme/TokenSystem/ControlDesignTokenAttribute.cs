namespace AtomUI.Theme.TokenSystem;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class ControlDesignTokenAttribute : Attribute
{
   public string Namespace { get; }
   
   public ControlDesignTokenAttribute(string resourceNamespace = $"{ResourceNamespace.Root}.{ResourceNamespace.Token}")
   {
      Namespace = resourceNamespace;
   }
}