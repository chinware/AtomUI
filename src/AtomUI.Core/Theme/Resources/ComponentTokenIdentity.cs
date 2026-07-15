using System.Collections.Concurrent;
using AtomUI.Theme.Styling;

namespace AtomUI.Theme.Resources;

internal readonly record struct ComponentTokenIdentity
{
    private static readonly ConcurrentDictionary<ComponentSharedTokenCacheKey, object> s_sharedTokenKeys = new();

    internal ComponentTokenIdentity(string? resourceCatalog, string tokenId)
    {
        ResourceCatalog = string.IsNullOrEmpty(resourceCatalog) ? null : resourceCatalog;
        TokenId         = tokenId;
    }

    public string? ResourceCatalog { get; }
    public string TokenId { get; }

    internal object GetSharedTokenResourceKey(SharedTokenKind kind)
    {
        return s_sharedTokenKeys.GetOrAdd(
            new ComponentSharedTokenCacheKey(this, kind),
            static key => new ComponentSharedTokenResourceKey(
                key.Identity.ResourceCatalog,
                key.Identity.TokenId,
                key.Kind));
    }

    public override string ToString()
    {
        return ResourceCatalog is null ? TokenId : $"{ResourceCatalog}:{TokenId}";
    }

    private readonly record struct ComponentSharedTokenCacheKey(
        ComponentTokenIdentity Identity,
        SharedTokenKind Kind);
}
