namespace AtomUI.Theme;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class SemanticPartAttribute : Attribute
{
    public SemanticPartAttribute(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name;
    }

    public string Name { get; }
    public string? Path { get; set; }
    public string? SelectorClass { get; set; }
    public string? SelectorRoute { get; set; }
    public Type? ContractType { get; set; }
    public SemanticPartCardinality Cardinality { get; set; } = SemanticPartCardinality.Single;
    public SemanticPartCustomization Customization { get; set; } = SemanticPartCustomization.Selector;
    public string? ThemePropertyName { get; set; }
    public bool CrossVisualRoot { get; set; }
    public string? Since { get; set; }
    public bool RuntimeCreated { get; set; }
    public bool CrossNestedOwners { get; set; }
}
