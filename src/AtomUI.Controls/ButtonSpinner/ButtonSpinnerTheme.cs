using AtomUI.IconPkg;
using AtomUI.IconPkg.AntDesign;
using AtomUI.Media;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class ButtonSpinnerTheme : BaseControlTheme
{
    public const string DecoratedBoxPart = "PART_DecoratedBox";
    public const string SpinnerInnerBoxPart = "PART_SpinnerInnerBox";
    public const string IncreaseButtonPart = "PART_IncreaseButton";
    public const string DecreaseButtonPart = "PART_DecreaseButton";
    public const string SpinnerHandleDecoratorPart = "PART_SpinnerHandleDecorator";

    public ButtonSpinnerTheme() : base(typeof(ButtonSpinner))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<ButtonSpinner>((buttonSpinner, scope) =>
        {
            var decoratedBox = BuildSpinnerDecoratedBox(buttonSpinner, scope);
            var innerBox     = BuildSpinnerContent(buttonSpinner, scope);
            decoratedBox.Content = innerBox;
            innerBox.RegisterInNameScope(scope);
            return decoratedBox;
        });
    }

    protected virtual ButtonSpinnerDecoratedBox BuildSpinnerDecoratedBox(ButtonSpinner buttonSpinner, INameScope scope)
    {
        var decoratedBox = new ButtonSpinnerDecoratedBox
        {
            Name      = DecoratedBoxPart,
            Focusable = true
        };
        CreateTemplateParentBinding(decoratedBox, AddOnDecoratedBox.StyleVariantProperty,
            ButtonSpinner.StyleVariantProperty);
        CreateTemplateParentBinding(decoratedBox, AddOnDecoratedBox.SizeTypeProperty, ButtonSpinner.SizeTypeProperty);
        CreateTemplateParentBinding(decoratedBox, AddOnDecoratedBox.StatusProperty, ButtonSpinner.StatusProperty);
        CreateTemplateParentBinding(decoratedBox, AddOnDecoratedBox.LeftAddOnProperty, ButtonSpinner.LeftAddOnProperty);
        CreateTemplateParentBinding(decoratedBox, AddOnDecoratedBox.RightAddOnProperty,
            ButtonSpinner.RightAddOnProperty);
        CreateTemplateParentBinding(decoratedBox, ButtonSpinnerDecoratedBox.ShowButtonSpinnerProperty,
            Avalonia.Controls.ButtonSpinner.ShowButtonSpinnerProperty);
        CreateTemplateParentBinding(decoratedBox, ButtonSpinnerDecoratedBox.ButtonSpinnerLocationProperty,
            Avalonia.Controls.ButtonSpinner.ButtonSpinnerLocationProperty);
        decoratedBox.RegisterInNameScope(scope);
        return decoratedBox;
    }

    protected virtual ButtonSpinnerInnerBox BuildSpinnerContent(ButtonSpinner buttonSpinner, INameScope scope)
    {
        var spinnerInnerBox = new ButtonSpinnerInnerBox
        {
            Name = SpinnerInnerBoxPart
        };
        CreateTemplateParentBinding(spinnerInnerBox, AddOnDecoratedInnerBox.LeftAddOnContentProperty,
            ButtonSpinner.InnerLeftContentProperty);
        CreateTemplateParentBinding(spinnerInnerBox, AddOnDecoratedInnerBox.RightAddOnContentProperty,
            ButtonSpinner.InnerRightContentProperty);
        CreateTemplateParentBinding(spinnerInnerBox, AddOnDecoratedInnerBox.StyleVariantProperty,
            ButtonSpinner.StyleVariantProperty);
        CreateTemplateParentBinding(spinnerInnerBox, AddOnDecoratedInnerBox.StatusProperty,
            ButtonSpinner.StatusProperty);
        CreateTemplateParentBinding(spinnerInnerBox, ContentControl.ContentProperty, ContentControl.ContentProperty);
        CreateTemplateParentBinding(spinnerInnerBox, AddOnDecoratedInnerBox.SizeTypeProperty,
            ButtonSpinner.SizeTypeProperty);
        CreateTemplateParentBinding(spinnerInnerBox, ContentControl.ContentTemplateProperty,
            ContentControl.ContentTemplateProperty);
        CreateTemplateParentBinding(spinnerInnerBox, ButtonSpinnerInnerBox.ShowButtonSpinnerProperty,
            Avalonia.Controls.ButtonSpinner.ShowButtonSpinnerProperty);
        CreateTemplateParentBinding(spinnerInnerBox, ButtonSpinnerInnerBox.ButtonSpinnerLocationProperty,
            Avalonia.Controls.ButtonSpinner.ButtonSpinnerLocationProperty);

        var decoratorLayout = new Panel();
        var spinnerHandleDecorator = new Border
        {
            Name             = SpinnerHandleDecoratorPart,
            BackgroundSizing = BackgroundSizing.InnerBorderEdge,
            ClipToBounds     = true
        };

        spinnerHandleDecorator.RegisterInNameScope(scope);
        decoratorLayout.Children.Add(spinnerHandleDecorator);

        var spinnerLayout = new UniformGrid
        {
            Columns = 1,
            Rows    = 2
        };
        
        decoratorLayout.Children.Add(spinnerLayout);

        buttonSpinner.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(spinnerLayout, Layoutable.WidthProperty,
            ButtonSpinnerTokenKey.HandleWidth));
        
        var increaseButtonIcon = AntDesignIconPackage.UpOutlined();

        buttonSpinner.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(increaseButtonIcon, Icon.ActiveFilledBrushProperty,
            ButtonSpinnerTokenKey.HandleHoverColor));
        buttonSpinner.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(increaseButtonIcon, Icon.SelectedFilledBrushProperty,
            SharedTokenKey.ColorPrimaryActive));

        var increaseButton = new IconButton
        {
            Name                = IncreaseButtonPart,
            Icon                = increaseButtonIcon,
            VerticalAlignment   = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            BackgroundSizing    = BackgroundSizing.InnerBorderEdge,
        };
        increaseButton.SetCurrentValue(TemplatedControl.BackgroundProperty, Brushes.Transparent);
        {
            var handleButtonStyle = new Style(selector => selector.Class(StdPseudoClass.Pressed));
            handleButtonStyle.Add(TemplatedControl.BackgroundProperty, ButtonSpinnerTokenKey.HandleActiveBg);
            increaseButton.Styles.Add(handleButtonStyle);
        }

        buttonSpinner.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(increaseButton, IconButton.IconWidthProperty,
            ButtonSpinnerTokenKey.HandleIconSize));
        buttonSpinner.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(increaseButton, IconButton.IconHeightProperty,
            ButtonSpinnerTokenKey.HandleIconSize));
        increaseButton.RegisterInNameScope(scope);

        var decreaseButtonIcon = AntDesignIconPackage.DownOutlined();

        buttonSpinner.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(decreaseButtonIcon, Icon.ActiveFilledBrushProperty,
            ButtonSpinnerTokenKey.HandleHoverColor));
        buttonSpinner.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(decreaseButtonIcon, Icon.SelectedFilledBrushProperty,
            SharedTokenKey.ColorPrimaryActive));

        var decreaseButton = new IconButton
        {
            Name                = DecreaseButtonPart,
            Icon                = decreaseButtonIcon,
            VerticalAlignment   = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            BackgroundSizing    = BackgroundSizing.InnerBorderEdge
        };
        decreaseButton.SetCurrentValue(TemplatedControl.BackgroundProperty, Brushes.Transparent);
        {
            var handleButtonStyle = new Style(selector => selector.Class(StdPseudoClass.Pressed));
            handleButtonStyle.Add(TemplatedControl.BackgroundProperty, ButtonSpinnerTokenKey.HandleActiveBg);
            decreaseButton.Styles.Add(handleButtonStyle);
        }
        decreaseButton.RegisterInNameScope(scope);
        buttonSpinner.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(decreaseButton, IconButton.IconWidthProperty,
            ButtonSpinnerTokenKey.HandleIconSize));
        buttonSpinner.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(decreaseButton, IconButton.IconHeightProperty,
            ButtonSpinnerTokenKey.HandleIconSize));

        spinnerLayout.Children.Add(increaseButton);
        spinnerLayout.Children.Add(decreaseButton);

        spinnerInnerBox.SpinnerContent = decoratorLayout;

        return spinnerInnerBox;
    }

    protected override void BuildStyles()
    {
        var commonStyle = new Style(selector => selector.Nesting());
        var largeStyle =
            new Style(selector =>
                selector.Nesting().PropertyEquals(AddOnDecoratedBox.SizeTypeProperty, SizeType.Large));
        largeStyle.Add(TemplatedControl.CornerRadiusProperty, SharedTokenKey.BorderRadiusLG);
        commonStyle.Add(largeStyle);

        var middleStyle =
            new Style(
                selector => selector.Nesting().PropertyEquals(AddOnDecoratedBox.SizeTypeProperty, SizeType.Middle));
        middleStyle.Add(TemplatedControl.CornerRadiusProperty, SharedTokenKey.BorderRadius);
        commonStyle.Add(middleStyle);

        var smallStyle =
            new Style(selector =>
                selector.Nesting().PropertyEquals(AddOnDecoratedBox.SizeTypeProperty, SizeType.Small));
        smallStyle.Add(TemplatedControl.CornerRadiusProperty, SharedTokenKey.BorderRadiusSM);
        commonStyle.Add(smallStyle);
        
        var isEnableMotionStyle = new Style(selector => selector.Nesting().PropertyEquals(ButtonSpinner.IsMotionEnabledProperty, true));
        var iconStyle = new Style(selector => selector.Nesting().Nesting().OfType<IconButton>());
        iconStyle.Add(IconButton.TransitionsProperty, new SetterValueFactory<Transitions>(() => new Transitions()
        {
            AnimationUtils.CreateTransition<SolidColorBrushTransition>(TemplatedControl.BackgroundProperty)
        }));
        isEnableMotionStyle.Add(iconStyle);
        commonStyle.Add(isEnableMotionStyle);

        Add(commonStyle);
    }
}