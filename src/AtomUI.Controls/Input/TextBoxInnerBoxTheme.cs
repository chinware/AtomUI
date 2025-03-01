using AtomUI.IconPkg;
using AtomUI.IconPkg.AntDesign;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class TextBoxInnerBoxTheme : AddOnDecoratedInnerBoxTheme
{
    public const string RevealButtonPart = "PART_RevealButton";
    
    public TextBoxInnerBoxTheme() : base(typeof(TextBoxInnerBox))
    {
    }

    protected override void BuildRightAddOnItems(AddOnDecoratedInnerBox addOnDecoratedInnerBox, StackPanel layout, INameScope scope)
    {
        base.BuildRightAddOnItems(addOnDecoratedInnerBox, layout, scope);
        BuildRevealButtonButton(layout, scope);
    }
    
    protected virtual void BuildRevealButtonButton(StackPanel addOnLayout, INameScope scope)
    {
        var eyeOutlinedIcon          = AntDesignIconPackage.EyeOutlined();
        var eyeInvisibleOutlinedIcon = AntDesignIconPackage.EyeInvisibleOutlined();
        
         TokenResourceBinder.CreateTokenBinding(eyeOutlinedIcon, Icon.NormalFilledBrushProperty,
            SharedTokenKey.ColorTextTertiary);
        TokenResourceBinder.CreateTokenBinding(eyeOutlinedIcon, Icon.ActiveFilledBrushProperty,
            SharedTokenKey.ColorText);
        
        TokenResourceBinder.CreateTokenBinding(eyeInvisibleOutlinedIcon, Icon.NormalFilledBrushProperty,
            SharedTokenKey.ColorTextTertiary);
        TokenResourceBinder.CreateTokenBinding(eyeInvisibleOutlinedIcon, Icon.ActiveFilledBrushProperty,
            SharedTokenKey.ColorText);
        
        var revealButtonIconButton = new ToggleIconButton
        {
            Name          = RevealButtonPart,
            CheckedIcon   = eyeOutlinedIcon,
            UnCheckedIcon = eyeInvisibleOutlinedIcon
        };

        CreateTemplateParentBinding(revealButtonIconButton, ToggleIconButton.IsCheckedProperty, TextBoxInnerBox.IsRevealButtonCheckedProperty, BindingMode.TwoWay);
        TokenResourceBinder.CreateTokenBinding(revealButtonIconButton, ToggleIconButton.IconHeightProperty,
            SharedTokenKey.IconSize);
        TokenResourceBinder.CreateTokenBinding(revealButtonIconButton, ToggleIconButton.IconWidthProperty,
            SharedTokenKey.IconSize);

        revealButtonIconButton.RegisterInNameScope(scope);
        CreateTemplateParentBinding(revealButtonIconButton, Visual.IsVisibleProperty,
            TextBoxInnerBox.IsRevealButtonVisibleProperty);
        addOnLayout.Children.Add(revealButtonIconButton);
    }

    protected override void BuildDisabledStyle()
    {
        var embedModeStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(TextBoxInnerBox.EmbedModeProperty, false));
        var disabledStyle  = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
        var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
        decoratorStyle.Add(Border.BackgroundProperty, SharedTokenKey.ColorBgContainerDisabled);
        disabledStyle.Add(decoratorStyle);
        embedModeStyle.Add(disabledStyle);
        Add(embedModeStyle);
    }
}