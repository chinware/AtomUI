namespace AtomUI.Theme.Resources;

internal readonly record struct ComponentTokenIdentity(string? ResourceCatalog, string TokenId)
{
    public override string ToString()
    {
        return ResourceCatalog is null ? TokenId : $"{ResourceCatalog}:{TokenId}";
    }
}
