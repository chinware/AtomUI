using AtomUI.Controls.Utils;
using AtomUI.Media;
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
                    AnimationUtils.CreateTransition<SolidColorBrushTransition>(Border.BackgroundProperty)
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
        commonStyle.Add(ScrollBarThumb.BackgroundProperty, ScrollBarTokenResourceKey.ThumbBg);
        var frameStyle = new Style(selector => selector.Nesting().Template().Name(FramePart));
        frameStyle.Add(Border.CornerRadiusProperty, ScrollBarTokenResourceKey.ThumbCornerRadius);
        commonStyle.Add(frameStyle);
        
        var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
        hoverStyle.Add(ScrollBarThumb.BackgroundProperty, ScrollBarTokenResourceKey.ThumbHoverBg);
        commonStyle.Add(hoverStyle);
        Add(commonStyle);
    }
}