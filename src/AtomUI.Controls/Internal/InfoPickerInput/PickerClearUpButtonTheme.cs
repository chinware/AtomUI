using AtomUI.Theme;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
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
        var closeIcon = new PathIcon
        {
            Kind = "CloseCircleFilled"
        };
        var clearButton = new IconButton
        {
            Name = ClearButtonPart,
            Icon = closeIcon
        };

        TokenResourceBinder.CreateGlobalTokenBinding(clearButton, IconButton.IconHeightProperty,
            GlobalTokenResourceKey.IconSize);
        TokenResourceBinder.CreateGlobalTokenBinding(clearButton, IconButton.IconWidthProperty,
            GlobalTokenResourceKey.IconSize);
        TokenResourceBinder.CreateGlobalTokenBinding(closeIcon, PathIcon.NormalFilledBrushProperty,
            GlobalTokenResourceKey.ColorTextQuaternary);
        TokenResourceBinder.CreateGlobalTokenBinding(closeIcon, PathIcon.ActiveFilledBrushProperty,
            GlobalTokenResourceKey.ColorTextTertiary);
        TokenResourceBinder.CreateGlobalTokenBinding(closeIcon, PathIcon.SelectedFilledBrushProperty,
            GlobalTokenResourceKey.ColorText);

        clearButton.RegisterInNameScope(scope);
        CreateTemplateParentBinding(clearButton, Visual.IsVisibleProperty,
            PickerClearUpButton.IsInClearModeProperty);
        layout.Children.Add(clearButton);
    }
    
    protected override void BuildInstanceStyles(Control control)
    {
        var iconStyle = new Style(selector => selector.Name(InfoIconContentPart).Child().OfType<PathIcon>());
        iconStyle.Add(PathIcon.WidthProperty, GlobalTokenResourceKey.IconSize);
        iconStyle.Add(PathIcon.HeightProperty, GlobalTokenResourceKey.IconSize);
        iconStyle.Add(PathIcon.NormalFilledBrushProperty, GlobalTokenResourceKey.ColorTextQuaternary);
        control.Styles.Add(iconStyle);
    }
}