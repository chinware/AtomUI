using Avalonia.Media;

namespace AtomUI.Theme;

public sealed record ThemeInfo(
    string Id,
    string Name,
    ThemeAppearance Appearance,
    bool IsDefault)
{
    public ThemeInfo(
        string id,
        string name,
        ThemeAppearance appearance,
        bool isDefault,
        Color? accentColor)
        : this(id, name, appearance, isDefault)
    {
        AccentColor = accentColor;
    }

    public Color? AccentColor { get; }
}
