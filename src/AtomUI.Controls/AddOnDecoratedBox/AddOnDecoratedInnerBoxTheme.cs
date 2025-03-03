using AtomUI.IconPkg;
using AtomUI.IconPkg.AntDesign;
using AtomUI.Media;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
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
        return new FuncControlTemplate<AddOnDecoratedInnerBox>((addOnDecoratedInnerBox, scope) =>
        {
            var frameLayout = new Panel();
            BuildFrame(addOnDecoratedInnerBox, frameLayout, scope);
            NotifyBuildExtraChild(addOnDecoratedInnerBox, frameLayout, scope);
            return frameLayout;
        });
    }

    protected virtual void NotifyBuildExtraChild(AddOnDecoratedInnerBox addOnDecoratedInnerBox, Panel layout,
                                                 INameScope scope)
    {
    }

    protected virtual void BuildFrame(AddOnDecoratedInnerBox addOnDecoratedInnerBox, Panel layout, INameScope scope)
    {
        var innerBoxDecorator = new Border
        {
            Name = InnerBoxDecoratorPart
        };

        innerBoxDecorator.RegisterInNameScope(scope);
        CreateTemplateParentBinding(innerBoxDecorator, Decorator.PaddingProperty,
            AddOnDecoratedInnerBox.EffectiveInnerBoxPaddingProperty);
        CreateTemplateParentBinding(innerBoxDecorator, Border.BorderThicknessProperty,
            TemplatedControl.BorderThicknessProperty);
        CreateTemplateParentBinding(innerBoxDecorator, Border.CornerRadiusProperty,
            TemplatedControl.CornerRadiusProperty);

        var mainLayout = BuildBoxMainLayout(addOnDecoratedInnerBox, scope);
        innerBoxDecorator.Child = mainLayout;

        layout.Children.Add(innerBoxDecorator);
    }

    protected virtual Panel BuildBoxMainLayout(AddOnDecoratedInnerBox addOnDecoratedInnerBox, INameScope scope)
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
        BuildGridChildren(addOnDecoratedInnerBox, mainLayout, scope);
        return mainLayout;
    }

    protected virtual void BuildGridChildren(AddOnDecoratedInnerBox addOnDecoratedInnerBox, Grid mainLayout,
                                             INameScope scope)
    {
        BuildLeftAddOn(addOnDecoratedInnerBox, mainLayout, scope);
        BuildContent(addOnDecoratedInnerBox, mainLayout, scope);
        BuildRightAddOn(addOnDecoratedInnerBox, mainLayout, scope);
    }

    protected virtual void BuildLeftAddOn(AddOnDecoratedInnerBox addOnDecoratedInnerBox, Grid layout, INameScope scope)
    {
        // 理论上可以支持多个，暂时先支持一个
        var addLayout = new StackPanel
        {
            Name        = LeftAddOnLayoutPart,
            Orientation = Orientation.Horizontal
        };
        RegisterTokenResourceBindings(addOnDecoratedInnerBox, () =>
        {
            addOnDecoratedInnerBox.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(addLayout,
                StackPanel.SpacingProperty,
                SharedTokenKey.PaddingXXS));
        });
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

    protected virtual void BuildContent(AddOnDecoratedInnerBox addOnDecoratedInnerBox, Grid layout, INameScope scope)
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

    protected virtual void BuildRightAddOn(AddOnDecoratedInnerBox addOnDecoratedInnerBox, Grid layout, INameScope scope)
    {
        var addLayout = new StackPanel
        {
            Name        = RightAddOnLayoutPart,
            Orientation = Orientation.Horizontal
        };
    
        addLayout.RegisterInNameScope(scope);
        
        RegisterTokenResourceBindings(addOnDecoratedInnerBox, () =>
        {
            addOnDecoratedInnerBox.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(addLayout,
                StackPanel.SpacingProperty,
                SharedTokenKey.PaddingXXS));
        });

        BuildRightAddOnItems(addOnDecoratedInnerBox, addLayout, scope);

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

    protected virtual void BuildRightAddOnItems(AddOnDecoratedInnerBox addOnDecoratedInnerBox, StackPanel layout,
                                                INameScope scope)
    {
        BuildClearButton(addOnDecoratedInnerBox, layout, scope);
    }

    protected virtual void BuildClearButton(AddOnDecoratedInnerBox addOnDecoratedInnerBox, StackPanel addOnLayout,
                                            INameScope scope)
    {
        var closeIcon = AntDesignIconPackage.CloseCircleFilled();
        var clearButton = new IconButton
        {
            Name = ClearButtonPart,
            Icon = closeIcon
        };

        RegisterTokenResourceBindings(addOnDecoratedInnerBox, () =>
        {
            addOnDecoratedInnerBox.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(clearButton,
                IconButton.IconHeightProperty,
                SharedTokenKey.IconSize));
            addOnDecoratedInnerBox.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(clearButton,
                IconButton.IconWidthProperty,
                SharedTokenKey.IconSize));
            addOnDecoratedInnerBox.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(closeIcon,
                Icon.NormalFilledBrushProperty,
                SharedTokenKey.ColorTextQuaternary));
            addOnDecoratedInnerBox.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(closeIcon,
                Icon.ActiveFilledBrushProperty,
                SharedTokenKey.ColorTextTertiary));
            addOnDecoratedInnerBox.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(closeIcon,
                Icon.SelectedFilledBrushProperty,
                SharedTokenKey.ColorText));
        });

        clearButton.RegisterInNameScope(scope);
        CreateTemplateParentBinding(clearButton, Visual.IsVisibleProperty,
            AddOnDecoratedInnerBox.IsClearButtonVisibleProperty);
        addOnLayout.Children.Add(clearButton);
    }

    protected override void BuildStyles()
    {
        BuildCommonStyle();
        BuildOutLineStyle();
        BuildFilledStyle();
        BuildAddOnStyle();
        BuildDisabledStyle();
    }

    private void BuildCommonStyle()
    {
        var commonStyle = new Style(selector => selector.Nesting());

        {
            var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
            decoratorStyle.Add(Visual.ZIndexProperty, AddOnDecoratedBoxTheme.NormalZIndex);
            commonStyle.Add(decoratorStyle);
        }

        {
            var transitionsStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(AddOnDecoratedInnerBox.IsMotionEnabledProperty, true));
            var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));

            decoratorStyle.Add(new Setter(Border.TransitionsProperty, new SetterValueFactory<Transitions>(() =>
                new Transitions
                {
                    AnimationUtils.CreateTransition<SolidColorBrushTransition>(Border.BorderBrushProperty),
                    AnimationUtils.CreateTransition<SolidColorBrushTransition>(Border.BackgroundProperty)
                })));
            transitionsStyle.Add(decoratorStyle);
            commonStyle.Add(transitionsStyle);
        }

        var largeStyle =
            new Style(selector =>
                selector.Nesting().PropertyEquals(AddOnDecoratedInnerBox.SizeTypeProperty, SizeType.Large));
        largeStyle.Add(AddOnDecoratedInnerBox.InnerBoxPaddingProperty, AddOnDecoratedBoxTokenKey.PaddingLG);

        {
            var innerBoxContentStyle = new Style(selector => selector.Nesting().Template().Name(ContentPresenterPart));
            innerBoxContentStyle.Add(ContentPresenter.LineHeightProperty, SharedTokenKey.FontHeightLG);
            innerBoxContentStyle.Add(Layoutable.MinHeightProperty, SharedTokenKey.FontHeightLG);
            largeStyle.Add(innerBoxContentStyle);
        }
        {
            var iconStyle = new Style(selector => selector.Nesting().Template().Descendant().OfType<Icon>());
            iconStyle.Add(Layoutable.WidthProperty, SharedTokenKey.IconSizeLG);
            iconStyle.Add(Layoutable.HeightProperty, SharedTokenKey.IconSizeLG);
            largeStyle.Add(iconStyle);
        }
        commonStyle.Add(largeStyle);

        var middleStyle =
            new Style(selector =>
                selector.Nesting().PropertyEquals(AddOnDecoratedInnerBox.SizeTypeProperty, SizeType.Middle));
        middleStyle.Add(AddOnDecoratedInnerBox.InnerBoxPaddingProperty, AddOnDecoratedBoxTokenKey.Padding);
        {
            var innerBoxContentStyle = new Style(selector => selector.Nesting().Template().Name(ContentPresenterPart));
            innerBoxContentStyle.Add(ContentPresenter.LineHeightProperty, SharedTokenKey.FontHeight);
            innerBoxContentStyle.Add(Layoutable.MinHeightProperty, SharedTokenKey.FontHeight);
            middleStyle.Add(innerBoxContentStyle);
        }
        {
            var iconStyle = new Style(selector => selector.Nesting().Template().Descendant().OfType<Icon>());
            iconStyle.Add(Layoutable.WidthProperty, SharedTokenKey.IconSize);
            iconStyle.Add(Layoutable.HeightProperty, SharedTokenKey.IconSize);
            middleStyle.Add(iconStyle);
        }
        commonStyle.Add(middleStyle);

        var smallStyle =
            new Style(selector =>
                selector.Nesting().PropertyEquals(AddOnDecoratedInnerBox.SizeTypeProperty, SizeType.Small));
        smallStyle.Add(AddOnDecoratedInnerBox.InnerBoxPaddingProperty, AddOnDecoratedBoxTokenKey.PaddingSM);
        {
            var innerBoxContentStyle = new Style(selector => selector.Nesting().Template().Name(ContentPresenterPart));
            innerBoxContentStyle.Add(TextBlock.LineHeightProperty, SharedTokenKey.FontHeightSM);
            innerBoxContentStyle.Add(Layoutable.MinHeightProperty, SharedTokenKey.FontHeightSM);
            smallStyle.Add(innerBoxContentStyle);
        }
        {
            var iconStyle = new Style(selector => selector.Nesting().Template().Descendant().OfType<Icon>());
            iconStyle.Add(Layoutable.WidthProperty, SharedTokenKey.IconSizeSM);
            iconStyle.Add(Layoutable.HeightProperty, SharedTokenKey.IconSizeSM);
            smallStyle.Add(iconStyle);
        }
        commonStyle.Add(smallStyle);

        Add(commonStyle);
    }

    private void BuildOutLineStyle()
    {
        var enableStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(AddOnDecoratedInnerBox.IsEnabledProperty, true));
        var outlineStyle =
            new Style(selector => selector.Nesting()
                                          .PropertyEquals(AddOnDecoratedBox.StyleVariantProperty,
                                              AddOnDecoratedVariant.Outline));

        {
            {
                var innerBoxDecoratorStyle =
                    new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
                innerBoxDecoratorStyle.Add(Border.BorderBrushProperty, SharedTokenKey.ColorBorder);
                innerBoxDecoratorStyle.Add(Border.BackgroundProperty, SharedTokenKey.ColorTransparent);
                outlineStyle.Add(innerBoxDecoratorStyle);
            }

            var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
            {
                var innerBoxDecoratorStyle =
                    new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
                innerBoxDecoratorStyle.Add(Border.BorderBrushProperty, AddOnDecoratedBoxTokenKey.HoverBorderColor);
                hoverStyle.Add(innerBoxDecoratorStyle);
            }

            outlineStyle.Add(hoverStyle);

            var focusStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.FocusWithIn));
            {
                var innerBoxDecoratorStyle =
                    new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
                innerBoxDecoratorStyle.Add(Border.BorderBrushProperty, AddOnDecoratedBoxTokenKey.ActiveBorderColor);
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
                innerBoxDecoratorStyle.Add(Border.BorderBrushProperty, SharedTokenKey.ColorError);
                errorStyle.Add(innerBoxDecoratorStyle);
            }

            var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
            {
                var innerBoxDecoratorStyle =
                    new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
                innerBoxDecoratorStyle.Add(Border.BorderBrushProperty, SharedTokenKey.ColorErrorBorderHover);
                hoverStyle.Add(innerBoxDecoratorStyle);
            }
            errorStyle.Add(hoverStyle);

            var focusStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.FocusWithIn));
            {
                var innerBoxDecoratorStyle =
                    new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
                innerBoxDecoratorStyle.Add(Border.BorderBrushProperty, SharedTokenKey.ColorError);
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
                innerBoxDecoratorStyle.Add(Border.BorderBrushProperty, SharedTokenKey.ColorWarning);
                warningStyle.Add(innerBoxDecoratorStyle);
            }

            var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
            {
                var innerBoxDecoratorStyle =
                    new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
                innerBoxDecoratorStyle.Add(Border.BorderBrushProperty, SharedTokenKey.ColorWarningBorderHover);
                hoverStyle.Add(innerBoxDecoratorStyle);
            }
            warningStyle.Add(hoverStyle);

            var focusStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.FocusWithIn));
            {
                var innerBoxDecoratorStyle =
                    new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
                innerBoxDecoratorStyle.Add(Border.BorderBrushProperty, SharedTokenKey.ColorWarning);
                focusStyle.Add(innerBoxDecoratorStyle);
            }
            warningStyle.Add(focusStyle);
            outlineStyle.Add(warningStyle);
        }
        enableStyle.Add(outlineStyle);
        Add(enableStyle);
    }

    private void BuildFilledStyle()
    {
        var enableStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(AddOnDecoratedInnerBox.IsEnabledProperty, true));
        var filledStyle =
            new Style(selector =>
                selector.Nesting()
                        .PropertyEquals(AddOnDecoratedBox.StyleVariantProperty, AddOnDecoratedVariant.Filled));

        {
            {
                var innerBoxDecoratorStyle =
                    new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));

                innerBoxDecoratorStyle.Add(Border.BorderBrushProperty, SharedTokenKey.ColorTransparent);
                innerBoxDecoratorStyle.Add(Border.BackgroundProperty, SharedTokenKey.ColorFillTertiary);
                filledStyle.Add(innerBoxDecoratorStyle);
            }

            var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
            {
                var innerBoxDecoratorStyle =
                    new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
                innerBoxDecoratorStyle.Add(Border.BackgroundProperty, SharedTokenKey.ColorFillSecondary);
                hoverStyle.Add(innerBoxDecoratorStyle);
            }
            filledStyle.Add(hoverStyle);

            var focusStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.FocusWithIn));
            {
                var innerBoxDecoratorStyle =
                    new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
                innerBoxDecoratorStyle.Add(Border.BorderBrushProperty, AddOnDecoratedBoxTokenKey.ActiveBorderColor);
                innerBoxDecoratorStyle.Add(Border.BackgroundProperty, SharedTokenKey.ColorTransparent);
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

                innerBoxDecoratorStyle.Add(Border.BorderBrushProperty, SharedTokenKey.ColorTransparent);
                innerBoxDecoratorStyle.Add(Border.BackgroundProperty, SharedTokenKey.ColorErrorBg);
                errorStyle.Add(innerBoxDecoratorStyle);
            }

            var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
            {
                var innerBoxDecoratorStyle =
                    new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
                innerBoxDecoratorStyle.Add(Border.BackgroundProperty, SharedTokenKey.ColorErrorBgHover);
                hoverStyle.Add(innerBoxDecoratorStyle);
            }
            errorStyle.Add(hoverStyle);

            var focusStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.FocusWithIn));
            {
                var innerBoxDecoratorStyle =
                    new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
                innerBoxDecoratorStyle.Add(Border.BorderBrushProperty, SharedTokenKey.ColorError);
                innerBoxDecoratorStyle.Add(Border.BackgroundProperty, AddOnDecoratedBoxTokenKey.ActiveBg);
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

                innerBoxDecoratorStyle.Add(Border.BorderBrushProperty, SharedTokenKey.ColorTransparent);
                innerBoxDecoratorStyle.Add(Border.BackgroundProperty, SharedTokenKey.ColorWarningBg);
                warningStyle.Add(innerBoxDecoratorStyle);
            }

            var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
            {
                var innerBoxDecoratorStyle =
                    new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
                innerBoxDecoratorStyle.Add(Border.BackgroundProperty, SharedTokenKey.ColorWarningBgHover);
                hoverStyle.Add(innerBoxDecoratorStyle);
            }
            warningStyle.Add(hoverStyle);

            var focusStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.FocusWithIn));
            {
                var innerBoxDecoratorStyle =
                    new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
                innerBoxDecoratorStyle.Add(Border.BorderBrushProperty, SharedTokenKey.ColorWarning);
                innerBoxDecoratorStyle.Add(Border.BackgroundProperty, AddOnDecoratedBoxTokenKey.ActiveBg);
                focusStyle.Add(innerBoxDecoratorStyle);
            }
            warningStyle.Add(focusStyle);

            filledStyle.Add(warningStyle);
        }
        enableStyle.Add(filledStyle);
        Add(enableStyle);
    }

    private void BuildAddOnStyle()
    {
        {
            var errorStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(AddOnDecoratedInnerBox.StatusProperty, AddOnDecoratedStatus.Error));
            {
                var iconStyle = new Style(selector => Selectors.Or(selector.Nesting().Template().Name(LeftAddOnPart),
                    selector.Nesting().Template().Name(RightAddOnPart)).Nesting().Descendant().OfType<Icon>());
                iconStyle.Add(Icon.NormalFilledBrushProperty, SharedTokenKey.ColorError);
                errorStyle.Add(iconStyle);
            }
            Add(errorStyle);
        }

        {
            var warningStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(AddOnDecoratedInnerBox.StatusProperty, AddOnDecoratedStatus.Warning));
            {
                var iconStyle = new Style(selector => Selectors.Or(selector.Nesting().Template().Name(LeftAddOnPart),
                    selector.Nesting().Template().Name(RightAddOnPart)).Nesting().Descendant().OfType<Icon>());
                iconStyle.Add(Icon.NormalFilledBrushProperty, SharedTokenKey.ColorWarning);
                warningStyle.Add(iconStyle);
            }
            Add(warningStyle);
        }
    }

    protected virtual void BuildDisabledStyle()
    {
        var disabledStyle  = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
        var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
        decoratorStyle.Add(Border.BackgroundProperty, SharedTokenKey.ColorBgContainerDisabled);
        disabledStyle.Add(decoratorStyle);
        Add(disabledStyle);
    }
}