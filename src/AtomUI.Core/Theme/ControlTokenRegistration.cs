using System.Diagnostics.CodeAnalysis;
using AtomUI.Theme.Resources;
using AtomUI.Theme.TokenSystem;

namespace AtomUI.Theme;

public readonly struct ControlTokenRegistration
{
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
    }

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor |
                                DynamicallyAccessedMemberTypes.PublicProperties |
                                DynamicallyAccessedMemberTypes.NonPublicProperties)]
    public Type TokenType { get; }

    internal string? TokenId { get; }

    internal string? ResourceCatalog { get; }

    internal bool HasIdentityMetadata => TokenId is not null;

    internal AbstractControlDesignToken? Activate()
    {
        return Activator.CreateInstance(TokenType) as AbstractControlDesignToken;
    }

    internal bool TryGetIdentity(out ComponentTokenIdentity identity)
    {
        if (TokenId is null)
        {
            identity = default;
            return false;
        }

        identity = new ComponentTokenIdentity(ResourceCatalog, TokenId);
        return true;
    }

    internal ComponentTokenIdentity GetIdentity(AbstractControlDesignToken token)
    {
        return TokenId is null
            ? new ComponentTokenIdentity(null, token.Id)
            : new ComponentTokenIdentity(ResourceCatalog, TokenId);
    }
}
