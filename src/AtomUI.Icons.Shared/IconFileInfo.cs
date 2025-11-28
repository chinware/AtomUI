using AtomUI.Controls;

namespace AtomUI.Icons;

public record IconFileInfo
{
    public string? Category;
    public string FilePath = string.Empty;
    public string Name = string.Empty;
    public IconThemeType ThemeType;
}