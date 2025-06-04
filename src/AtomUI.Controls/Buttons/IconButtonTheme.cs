using AtomUI.IconPkg;
using AtomUI.Media;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Animation;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Styling;
using AnimationUtils = AtomUI.Controls.Utils.AnimationUtils;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class IconButtonTheme : BaseControlTheme
{
    public const string IconContentPart = "PART_IconContent";

    public IconButtonTheme()
        : base(typeof(IconButton))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<IconButton>((button, scope) =>
        {
            var iconContent = new ContentPresenter
            {
                Name = IconContentPart,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
            };
            CreateTemplateParentBinding(iconContent, ContentPresenter.BorderThicknessProperty,
                TemplatedControl.BorderThicknessProperty);
            CreateTemplateParentBinding(iconContent, ContentPresenter.BorderBrushProperty,
                TemplatedControl.BorderBrushProperty);
            CreateTemplateParentBinding(iconContent, ContentPresenter.CornerRadiusProperty,
                TemplatedControl.CornerRadiusProperty);
            CreateTemplateParentBinding(iconContent, ContentPresenter.BackgroundProperty,
                TemplatedControl.BackgroundProperty);
            CreateTemplateParentBinding(iconContent, ContentPresenter.ContentProperty, IconButton.IconProperty);
            CreateTemplateParentBinding(iconContent, ContentPresenter.PaddingProperty,
                TemplatedControl.PaddingProperty);
            return iconContent;
        });
    }

    protected override void BuildStyles()
    {
        var commonStyle = new Style(selector => selector.Nesting());
        commonStyle.Add(IconButton.CursorProperty, new SetterValueFactory<Cursor>(() => new Cursor(StandardCursorType.Hand)));
     
        {
            var isMotionEnabledStyle = new Style(selector => selector.Nesting().PropertyEquals(IconButton.IsMotionEnabledProperty, true));
            
            var contentStyle = new Style(selector => selector.Nesting().Template().Name(IconContentPart));
            contentStyle.Add(ContentPresenter.BackgroundProperty, SharedTokenKey.ColorTransparent);
            contentStyle.Add(ContentPresenter.TransitionsProperty, new SetterValueFactory<Transitions>(() => new Transitions()
            {
                AnimationUtils.CreateTransition<SolidColorBrushTransition>(ContentPresenter.BackgroundProperty)
            }));
            isMotionEnabledStyle.Add(contentStyle);
            commonStyle.Add(isMotionEnabledStyle);
        }

        var enableHoverStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(IconButton.IsEnableHoverEffectProperty, true));
        {
            var hoverBgStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
            {
                var contentStyle = new Style(selector => selector.Nesting().Template().Name(IconContentPart));
                contentStyle.Add(ContentPresenter.BackgroundProperty, SharedTokenKey.ColorBgTextHover);
                hoverBgStyle.Add(contentStyle);
            }
            enableHoverStyle.Add(hoverBgStyle);
            
            var pressedBgStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Pressed));
            {
                var contentStyle = new Style(selector => selector.Nesting().Template().Name(IconContentPart));
                contentStyle.Add(ContentPresenter.BackgroundProperty, SharedTokenKey.ColorBgTextActive);
                pressedBgStyle.Add(contentStyle);
            }
            enableHoverStyle.Add(pressedBgStyle);
        }
        
        commonStyle.Add(enableHoverStyle);
        
        var iconStyle = new Style(selector => selector.Nesting().Template().Name(IconContentPart).Descendant().OfType<Icon>());
        
        iconStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center);
        iconStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center);
        
        commonStyle.Add(iconStyle);
        Add(commonStyle);

        BuildIconModeStyle();
    }

    private void BuildIconModeStyle()
    {
        var isEnabledStyle = new Style(selector => selector.Nesting().PropertyEquals(IconButton.IsEnabledProperty, true));
        isEnabledStyle.Add(IconButton.IconModeProperty, IconMode.Normal);
        var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
        hoverStyle.Add(IconButton.IconModeProperty, IconMode.Active);
        isEnabledStyle.Add(hoverStyle);
        
        var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Pressed));
        pressedStyle.Add(IconButton.IconModeProperty, IconMode.Selected);
        isEnabledStyle.Add(pressedStyle);
        
        Add(isEnabledStyle);
        
        var disabledStyle = new Style(selector => selector.Nesting().PropertyEquals(IconButton.IsEnabledProperty, false));
        disabledStyle.Add(IconButton.IconModeProperty, IconMode.Disabled);
        Add(disabledStyle);
    }
}