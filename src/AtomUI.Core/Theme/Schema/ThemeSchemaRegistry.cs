using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using AtomUI.Theme.Resources;
using AtomUI.Theme.Styling;

namespace AtomUI.Theme.Schema;

internal sealed class ThemeSchemaRegistry
{
    private readonly FrozenDictionary<string, TokenDescriptor> _globalTokensByName;
    private readonly FrozenDictionary<ControlTokenIdentity, ControlTokenDescriptor> _controlsByIdentity;
    private readonly FrozenDictionary<string, ThemeAlgorithmDescriptor> _algorithmsById;
    private readonly FrozenDictionary<object, int> _controlResourceSlotsByKey;
    private readonly object?[] _sharedResourceKeys;
    private readonly object?[][] _controlSharedResourceKeys;

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
        Revision              = ComputeRevision(globalArray, controlArray, algorithmArray);
        _globalTokensByName   = globalMap.ToFrozenDictionary(StringComparer.Ordinal);
        _controlsByIdentity   = controlMap.ToFrozenDictionary();
        _algorithmsById       = algorithmMap.ToFrozenDictionary(StringComparer.Ordinal);
        _controlResourceSlotsByKey = CreateControlResourceSlotMap(controlArray);
        _sharedResourceKeys = CreateSharedResourceKeys(globalArray);
        _controlSharedResourceKeys = CreateControlSharedResourceKeys(controlArray.Length);
    }

    public IReadOnlyList<TokenDescriptor> GlobalTokens { get; }
    public IReadOnlyList<ControlTokenDescriptor> Controls { get; }
    public IReadOnlyList<ThemeAlgorithmDescriptor> Algorithms { get; }
    public ThemeSchemaRevision Revision { get; }

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

    internal bool TryGetControlResourceSlot(object resourceKey, out int controlSlot)
    {
        return _controlResourceSlotsByKey.TryGetValue(resourceKey, out controlSlot);
    }

    internal object GetSharedResourceKey(SharedTokenKind kind)
    {
        var kindSlot = (int)kind;
        if ((uint)kindSlot >= (uint)_sharedResourceKeys.Length ||
            _sharedResourceKeys[kindSlot] is not { } key)
        {
            throw new ArgumentOutOfRangeException(nameof(kind));
        }
        return key;
    }

    internal object GetControlSharedResourceKey(int controlSlot, SharedTokenKind kind)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(controlSlot);
        if ((uint)controlSlot >= (uint)_controlSharedResourceKeys.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(controlSlot));
        }

        var kindSlot = (int)kind;
        if ((uint)kindSlot >= (uint)_controlSharedResourceKeys[controlSlot].Length ||
            _controlSharedResourceKeys[controlSlot][kindSlot] is not { } key)
        {
            throw new ArgumentOutOfRangeException(nameof(kind));
        }
        return key;
    }

    internal object GetControlSharedResourceKey(
        ControlTokenIdentity identity,
        SharedTokenKind kind)
    {
        if (!TryGetControl(identity, out var descriptor))
        {
            throw new KeyNotFoundException($"Control Token identity '{identity}' is not registered.");
        }
        return GetControlSharedResourceKey(descriptor.Slot, kind);
    }

    private static object?[][] CreateControlSharedResourceKeys(int controlCount)
    {
        var kinds = Enum.GetValues<SharedTokenKind>();
        var length = kinds.Length == 0 ? 0 : kinds.Max(static kind => (int)kind) + 1;
        var result = new object?[controlCount][];
        for (var controlSlot = 0; controlSlot < controlCount; controlSlot++)
        {
            var keys = new object?[length];
            foreach (var kind in kinds)
            {
                keys[(int)kind] = new ControlSharedTokenResourceKey(controlSlot, kind);
            }
            result[controlSlot] = keys;
        }
        return result;
    }

    private static object?[] CreateSharedResourceKeys(
        IReadOnlyList<TokenDescriptor> globalTokens)
    {
        var kinds = Enum.GetValues<SharedTokenKind>();
        var length = kinds.Length == 0 ? 0 : kinds.Max(static kind => (int)kind) + 1;
        var result = new object?[length];
        foreach (var descriptor in globalTokens)
        {
            if (descriptor.ResourceKey is SharedTokenKind kind)
            {
                result[(int)kind] = descriptor.ResourceKey;
            }
        }
        return result;
    }

    private static FrozenDictionary<object, int> CreateControlResourceSlotMap(
        IReadOnlyList<ControlTokenDescriptor> controls)
    {
        var result = new Dictionary<object, int>();
        foreach (var control in controls)
        {
            foreach (var token in control.OwnTokens)
            {
                if (!result.TryAdd(token.ResourceKey, control.Slot))
                {
                    throw new ThemeSchemaException(
                        $"Control Token resource key '{token.ResourceKey}' is registered more than once.");
                }
            }
        }

        return result.ToFrozenDictionary();
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

    private static ThemeSchemaRevision ComputeRevision(
        IReadOnlyList<TokenDescriptor> globalTokens,
        IReadOnlyList<ControlTokenDescriptor> controls,
        IReadOnlyList<ThemeAlgorithmDescriptor> algorithms)
    {
        var fingerprint = new SchemaFingerprintBuilder();
        fingerprint.Add(globalTokens.Count);
        foreach (var token in globalTokens)
        {
            AddToken(ref fingerprint, token);
        }

        fingerprint.Add(controls.Count);
        foreach (var control in controls)
        {
            fingerprint.Add(control.Identity.Catalog);
            fingerprint.Add(control.Identity.Id);
            fingerprint.Add(control.Slot);
            fingerprint.Add(control.OwnTokens.Count);
            foreach (var token in control.OwnTokens)
            {
                AddToken(ref fingerprint, token);
            }
        }

        fingerprint.Add(algorithms.Count);
        foreach (var algorithm in algorithms)
        {
            fingerprint.Add(algorithm.Id);
            fingerprint.Add(algorithm.Revision);
            fingerprint.Add((byte)algorithm.AppearanceEffect);
        }

        return new ThemeSchemaRevision(fingerprint.Value);
    }

    private static void AddToken(ref SchemaFingerprintBuilder fingerprint, TokenDescriptor token)
    {
        fingerprint.Add(token.Name);
        fingerprint.Add(token.Slot);
        fingerprint.Add((byte)token.Stage);
        fingerprint.Add(token.ValueType.Assembly.GetName().Name ?? string.Empty);
        fingerprint.Add(token.ValueType.FullName ?? token.ValueType.Name);
        AddResourceKey(ref fingerprint, token.ResourceKey);
    }

    private static void AddResourceKey(ref SchemaFingerprintBuilder fingerprint, object resourceKey)
    {
        var keyType = resourceKey.GetType();
        fingerprint.Add(keyType.Assembly.GetName().Name ?? string.Empty);
        fingerprint.Add(keyType.FullName ?? keyType.Name);
        switch (resourceKey)
        {
            case Enum enumValue:
                fingerprint.Add(Convert.ToUInt64(enumValue, CultureInfo.InvariantCulture));
                break;
            case string stringValue:
                fingerprint.Add(stringValue);
                break;
            default:
                throw new ThemeSchemaException(
                    $"Resource key type '{keyType.FullName}' must be an enum or string to participate in a deterministic schema.");
        }
    }

    private struct SchemaFingerprintBuilder
    {
        private const ulong Offset = 14695981039346656037UL;
        private const ulong Prime = 1099511628211UL;
        private ulong _value;

        internal ulong Value => _value == 0 ? Offset : _value;

        internal void Add(string value)
        {
            Add(value.Length);
            foreach (var character in value)
            {
                Add((ushort)character);
            }
        }

        internal void Add(int value) => Add(unchecked((ulong)(uint)value));

        internal void Add(byte value) => Add((ulong)value);

        internal void Add(ushort value) => Add((ulong)value);

        internal void Add(ulong value)
        {
            if (_value == 0)
            {
                _value = Offset;
            }

            unchecked
            {
                for (var shift = 0; shift < 64; shift += 8)
                {
                    _value ^= (byte)(value >> shift);
                    _value *= Prime;
                }
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
