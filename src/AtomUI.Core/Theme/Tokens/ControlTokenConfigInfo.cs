using System.Collections.ObjectModel;

namespace AtomUI.Theme.TokenSystem;

internal class ControlTokenConfigInfo
{
    private bool _enableAlgorithm;
    private string _tokenId = string.Empty;
    private IDictionary<string, string> _tokens;
    private IDictionary<string, string> _sharedTokens;
    private bool _isReadOnly;

    public bool EnableAlgorithm
    {
        get => _enableAlgorithm;
        set
        {
            ThrowIfReadOnly();
            _enableAlgorithm = value;
        }
    }
    
    public string TokenId
    {
        get => _tokenId;
        set
        {
            ThrowIfReadOnly();
            _tokenId = value;
        }
    }
    
    public IDictionary<string, string> Tokens
    {
        get => _tokens;
        set
        {
            ThrowIfReadOnly();
            _tokens = value;
        }
    }

    public IDictionary<string, string> SharedTokens
    {
        get => _sharedTokens;
        set
        {
            ThrowIfReadOnly();
            _sharedTokens = value;
        }
    }

    public ControlTokenConfigInfo() : this(0, 0)
    {
    }

    private ControlTokenConfigInfo(int tokenCapacity, int sharedTokenCapacity)
    {
        _tokens       = new Dictionary<string, string>(tokenCapacity);
        _sharedTokens = new Dictionary<string, string>(sharedTokenCapacity);
    }

    internal ControlTokenConfigInfo Clone()
    {
        var cloned = new ControlTokenConfigInfo(Tokens.Count, SharedTokens.Count)
        {
            EnableAlgorithm = EnableAlgorithm,
            TokenId         = TokenId
        };

        foreach (var token in Tokens)
        {
            cloned.Tokens.Add(token.Key, token.Value);
        }

        foreach (var sharedToken in SharedTokens)
        {
            cloned.SharedTokens.Add(sharedToken.Key, sharedToken.Value);
        }
        return cloned;
    }

    internal ControlTokenConfigInfo CloneImmutable()
    {
        var cloned = Clone();
        cloned._tokens = new ReadOnlyDictionary<string, string>(
            new Dictionary<string, string>(cloned._tokens, StringComparer.Ordinal));
        cloned._sharedTokens = new ReadOnlyDictionary<string, string>(
            new Dictionary<string, string>(cloned._sharedTokens, StringComparer.Ordinal));
        cloned._isReadOnly = true;
        return cloned;
    }

    private void ThrowIfReadOnly()
    {
        if (_isReadOnly)
        {
            throw new NotSupportedException("This control token configuration is read-only.");
        }
    }
}
