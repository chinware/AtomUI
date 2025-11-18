using Avalonia.Markup.Xaml;

namespace AtomUI.Controls;

public abstract class IconInfoProvider : MarkupExtension
{
    public string Kind { get; set; }

    public IconInfoProvider()
    {
        Kind = string.Empty;
    }

    public IconInfoProvider(string kind)
    {
        Kind = kind;
    }
}