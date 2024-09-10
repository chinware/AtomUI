using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class MenuSeparatorTheme : BaseControlTheme
{
    public MenuSeparatorTheme()
        : base(typeof(MenuSeparator))
    {
    }

    protected override void BuildStyles()
    {
        var commonStyle = new Style(selector => selector.Nesting());
        commonStyle.Add(Layoutable.MinHeightProperty, MenuTokenResourceKey.SeparatorItemHeight);
        commonStyle.Add(TemplatedControl.BorderBrushProperty, GlobalTokenResourceKey.ColorBorder);
        Add(commonStyle);
    }
}