using AtomUI.Theme.Schema;

namespace AtomUI.Theme.Algorithms;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ThemeAlgorithmAttribute : Attribute
{
    public ThemeAlgorithmAttribute(string id, ThemeAppearanceEffect appearanceEffect)
    {
        Id = id;
        AppearanceEffect = appearanceEffect;
    }

    public string Id { get; }

    public ThemeAppearanceEffect AppearanceEffect { get; }
}
