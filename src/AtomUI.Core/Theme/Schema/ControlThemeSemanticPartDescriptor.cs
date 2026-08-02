namespace AtomUI.Theme.Schema;

public sealed record ControlThemeSemanticPartDescriptor
{
    public ControlThemeSemanticPartDescriptor(string propertyName, string targetTypeName)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            throw new ArgumentException("Semantic Part Theme property name cannot be empty.", nameof(propertyName));
        }
        if (string.IsNullOrWhiteSpace(targetTypeName))
        {
            throw new ArgumentException("Semantic Part Theme target type cannot be empty.", nameof(targetTypeName));
        }

        PropertyName = propertyName;
        TargetTypeName = targetTypeName;
    }

    public string PropertyName { get; }
    public string TargetTypeName { get; }
}
