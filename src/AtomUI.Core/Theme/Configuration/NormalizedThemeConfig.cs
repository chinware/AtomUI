using System.Globalization;
using AtomUI.Theme.Schema;

namespace AtomUI.Theme.Configuration;

internal readonly record struct ThemeConfigFingerprint(ulong Value)
{
    public override string ToString() => Value.ToString("X16", CultureInfo.InvariantCulture);
}

internal sealed class NormalizedTokenValue : IEquatable<NormalizedTokenValue>
{
    internal NormalizedTokenValue(TokenDescriptor descriptor, object? value, string canonicalValue)
    {
        ArgumentNullException.ThrowIfNull(descriptor);
        ArgumentNullException.ThrowIfNull(canonicalValue);
        Descriptor     = descriptor;
        Value          = value;
        CanonicalValue = canonicalValue;
    }

    public TokenDescriptor Descriptor { get; }
    public object? Value { get; }
    internal string CanonicalValue { get; }

    public bool Equals(NormalizedTokenValue? other)
    {
        return other is not null &&
               ReferenceEquals(Descriptor, other.Descriptor) &&
               string.Equals(CanonicalValue, other.CanonicalValue, StringComparison.Ordinal);
    }

    public override bool Equals(object? obj) => Equals(obj as NormalizedTokenValue);

    public override int GetHashCode() => HashCode.Combine(Descriptor, CanonicalValue);
}

internal sealed class NormalizedControlThemeConfig : IEquatable<NormalizedControlThemeConfig>
{
    private readonly ThemeAlgorithmDescriptor[] _algorithms;
    private readonly NormalizedTokenValue[] _globalTokens;
    private readonly NormalizedTokenValue[] _ownTokens;

    internal NormalizedControlThemeConfig(
        ControlTokenIdentity identity,
        ControlAlgorithmMode algorithmMode,
        IEnumerable<ThemeAlgorithmDescriptor> algorithms,
        IEnumerable<NormalizedTokenValue> globalTokens,
        IEnumerable<NormalizedTokenValue> ownTokens)
    {
        ArgumentNullException.ThrowIfNull(algorithms);
        ArgumentNullException.ThrowIfNull(globalTokens);
        ArgumentNullException.ThrowIfNull(ownTokens);

        Identity      = identity;
        AlgorithmMode = algorithmMode;
        _algorithms   = ThemeConfigArray.Copy(algorithms);
        _globalTokens = SortTokens(globalTokens);
        _ownTokens    = SortTokens(ownTokens);
        Algorithms    = Array.AsReadOnly(_algorithms);
        GlobalTokens  = Array.AsReadOnly(_globalTokens);
        OwnTokens     = Array.AsReadOnly(_ownTokens);
    }

    public ControlTokenIdentity Identity { get; }
    public ControlAlgorithmMode AlgorithmMode { get; }
    public IReadOnlyList<ThemeAlgorithmDescriptor> Algorithms { get; }
    public IReadOnlyList<NormalizedTokenValue> GlobalTokens { get; }
    public IReadOnlyList<NormalizedTokenValue> OwnTokens { get; }

    public bool Equals(NormalizedControlThemeConfig? other)
    {
        return other is not null &&
               Identity == other.Identity &&
               AlgorithmMode == other.AlgorithmMode &&
               SequenceEqual(_algorithms, other._algorithms) &&
               SequenceEqual(_globalTokens, other._globalTokens) &&
               SequenceEqual(_ownTokens, other._ownTokens);
    }

    public override bool Equals(object? obj) => Equals(obj as NormalizedControlThemeConfig);

    public override int GetHashCode() => HashCode.Combine(Identity, AlgorithmMode);

    private static NormalizedTokenValue[] SortTokens(IEnumerable<NormalizedTokenValue> values)
    {
        var result = ThemeConfigArray.Copy(values);
        Array.Sort(result, static (left, right) => left.Descriptor.Slot.CompareTo(right.Descriptor.Slot));
        return result;
    }

    private static bool SequenceEqual<T>(IReadOnlyList<T> left, IReadOnlyList<T> right)
    {
        if (left.Count != right.Count)
        {
            return false;
        }

        var comparer = EqualityComparer<T>.Default;
        for (var index = 0; index < left.Count; index++)
        {
            if (!comparer.Equals(left[index], right[index]))
            {
                return false;
            }
        }

        return true;
    }
}

internal sealed class NormalizedThemeConfig : IEquatable<NormalizedThemeConfig>
{
    private readonly ThemeAlgorithmDescriptor[] _algorithms;
    private readonly NormalizedTokenValue[] _globalTokens;
    private readonly NormalizedControlThemeConfig[] _controls;

    internal NormalizedThemeConfig(
        bool inherit,
        bool algorithmsSpecified,
        IEnumerable<ThemeAlgorithmDescriptor> algorithms,
        IEnumerable<NormalizedTokenValue> globalTokens,
        IEnumerable<NormalizedControlThemeConfig> controls)
    {
        ArgumentNullException.ThrowIfNull(algorithms);
        ArgumentNullException.ThrowIfNull(globalTokens);
        ArgumentNullException.ThrowIfNull(controls);

        Inherit              = inherit;
        AlgorithmsSpecified = algorithmsSpecified;
        _algorithms          = ThemeConfigArray.Copy(algorithms);
        _globalTokens        = ThemeConfigArray.Copy(globalTokens);
        _controls            = ThemeConfigArray.Copy(controls);
        Array.Sort(_globalTokens, static (left, right) => left.Descriptor.Slot.CompareTo(right.Descriptor.Slot));
        Array.Sort(_controls, static (left, right) => CompareIdentity(left.Identity, right.Identity));
        Algorithms  = Array.AsReadOnly(_algorithms);
        GlobalTokens = Array.AsReadOnly(_globalTokens);
        Controls     = Array.AsReadOnly(_controls);
        Fingerprint  = ThemeConfigFingerprintBuilder.Compute(this);
    }

    public bool Inherit { get; }
    public bool AlgorithmsSpecified { get; }
    public IReadOnlyList<ThemeAlgorithmDescriptor> Algorithms { get; }
    public IReadOnlyList<NormalizedTokenValue> GlobalTokens { get; }
    public IReadOnlyList<NormalizedControlThemeConfig> Controls { get; }
    public ThemeConfigFingerprint Fingerprint { get; }

    public bool Equals(NormalizedThemeConfig? other)
    {
        return other is not null &&
               Fingerprint == other.Fingerprint &&
               Inherit == other.Inherit &&
               AlgorithmsSpecified == other.AlgorithmsSpecified &&
               SequenceEqual(_algorithms, other._algorithms) &&
               SequenceEqual(_globalTokens, other._globalTokens) &&
               SequenceEqual(_controls, other._controls);
    }

    public override bool Equals(object? obj) => Equals(obj as NormalizedThemeConfig);

    public override int GetHashCode() => HashCode.Combine(Fingerprint, Inherit, AlgorithmsSpecified);

    private static int CompareIdentity(ControlTokenIdentity left, ControlTokenIdentity right)
    {
        var catalog = string.Compare(left.Catalog, right.Catalog, StringComparison.Ordinal);
        return catalog != 0 ? catalog : string.Compare(left.Id, right.Id, StringComparison.Ordinal);
    }

    private static bool SequenceEqual<T>(IReadOnlyList<T> left, IReadOnlyList<T> right)
    {
        if (left.Count != right.Count)
        {
            return false;
        }

        var comparer = EqualityComparer<T>.Default;
        for (var index = 0; index < left.Count; index++)
        {
            if (!comparer.Equals(left[index], right[index]))
            {
                return false;
            }
        }

        return true;
    }
}

internal struct ThemeConfigFingerprintBuilder
{
    private const ulong Offset = 14695981039346656037UL;
    private const ulong Prime = 1099511628211UL;
    private ulong _value;

    internal static ThemeConfigFingerprint Compute(NormalizedThemeConfig config)
    {
        var builder = new ThemeConfigFingerprintBuilder
        {
            _value = Offset
        };
        builder.Add(config.Inherit);
        builder.Add(config.AlgorithmsSpecified);
        builder.Add(config.Algorithms.Count);
        foreach (var algorithm in config.Algorithms)
        {
            builder.Add(algorithm.Id);
        }

        builder.Add(config.GlobalTokens.Count);
        foreach (var token in config.GlobalTokens)
        {
            builder.Add(token.Descriptor.Slot);
            builder.Add(token.Descriptor.Name);
            builder.Add(token.CanonicalValue);
        }

        builder.Add(config.Controls.Count);
        foreach (var control in config.Controls)
        {
            builder.Add(control.Identity.Catalog);
            builder.Add(control.Identity.Id);
            builder.Add((int)control.AlgorithmMode);
            builder.Add(control.Algorithms.Count);
            foreach (var algorithm in control.Algorithms)
            {
                builder.Add(algorithm.Id);
            }

            AddTokens(ref builder, control.GlobalTokens);
            AddTokens(ref builder, control.OwnTokens);
        }

        return new ThemeConfigFingerprint(builder._value);
    }

    private static void AddTokens(ref ThemeConfigFingerprintBuilder builder, IReadOnlyList<NormalizedTokenValue> tokens)
    {
        builder.Add(tokens.Count);
        foreach (var token in tokens)
        {
            builder.Add(token.Descriptor.Slot);
            builder.Add(token.Descriptor.Name);
            builder.Add(token.CanonicalValue);
        }
    }

    private void Add(bool value) => Add(value ? 1 : 0);

    private void Add(int value)
    {
        unchecked
        {
            Add((char)value);
            Add((char)(value >> 16));
        }
    }

    private void Add(string value)
    {
        Add(value.Length);
        foreach (var character in value)
        {
            Add(character);
        }
    }

    private void Add(char value)
    {
        unchecked
        {
            _value ^= (byte)value;
            _value *= Prime;
            _value ^= (byte)(value >> 8);
            _value *= Prime;
        }
    }
}

internal static class ThemeConfigArray
{
    internal static T[] Copy<T>(IEnumerable<T> source)
    {
        return source switch
        {
            T[] array => (T[])array.Clone(),
            ICollection<T> collection => CopyCollection(collection),
            _ => new List<T>(source).ToArray()
        };
    }

    private static T[] CopyCollection<T>(ICollection<T> source)
    {
        if (source.Count == 0)
        {
            return Array.Empty<T>();
        }

        var result = new T[source.Count];
        source.CopyTo(result, 0);
        return result;
    }
}
