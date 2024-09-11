using AtomUI.Data;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;

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
            VerticalContentAlignment = VerticalAlignment.Stretch
        };
        pickerInnerBox.RegisterInNameScope(scope);
        CreateTemplateParentBinding(pickerInnerBox, AddOnDecoratedInnerBox.LeftAddOnContentProperty,
            RangeTimePicker.InnerLeftContentProperty);
        CreateTemplateParentBinding(pickerInnerBox, AddOnDecoratedInnerBox.StyleVariantProperty,
            RangeTimePicker.StyleVariantProperty);
        CreateTemplateParentBinding(pickerInnerBox, AddOnDecoratedInnerBox.StatusProperty,
            RangeTimePicker.StatusProperty);
        CreateTemplateParentBinding(pickerInnerBox, AddOnDecoratedInnerBox.SizeTypeProperty,
            RangeTimePicker.SizeTypeProperty);

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

    protected virtual PathIcon? BuildInfoIcon(InfoPickerInput infoPickerInput, INameScope scope)
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
        CreateTemplateParentBinding(decoratedBox, AddOnDecoratedBox.StyleVariantProperty,
            RangeTimePicker.StyleVariantProperty);
        CreateTemplateParentBinding(decoratedBox, AddOnDecoratedBox.SizeTypeProperty, RangeTimePicker.SizeTypeProperty);
        CreateTemplateParentBinding(decoratedBox, AddOnDecoratedBox.StatusProperty, RangeTimePicker.StatusProperty);
        CreateTemplateParentBinding(decoratedBox, AddOnDecoratedBox.LeftAddOnProperty,
            RangeTimePicker.LeftAddOnProperty);
        CreateTemplateParentBinding(decoratedBox, AddOnDecoratedBox.RightAddOnProperty,
            RangeTimePicker.RightAddOnProperty);
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
        CreateTemplateParentBinding(pickerTextBox, TextBox.SizeTypeProperty, InfoPickerInput.SizeTypeProperty);
        CreateTemplateParentBinding(pickerTextBox, TextBox.IsReadOnlyProperty,
            InfoPickerInput.IsReadOnlyProperty);

        return pickerTextBox;
    }
    
    protected virtual void BuildExtraElement(Panel containerLayout, InfoPickerInput infoPickerInput, INameScope scope)
    {
    }
    
}