using AtomUI.Theme.Styling;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls;

using AvaloniaTextBlock = Avalonia.Controls.TextBlock;

public class TextBlock : AvaloniaTextBlock
{
    static TextBlock()
    {
        FontStyleProperty.OverrideDefaultValue<TextBlock>(FontStyle.Normal);
    }

    public TextBlock()
    {
        var styles = new Style();
        styles.Add(ClipToBoundsProperty, false);
        Styles.Add(styles);
    }
}