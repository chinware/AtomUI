namespace AtomUI.Localization;

public sealed record LanguageCatalogUnitDescriptor
{
    public LanguageCatalogUnitDescriptor(string key, bool isFormatted = false)
    {
        ArgumentNullException.ThrowIfNull(key);
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("A language unit Key cannot be empty or whitespace.", nameof(key));
        }

        Key = key;
        IsFormatted = isFormatted;
    }

    public string Key { get; }

    public bool IsFormatted { get; }
}
