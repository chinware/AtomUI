using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;

namespace AtomUI.Theme.Schema;

internal sealed class ThemeSchemaRegistry
{
    private readonly FrozenDictionary<string, TokenDescriptor> _globalTokensByName;
    private readonly FrozenDictionary<ControlTokenIdentity, ControlTokenDescriptor> _controlsByIdentity;
    private readonly FrozenDictionary<string, ThemeAlgorithmDescriptor> _algorithmsById;

    internal ThemeSchemaRegistry(
        IEnumerable<TokenDescriptor> globalTokens,
        IEnumerable<ControlTokenDescriptor> controls,
        IEnumerable<ThemeAlgorithmDescriptor> algorithms)
    {
        ArgumentNullException.ThrowIfNull(globalTokens);
        ArgumentNullException.ThrowIfNull(controls);
        ArgumentNullException.ThrowIfNull(algorithms);

        var globalArray = globalTokens.OrderBy(static descriptor => descriptor.Slot).ToArray();
        var controlInputs = controls.OrderBy(static descriptor => descriptor.Identity.Catalog, StringComparer.Ordinal)
                                    .ThenBy(static descriptor => descriptor.Identity.Id, StringComparer.Ordinal)
                                    .ToArray();
        var algorithmArray = algorithms.OrderBy(static descriptor => descriptor.Id, StringComparer.Ordinal).ToArray();

        ValidateDenseSlots(globalArray, static descriptor => descriptor.Slot, "global Token");

        var globalMap = BuildUniqueMap(
            globalArray,
            static descriptor => descriptor.Name,
            StringComparer.Ordinal,
            "global Token");
        foreach (var descriptor in globalArray)
        {
            if (descriptor.Stage == TokenStage.Control)
            {
                throw new ThemeSchemaException(
                    $"Global Token '{descriptor.Name}' cannot use the Control stage.");
            }
        }

        BuildUniqueMap(
            controlInputs,
            static descriptor => descriptor.Identity,
            EqualityComparer<ControlTokenIdentity>.Default,
            "Control");
        foreach (var descriptor in controlInputs)
        {
            ValidateControlDescriptor(descriptor);
        }

        var globalTokensView = Array.AsReadOnly(globalArray);
        var controlArray = new ControlTokenDescriptor[controlInputs.Length];
        for (var slot = 0; slot < controlInputs.Length; slot++)
        {
            controlArray[slot] = controlInputs[slot].Bind(slot, globalTokensView);
        }
        var controlMap = BuildUniqueMap(
            controlArray,
            static descriptor => descriptor.Identity,
            EqualityComparer<ControlTokenIdentity>.Default,
            "Control");

        var algorithmMap = BuildUniqueMap(
            algorithmArray,
            static descriptor => descriptor.Id,
            StringComparer.Ordinal,
            "algorithm");

        GlobalTokens          = globalTokensView;
        Controls              = Array.AsReadOnly(controlArray);
        Algorithms            = Array.AsReadOnly(algorithmArray);
        _globalTokensByName   = globalMap.ToFrozenDictionary(StringComparer.Ordinal);
        _controlsByIdentity   = controlMap.ToFrozenDictionary();
        _algorithmsById       = algorithmMap.ToFrozenDictionary(StringComparer.Ordinal);
    }

    public IReadOnlyList<TokenDescriptor> GlobalTokens { get; }
    public IReadOnlyList<ControlTokenDescriptor> Controls { get; }
    public IReadOnlyList<ThemeAlgorithmDescriptor> Algorithms { get; }

    internal bool TryGetGlobalToken(
        string name,
        [NotNullWhen(true)] out TokenDescriptor? descriptor)
    {
        return _globalTokensByName.TryGetValue(name, out descriptor);
    }

    internal bool TryGetControl(
        ControlTokenIdentity identity,
        [NotNullWhen(true)] out ControlTokenDescriptor? descriptor)
    {
        return _controlsByIdentity.TryGetValue(identity, out descriptor);
    }

    internal bool TryGetAlgorithm(
        string id,
        [NotNullWhen(true)] out ThemeAlgorithmDescriptor? descriptor)
    {
        return _algorithmsById.TryGetValue(id, out descriptor);
    }

    private static void ValidateControlDescriptor(ControlTokenDescriptor descriptor)
    {
        ValidateDenseSlots(descriptor.OwnTokens, static token => token.Slot, $"{descriptor.Identity} own Token");
        var ownNames = new HashSet<string>(StringComparer.Ordinal);
        foreach (var token in descriptor.OwnTokens)
        {
            if (token.Stage != TokenStage.Control)
            {
                throw new ThemeSchemaException(
                    $"Control Token '{descriptor.Identity}:{token.Name}' must use the Control stage.");
            }

            if (!ownNames.Add(token.Name))
            {
                throw new ThemeSchemaException(
                    $"Control '{descriptor.Identity}' contains duplicate own Token '{token.Name}'.");
            }
        }
    }

    private static Dictionary<TKey, TDescriptor> BuildUniqueMap<TDescriptor, TKey>(
        IEnumerable<TDescriptor> descriptors,
        Func<TDescriptor, TKey> keySelector,
        IEqualityComparer<TKey> comparer,
        string kind)
        where TDescriptor : class
        where TKey : notnull
    {
        var map = new Dictionary<TKey, TDescriptor>(comparer);
        foreach (var descriptor in descriptors)
        {
            var key = keySelector(descriptor);
            if (!map.TryAdd(key, descriptor))
            {
                throw new ThemeSchemaException($"Duplicate {kind} identity '{key}'.");
            }
        }

        return map;
    }

    private static void ValidateDenseSlots<TDescriptor>(
        IReadOnlyList<TDescriptor> descriptors,
        Func<TDescriptor, int> slotSelector,
        string kind)
    {
        for (var index = 0; index < descriptors.Count; index++)
        {
            var slot = slotSelector(descriptors[index]);
            if (slot != index)
            {
                throw new ThemeSchemaException(
                    $"The {kind} slot table must be contiguous from zero; expected {index}, found {slot}.");
            }
        }
    }
}

internal sealed class ThemeSchemaException : Exception
{
    internal ThemeSchemaException(string message)
        : base(message)
    {
    }
}
