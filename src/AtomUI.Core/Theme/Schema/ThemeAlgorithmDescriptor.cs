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
    private readonly Func<IThemeAlgorithm?, IThemeAlgorithm> _factory;

    public ThemeAlgorithmDescriptor(
        string id,
        ThemeAppearanceEffect appearanceEffect,
        bool requiresBase,
        Func<IThemeAlgorithm?, IThemeAlgorithm> factory)
    {
        SchemaIdentifier.Validate(id, nameof(id));
        ArgumentNullException.ThrowIfNull(factory);
        Id               = id;
        AppearanceEffect = appearanceEffect;
        RequiresBase     = requiresBase;
        _factory         = factory;
    }

    public string Id { get; }
    public ThemeAppearanceEffect AppearanceEffect { get; }
    public bool RequiresBase { get; }

    public IThemeAlgorithm Create(IThemeAlgorithm? baseAlgorithm)
    {
        if (RequiresBase && baseAlgorithm is null)
        {
            throw new InvalidOperationException($"Theme algorithm '{Id}' requires a base algorithm.");
        }

        return _factory(baseAlgorithm) ??
               throw new InvalidOperationException($"Theme algorithm '{Id}' factory returned null.");
    }
}
