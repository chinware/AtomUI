using AtomUI.Controls.Utils;
using AtomUI.Media;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Animation;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class HyperLinkTextBlockTheme : BaseControlTheme
{
    public const string TextPart = "PART_Text";
    
    public HyperLinkTextBlockTheme()
        : base(typeof(HyperLinkTextBlock))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<HyperLinkTextBlock>((hyperLinkTextBlock, scope) =>
        {
            var textBlock = new TextBlock
            {
                Name = TextPart
            };
            CreateTemplateParentBinding(textBlock, TextBlock.FontWeightProperty, HyperLinkTextBlock.FontWeightProperty);
            CreateTemplateParentBinding(textBlock, TextBlock.FontStyleProperty, HyperLinkTextBlock.FontStyleProperty);
            CreateTemplateParentBinding(textBlock, TextBlock.FontSizeProperty, HyperLinkTextBlock.FontSizeProperty);
            CreateTemplateParentBinding(textBlock, TextBlock.TextProperty, HyperLinkTextBlock.TextProperty);
            return textBlock;
        });
    }

    protected override void BuildStyles()
    {
        var commonStyle  = new Style(selector => selector.Nesting());
        
        {
            var isMotionEnabledStyle = new Style(selector => selector.Nesting().PropertyEquals(HyperLinkTextBlock.IsMotionEnabledProperty, true));
            isMotionEnabledStyle.Add(HyperLinkTextBlock.TransitionsProperty, new SetterValueFactory<Transitions>(() => new Transitions()
            {
                AnimationUtils.CreateTransition<SolidColorBrushTransition>(ContentPresenter.ForegroundProperty)
            }));
            commonStyle.Add(isMotionEnabledStyle);
        }
        
        commonStyle.Add(TemplatedControl.CursorProperty, new SetterValueFactory<Cursor>(() => new Cursor(StandardCursorType.Hand)));
        commonStyle.Add(TemplatedControl.BackgroundProperty, SharedTokenKey.ColorBgContainer);
        
        var enabledStyle = new Style(selector => selector.Nesting().PropertyEquals(HyperLinkTextBlock.IsEnabledProperty, true));
        // 正常状态
        enabledStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorLink);

        // 正常 hover
        {
            var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
            hoverStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorLinkHover);
            enabledStyle.Add(hoverStyle);
        }
        // 正常按下
        {
            var pressedStyle = new Style(selector =>
                selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
            pressedStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorLinkActive);
            enabledStyle.Add(pressedStyle);
        }

        commonStyle.Add(enabledStyle);
        Add(commonStyle);
        
        var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
        disabledStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorTextDisabled);
        Add(disabledStyle);
    }
}