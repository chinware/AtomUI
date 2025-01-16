using AtomUI.IconPkg;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class ToggleIconButtonTheme : BaseControlTheme
{
    public const string RootLayoutPart = "PART_RootLayout";
    public const string CheckedIconPresenterPart = "PART_CheckedIconPresenter";
    public const string UnCheckedIconPresenterPart = "PART_UnCheckedIconPresenter";

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
            var rootLayout = new Panel()
            {
                Name = RootLayoutPart
            };
            var checkedIconPresenter = new ContentPresenter
            {
                Name = CheckedIconPresenterPart
            };

            var uncheckedIconPresenter = new ContentPresenter()
            {
                Name = UnCheckedIconPresenterPart
            };

            rootLayout.Children.Add(checkedIconPresenter);
            rootLayout.Children.Add(uncheckedIconPresenter);

            CreateTemplateParentBinding(checkedIconPresenter, ContentPresenter.ContentProperty,
                ToggleIconButton.CheckedIconProperty);
            CreateTemplateParentBinding(checkedIconPresenter, ContentPresenter.IsVisibleProperty,
                ToggleIconButton.IsCheckedProperty, BindingMode.Default,
                new FuncValueConverter<bool?, bool>(input => input.HasValue && input.Value));

            CreateTemplateParentBinding(uncheckedIconPresenter, ContentPresenter.ContentProperty,
                ToggleIconButton.UnCheckedIconProperty);
            CreateTemplateParentBinding(uncheckedIconPresenter, ContentPresenter.IsVisibleProperty,
                ToggleIconButton.IsCheckedProperty, BindingMode.Default,
                new FuncValueConverter<bool?, bool>(input => !input.HasValue || !input.Value));

            return rootLayout;
        });
    }

    protected override void BuildStyles()
    {
        {
            var iconStyle = new Style(selector => selector.Nesting().Template().OfType<Icon>());
            iconStyle.Add(Icon.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            iconStyle.Add(Icon.VerticalAlignmentProperty, VerticalAlignment.Center);
            Add(iconStyle);
        }
        
        var enabledStyle =
            new Style(selector => selector.Nesting().PropertyEquals(InputElement.IsEnabledProperty, true));
        enabledStyle.Add(ToggleIconButton.CursorProperty,
            new SetterValueFactory<Cursor>(() => new Cursor(StandardCursorType.Hand)));
        {
            {
                var iconStyle = new Style(selector => selector.Nesting().Template().OfType<Icon>());
                iconStyle.Add(Icon.IconModeProperty, IconMode.Normal);
                enabledStyle.Add(iconStyle);
            }

            var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
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