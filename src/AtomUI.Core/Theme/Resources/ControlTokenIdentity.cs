using System.Collections.Concurrent;
using AtomUI.Theme.Styling;
using AtomUI.Theme.TokenSystem;

namespace AtomUI.Theme.Resources;

internal readonly record struct ControlTokenIdentity
{
    private static readonly ConcurrentDictionary<ControlSharedTokenCacheKey, object> s_sharedTokenKeys = new();

    internal ControlTokenIdentity(string? resourceCatalog, string tokenId)
    {
        ResourceCatalog = string.IsNullOrEmpty(resourceCatalog)
            ? ControlDesignTokenAttribute.DefaultCatalog
            : resourceCatalog;
        TokenId         = tokenId;
    }

    public string? ResourceCatalog { get; }
    public string TokenId { get; }

    internal object GetSharedTokenResourceKey(SharedTokenKind kind)
    {
        return s_sharedTokenKeys.GetOrAdd(
            new ControlSharedTokenCacheKey(this, kind),
            static key => new ControlSharedTokenResourceKey(
                key.Identity.ResourceCatalog,
                key.Identity.TokenId,
                key.Kind));
    }

    public override string ToString()
    {
        return $"{ResourceCatalog}:{TokenId}";
    }

    private readonly record struct ControlSharedTokenCacheKey(
        ControlTokenIdentity Identity,
        SharedTokenKind Kind);
}
