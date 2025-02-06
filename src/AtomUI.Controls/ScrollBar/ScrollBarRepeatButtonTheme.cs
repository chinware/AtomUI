using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class ScrollBarRepeatButtonTheme : BaseControlTheme
{
    public ScrollBarRepeatButtonTheme()
        : base(typeof(ScrollBarRepeatButton))
    {
    }
    
    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<ScrollBarRepeatButton>((button, scope) =>
        {
            var frame = new Border();
            CreateTemplateParentBinding(frame, Border.BackgroundProperty, ScrollBarRepeatButton.BackgroundProperty);
            return frame;
        });
    }
    
    protected override void BuildStyles()
    {
        var commonStyle = new Style(selector => selector.Nesting());
        commonStyle.Add(new Setter(ScrollBarRepeatButton.BackgroundProperty, Brushes.Transparent));
        commonStyle.Add(new Setter(ScrollBarRepeatButton.VerticalAlignmentProperty, VerticalAlignment.Stretch));
        commonStyle.Add(new Setter(ScrollBarRepeatButton.HorizontalAlignmentProperty, HorizontalAlignment.Stretch));
        commonStyle.Add(new Setter(ScrollBarRepeatButton.OpacityProperty, 0d));
        Add(commonStyle);
    }
}