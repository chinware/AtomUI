using AtomUI.Theme.TokenSystem;
using AtomUI.Theme.Styling;
using Avalonia.Controls;

namespace AtomUI.Theme.Compilation;

internal sealed class ControlThemeSnapshot
{
    private readonly DesignToken _effectiveSharedToken;
    private readonly IControlDesignToken _controlToken;

    internal ControlThemeSnapshot(
        DesignToken effectiveSharedToken,
        IReadOnlyDictionary<object, object?> sharedResourceDelta,
        IControlDesignToken controlToken,
        IReadOnlyDictionary<object, object?> controlResources)
    {
        _effectiveSharedToken = effectiveSharedToken;
        SharedResourceDelta  = sharedResourceDelta;
        _controlToken         = controlToken;
        ControlResources     = controlResources;
    }

    public DesignToken EffectiveSharedToken => DesignTokenClone.DeepClone(_effectiveSharedToken);
    public IReadOnlyDictionary<object, object?> SharedResourceDelta { get; }
    public IControlDesignToken ControlToken => CloneControlToken(_controlToken, _effectiveSharedToken);
    public IReadOnlyDictionary<object, object?> ControlResources { get; }
    internal DesignToken EffectiveSharedTokenCore => _effectiveSharedToken;
    internal IControlDesignToken ControlTokenCore => _controlToken;

    internal static IControlDesignToken CloneControlToken(
        IControlDesignToken source,
        DesignToken effectiveSharedToken)
    {
        var clone = (IControlDesignToken)source.Clone();
        clone.AssignSharedToken(DesignTokenClone.DeepClone(effectiveSharedToken));
        clone.SetHasCustomTokenConfig(source.HasCustomTokenConfig());
        clone.SetCustomTokens(source.GetCustomTokens().ToList());
        CopyResourceDictionary(source.GetSharedResourceDeltaDictionary(), clone.GetSharedResourceDeltaDictionary());
        return clone;
    }

    internal bool TryGetSharedResource(
        SharedTokenKind kind,
        IReadOnlyDictionary<object, object?> globalResources,
        out object? value)
    {
        return SharedResourceDelta.TryGetValue(kind, out value) ||
               globalResources.TryGetValue(kind, out value);
    }

    private static void CopyResourceDictionary(
        IResourceDictionary source,
        IResourceDictionary destination)
    {
        foreach (var key in source.Keys)
        {
            destination[key] = source[key];
        }
    }
}
