﻿using AtomUI.Media;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Theme.Utils;
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

        var spinnerHandleDecorator = new Border
        {
            Name             = SpinnerHandleDecoratorPart,
            BackgroundSizing = BackgroundSizing.InnerBorderEdge,
            ClipToBounds     = true
        };

        spinnerHandleDecorator.RegisterInNameScope(scope);

        var spinnerLayout = new UniformGrid
        {
            Columns = 1,
            Rows    = 2
        };

        spinnerHandleDecorator.Child = spinnerLayout;

        TokenResourceBinder.CreateTokenBinding(spinnerLayout, Layoutable.WidthProperty,
            ButtonSpinnerTokenResourceKey.HandleWidth);

        var increaseButtonIcon = new PathIcon
        {
            Kind = "UpOutlined"
        };

        TokenResourceBinder.CreateGlobalTokenBinding(increaseButtonIcon, PathIcon.ActiveFilledBrushProperty,
            ButtonSpinnerTokenResourceKey.HandleHoverColor);
        TokenResourceBinder.CreateGlobalTokenBinding(increaseButtonIcon, PathIcon.SelectedFilledBrushProperty,
            GlobalTokenResourceKey.ColorPrimaryActive);

        var increaseButton = new IconButton
        {
            Name                = IncreaseButtonPart,
            Icon                = increaseButtonIcon,
            VerticalAlignment   = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            BackgroundSizing    = BackgroundSizing.InnerBorderEdge,
            Transitions = new Transitions
            {
                AnimationUtils.CreateTransition<SolidColorBrushTransition>(TemplatedControl.BackgroundProperty)
            }
        };
        increaseButton.SetCurrentValue(TemplatedControl.BackgroundProperty, new SolidColorBrush(Colors.Transparent));
        {
            var handleButtonStyle = new Style(selector => selector.Class(StdPseudoClass.Pressed));
            handleButtonStyle.Add(TemplatedControl.BackgroundProperty, ButtonSpinnerTokenResourceKey.HandleActiveBg);
            increaseButton.Styles.Add(handleButtonStyle);
        }

        TokenResourceBinder.CreateTokenBinding(increaseButton, IconButton.IconWidthProperty,
            ButtonSpinnerTokenResourceKey.HandleIconSize);
        TokenResourceBinder.CreateTokenBinding(increaseButton, IconButton.IconHeightProperty,
            ButtonSpinnerTokenResourceKey.HandleIconSize);
        increaseButton.RegisterInNameScope(scope);

        var decreaseButtonIcon = new PathIcon
        {
            Kind = "DownOutlined"
        };

        TokenResourceBinder.CreateGlobalTokenBinding(decreaseButtonIcon, PathIcon.ActiveFilledBrushProperty,
            ButtonSpinnerTokenResourceKey.HandleHoverColor);
        TokenResourceBinder.CreateGlobalTokenBinding(decreaseButtonIcon, PathIcon.SelectedFilledBrushProperty,
            GlobalTokenResourceKey.ColorPrimaryActive);

        var decreaseButton = new IconButton
        {
            Name                = DecreaseButtonPart,
            Icon                = decreaseButtonIcon,
            VerticalAlignment   = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            BackgroundSizing    = BackgroundSizing.InnerBorderEdge,
            Transitions = new Transitions
            {
                AnimationUtils.CreateTransition<SolidColorBrushTransition>(TemplatedControl.BackgroundProperty)
            }
        };
        decreaseButton.SetCurrentValue(TemplatedControl.BackgroundProperty, new SolidColorBrush(Colors.Transparent));
        {
            var handleButtonStyle = new Style(selector => selector.Class(StdPseudoClass.Pressed));
            handleButtonStyle.Add(TemplatedControl.BackgroundProperty, ButtonSpinnerTokenResourceKey.HandleActiveBg);
            decreaseButton.Styles.Add(handleButtonStyle);
        }
        decreaseButton.RegisterInNameScope(scope);
        TokenResourceBinder.CreateTokenBinding(decreaseButton, IconButton.IconWidthProperty,
            ButtonSpinnerTokenResourceKey.HandleIconSize);
        TokenResourceBinder.CreateTokenBinding(decreaseButton, IconButton.IconHeightProperty,
            ButtonSpinnerTokenResourceKey.HandleIconSize);

        spinnerLayout.Children.Add(increaseButton);
        spinnerLayout.Children.Add(decreaseButton);

        spinnerInnerBox.SpinnerContent = spinnerHandleDecorator;

        return spinnerInnerBox;
    }

    protected override void BuildStyles()
    {
        var commonStyle = new Style(selector => selector.Nesting());
        var largeStyle =
            new Style(selector =>
                selector.Nesting().PropertyEquals(AddOnDecoratedBox.SizeTypeProperty, SizeType.Large));
        largeStyle.Add(TemplatedControl.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadiusLG);
        commonStyle.Add(largeStyle);

        var middleStyle =
            new Style(
                selector => selector.Nesting().PropertyEquals(AddOnDecoratedBox.SizeTypeProperty, SizeType.Middle));
        middleStyle.Add(TemplatedControl.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadius);
        commonStyle.Add(middleStyle);

        var smallStyle =
            new Style(selector =>
                selector.Nesting().PropertyEquals(AddOnDecoratedBox.SizeTypeProperty, SizeType.Small));
        smallStyle.Add(TemplatedControl.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadiusSM);
        commonStyle.Add(smallStyle);

        Add(commonStyle);
    }
}