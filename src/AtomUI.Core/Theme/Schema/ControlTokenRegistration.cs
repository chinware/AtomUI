using System.Diagnostics.CodeAnalysis;
using AtomUI.Theme.Resources;
using AtomUI.Theme.Schema;
using AtomUI.Theme.TokenSystem;

namespace AtomUI.Theme;

public readonly struct ControlTokenRegistration
{
    public ControlTokenRegistration(ControlTokenDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);
        Descriptor      = descriptor;
        TokenType       = typeof(AbstractControlDesignToken);
        TokenId         = descriptor.Identity.Id;
        ResourceCatalog = descriptor.Identity.Catalog;
    }

    public ControlTokenRegistration(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor |
                                    DynamicallyAccessedMemberTypes.PublicProperties |
                                    DynamicallyAccessedMemberTypes.NonPublicProperties)]
        Type tokenType)
        : this(tokenType, null, null, true)
    {
    }

    public ControlTokenRegistration(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor |
                                    DynamicallyAccessedMemberTypes.PublicProperties |
                                    DynamicallyAccessedMemberTypes.NonPublicProperties)]
        Type tokenType,
        string tokenId,
        string? resourceCatalog = null)
        : this(tokenType, tokenId, resourceCatalog, true)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tokenId);
    }

    private ControlTokenRegistration(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor |
                                    DynamicallyAccessedMemberTypes.PublicProperties |
                                    DynamicallyAccessedMemberTypes.NonPublicProperties)]
        Type tokenType,
        string? tokenId,
        string? resourceCatalog,
        bool _)
    {
        TokenType        = tokenType;
        TokenId          = string.IsNullOrEmpty(tokenId) ? null : tokenId;
        ResourceCatalog  = string.IsNullOrEmpty(resourceCatalog) ? null : resourceCatalog;
        Descriptor       = null;
    }

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor |
                                DynamicallyAccessedMemberTypes.PublicProperties |
                                DynamicallyAccessedMemberTypes.NonPublicProperties)]
    public Type TokenType { get; }

    public ControlTokenDescriptor? Descriptor { get; }

    internal string? TokenId { get; }

    internal string? ResourceCatalog { get; }

    internal bool HasIdentityMetadata => TokenId is not null;

    internal AbstractControlDesignToken? Activate()
    {
        return Descriptor?.CreateBuilder() ??
               Activator.CreateInstance(TokenType) as AbstractControlDesignToken;
    }

    internal bool TryGetIdentity(out Schema.ControlTokenIdentity identity)
    {
        if (TokenId is null)
        {
            identity = default;
            return false;
        }

        identity = new Schema.ControlTokenIdentity(
            ResourceCatalog ?? ControlDesignTokenAttribute.DefaultCatalog,
            TokenId);
        return true;
    }

    internal Resources.ControlTokenIdentity GetResourceIdentity(AbstractControlDesignToken token)
    {
        if (Descriptor is not null)
        {
            return new Resources.ControlTokenIdentity(
                Descriptor.Identity.Catalog,
                Descriptor.Identity.Id);
        }

        return TokenId is null
            ? new Resources.ControlTokenIdentity(null, token.Id)
            : new Resources.ControlTokenIdentity(ResourceCatalog, TokenId);
    }
}
