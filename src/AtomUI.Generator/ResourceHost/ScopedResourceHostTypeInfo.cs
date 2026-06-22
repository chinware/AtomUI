namespace AtomUI.Generator.ResourceHost;

internal sealed class ScopedResourceHostTypeInfo
{
    public ScopedResourceHostTypeInfo(string? @namespace, string accessibility, string typeName)
    {
        Namespace     = @namespace;
        Accessibility = accessibility;
        TypeName      = typeName;
    }

    public string? Namespace { get; }

    public string Accessibility { get; }

    public string TypeName { get; }
}
