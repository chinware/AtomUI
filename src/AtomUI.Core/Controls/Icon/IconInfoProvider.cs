using Avalonia.Markup.Xaml;

namespace AtomUI.Controls;

public abstract class IconInfoProvider<TIconKind> : MarkupExtension
    where TIconKind : Enum
{
    public TIconKind? Kind { get; set; }
    
    public IconInfoProvider()
    {}

    public IconInfoProvider(TIconKind kind)
    {
        Kind = kind;
    }
}