using AtomUI.IconPkg;
using AtomUI.IconPkg.AntDesign;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Styling;

namespace AtomUI.Controls;

internal static class DropdownButtonThemeUtils
{
    public const string OpenIndicatorPart = "PART_OpenIndicator";
    
    public static Style BuildStyles()
    {
        var commonStyle = new Style(selector => selector.Nesting());

        commonStyle.Add(DropdownButton.MarginToAnchorProperty, SharedTokenKey.MarginXXS);
     
        {
            var openIndicatorStyle = new Style(selector => selector.Nesting().Template().Name(OpenIndicatorPart));
            openIndicatorStyle.Add(Icon.WidthProperty, SharedTokenKey.IconSizeSM);
            openIndicatorStyle.Add(Icon.HeightProperty, SharedTokenKey.IconSizeSM);
            openIndicatorStyle.Add(Icon.IconModeProperty, IconMode.Normal);
            commonStyle.Add(openIndicatorStyle);
        }
        
        var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
        {
            var openIndicatorStyle = new Style(selector => selector.Nesting().Template().Name(OpenIndicatorPart));
            openIndicatorStyle.Add(Icon.IconModeProperty, IconMode.Active);
            hoverStyle.Add(openIndicatorStyle);
        }
        commonStyle.Add(hoverStyle);
        var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Pressed));
        {
            var openIndicatorStyle = new Style(selector => selector.Nesting().Template().Name(OpenIndicatorPart));
            openIndicatorStyle.Add(Icon.IconModeProperty, IconMode.Selected);
            pressedStyle.Add(openIndicatorStyle);
        }
        commonStyle.Add(pressedStyle);
        var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
        {
            var openIndicatorStyle = new Style(selector => selector.Nesting().Template().Name(OpenIndicatorPart));
            openIndicatorStyle.Add(Icon.IconModeProperty, IconMode.Disabled);
            disabledStyle.Add(openIndicatorStyle);
        }
        commonStyle.Add(disabledStyle);
        
        return commonStyle;
    }
}

[ControlThemeProvider]
internal class DefaultDropdownButtonTheme : BaseDefaultButtonTheme
{
    public const string ID = "DefaultDropdownButton";
    
    public DefaultDropdownButtonTheme()
        : base(typeof(DropdownButton))
    {
    }
    
    public override string ThemeResourceKey()
    {
        return ID;
    }
    
    protected override void BuildExtraContainer(StackPanel containerLayout, INameScope scope)
    {
        var openIndicatorIcon = AntDesignIconPackage.DownOutlined();
        openIndicatorIcon.Name = DropdownButtonThemeUtils.OpenIndicatorPart;
        CreateTemplateParentBinding(openIndicatorIcon, Icon.NormalFilledBrushProperty, DropdownButton.IconNormalColorProperty);
        CreateTemplateParentBinding(openIndicatorIcon, Icon.ActiveFilledBrushProperty, DropdownButton.IconHoverColorProperty);
        CreateTemplateParentBinding(openIndicatorIcon, Icon.SelectedFilledBrushProperty, DropdownButton.IconPressedColorProperty);
        CreateTemplateParentBinding(openIndicatorIcon, Icon.DisabledFilledBrushProperty, DropdownButton.IconDisabledColorProperty);
        containerLayout.Children.Add(openIndicatorIcon);
        base.BuildExtraContainer(containerLayout, scope);
    }

    protected override void BuildStyles()
    {
        base.BuildStyles();
        Add(DropdownButtonThemeUtils.BuildStyles());
    }
}

[ControlThemeProvider]
internal class LinkDropdownButtonTheme : BaseLinkButtonTheme
{
    public const string ID = "LinkDropdownButton";
    
    public LinkDropdownButtonTheme()
        : base(typeof(DropdownButton))
    {
    }
    
    public override string ThemeResourceKey()
    {
        return ID;
    }
    
    protected override void BuildExtraContainer(StackPanel containerLayout, INameScope scope)
    {
        var openIndicatorIcon = AntDesignIconPackage.DownOutlined();
        openIndicatorIcon.Name = DropdownButtonThemeUtils.OpenIndicatorPart;
        CreateTemplateParentBinding(openIndicatorIcon, Icon.NormalFilledBrushProperty, DropdownButton.IconNormalColorProperty);
        CreateTemplateParentBinding(openIndicatorIcon, Icon.ActiveFilledBrushProperty, DropdownButton.IconHoverColorProperty);
        CreateTemplateParentBinding(openIndicatorIcon, Icon.SelectedFilledBrushProperty, DropdownButton.IconPressedColorProperty);
        CreateTemplateParentBinding(openIndicatorIcon, Icon.DisabledFilledBrushProperty, DropdownButton.IconDisabledColorProperty);
        containerLayout.Children.Add(openIndicatorIcon);
        base.BuildExtraContainer(containerLayout, scope);
    }

    protected override void BuildStyles()
    {
        base.BuildStyles();
        Add(DropdownButtonThemeUtils.BuildStyles());
    }
}

[ControlThemeProvider]
internal class PrimaryDropdownButtonTheme : BasePrimaryButtonTheme
{
    public const string ID = "PrimaryDropdownButton";
    
    public PrimaryDropdownButtonTheme()
        : base(typeof(DropdownButton))
    {
    }
    
    public override string ThemeResourceKey()
    {
        return ID;
    }
    
    protected override void BuildExtraContainer(StackPanel containerLayout, INameScope scope)
    {
        var openIndicatorIcon = AntDesignIconPackage.DownOutlined();
        openIndicatorIcon.Name = DropdownButtonThemeUtils.OpenIndicatorPart;
        CreateTemplateParentBinding(openIndicatorIcon, Icon.NormalFilledBrushProperty, DropdownButton.IconNormalColorProperty);
        CreateTemplateParentBinding(openIndicatorIcon, Icon.ActiveFilledBrushProperty, DropdownButton.IconHoverColorProperty);
        CreateTemplateParentBinding(openIndicatorIcon, Icon.SelectedFilledBrushProperty, DropdownButton.IconPressedColorProperty);
        CreateTemplateParentBinding(openIndicatorIcon, Icon.DisabledFilledBrushProperty, DropdownButton.IconDisabledColorProperty);
        containerLayout.Children.Add(openIndicatorIcon);
        base.BuildExtraContainer(containerLayout, scope);
    }

    protected override void BuildStyles()
    {
        base.BuildStyles();
        Add(DropdownButtonThemeUtils.BuildStyles());
    }
}

[ControlThemeProvider]
internal class TextDropdownButtonTheme : BaseTextButtonTheme
{
    public const string ID = "TextDropdownButton";
    
    public TextDropdownButtonTheme()
        : base(typeof(DropdownButton))
    {
    }
    
    public override string ThemeResourceKey()
    {
        return ID;
    }
    
    protected override void BuildExtraContainer(StackPanel containerLayout, INameScope scope)
    {
        var openIndicatorIcon = AntDesignIconPackage.DownOutlined();
        openIndicatorIcon.Name = DropdownButtonThemeUtils.OpenIndicatorPart;
        CreateTemplateParentBinding(openIndicatorIcon, Icon.NormalFilledBrushProperty, DropdownButton.IconNormalColorProperty);
        CreateTemplateParentBinding(openIndicatorIcon, Icon.ActiveFilledBrushProperty, DropdownButton.IconHoverColorProperty);
        CreateTemplateParentBinding(openIndicatorIcon, Icon.SelectedFilledBrushProperty, DropdownButton.IconPressedColorProperty);
        CreateTemplateParentBinding(openIndicatorIcon, Icon.DisabledFilledBrushProperty, DropdownButton.IconDisabledColorProperty);
        containerLayout.Children.Add(openIndicatorIcon);
        base.BuildExtraContainer(containerLayout, scope);
    }

    protected override void BuildStyles()
    {
        base.BuildStyles();
        Add(DropdownButtonThemeUtils.BuildStyles());
    }
}
