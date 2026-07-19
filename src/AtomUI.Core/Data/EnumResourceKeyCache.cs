using System.Collections.Concurrent;

namespace AtomUI.Data;

internal readonly record struct EnumResourceKeyEntry(object Value, string Name);

internal static class EnumResourceKeyCache
{
    private static readonly ConcurrentDictionary<Type, EnumResourceKeyEntry[]> s_entriesByType = new();

    internal static EnumResourceKeyEntry[] GetEntries(Type enumType)
    {
        ArgumentNullException.ThrowIfNull(enumType);
        return s_entriesByType.GetOrAdd(enumType, static type =>
        {
            var names = Enum.GetNames(type);
            var entries = new EnumResourceKeyEntry[names.Length];
            for (var index = 0; index < names.Length; index++)
            {
                entries[index] = new EnumResourceKeyEntry(
                    Enum.Parse(type, names[index]),
                    names[index]);
            }
            return entries;
        });
    }
}
