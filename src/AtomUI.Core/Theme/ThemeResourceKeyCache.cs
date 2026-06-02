using System.Collections.Concurrent;

namespace AtomUI.Theme;

internal readonly struct ThemeResourceKeyEntry
{
    public ThemeResourceKeyEntry(object value, string name)
    {
        Value = value;
        Name  = name;
    }

    public object Value { get; }
    public string Name { get; }
}

internal static class ThemeResourceKeyCache
{
    private static readonly ConcurrentDictionary<Type, ThemeResourceKeyEntry[]> s_enumEntriesByType = new();

    public static ThemeResourceKeyEntry[] GetEnumEntries(Type enumType)
    {
        return s_enumEntriesByType.GetOrAdd(enumType, BuildEnumEntries);
    }

    private static ThemeResourceKeyEntry[] BuildEnumEntries(Type enumType)
    {
        var values  = Enum.GetValues(enumType);
        var entries = new ThemeResourceKeyEntry[values.Length];
        for (int i = 0; i < values.Length; i++)
        {
            var value = values.GetValue(i)!;
            var name  = Enum.GetName(enumType, value);
            if (name is null)
            {
                throw new InvalidOperationException($"Enum value '{value}' has no name in {enumType.FullName}.");
            }

            entries[i] = new ThemeResourceKeyEntry(value, name);
        }

        return entries;
    }
}
