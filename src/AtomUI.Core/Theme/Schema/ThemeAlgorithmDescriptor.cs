using AtomUI.Theme.Algorithms;

namespace AtomUI.Theme.Schema;

public enum ThemeAppearanceEffect : byte
{
    Preserve,
    Light,
    Dark
}

public sealed class ThemeAlgorithmDescriptor
{
    private readonly Func<IThemeAlgorithm> _factory;

    public ThemeAlgorithmDescriptor(
        string id,
        int revision,
        ThemeAppearanceEffect appearanceEffect,
        Func<IThemeAlgorithm> factory)
    {
        SchemaIdentifier.Validate(id, nameof(id));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(revision);
        ArgumentNullException.ThrowIfNull(factory);
        Id               = id;
        Revision         = revision;
        AppearanceEffect = appearanceEffect;
        _factory         = factory;
    }

    public string Id { get; }
    public int Revision { get; }
    public ThemeAppearanceEffect AppearanceEffect { get; }

    public IThemeAlgorithm Create()
    {
        return _factory() ??
               throw new InvalidOperationException($"Theme algorithm '{Id}' factory returned null.");
    }
}
