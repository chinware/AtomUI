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
        var names   = Enum.GetNames(enumType);
        var entries = new ThemeResourceKeyEntry[names.Length];
        for (int i = 0; i < names.Length; i++)
        {
            var name  = names[i];
            var value = Enum.Parse(enumType, name);

            entries[i] = new ThemeResourceKeyEntry(value, name);
        }

        return entries;
    }
}
