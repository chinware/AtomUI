using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;

namespace AtomUI.Theme.Schema;

public sealed class SemanticPartRegistry
{
    public static SemanticPartRegistry Empty { get; } = new(Array.Empty<ControlSemanticDescriptor>());

    private readonly FrozenDictionary<Type, ControlSemanticDescriptor> _controlsByType;
    private readonly FrozenDictionary<ControlTokenIdentity, ControlSemanticDescriptor> _controlsByIdentity;

    public SemanticPartRegistry(IEnumerable<ControlSemanticDescriptor> controls)
    {
        ArgumentNullException.ThrowIfNull(controls);
        ControlSemanticDescriptor?[] nullableControls = controls.ToArray();
        if (nullableControls.Any(static descriptor => descriptor is null))
        {
            throw new ArgumentException("Semantic control descriptors cannot contain null.", nameof(controls));
        }
        var controlArray = nullableControls
                           .Select(static descriptor => descriptor!)
                           .OrderBy(
                                static descriptor => descriptor.Identity.Catalog,
                                StringComparer.Ordinal)
                           .ThenBy(static descriptor => descriptor.Identity.Id, StringComparer.Ordinal)
                           .ToArray();

        _controlsByType = BuildUniqueMap(
                              controlArray,
                              static descriptor => descriptor.ControlType,
                              "Control type")
                          .ToFrozenDictionary();
        _controlsByIdentity = BuildUniqueMap(
                                  controlArray,
                                  static descriptor => descriptor.Identity,
                                  "Control identity")
                              .ToFrozenDictionary();
        Controls = Array.AsReadOnly(controlArray);
    }

    public IReadOnlyList<ControlSemanticDescriptor> Controls { get; }

    public bool TryGetControl(
        Type controlType,
        [NotNullWhen(true)] out ControlSemanticDescriptor? descriptor)
    {
        ArgumentNullException.ThrowIfNull(controlType);
        return _controlsByType.TryGetValue(controlType, out descriptor);
    }

    public bool TryGetControl(
        ControlTokenIdentity identity,
        [NotNullWhen(true)] out ControlSemanticDescriptor? descriptor)
    {
        return _controlsByIdentity.TryGetValue(identity, out descriptor);
    }

    private static Dictionary<TKey, ControlSemanticDescriptor> BuildUniqueMap<TKey>(
        IEnumerable<ControlSemanticDescriptor> controls,
        Func<ControlSemanticDescriptor, TKey> keySelector,
        string kind)
        where TKey : notnull
    {
        var result = new Dictionary<TKey, ControlSemanticDescriptor>();
        foreach (var descriptor in controls)
        {
            var key = keySelector(descriptor);
            if (!result.TryAdd(key, descriptor))
            {
                throw new ArgumentException(
                    $"Semantic Part registry contains duplicate {kind} '{key}'.",
                    nameof(controls));
            }
        }
        return result;
    }
}
