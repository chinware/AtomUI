using AtomUI.Media;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class AddOnDecoratedInnerBoxTheme : BaseControlTheme
{
    public const string MainLayoutPart = "PART_MainLayout";
    public const string ContentPresenterPart = "PART_ContentPresenter";
    public const string LeftAddOnPart = "PART_LeftAddOn";
    public const string RightAddOnPart = "PART_RightAddOn";
    public const string LeftAddOnLayoutPart = "PART_LeftAddOnLayout";
    public const string RightAddOnLayoutPart = "PART_RightAddOnLayout";
    public const string ClearButtonPart = "PART_ClearButton";
    public const string InnerBoxDecoratorPart = "PART_InnerBoxDecorator";

    public AddOnDecoratedInnerBoxTheme() : base(typeof(AddOnDecoratedInnerBox))
    {
    }

    protected AddOnDecoratedInnerBoxTheme(Type targetType) : base(targetType)
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<AddOnDecoratedInnerBox>((decoratedBox, scope) =>
        {
            var frameLayout = new Panel();
            BuildFrameDecorator(frameLayout, decoratedBox, scope);
            NotifyBuildExtraChild(frameLayout, decoratedBox, scope);
            return frameLayout;
        });
    }

    protected virtual void NotifyBuildExtraChild(Panel layout, AddOnDecoratedInnerBox decoratedBox, INameScope scope)
    {
    }

    protected virtual void BuildFrameDecorator(Panel layout, AddOnDecoratedInnerBox decoratedBox, INameScope scope)
    {
        var innerBoxDecorator = new Border
        {
            Name = InnerBoxDecoratorPart,
            Transitions = new Transitions
            {
                AnimationUtils.CreateTransition<SolidColorBrushTransition>(Border.BorderBrushProperty),
                AnimationUtils.CreateTransition<SolidColorBrushTransition>(Border.BackgroundProperty)
            }
        };

        innerBoxDecorator.RegisterInNameScope(scope);
        CreateTemplateParentBinding(innerBoxDecorator, Decorator.PaddingProperty,
            AddOnDecoratedInnerBox.EffectiveInnerBoxPaddingProperty);
        CreateTemplateParentBinding(innerBoxDecorator, Border.BorderThicknessProperty,
            TemplatedControl.BorderThicknessProperty);
        CreateTemplateParentBinding(innerBoxDecorator, Border.CornerRadiusProperty,
            TemplatedControl.CornerRadiusProperty);

        var mainLayout = BuildBoxMainLayout(decoratedBox, scope);
        innerBoxDecorator.Child = mainLayout;

        layout.Children.Add(innerBoxDecorator);
    }

    protected virtual Panel BuildBoxMainLayout(AddOnDecoratedInnerBox decoratedBox, INameScope scope)
    {
        var mainLayout = new Grid
        {
            Name = MainLayoutPart,
            ColumnDefinitions = new ColumnDefinitions
            {
                new(GridLength.Auto),
                new(GridLength.Star),
                new(GridLength.Auto)
            }
        };
        BuildGridChildren(decoratedBox, mainLayout, scope);
        return mainLayout;
    }

    protected virtual void BuildGridChildren(AddOnDecoratedInnerBox decoratedBox, Grid mainLayout, INameScope scope)
    {
        BuildLeftAddOn(mainLayout, scope);
        BuildContent(decoratedBox, mainLayout, scope);
        BuildRightAddOn(mainLayout, scope);
    }

    protected virtual void BuildLeftAddOn(Grid layout, INameScope scope)
    {
        // 理论上可以支持多个，暂时先支持一个
        var addLayout = new StackPanel
        {
            Name        = LeftAddOnLayoutPart,
            Orientation = Orientation.Horizontal
        };
        TokenResourceBinder.CreateGlobalTokenBinding(addLayout, StackPanel.SpacingProperty,
            GlobalTokenResourceKey.PaddingXXS);
        addLayout.RegisterInNameScope(scope);

        var leftAddOnContentPresenter = new ContentPresenter
        {
            Name                     = LeftAddOnPart,
            VerticalAlignment        = VerticalAlignment.Stretch,
            VerticalContentAlignment = VerticalAlignment.Center,
            HorizontalAlignment      = HorizontalAlignment.Left,
            Focusable                = false
        };

        CreateTemplateParentBinding(leftAddOnContentPresenter, ContentPresenter.ContentProperty,
            AddOnDecoratedInnerBox.LeftAddOnContentProperty);
        leftAddOnContentPresenter.RegisterInNameScope(scope);

        addLayout.Children.Add(leftAddOnContentPresenter);

        Grid.SetColumn(addLayout, 0);
        layout.Children.Add(addLayout);
    }

    protected virtual void BuildContent(AddOnDecoratedInnerBox decoratedBox, Grid layout, INameScope scope)
    {
        var innerBox = new ContentPresenter
        {
            Name = ContentPresenterPart
        };
        innerBox.RegisterInNameScope(scope);

        CreateTemplateParentBinding(innerBox, Layoutable.MarginProperty,
            AddOnDecoratedInnerBox.ContentPresenterMarginProperty);
        CreateTemplateParentBinding(innerBox, ContentPresenter.ContentProperty, ContentControl.ContentProperty);
        CreateTemplateParentBinding(innerBox, ContentPresenter.ContentTemplateProperty,
            ContentControl.ContentTemplateProperty);

        layout.Children.Add(innerBox);
        Grid.SetColumn(innerBox, 1);
    }

    private void BuildRightAddOn(Grid layout, INameScope scope)
    {
        var addLayout = new StackPanel
        {
            Name        = RightAddOnLayoutPart,
            Orientation = Orientation.Horizontal
        };
        TokenResourceBinder.CreateGlobalTokenBinding(addLayout, StackPanel.SpacingProperty,
            GlobalTokenResourceKey.PaddingXXS);
        addLayout.RegisterInNameScope(scope);

        BuildRightAddOnItems(addLayout, scope);

        var rightAddOnContentPresenter = new ContentPresenter
        {
            Name                     = RightAddOnPart,
            VerticalAlignment        = VerticalAlignment.Stretch,
            VerticalContentAlignment = VerticalAlignment.Center,
            HorizontalAlignment      = HorizontalAlignment.Right,
            Focusable                = false
        };
        CreateTemplateParentBinding(rightAddOnContentPresenter, ContentPresenter.ContentProperty,
            AddOnDecoratedInnerBox.RightAddOnContentProperty);

        rightAddOnContentPresenter.RegisterInNameScope(scope);
        addLayout.Children.Add(rightAddOnContentPresenter);

        layout.Children.Add(addLayout);
        Grid.SetColumn(addLayout, 2);
    }

    protected virtual void BuildRightAddOnItems(StackPanel layout, INameScope scope)
    {
        BuildClearButton(layout, scope);
    }

    protected virtual void BuildClearButton(StackPanel addOnLayout, INameScope scope)
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
            AddOnDecoratedInnerBox.IsClearButtonVisibleProperty);
        addOnLayout.Children.Add(clearButton);
    }

    protected override void BuildStyles()
    {
        BuildCommonStyle();
        BuildDisabledStyle();
        BuildOutLineStyle();
        BuildFilledStyle();
        BuildAddOnStyle();
    }

    private void BuildCommonStyle()
    {
        var commonStyle = new Style(selector => selector.Nesting());

        var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
        decoratorStyle.Add(Visual.ZIndexProperty, AddOnDecoratedBoxTheme.NormalZIndex);
        commonStyle.Add(decoratorStyle);

        var largeStyle =
            new Style(selector =>
                selector.Nesting().PropertyEquals(AddOnDecoratedInnerBox.SizeTypeProperty, SizeType.Large));
        largeStyle.Add(AddOnDecoratedInnerBox.InnerBoxPaddingProperty, AddOnDecoratedBoxTokenResourceKey.PaddingLG);

        {
            var innerBoxContentStyle = new Style(selector => selector.Nesting().Template().Name(ContentPresenterPart));
            innerBoxContentStyle.Add(TextBlock.LineHeightProperty, GlobalTokenResourceKey.FontHeightLG);
            innerBoxContentStyle.Add(Layoutable.MinHeightProperty, GlobalTokenResourceKey.FontHeightLG);
            largeStyle.Add(innerBoxContentStyle);
        }
        {
            var iconStyle = new Style(selector => selector.Nesting().Template().Descendant().OfType<PathIcon>());
            iconStyle.Add(Layoutable.WidthProperty, GlobalTokenResourceKey.IconSizeLG);
            iconStyle.Add(Layoutable.HeightProperty, GlobalTokenResourceKey.IconSizeLG);
            largeStyle.Add(iconStyle);
        }
        commonStyle.Add(largeStyle);

        var middleStyle =
            new Style(selector =>
                selector.Nesting().PropertyEquals(AddOnDecoratedInnerBox.SizeTypeProperty, SizeType.Middle));
        middleStyle.Add(AddOnDecoratedInnerBox.InnerBoxPaddingProperty, AddOnDecoratedBoxTokenResourceKey.Padding);
        {
            var innerBoxContentStyle = new Style(selector => selector.Nesting().Template().Name(ContentPresenterPart));
            innerBoxContentStyle.Add(TextBlock.LineHeightProperty, GlobalTokenResourceKey.FontHeight);
            innerBoxContentStyle.Add(Layoutable.MinHeightProperty, GlobalTokenResourceKey.FontHeight);
            middleStyle.Add(innerBoxContentStyle);
        }
        {
            var iconStyle = new Style(selector => selector.Nesting().Template().Descendant().OfType<PathIcon>());
            iconStyle.Add(Layoutable.WidthProperty, GlobalTokenResourceKey.IconSize);
            iconStyle.Add(Layoutable.HeightProperty, GlobalTokenResourceKey.IconSize);
            middleStyle.Add(iconStyle);
        }
        commonStyle.Add(middleStyle);

        var smallStyle =
            new Style(selector =>
                selector.Nesting().PropertyEquals(AddOnDecoratedInnerBox.SizeTypeProperty, SizeType.Small));
        smallStyle.Add(AddOnDecoratedInnerBox.InnerBoxPaddingProperty, AddOnDecoratedBoxTokenResourceKey.PaddingSM);
        {
            var innerBoxContentStyle = new Style(selector => selector.Nesting().Template().Name(ContentPresenterPart));
            innerBoxContentStyle.Add(TextBlock.LineHeightProperty, GlobalTokenResourceKey.FontHeightSM);
            innerBoxContentStyle.Add(Layoutable.MinHeightProperty, GlobalTokenResourceKey.FontHeightSM);
            smallStyle.Add(innerBoxContentStyle);
        }
        {
            var iconStyle = new Style(selector => selector.Nesting().Template().Descendant().OfType<PathIcon>());
            iconStyle.Add(Layoutable.WidthProperty, GlobalTokenResourceKey.IconSizeSM);
            iconStyle.Add(Layoutable.HeightProperty, GlobalTokenResourceKey.IconSizeSM);
            smallStyle.Add(iconStyle);
        }
        commonStyle.Add(smallStyle);

        Add(commonStyle);
    }

    private void BuildOutLineStyle()
    {
        var outlineStyle =
            new Style(selector => selector.Nesting()
                .PropertyEquals(AddOnDecoratedBox.StyleVariantProperty, AddOnDecoratedVariant.Outline));

        {
            {
                var innerBoxDecoratorStyle =
                    new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
                innerBoxDecoratorStyle.Add(Border.BorderBrushProperty, GlobalTokenResourceKey.ColorBorder);
                innerBoxDecoratorStyle.Add(Border.BackgroundProperty, GlobalTokenResourceKey.ColorTransparent);
                outlineStyle.Add(innerBoxDecoratorStyle);
            }

            var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
            {
                var innerBoxDecoratorStyle =
                    new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
                innerBoxDecoratorStyle.Add(Border.BorderBrushProperty,
                    AddOnDecoratedBoxTokenResourceKey.HoverBorderColor);
                hoverStyle.Add(innerBoxDecoratorStyle);
            }

            outlineStyle.Add(hoverStyle);

            var focusStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.FocusWithIn));
            {
                var innerBoxDecoratorStyle =
                    new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
                innerBoxDecoratorStyle.Add(Border.BorderBrushProperty,
                    AddOnDecoratedBoxTokenResourceKey.ActiveBorderColor);
                focusStyle.Add(innerBoxDecoratorStyle);
            }
            outlineStyle.Add(focusStyle);
        }
        {
            var errorStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(AddOnDecoratedInnerBox.StatusProperty, AddOnDecoratedStatus.Error));

            {
                var innerBoxDecoratorStyle =
                    new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
                innerBoxDecoratorStyle.Add(Border.BorderBrushProperty, GlobalTokenResourceKey.ColorError);
                errorStyle.Add(innerBoxDecoratorStyle);
            }

            var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
            {
                var innerBoxDecoratorStyle =
                    new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
                innerBoxDecoratorStyle.Add(Border.BorderBrushProperty, GlobalTokenResourceKey.ColorErrorBorderHover);
                hoverStyle.Add(innerBoxDecoratorStyle);
            }
            errorStyle.Add(hoverStyle);

            var focusStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.FocusWithIn));
            {
                var innerBoxDecoratorStyle =
                    new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
                innerBoxDecoratorStyle.Add(Border.BorderBrushProperty, GlobalTokenResourceKey.ColorError);
                focusStyle.Add(innerBoxDecoratorStyle);
            }
            errorStyle.Add(focusStyle);
            outlineStyle.Add(errorStyle);
        }

        {
            var warningStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(AddOnDecoratedInnerBox.StatusProperty, AddOnDecoratedStatus.Warning));

            {
                var innerBoxDecoratorStyle =
                    new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
                innerBoxDecoratorStyle.Add(Border.BorderBrushProperty, GlobalTokenResourceKey.ColorWarning);
                warningStyle.Add(innerBoxDecoratorStyle);
            }

            var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
            {
                var innerBoxDecoratorStyle =
                    new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
                innerBoxDecoratorStyle.Add(Border.BorderBrushProperty, GlobalTokenResourceKey.ColorWarningBorderHover);
                hoverStyle.Add(innerBoxDecoratorStyle);
            }
            warningStyle.Add(hoverStyle);

            var focusStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.FocusWithIn));
            {
                var innerBoxDecoratorStyle =
                    new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
                innerBoxDecoratorStyle.Add(Border.BorderBrushProperty, GlobalTokenResourceKey.ColorWarning);
                focusStyle.Add(innerBoxDecoratorStyle);
            }
            warningStyle.Add(focusStyle);
            outlineStyle.Add(warningStyle);
        }

        Add(outlineStyle);
    }

    private void BuildFilledStyle()
    {
        var filledStyle =
            new Style(selector =>
                selector.Nesting()
                    .PropertyEquals(AddOnDecoratedBox.StyleVariantProperty, AddOnDecoratedVariant.Filled));

        {
            {
                var innerBoxDecoratorStyle =
                    new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));

                innerBoxDecoratorStyle.Add(Border.BorderBrushProperty, GlobalTokenResourceKey.ColorTransparent);
                innerBoxDecoratorStyle.Add(Border.BackgroundProperty, GlobalTokenResourceKey.ColorFillTertiary);
                filledStyle.Add(innerBoxDecoratorStyle);
            }

            var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
            {
                var innerBoxDecoratorStyle =
                    new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
                innerBoxDecoratorStyle.Add(Border.BackgroundProperty, GlobalTokenResourceKey.ColorFillSecondary);
                hoverStyle.Add(innerBoxDecoratorStyle);
            }
            filledStyle.Add(hoverStyle);

            var focusStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.FocusWithIn));
            {
                var innerBoxDecoratorStyle =
                    new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
                innerBoxDecoratorStyle.Add(Border.BorderBrushProperty,
                    AddOnDecoratedBoxTokenResourceKey.ActiveBorderColor);
                innerBoxDecoratorStyle.Add(Border.BackgroundProperty, GlobalTokenResourceKey.ColorTransparent);
                focusStyle.Add(innerBoxDecoratorStyle);
            }
            filledStyle.Add(focusStyle);
        }

        {
            var errorStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(AddOnDecoratedInnerBox.StatusProperty, AddOnDecoratedStatus.Error));

            {
                var innerBoxDecoratorStyle =
                    new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));

                innerBoxDecoratorStyle.Add(Border.BorderBrushProperty, GlobalTokenResourceKey.ColorTransparent);
                innerBoxDecoratorStyle.Add(Border.BackgroundProperty, GlobalTokenResourceKey.ColorErrorBg);
                errorStyle.Add(innerBoxDecoratorStyle);
            }

            var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
            {
                var innerBoxDecoratorStyle =
                    new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
                innerBoxDecoratorStyle.Add(Border.BackgroundProperty, GlobalTokenResourceKey.ColorErrorBgHover);
                hoverStyle.Add(innerBoxDecoratorStyle);
            }
            errorStyle.Add(hoverStyle);

            var focusStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.FocusWithIn));
            {
                var innerBoxDecoratorStyle =
                    new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
                innerBoxDecoratorStyle.Add(Border.BorderBrushProperty, GlobalTokenResourceKey.ColorError);
                innerBoxDecoratorStyle.Add(Border.BackgroundProperty, AddOnDecoratedBoxTokenResourceKey.ActiveBg);
                focusStyle.Add(innerBoxDecoratorStyle);
            }

            errorStyle.Add(focusStyle);
            filledStyle.Add(errorStyle);
        }

        {
            var warningStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(AddOnDecoratedInnerBox.StatusProperty, AddOnDecoratedStatus.Warning));

            {
                var innerBoxDecoratorStyle =
                    new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));

                innerBoxDecoratorStyle.Add(Border.BorderBrushProperty, GlobalTokenResourceKey.ColorTransparent);
                innerBoxDecoratorStyle.Add(Border.BackgroundProperty, GlobalTokenResourceKey.ColorWarningBg);
                warningStyle.Add(innerBoxDecoratorStyle);
            }

            var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
            {
                var innerBoxDecoratorStyle =
                    new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
                innerBoxDecoratorStyle.Add(Border.BackgroundProperty, GlobalTokenResourceKey.ColorWarningBgHover);
                hoverStyle.Add(innerBoxDecoratorStyle);
            }
            warningStyle.Add(hoverStyle);

            var focusStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.FocusWithIn));
            {
                var innerBoxDecoratorStyle =
                    new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
                innerBoxDecoratorStyle.Add(Border.BorderBrushProperty, GlobalTokenResourceKey.ColorWarning);
                innerBoxDecoratorStyle.Add(Border.BackgroundProperty, AddOnDecoratedBoxTokenResourceKey.ActiveBg);
                focusStyle.Add(innerBoxDecoratorStyle);
            }
            warningStyle.Add(focusStyle);

            filledStyle.Add(warningStyle);
        }

        Add(filledStyle);
    }

    private void BuildAddOnStyle()
    {
        {
            var errorStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(AddOnDecoratedInnerBox.StatusProperty, AddOnDecoratedStatus.Error));
            {
                var iconStyle = new Style(selector => Selectors.Or(selector.Nesting().Template().Name(LeftAddOnPart),
                    selector.Nesting().Template().Name(RightAddOnPart)).Nesting().Descendant().OfType<PathIcon>());
                iconStyle.Add(PathIcon.NormalFilledBrushProperty, GlobalTokenResourceKey.ColorError);
                errorStyle.Add(iconStyle);
            }
            Add(errorStyle);
        }

        {
            var warningStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(AddOnDecoratedInnerBox.StatusProperty, AddOnDecoratedStatus.Warning));
            {
                var iconStyle = new Style(selector => Selectors.Or(selector.Nesting().Template().Name(LeftAddOnPart),
                    selector.Nesting().Template().Name(RightAddOnPart)).Nesting().Descendant().OfType<PathIcon>());
                iconStyle.Add(PathIcon.NormalFilledBrushProperty, GlobalTokenResourceKey.ColorWarning);
                warningStyle.Add(iconStyle);
            }
            Add(warningStyle);
        }
    }

    protected virtual void BuildDisabledStyle()
    {
        var disabledStyle  = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
        var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
        decoratorStyle.Add(Border.BackgroundProperty, GlobalTokenResourceKey.ColorBgContainerDisabled);
        disabledStyle.Add(decoratorStyle);
        Add(disabledStyle);
    }
}