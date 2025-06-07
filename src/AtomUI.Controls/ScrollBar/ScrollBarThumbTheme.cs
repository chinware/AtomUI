using AtomUI.Animations;
using AtomUI.Controls.Utils;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class ScrollBarThumbTheme : BaseControlTheme
{
    internal const string FramePart = "PART_Frame";
    
    public ScrollBarThumbTheme()
        : base(typeof(ScrollBarThumb))
    {
    }
    
    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<ScrollBarThumb>((thumb, scope) =>
        {
            var frame = new Border
            {
                Name = FramePart,
                Transitions = new Transitions
                {
                    TransitionUtils.CreateTransition<SolidColorBrushTransition>(Border.BackgroundProperty)
                }
            };
            CreateTemplateParentBinding(frame, Border.BackgroundProperty, ScrollBarThumb.BackgroundProperty);
            CreateTemplateParentBinding(frame, Border.MarginProperty, ScrollBarThumb.MarginProperty);
            frame.UseLayoutRounding = false;
            return frame;
        });
    }
    
    protected override void BuildStyles()
    {
        var commonStyle = new Style(selector => selector.Nesting());
        commonStyle.Add(ScrollBarThumb.BackgroundProperty, ScrollBarTokenKey.ThumbBg);
        var frameStyle = new Style(selector => selector.Nesting().Template().Name(FramePart));
        frameStyle.Add(Border.CornerRadiusProperty, ScrollBarTokenKey.ThumbCornerRadius);
        commonStyle.Add(frameStyle);
        
        var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
        hoverStyle.Add(ScrollBarThumb.BackgroundProperty, ScrollBarTokenKey.ThumbHoverBg);
        commonStyle.Add(hoverStyle);
        Add(commonStyle);
    }
}