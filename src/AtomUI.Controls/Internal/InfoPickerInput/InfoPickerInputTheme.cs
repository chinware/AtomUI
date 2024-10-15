﻿using AtomUI.Data;
using AtomUI.IconPkg;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls.Internal;

internal class InfoPickerInputTheme : BaseControlTheme
{
    public const string DecoratedBoxPart = "PART_DecoratedBox";
    public const string PickerInnerPart = "PART_PickerInner";
    public const string InfoInputBoxPart = "PART_InfoInputBox";
    public const string ClearUpButtonPart = "PART_ClearUpButton";
    
    public InfoPickerInputTheme() : this(typeof(InfoPickerInput))
    {
    }
    
    protected InfoPickerInputTheme(Type targetType) : base(targetType)
    {
    }
    
    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<InfoPickerInput>((infoPickerInput, scope) =>
        {
            var indicatorLayout = new Panel();
            var decoratedBox    = BuildPickerDecoratedBox(infoPickerInput, scope);
            var innerBox        = BuildPickerInnerBox(infoPickerInput, scope);
            decoratedBox.Content = innerBox;
            indicatorLayout.Children.Add(decoratedBox);
            BuildExtraElement(indicatorLayout, infoPickerInput, scope);
            return indicatorLayout;
        });
    }
    
     private AddOnDecoratedInnerBox BuildPickerInnerBox(InfoPickerInput infoPickerInput, INameScope scope)
    {
        var pickerInnerBox = new AddOnDecoratedInnerBox
        {
            Name                     = PickerInnerPart,
            HorizontalAlignment      = HorizontalAlignment.Stretch,
            VerticalContentAlignment = VerticalAlignment.Stretch,
        };
        pickerInnerBox.RegisterInNameScope(scope);
        CreateTemplateParentBinding(pickerInnerBox, AddOnDecoratedInnerBox.LeftAddOnContentProperty,
            InfoPickerInput.InnerLeftContentProperty);
        CreateTemplateParentBinding(pickerInnerBox, AddOnDecoratedInnerBox.StyleVariantProperty,
            InfoPickerInput.StyleVariantProperty);
        CreateTemplateParentBinding(pickerInnerBox, AddOnDecoratedInnerBox.StatusProperty,
            InfoPickerInput.StatusProperty);
        CreateTemplateParentBinding(pickerInnerBox, AddOnDecoratedInnerBox.SizeTypeProperty,
            InfoPickerInput.SizeTypeProperty);

        var clearUpButton = new PickerClearUpButton()
        {
            Name = ClearUpButtonPart,
            Icon = BuildInfoIcon(infoPickerInput, scope)
        };
        clearUpButton.RegisterInNameScope(scope);
        pickerInnerBox.RightAddOnContent = clearUpButton;
        
        pickerInnerBox.Content = BuildPickerContent(infoPickerInput, scope);

        return pickerInnerBox;
    }

    protected virtual Icon? BuildInfoIcon(InfoPickerInput infoPickerInput, INameScope scope)
    {
        return default;
    }

    protected virtual Control BuildPickerContent(InfoPickerInput infoPickerInput, INameScope scope)
    {
        var pickerTextBox = BuildPickerTextBox(InfoInputBoxPart);
        CreateTemplateParentBinding(pickerTextBox, TextBox.TextProperty, InfoPickerInput.TextProperty);
        CreateTemplateParentBinding(pickerTextBox, TextBox.WatermarkProperty, InfoPickerInput.WatermarkProperty);
        pickerTextBox.RegisterInNameScope(scope);
        return pickerTextBox;
    }
    
    protected virtual AddOnDecoratedBox BuildPickerDecoratedBox(InfoPickerInput infoPickerInput, INameScope scope)
    {
        var decoratedBox = new AddOnDecoratedBox
        {
            Name      = DecoratedBoxPart,
            Focusable = true
        };
        CreateTemplateParentBinding(decoratedBox, AddOnDecoratedBox.MinWidthProperty,
            InfoPickerInput.MinWidthProperty);
        CreateTemplateParentBinding(decoratedBox, AddOnDecoratedBox.StyleVariantProperty,
            InfoPickerInput.StyleVariantProperty);
        CreateTemplateParentBinding(decoratedBox, AddOnDecoratedBox.SizeTypeProperty, InfoPickerInput.SizeTypeProperty);
        CreateTemplateParentBinding(decoratedBox, AddOnDecoratedBox.StatusProperty, InfoPickerInput.StatusProperty);
        CreateTemplateParentBinding(decoratedBox, AddOnDecoratedBox.LeftAddOnProperty,
            InfoPickerInput.LeftAddOnProperty);
        CreateTemplateParentBinding(decoratedBox, AddOnDecoratedBox.RightAddOnProperty,
            InfoPickerInput.RightAddOnProperty);
        decoratedBox.RegisterInNameScope(scope);
        return decoratedBox;
    }

    protected TextBox BuildPickerTextBox(string name)
    {
        var pickerTextBox = new TextBox
        {
            Name                     = name,
            VerticalContentAlignment = VerticalAlignment.Center,
            VerticalAlignment        = VerticalAlignment.Stretch,
            HorizontalAlignment      = HorizontalAlignment.Stretch,
            BorderThickness          = new Thickness(0),
            TextWrapping             = TextWrapping.NoWrap,
            AcceptsReturn            = false,
            EmbedMode                = true,
            IsEnableClearButton      = false,
            IsEnableRevealButton     = false
        };

        BindUtils.RelayBind(this, DataValidationErrors.ErrorsProperty, pickerTextBox,
            DataValidationErrors.ErrorsProperty);
        CreateTemplateParentBinding(pickerTextBox, TextBox.ForegroundProperty, InfoPickerInput.InputTextBrushProperty);
        CreateTemplateParentBinding(pickerTextBox, TextBox.SizeTypeProperty, InfoPickerInput.SizeTypeProperty);
        CreateTemplateParentBinding(pickerTextBox, TextBox.IsReadOnlyProperty,
            InfoPickerInput.IsReadOnlyProperty);

        return pickerTextBox;
    }
    
    protected virtual void BuildExtraElement(Panel containerLayout, InfoPickerInput infoPickerInput, INameScope scope)
    {
    }
    
    protected override void BuildStyles()
    {
        base.BuildStyles();
        var enableStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(InfoPickerInput.IsEnabledProperty, true));
        enableStyle.Add(InfoPickerInput.InputTextBrushProperty, GlobalTokenResourceKey.ColorText);
        
        var choosingStyle = new Style(selector => selector.Nesting().Class(InfoPickerInput.ChoosingPC));
        choosingStyle.Add(InfoPickerInput.InputTextBrushProperty, GlobalTokenResourceKey.ColorTextTertiary);
        enableStyle.Add(choosingStyle);
        
        Add(enableStyle);
        
        var disabledStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(InfoPickerInput.IsEnabledProperty, false));
        disabledStyle.Add(InfoPickerInput.InputTextBrushProperty, GlobalTokenResourceKey.ColorTextDisabled);
        Add(disabledStyle);
    }
}