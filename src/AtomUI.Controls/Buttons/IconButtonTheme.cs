using AtomUI.Controls.Utils;
using AtomUI.IconPkg;
using AtomUI.Media;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Styling;

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
            BuildInstanceStyles(button);
            var iconContent = new ContentPresenter
            {
                Name = IconContentPart,
            };
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
            var contentStyle = new Style(selector => selector.Nesting().Template().Name(IconContentPart));
            contentStyle.Add(ContentPresenter.BackgroundProperty, SharedTokenKey.ColorBgContainer);
            commonStyle.Add(contentStyle);
        }
        {
            var isMotionEnabledStyle = new Style(selector => selector.Nesting().PropertyEquals(IconButton.IsMotionEnabledProperty, true));
            var contentStyle = new Style(selector => selector.Nesting().Template().Name(IconContentPart));
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
        Add(commonStyle);
    }

    protected override void BuildInstanceStyles(Control control)
    {
        var iconStyle = new Style(selector => selector.Name(IconContentPart).Child().OfType<Icon>());
        iconStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center);
        iconStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center);
        control.Styles.Add(iconStyle);
    }
}