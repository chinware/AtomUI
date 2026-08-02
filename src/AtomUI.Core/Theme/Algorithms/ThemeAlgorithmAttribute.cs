using AtomUI.Theme.Schema;

namespace AtomUI.Theme.Algorithms;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ThemeAlgorithmAttribute : Attribute
{
    public ThemeAlgorithmAttribute(
        ThemeAlgorithm algorithm,
        int revision,
        ThemeAppearanceEffect appearanceEffect)
    {
        if (!Enum.IsDefined(algorithm))
        {
            throw new ArgumentOutOfRangeException(nameof(algorithm), algorithm, "Theme algorithm must be defined.");
        }
        Algorithm        = algorithm;
        Revision         = revision;
        AppearanceEffect = appearanceEffect;
    }

    public ThemeAlgorithm Algorithm { get; }

    public int Revision { get; }

    public ThemeAppearanceEffect AppearanceEffect { get; }
}
