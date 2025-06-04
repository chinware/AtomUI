using AtomUI.IconPkg.AntDesign;
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

    protected override void BuildRightAddOnItems(AddOnDecoratedInnerBox addOnDecoratedInnerBox, StackPanel layout,
                                                 INameScope scope)
    {
        base.BuildRightAddOnItems(addOnDecoratedInnerBox, layout, scope);
        BuildRevealButtonButton((TextBoxInnerBox)addOnDecoratedInnerBox, layout, scope);
    }

    protected virtual void BuildRevealButtonButton(TextBoxInnerBox textBoxInnerBox, StackPanel addOnLayout,
                                                   INameScope scope)
    {
        var revealButtonIconButton = new ToggleIconButton
        {
            Name          = RevealButtonPart,
            CheckedIcon   = AntDesignIconPackage.EyeOutlined(),
            UnCheckedIcon = AntDesignIconPackage.EyeInvisibleOutlined()
        };

        CreateTemplateParentBinding(revealButtonIconButton, ToggleIconButton.IsCheckedProperty,
            TextBoxInnerBox.IsRevealButtonCheckedProperty, BindingMode.TwoWay);
        
        revealButtonIconButton.RegisterInNameScope(scope);
        CreateTemplateParentBinding(revealButtonIconButton, Visual.IsVisibleProperty,
            TextBoxInnerBox.IsRevealButtonVisibleProperty);
        addOnLayout.Children.Add(revealButtonIconButton);
    }

    protected override void BuildStyles()
    {
        base.BuildStyles();
        BuildRevealButtonButtonStyle();
    }

    private void BuildRevealButtonButtonStyle()
    {
        var revealButtonStyle = new Style(selector => selector.Nesting().Template().Name(RevealButtonPart));
        revealButtonStyle.Add(ToggleIconButton.IconWidthProperty, SharedTokenKey.IconSize);
        revealButtonStyle.Add(ToggleIconButton.IconHeightProperty, SharedTokenKey.IconSize);
        revealButtonStyle.Add(ToggleIconButton.NormalIconBrushProperty, SharedTokenKey.ColorTextTertiary);
        revealButtonStyle.Add(ToggleIconButton.ActiveIconBrushProperty, SharedTokenKey.ColorText);
        revealButtonStyle.Add(ToggleIconButton.SelectedIconBrushProperty, SharedTokenKey.ColorText);
        Add(revealButtonStyle);
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