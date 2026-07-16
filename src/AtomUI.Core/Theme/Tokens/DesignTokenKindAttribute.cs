namespace AtomUI.Theme.TokenSystem;

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple=false)]
public class DesignTokenKindAttribute : Attribute
{
    public DesignTokenKind Kind { get; }

    public DesignTokenKindAttribute(DesignTokenKind kind)
    {
        Kind = kind;
    }
}