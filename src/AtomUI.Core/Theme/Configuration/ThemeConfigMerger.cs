using AtomUI.Theme.Schema;

namespace AtomUI.Theme.Configuration;

internal static class ThemeConfigMerger
{
    internal static ThemeConfigMergeResult Merge(
        NormalizedThemeConfig defaults,
        NormalizedThemeConfig? parent,
        NormalizedThemeConfig current)
    {
        ArgumentNullException.ThrowIfNull(defaults);
        ArgumentNullException.ThrowIfNull(current);

        var effectiveDefaults = ResolveDefaults(defaults);
        var effectiveParent = parent is null
            ? effectiveDefaults
            : Apply(effectiveDefaults, parent);
        var mergeBase = current.Inherit ? effectiveParent : effectiveDefaults;
        var candidate = Apply(mergeBase, current);
        var effective = candidate.Equals(effectiveParent) ? effectiveParent : candidate;
        var changeSet = CreateChangeSet(effectiveParent, effective);
        return new ThemeConfigMergeResult(effective, changeSet);
    }

    private static NormalizedThemeConfig ResolveDefaults(NormalizedThemeConfig defaults)
    {
        var controls = new NormalizedControlThemeConfig[defaults.Controls.Count];
        for (var index = 0; index < controls.Length; index++)
        {
            var control = defaults.Controls[index];
            var mode = control.AlgorithmMode == ControlAlgorithmMode.Unspecified
                ? ControlAlgorithmMode.Disabled
                : control.AlgorithmMode;
            controls[index] = new NormalizedControlThemeConfig(
                control.Identity,
                mode,
                mode == ControlAlgorithmMode.Custom
                    ? control.Algorithms
                    : Array.Empty<ThemeAlgorithmDescriptor>(),
                control.GlobalTokens,
                control.OwnTokens);
        }

        return new NormalizedThemeConfig(
            false,
            true,
            defaults.AlgorithmsSpecified
                ? defaults.Algorithms
                : Array.Empty<ThemeAlgorithmDescriptor>(),
            defaults.GlobalTokens,
            controls);
    }

    private static NormalizedThemeConfig Apply(
        NormalizedThemeConfig effectiveBase,
        NormalizedThemeConfig current)
    {
        var algorithms = current.AlgorithmsSpecified
            ? current.Algorithms
            : effectiveBase.Algorithms;
        var globalTokens = MergeTokens(effectiveBase.GlobalTokens, current.GlobalTokens);
        var controls = MergeControls(effectiveBase.Controls, current.Controls);
        return new NormalizedThemeConfig(false, true, algorithms, globalTokens, controls);
    }

    private static IReadOnlyList<NormalizedTokenValue> MergeTokens(
        IReadOnlyList<NormalizedTokenValue> inherited,
        IReadOnlyList<NormalizedTokenValue> current)
    {
        if (current.Count == 0)
        {
            return inherited;
        }

        var values = new Dictionary<int, NormalizedTokenValue>(inherited.Count + current.Count);
        foreach (var token in inherited)
        {
            values[token.Descriptor.Slot] = token;
        }
        foreach (var token in current)
        {
            values[token.Descriptor.Slot] = token;
        }

        var result = new NormalizedTokenValue[values.Count];
        values.Values.CopyTo(result, 0);
        Array.Sort(result, static (left, right) => left.Descriptor.Slot.CompareTo(right.Descriptor.Slot));
        return result;
    }

    private static IReadOnlyList<NormalizedControlThemeConfig> MergeControls(
        IReadOnlyList<NormalizedControlThemeConfig> inherited,
        IReadOnlyList<NormalizedControlThemeConfig> current)
    {
        var controls = new Dictionary<ControlTokenIdentity, NormalizedControlThemeConfig>(
            inherited.Count + current.Count);
        foreach (var control in inherited)
        {
            controls[control.Identity] = ResolveInheritedControl(control);
        }

        foreach (var local in current)
        {
            controls.TryGetValue(local.Identity, out var parent);
            var mode = local.AlgorithmMode == ControlAlgorithmMode.Unspecified
                ? parent?.AlgorithmMode ?? ControlAlgorithmMode.Disabled
                : local.AlgorithmMode;
            IReadOnlyList<ThemeAlgorithmDescriptor> algorithms;
            if (local.AlgorithmMode == ControlAlgorithmMode.Unspecified)
            {
                algorithms = mode == ControlAlgorithmMode.Custom && parent is not null
                    ? parent.Algorithms
                    : Array.Empty<ThemeAlgorithmDescriptor>();
            }
            else
            {
                algorithms = mode == ControlAlgorithmMode.Custom
                    ? local.Algorithms
                    : Array.Empty<ThemeAlgorithmDescriptor>();
            }

            controls[local.Identity] = new NormalizedControlThemeConfig(
                local.Identity,
                mode,
                algorithms,
                MergeTokens(parent?.GlobalTokens ?? Array.Empty<NormalizedTokenValue>(), local.GlobalTokens),
                MergeTokens(parent?.OwnTokens ?? Array.Empty<NormalizedTokenValue>(), local.OwnTokens));
        }

        var result = new NormalizedControlThemeConfig[controls.Count];
        controls.Values.CopyTo(result, 0);
        return result;
    }

    private static NormalizedControlThemeConfig ResolveInheritedControl(NormalizedControlThemeConfig control)
    {
        if (control.AlgorithmMode != ControlAlgorithmMode.Unspecified)
        {
            return control;
        }

        return new NormalizedControlThemeConfig(
            control.Identity,
            ControlAlgorithmMode.Disabled,
            Array.Empty<ThemeAlgorithmDescriptor>(),
            control.GlobalTokens,
            control.OwnTokens);
    }

    private static ThemeConfigChangeSet CreateChangeSet(
        NormalizedThemeConfig previous,
        NormalizedThemeConfig current)
    {
        var algorithmsChanged = !AlgorithmSequenceEqual(previous.Algorithms, current.Algorithms);
        var globalTokensChanged = !TokenSequenceEqual(previous.GlobalTokens, current.GlobalTokens);
        var changedControls = FindChangedControls(previous.Controls, current.Controls);
        return new ThemeConfigChangeSet(algorithmsChanged, globalTokensChanged, changedControls);
    }

    private static IReadOnlyList<ControlTokenIdentity> FindChangedControls(
        IReadOnlyList<NormalizedControlThemeConfig> previous,
        IReadOnlyList<NormalizedControlThemeConfig> current)
    {
        var result = new List<ControlTokenIdentity>();
        var previousIndex = 0;
        var currentIndex = 0;
        while (previousIndex < previous.Count || currentIndex < current.Count)
        {
            if (previousIndex == previous.Count)
            {
                result.Add(current[currentIndex++].Identity);
                continue;
            }
            if (currentIndex == current.Count)
            {
                result.Add(previous[previousIndex++].Identity);
                continue;
            }

            var previousControl = previous[previousIndex];
            var currentControl = current[currentIndex];
            var comparison = CompareIdentity(previousControl.Identity, currentControl.Identity);
            if (comparison < 0)
            {
                result.Add(previousControl.Identity);
                previousIndex++;
            }
            else if (comparison > 0)
            {
                result.Add(currentControl.Identity);
                currentIndex++;
            }
            else
            {
                if (!previousControl.Equals(currentControl))
                {
                    result.Add(currentControl.Identity);
                }
                previousIndex++;
                currentIndex++;
            }
        }

        return result;
    }

    private static bool AlgorithmSequenceEqual(
        IReadOnlyList<ThemeAlgorithmDescriptor> left,
        IReadOnlyList<ThemeAlgorithmDescriptor> right)
    {
        if (left.Count != right.Count)
        {
            return false;
        }
        for (var index = 0; index < left.Count; index++)
        {
            if (!ReferenceEquals(left[index], right[index]))
            {
                return false;
            }
        }
        return true;
    }

    private static bool TokenSequenceEqual(
        IReadOnlyList<NormalizedTokenValue> left,
        IReadOnlyList<NormalizedTokenValue> right)
    {
        if (left.Count != right.Count)
        {
            return false;
        }
        for (var index = 0; index < left.Count; index++)
        {
            if (!left[index].Equals(right[index]))
            {
                return false;
            }
        }
        return true;
    }

    private static int CompareIdentity(ControlTokenIdentity left, ControlTokenIdentity right)
    {
        var catalog = string.Compare(left.Catalog, right.Catalog, StringComparison.Ordinal);
        return catalog != 0 ? catalog : string.Compare(left.Id, right.Id, StringComparison.Ordinal);
    }
}

internal sealed class ThemeConfigMergeResult
{
    internal ThemeConfigMergeResult(NormalizedThemeConfig effectiveConfig, ThemeConfigChangeSet changeSet)
    {
        EffectiveConfig = effectiveConfig;
        ChangeSet       = changeSet;
    }

    public NormalizedThemeConfig EffectiveConfig { get; }
    public ThemeConfigChangeSet ChangeSet { get; }
}

internal sealed class ThemeConfigChangeSet
{
    internal ThemeConfigChangeSet(
        bool algorithmsChanged,
        bool globalTokensChanged,
        IEnumerable<ControlTokenIdentity> changedControls)
    {
        AlgorithmsChanged  = algorithmsChanged;
        GlobalTokensChanged = globalTokensChanged;
        ChangedControls     = Array.AsReadOnly(ThemeConfigArray.Copy(changedControls));
    }

    public bool AlgorithmsChanged { get; }
    public bool GlobalTokensChanged { get; }
    public IReadOnlyList<ControlTokenIdentity> ChangedControls { get; }
    public bool IsEmpty => !AlgorithmsChanged && !GlobalTokensChanged && ChangedControls.Count == 0;
}
