using AtomUI.IconPkg;
using AtomUI.IconPkg.AntDesign;
using AtomUI.Theme;
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
            PickerClearUpButton.IsInClearModeProperty,
            BindingMode.Default,
            BoolConverters.Not);
        layout.Children.Add(iconContent);
    }

    private void BuildClearButton(Panel layout, INameScope scope)
    {
        var clearButton = new IconButton
        {
            Name = ClearButtonPart,
            Icon = AntDesignIconPackage.CloseCircleFilled()
        };
        clearButton.RegisterInNameScope(scope);
        CreateTemplateParentBinding(clearButton, Visual.IsVisibleProperty,
            PickerClearUpButton.IsInClearModeProperty);
        layout.Children.Add(clearButton);
    }
    
    protected override void BuildStyles()
    {
        var clearUpButtonStyle = new Style(selector => selector.Nesting().Template().Name(ClearButtonPart));
        clearUpButtonStyle.Add(IconButton.IconHeightProperty, SharedTokenKey.IconSize);
        clearUpButtonStyle.Add(IconButton.IconWidthProperty, SharedTokenKey.IconSize);
        clearUpButtonStyle.Add(IconButton.NormalIconColorProperty, SharedTokenKey.ColorTextQuaternary);
        clearUpButtonStyle.Add(IconButton.ActiveIconColorProperty, SharedTokenKey.ColorTextQuaternary);
        clearUpButtonStyle.Add(IconButton.SelectedIconColorProperty, SharedTokenKey.ColorText);
        Add(clearUpButtonStyle);
        
        var iconStyle = new Style(selector => selector.Nesting().Template().Name(InfoIconContentPart).Descendant().OfType<Icon>());
        iconStyle.Add(Icon.WidthProperty, SharedTokenKey.IconSize);
        iconStyle.Add(Icon.HeightProperty, SharedTokenKey.IconSize);
        iconStyle.Add(Icon.NormalFilledBrushProperty, SharedTokenKey.ColorTextQuaternary);
        Add(iconStyle);
    }
}