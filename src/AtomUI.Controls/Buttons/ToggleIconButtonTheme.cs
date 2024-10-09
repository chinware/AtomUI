using AtomUI.IconPkg;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class ToggleIconButtonTheme : BaseControlTheme
{
    public const string ContentPresenterPart = "PART_ContentPresenter";

    public ToggleIconButtonTheme()
        : base(typeof(ToggleIconButton))
    {
    }

    public ToggleIconButtonTheme(Type targetType) : base(targetType)
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<ToggleIconButton>((button, scope) =>
        {
            var contentPresenter = new ContentPresenter
            {
                Name = ContentPresenterPart
            };
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty,
                ContentControl.ContentProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentTemplateProperty,
                ContentControl.ContentTemplateProperty);
            return contentPresenter;
        });
    }

    protected override void BuildStyles()
    {
        var enabledStyle =
            new Style(selector => selector.Nesting().PropertyEquals(InputElement.IsEnabledProperty, true));
        {
            {
                var iconStyle = new Style(selector => selector.Nesting().Template().OfType<Icon>());
                iconStyle.Add(Icon.IconModeProperty, IconMode.Normal);
                enabledStyle.Add(iconStyle);
            }

            var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Selected));
            {
                var iconStyle = new Style(selector => selector.Nesting().Template().OfType<Icon>());
                iconStyle.Add(Icon.IconModeProperty, IconMode.Active);
                hoverStyle.Add(iconStyle);
            }
            enabledStyle.Add(hoverStyle);

            var checkedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Selected));
            {
                var iconStyle = new Style(selector => selector.Nesting().Template().OfType<Icon>());
                iconStyle.Add(Icon.IconModeProperty, IconMode.Selected);
                checkedStyle.Add(iconStyle);
            }
            enabledStyle.Add(checkedStyle);
        }
        Add(enabledStyle);

        var disabledStyle =
            new Style(selector => selector.Nesting().PropertyEquals(InputElement.IsEnabledProperty, false));
        {
            var iconStyle = new Style(selector => selector.Nesting().Template().OfType<Icon>());
            iconStyle.Add(Icon.IconModeProperty, IconMode.Disabled);
            disabledStyle.Add(iconStyle);
        }
        Add(disabledStyle);
    }
}