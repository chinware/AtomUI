using AtomUI.Theme.DesignTokens;

namespace AtomUI.Theme.Schema;

public static class ThemeTokenValueParser
{
    private static readonly IReadOnlyDictionary<Type, ITokenValueConverter> s_converters =
        TokenValueConverterRegistry.Create();

    public static T Parse<T>(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (!s_converters.TryGetValue(typeof(T), out var converter))
        {
            throw new InvalidOperationException(
                $"No theme Token value converter is registered for '{typeof(T).FullName}'.");
        }

        var converted = converter.Convert(value);
        if (converted is T typed)
        {
            return typed;
        }

        throw new InvalidOperationException(
            $"Theme Token value converter for '{typeof(T).FullName}' returned an incompatible value.");
    }
}
