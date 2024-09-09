using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class SplitButtonTheme : BaseControlTheme
{
    public const string MainLayoutPart = "PART_MainLayout";
    public const string PrimaryButtonPart = "PART_PrimaryButton";
    public const string SecondaryButtonPart = "PART_SecondaryButton";
    public const string SeparatorLinePart = "PART_SeparatorLine";

    public const int NormalZIndex = 1000;
    public const int ActivatedZIndex = 2000;

    public SplitButtonTheme()
        : base(typeof(SplitButton))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<SplitButton>((button, scope) =>
        {
            var mainLayout = new DockPanel
            {
                Name = MainLayoutPart
            };
            var primaryButton = new Button
            {
                Name = PrimaryButtonPart
            };
            CreateTemplateParentBinding(primaryButton, ContentControl.ContentProperty, ContentControl.ContentProperty);
            CreateTemplateParentBinding(primaryButton, ContentControl.ContentTemplateProperty,
                ContentControl.ContentTemplateProperty);
            CreateTemplateParentBinding(primaryButton, TemplatedControl.CornerRadiusProperty,
                SplitButton.PrimaryButtonCornerRadiusProperty);
            CreateTemplateParentBinding(primaryButton, Button.SizeTypeProperty, SplitButton.SizeTypeProperty);
            CreateTemplateParentBinding(primaryButton, Button.IconProperty, SplitButton.IconProperty);
            CreateTemplateParentBinding(primaryButton, InputElement.IsEnabledProperty, InputElement.IsEnabledProperty);
            CreateTemplateParentBinding(primaryButton, Button.IsDangerProperty, SplitButton.IsDangerProperty);
            CreateTemplateParentBinding(primaryButton, Button.ButtonTypeProperty,
                SplitButton.EffectiveButtonTypeProperty);

            primaryButton.RegisterInNameScope(scope);
            var secondaryButton = new Button
            {
                Name = SecondaryButtonPart
            };
            CreateTemplateParentBinding(secondaryButton, TemplatedControl.CornerRadiusProperty,
                SplitButton.SecondaryButtonCornerRadiusProperty);
            CreateTemplateParentBinding(secondaryButton, Button.SizeTypeProperty, SplitButton.SizeTypeProperty);
            CreateTemplateParentBinding(secondaryButton, Button.IconProperty, SplitButton.FlyoutButtonIconProperty);
            CreateTemplateParentBinding(secondaryButton, InputElement.IsEnabledProperty,
                InputElement.IsEnabledProperty);
            CreateTemplateParentBinding(secondaryButton, Button.IsDangerProperty, SplitButton.IsDangerProperty);
            CreateTemplateParentBinding(secondaryButton, Button.ButtonTypeProperty,
                SplitButton.EffectiveButtonTypeProperty);

            secondaryButton.RegisterInNameScope(scope);
            mainLayout.Children.Add(primaryButton);
            mainLayout.Children.Add(secondaryButton);
            return mainLayout;
        });
    }

    protected override void BuildStyles()
    {
        BuildSizeStyle();
        BuildZIndexStyle();
    }

    private void BuildSizeStyle()
    {
        var largeSizeStyle =
            new Style(selector => selector.Nesting().PropertyEquals(Button.SizeTypeProperty, SizeType.Large));
        largeSizeStyle.Add(TemplatedControl.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadiusLG);
        Add(largeSizeStyle);

        var middleSizeStyle =
            new Style(selector => selector.Nesting().PropertyEquals(Button.SizeTypeProperty, SizeType.Middle));
        middleSizeStyle.Add(TemplatedControl.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadius);
        Add(middleSizeStyle);

        var smallSizeStyle =
            new Style(selector => selector.Nesting().PropertyEquals(Button.SizeTypeProperty, SizeType.Small));
        smallSizeStyle.Add(TemplatedControl.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadiusSM);
        Add(smallSizeStyle);
    }

    private void BuildZIndexStyle()
    {
        var buttonStyle = new Style(selector =>
            selector.Nesting().Template().Name(MainLayoutPart).Child().OfType<Button>());
        buttonStyle.Add(Visual.ZIndexProperty, NormalZIndex);
        Add(buttonStyle);
        var buttonHoverStyle = new Style(selector =>
            selector.Nesting().Template().Name(MainLayoutPart).Child().OfType<Button>()
                .Class(StdPseudoClass.PointerOver));
        buttonHoverStyle.Add(Visual.ZIndexProperty, ActivatedZIndex);
        Add(buttonHoverStyle);
    }
}