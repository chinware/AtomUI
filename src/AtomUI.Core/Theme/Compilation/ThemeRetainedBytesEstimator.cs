using AtomUI.Theme.Algorithms;
using Avalonia.Media;

namespace AtomUI.Theme.Compilation;

internal static class ThemeRetainedBytesEstimator
{
    private const long ObjectOverhead = 32;
    private const long ReferenceBytes = 8;
    private const long DictionaryEntryBytes = 48;
    private const long ImmutableBrushBytes = 96;

    internal static long EstimateDenseValues(IReadOnlyList<object?> values)
    {
        var retainedBytes = Add(ObjectOverhead, values.Count * ReferenceBytes);
        foreach (var value in values)
        {
            retainedBytes = Add(retainedBytes, EstimateValue(value));
        }
        return retainedBytes;
    }

    internal static long EstimateControl(ControlThemeSnapshot control)
    {
        var retainedBytes = ObjectOverhead;
        retainedBytes = Add(retainedBytes, control.ControlTokenValues.EstimatedRetainedBytes);
        retainedBytes = Add(retainedBytes, EstimateSlotMap(control.EffectiveGlobalTokenDelta));
        retainedBytes = Add(retainedBytes, EstimateResourceMap(control.EffectiveGlobalResourceDelta));
        retainedBytes = Add(retainedBytes, EstimateResourceMap(control.ControlResources));
        return retainedBytes;
    }

    internal static long EstimateSnapshot(ThemeSnapshot snapshot)
    {
        var retainedBytes = Add(ObjectOverhead, EstimateValue(snapshot.ThemeId));
        retainedBytes = Add(retainedBytes, EstimateValue(snapshot.DefinitionRevision.SourceIdentity));
        retainedBytes = Add(retainedBytes, EstimateValue(snapshot.DefinitionRevision.SourceRevision));
        retainedBytes = Add(retainedBytes, EstimateValue(snapshot.DefinitionRevision.ContentDigest));
        retainedBytes = Add(retainedBytes, snapshot.GlobalTokenValues.EstimatedRetainedBytes);
        retainedBytes = Add(retainedBytes, EstimateResourceMap(snapshot.GlobalResources));
        retainedBytes = Add(retainedBytes, EstimatePalettes(snapshot.PresetColorPalettes));
        retainedBytes = Add(retainedBytes, ObjectOverhead + snapshot.Controls.Count * ReferenceBytes);
        foreach (var control in snapshot.Controls)
        {
            retainedBytes = Add(retainedBytes, control.EstimatedRetainedBytes);
        }

        return retainedBytes;
    }

    private static long EstimateSlotMap(IReadOnlyDictionary<int, object?> values)
    {
        var retainedBytes = Add(ObjectOverhead, values.Count * DictionaryEntryBytes);
        foreach (var value in values.Values)
        {
            retainedBytes = Add(retainedBytes, EstimateValue(value));
        }
        return retainedBytes;
    }

    private static long EstimateResourceMap(IReadOnlyDictionary<object, object?> values)
    {
        var retainedBytes = Add(ObjectOverhead, values.Count * DictionaryEntryBytes);
        foreach (var (key, value) in values)
        {
            retainedBytes = Add(retainedBytes, EstimateValue(key));
            retainedBytes = Add(retainedBytes, EstimateValue(value));
        }
        return retainedBytes;
    }

    private static long EstimatePalettes(IReadOnlyDictionary<PresetPrimaryColor, PaletteInfo> palettes)
    {
        var retainedBytes = Add(ObjectOverhead, palettes.Count * DictionaryEntryBytes);
        foreach (var palette in palettes.Values)
        {
            retainedBytes = Add(retainedBytes, ObjectOverhead + 8 + palette.ColorSequence.Count * 4L);
        }
        return retainedBytes;
    }

    private static long EstimateValue(object? value)
    {
        return value switch
        {
            null => 0,
            string text => ObjectOverhead + text.Length * 2L,
            IBrush => ImmutableBrushBytes,
            FontFamily family => ObjectOverhead + EstimateValue(family.Name),
            ValueType => 24,
            _ => 64
        };
    }

    private static long Add(long left, long right)
    {
        if (right < 0 || left > long.MaxValue - right)
        {
            return long.MaxValue;
        }
        return left + right;
    }
}
