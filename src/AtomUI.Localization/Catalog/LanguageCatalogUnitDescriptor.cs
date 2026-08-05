namespace AtomUI.Localization;

public sealed record LanguageCatalogUnitDescriptor
{
    public LanguageCatalogUnitDescriptor(int id, string name, bool isFormatted = false)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), id, "A language unit ID must be positive.");
        }

        ArgumentNullException.ThrowIfNull(name);
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("A language unit name cannot be empty or whitespace.", nameof(name));
        }

        Id = id;
        Name = name;
        IsFormatted = isFormatted;
    }

    public int Id { get; }

    public string Name { get; }

    public bool IsFormatted { get; }
}
