using AtomUI.Theme.Schema;

namespace AtomUI.Theme.Algorithms;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ThemeAlgorithmAttribute : Attribute
{
    public ThemeAlgorithmAttribute(
        string id,
        int revision,
        ThemeAppearanceEffect appearanceEffect)
    {
        Id               = id;
        Revision         = revision;
        AppearanceEffect = appearanceEffect;
    }

    public string Id { get; }

    public int Revision { get; }

    public ThemeAppearanceEffect AppearanceEffect { get; }
}
