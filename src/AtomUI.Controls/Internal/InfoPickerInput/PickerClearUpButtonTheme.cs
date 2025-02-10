using AtomUI.IconPkg;
using AtomUI.IconPkg.AntDesign;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class PickerClearUpButtonTheme : BaseControlTheme
{
    public const string ClearButtonPart = "PART_ClearButton";
    public const string InfoIconContentPart = "PART_InfoIconContent";

    public PickerClearUpButtonTheme() : base(typeof(PickerClearUpButton))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<PickerClearUpButton>((pickerClearUpButton, scope) =>
        {
            BuildInstanceStyles(pickerClearUpButton);
            var container = new Panel();
            BuildClearButton(container, scope);
            BuildClockIconContent(container, scope);
            return container;
        });
    }

    private void BuildClockIconContent(Panel layout, INameScope scope)
    {
        var iconContent = new ContentPresenter()
        {
            Name = InfoIconContentPart
        };
        CreateTemplateParentBinding(iconContent, ContentPresenter.ContentProperty, PickerClearUpButton.IconProperty);
        CreateTemplateParentBinding(iconContent, Visual.IsVisibleProperty,
            PickerClearUpButton.IsInClearModeProperty, BindingMode.Default,
            BoolConverters.Not);
        layout.Children.Add(iconContent);
    }

    private void BuildClearButton(Panel layout, INameScope scope)
    {
        var closeIcon = AntDesignIconPackage.CloseCircleFilled();
        var clearButton = new IconButton
        {
            Name = ClearButtonPart,
            Icon = closeIcon
        };

        TokenResourceBinder.CreateTokenBinding(clearButton, IconButton.IconHeightProperty,
            SharedTokenKey.IconSize);
        TokenResourceBinder.CreateTokenBinding(clearButton, IconButton.IconWidthProperty,
            SharedTokenKey.IconSize);
        TokenResourceBinder.CreateTokenBinding(closeIcon, Icon.NormalFilledBrushProperty,
            SharedTokenKey.ColorTextQuaternary);
        TokenResourceBinder.CreateTokenBinding(closeIcon, Icon.ActiveFilledBrushProperty,
            SharedTokenKey.ColorTextTertiary);
        TokenResourceBinder.CreateTokenBinding(closeIcon, Icon.SelectedFilledBrushProperty,
            SharedTokenKey.ColorText);

        clearButton.RegisterInNameScope(scope);
        CreateTemplateParentBinding(clearButton, Visual.IsVisibleProperty,
            PickerClearUpButton.IsInClearModeProperty);
        layout.Children.Add(clearButton);
    }

    protected override void BuildInstanceStyles(Control control)
    {
        var iconStyle = new Style(selector => selector.Name(InfoIconContentPart).Child().OfType<Icon>());
        iconStyle.Add(Icon.WidthProperty, SharedTokenKey.IconSize);
        iconStyle.Add(Icon.HeightProperty, SharedTokenKey.IconSize);
        iconStyle.Add(Icon.NormalFilledBrushProperty, SharedTokenKey.ColorTextQuaternary);
        control.Styles.Add(iconStyle);
    }
}