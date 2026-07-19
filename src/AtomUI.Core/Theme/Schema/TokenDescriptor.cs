using AtomUI.Theme.DesignTokens;

namespace AtomUI.Theme.Schema;

public sealed class TokenDescriptor
{
    private readonly Func<string, object?> _parser;
    private readonly Func<object?, string> _formatter;
    private readonly Func<AbstractDesignToken, object?> _getter;
    private readonly Action<AbstractDesignToken, object?> _setter;
    private readonly Func<AbstractDesignToken, object?> _resourceProjector;

    public TokenDescriptor(
        string name,
        int slot,
        TokenStage stage,
        Type valueType,
        object resourceKey,
        Func<string, object?> parser,
        Func<object?, string> formatter,
        Func<AbstractDesignToken, object?> getter,
        Action<AbstractDesignToken, object?> setter,
        Func<AbstractDesignToken, object?> resourceProjector)
    {
        SchemaIdentifier.Validate(name, nameof(name));
        ArgumentOutOfRangeException.ThrowIfNegative(slot);
        ArgumentNullException.ThrowIfNull(valueType);
        ArgumentNullException.ThrowIfNull(resourceKey);
        ArgumentNullException.ThrowIfNull(parser);
        ArgumentNullException.ThrowIfNull(formatter);
        ArgumentNullException.ThrowIfNull(getter);
        ArgumentNullException.ThrowIfNull(setter);
        ArgumentNullException.ThrowIfNull(resourceProjector);

        Name               = name;
        Slot               = slot;
        Stage              = stage;
        ValueType          = valueType;
        ResourceKey        = resourceKey;
        _parser            = parser;
        _formatter         = formatter;
        _getter            = getter;
        _setter            = setter;
        _resourceProjector = resourceProjector;
    }

    public string Name { get; }
    public int Slot { get; }
    public TokenStage Stage { get; }
    public Type ValueType { get; }
    public object ResourceKey { get; }

    public object? Parse(string value) => _parser(value);

    public string Format(object? value) => _formatter(value);

    public object? GetValue(AbstractDesignToken token)
    {
        ArgumentNullException.ThrowIfNull(token);
        return _getter(token);
    }

    public void SetValue(AbstractDesignToken token, object? value)
    {
        ArgumentNullException.ThrowIfNull(token);
        _setter(token, value);
    }

    public object? ProjectResourceValue(AbstractDesignToken token)
    {
        ArgumentNullException.ThrowIfNull(token);
        return _resourceProjector(token);
    }
}
