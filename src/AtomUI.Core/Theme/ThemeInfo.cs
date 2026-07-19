namespace AtomUI.Theme;

public sealed record ThemeInfo(
    string Id,
    string Name,
    ThemeAppearance Appearance,
    bool IsDefault);
