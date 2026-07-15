using AtomUI.Theme.TokenSystem;
using AtomUI.Theme.Styling;

namespace AtomUI.Theme.Compilation;

internal sealed class ComponentThemeSnapshot
{
    internal ComponentThemeSnapshot(
        DesignToken effectiveSharedToken,
        IReadOnlyDictionary<object, object?> sharedResourceDelta,
        IControlDesignToken controlToken,
        IReadOnlyDictionary<object, object?> controlResources)
    {
        EffectiveSharedToken = effectiveSharedToken;
        SharedResourceDelta  = sharedResourceDelta;
        ControlToken         = controlToken;
        ControlResources     = controlResources;
    }

    public DesignToken EffectiveSharedToken { get; }
    public IReadOnlyDictionary<object, object?> SharedResourceDelta { get; }
    public IControlDesignToken ControlToken { get; }
    public IReadOnlyDictionary<object, object?> ControlResources { get; }

    internal bool TryGetSharedResource(
        SharedTokenKind kind,
        IReadOnlyDictionary<object, object?> globalResources,
        out object? value)
    {
        return SharedResourceDelta.TryGetValue(kind, out value) ||
               globalResources.TryGetValue(kind, out value);
    }
}
