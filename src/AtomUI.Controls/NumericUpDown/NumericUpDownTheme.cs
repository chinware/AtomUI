using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class NumericUpDownTheme : BaseControlTheme
{
    public const string SpinnerPart = "PART_Spinner";
    public const string TextBoxPart = "PART_TextBox";

    public NumericUpDownTheme() : base(typeof(NumericUpDown))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<NumericUpDown>((numericUpDown, scope) =>
        {
            var buttonSpinner = BuildButtonSpinner(numericUpDown, scope);
            var textBlock     = BuildTextBox(numericUpDown, scope);
            buttonSpinner.Content = textBlock;

            return buttonSpinner;
        });
    }

    private ButtonSpinner BuildButtonSpinner(NumericUpDown numericUpDown, INameScope scope)
    {
        var buttonSpinner = new ButtonSpinner
        {
            Name = SpinnerPart
        };
        CreateTemplateParentBinding(buttonSpinner, InputElement.IsEnabledProperty, InputElement.IsEnabledProperty);
        CreateTemplateParentBinding(buttonSpinner, ButtonSpinner.SizeTypeProperty, NumericUpDown.SizeTypeProperty);
        CreateTemplateParentBinding(buttonSpinner, ButtonSpinner.StyleVariantProperty,
            NumericUpDown.StyleVariantProperty);
        CreateTemplateParentBinding(buttonSpinner, ButtonSpinner.StatusProperty, NumericUpDown.StatusProperty);
        CreateTemplateParentBinding(buttonSpinner, ButtonSpinner.LeftAddOnProperty, NumericUpDown.LeftAddOnProperty);
        CreateTemplateParentBinding(buttonSpinner, ButtonSpinner.RightAddOnProperty, NumericUpDown.RightAddOnProperty);
        CreateTemplateParentBinding(buttonSpinner, ButtonSpinner.InnerLeftContentProperty,
            Avalonia.Controls.NumericUpDown.InnerLeftContentProperty);
        CreateTemplateParentBinding(buttonSpinner, ButtonSpinner.InnerRightContentProperty,
            Avalonia.Controls.NumericUpDown.InnerRightContentProperty);
        buttonSpinner.RegisterInNameScope(scope);
        return buttonSpinner;
    }

    private TextBox BuildTextBox(NumericUpDown numericUpDown, INameScope scope)
    {
        var textBox = new TextBox
        {
            Name                     = TextBoxPart,
            VerticalContentAlignment = VerticalAlignment.Center,
            VerticalAlignment        = VerticalAlignment.Stretch,
            HorizontalAlignment      = HorizontalAlignment.Stretch,
            BorderThickness          = new Thickness(0),
            TextWrapping             = TextWrapping.NoWrap,
            AcceptsReturn            = false,
            EmbedMode                = true
        };

        BindUtils.RelayBind(this, DataValidationErrors.ErrorsProperty, textBox, DataValidationErrors.ErrorsProperty);
        CreateTemplateParentBinding(textBox, TextBox.SizeTypeProperty, NumericUpDown.SizeTypeProperty);
        CreateTemplateParentBinding(textBox, Avalonia.Controls.TextBox.IsReadOnlyProperty,
            Avalonia.Controls.NumericUpDown.IsReadOnlyProperty);
        CreateTemplateParentBinding(textBox, Avalonia.Controls.TextBox.TextProperty,
            Avalonia.Controls.NumericUpDown.TextProperty);
        CreateTemplateParentBinding(textBox, Avalonia.Controls.TextBox.WatermarkProperty,
            Avalonia.Controls.NumericUpDown.WatermarkProperty);
        CreateTemplateParentBinding(textBox, TextBox.IsEnableClearButtonProperty,
            NumericUpDown.IsEnableClearButtonProperty);
        textBox.RegisterInNameScope(scope);
        return textBox;
    }
}