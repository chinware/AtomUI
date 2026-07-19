using System.Diagnostics.CodeAnalysis;
using AtomUI.Theme.DesignTokens;

namespace AtomUI.Theme.Schema;

public sealed class ControlTokenDescriptor
{
    private readonly Func<AbstractControlDesignToken> _factory;
    private readonly Action<AbstractControlDesignToken, ThemeAppearance> _evaluator;

    public ControlTokenDescriptor(
        ControlTokenIdentity identity,
        IEnumerable<TokenDescriptor> ownTokens,
        Func<AbstractControlDesignToken> factory,
        Action<AbstractControlDesignToken, ThemeAppearance> evaluator)
        : this(identity, -1, ownTokens, Array.Empty<TokenDescriptor>(), factory, evaluator)
    {
    }

    private ControlTokenDescriptor(
        ControlTokenIdentity identity,
        int slot,
        IEnumerable<TokenDescriptor> ownTokens,
        IReadOnlyList<TokenDescriptor> inheritedTokens,
        Func<AbstractControlDesignToken> factory,
        Action<AbstractControlDesignToken, ThemeAppearance> evaluator)
    {
        ArgumentNullException.ThrowIfNull(ownTokens);
        ArgumentNullException.ThrowIfNull(inheritedTokens);
        ArgumentNullException.ThrowIfNull(factory);
        ArgumentNullException.ThrowIfNull(evaluator);

        Identity        = identity;
        Slot            = slot;
        OwnTokens       = Array.AsReadOnly(ownTokens.OrderBy(static token => token.Slot).ToArray());
        InheritedTokens = inheritedTokens;
        _factory        = factory;
        _evaluator      = evaluator;
    }

    public ControlTokenIdentity Identity { get; }
    public int Slot { get; }
    public IReadOnlyList<TokenDescriptor> OwnTokens { get; }
    public IReadOnlyList<TokenDescriptor> InheritedTokens { get; }

    public AbstractControlDesignToken CreateBuilder() => _factory();

    public void Evaluate(AbstractControlDesignToken token, ThemeAppearance appearance)
    {
        ArgumentNullException.ThrowIfNull(token);
        _evaluator(token, appearance);
    }

    internal ControlTokenDescriptor Bind(int slot, IReadOnlyList<TokenDescriptor> inheritedTokens)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(slot);
        return new ControlTokenDescriptor(Identity, slot, OwnTokens, inheritedTokens, _factory, _evaluator);
    }

    internal bool TryGetOwnToken(
        string name,
        [NotNullWhen(true)] out TokenDescriptor? descriptor)
    {
        foreach (var candidate in OwnTokens)
        {
            if (string.Equals(candidate.Name, name, StringComparison.Ordinal))
            {
                descriptor = candidate;
                return true;
            }
        }

        descriptor = null;
        return false;
    }

    internal bool TryGetInheritedToken(
        string name,
        [NotNullWhen(true)] out TokenDescriptor? descriptor)
    {
        foreach (var candidate in InheritedTokens)
        {
            if (string.Equals(candidate.Name, name, StringComparison.Ordinal))
            {
                descriptor = candidate;
                return true;
            }
        }

        descriptor = null;
        return false;
    }
}
