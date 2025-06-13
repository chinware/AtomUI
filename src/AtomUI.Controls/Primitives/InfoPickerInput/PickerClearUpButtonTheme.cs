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
    public const string InfoIconPresenterPart = "PART_InfoIconContent";

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
        var infoIconPresenter = new ContentPresenter()
        {
            Name = InfoIconPresenterPart
        };
        CreateTemplateParentBinding(infoIconPresenter, ContentPresenter.ContentProperty, PickerClearUpButton.IconProperty);
        CreateTemplateParentBinding(infoIconPresenter, Visual.IsVisibleProperty,
            PickerClearUpButton.IsInClearModeProperty,
            BindingMode.Default,
            BoolConverters.Not);
        layout.Children.Add(infoIconPresenter);
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
        clearUpButtonStyle.Add(IconButton.NormalIconBrushProperty, SharedTokenKey.ColorTextQuaternary);
        clearUpButtonStyle.Add(IconButton.ActiveIconBrushProperty, SharedTokenKey.ColorTextQuaternary);
        clearUpButtonStyle.Add(IconButton.SelectedIconBrushProperty, SharedTokenKey.ColorText);
        Add(clearUpButtonStyle);
        
        var infoIconStyle = new Style(selector => selector.Nesting().Template().Name(InfoIconPresenterPart).Descendant().OfType<Icon>());
        infoIconStyle.Add(Icon.WidthProperty, SharedTokenKey.IconSize);
        infoIconStyle.Add(Icon.HeightProperty, SharedTokenKey.IconSize);
        infoIconStyle.Add(Icon.NormalFilledBrushProperty, SharedTokenKey.ColorTextQuaternary);
        Add(infoIconStyle);
    }
}