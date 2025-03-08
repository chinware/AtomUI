using AtomUI.IconPkg;
using AtomUI.MotionScene;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Transformation;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class ExpanderTheme : BaseControlTheme
{
    public const string FramePart = "PART_Frame";
    public const string MainLayoutPart = "PART_MainLayout";
    public const string ExpandButtonPart = "PART_ExpandButton";
    public const string HeaderLayoutTransformPart = "PART_HeaderLayoutTransform";
    public const string HeaderLayoutPart = "PART_HeaderLayout";
    public const string HeaderPresenterPart = "PART_HeaderPresenter";
    public const string HeaderDecoratorPart = "PART_HeaderDecorator";
    public const string ContentPresenterPart = "PART_ContentPresenter";
    public const string AddOnContentPresenterPart = "PART_AddOnContentPresenter";
    public const string ContentMotionActorPart = "PART_ContentMotionActor";

    public ExpanderTheme() : base(typeof(Expander))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<Expander>((expander, scope) =>
        {
            var frame = new Border
            {
                Name         = FramePart,
                ClipToBounds = true
            };

            CreateTemplateParentBinding(frame, Border.BorderThicknessProperty,
                Expander.EffectiveBorderThicknessProperty);

            var mainLayout = new DockPanel
            {
                Name          = MainLayoutPart,
                LastChildFill = true
            };

            BuildHeader(expander, mainLayout, scope);
            var motionActor = new MotionActorControl
            {
                Name         = ContentMotionActorPart,
                ClipToBounds = true
            };
            var contentPresenter = new ContentPresenter
            {
                Name = ContentPresenterPart
            };
            motionActor.SetCurrentValue(Visual.IsVisibleProperty, false);
            motionActor.Child = contentPresenter;
            
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty,
                ContentControl.ContentProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentTemplateProperty,
                ContentControl.ContentTemplateProperty);

            mainLayout.Children.Add(motionActor);
            motionActor.RegisterInNameScope(scope);
            contentPresenter.RegisterInNameScope(scope);

            frame.Child = mainLayout;
            return frame;
        });
    }

    private void BuildHeader(Expander expander, DockPanel layout, INameScope scope)
    {
        var headerLayoutTransformDecorator = new LayoutTransformControl
        {
            Name = HeaderLayoutTransformPart
        };
        var headerDecorator = new Border
        {
            Name = HeaderDecoratorPart
        };
        headerDecorator.RegisterInNameScope(scope);

        CreateTemplateParentBinding(headerDecorator, Border.BorderThicknessProperty,
            Expander.HeaderBorderThicknessProperty);

        var headerLayout = new Grid
        {
            Name = HeaderLayoutPart,
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Auto),
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto),
                new ColumnDefinition(GridLength.Auto)
            }
        };
        headerLayout.RegisterInNameScope(scope);
        var expandButton = new IconButton
        {
            Name                = ExpandButtonPart,
            VerticalAlignment   = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Left,
        };

        expandButton.RegisterInNameScope(scope);
        CreateTemplateParentBinding(expandButton, IconButton.IconProperty, Expander.ExpandIconProperty);
        CreateTemplateParentBinding(expandButton, Visual.IsVisibleProperty, Expander.IsShowExpandIconProperty);
        CreateTemplateParentBinding(expandButton, InputElement.IsEnabledProperty, InputElement.IsEnabledProperty);
        headerLayout.Children.Add(expandButton);

        var headerPresenter = new ContentPresenter
        {
            Name                       = HeaderPresenterPart,
            HorizontalAlignment        = HorizontalAlignment.Stretch,
            HorizontalContentAlignment = HorizontalAlignment.Left,
            VerticalContentAlignment   = VerticalAlignment.Center,
            VerticalAlignment          = VerticalAlignment.Center,
            Margin                     = new Thickness(0, 1, 0, 0)
        };
        Grid.SetColumn(headerPresenter, 1);
        CreateTemplateParentBinding(headerPresenter, ContentPresenter.ContentProperty,
            HeaderedContentControl.HeaderProperty,
            BindingMode.Default,
            new FuncValueConverter<object?, object?>(
                o =>
                {
                    if (o is string str)
                    {
                        return new TextBlock()
                        {
                            Text              = str,
                            VerticalAlignment = VerticalAlignment.Center
                        };
                    }

                    return o;
                }));
        CreateTemplateParentBinding(headerPresenter, ContentPresenter.ContentTemplateProperty,
            HeaderedContentControl.HeaderTemplateProperty);
        headerPresenter.RegisterInNameScope(scope);
        headerLayout.Children.Add(headerPresenter);

        var addOnContentPresenter = new ContentPresenter
        {
            Name                = AddOnContentPresenterPart,
            VerticalAlignment   = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Left
        };
        Grid.SetColumn(addOnContentPresenter, 2);
        CreateTemplateParentBinding(addOnContentPresenter, ContentPresenter.ContentProperty,
            Expander.AddOnContentProperty);
        CreateTemplateParentBinding(addOnContentPresenter, ContentPresenter.ContentTemplateProperty,
            Expander.AddOnContentTemplateProperty);
        headerLayout.Children.Add(addOnContentPresenter);

        headerDecorator.Child                = headerLayout;
        headerLayoutTransformDecorator.Child = headerDecorator;
        layout.Children.Add(headerLayoutTransformDecorator);
    }

    protected override void BuildStyles()
    {
        BuildCommonStyle();
        BuildExpandButtonPositionStyle();
        BuildExpandDirectionStyle();
        BuildTriggerStyle();
        BuildSizeTypeStyle();
        BuildSelectedStyle();
        BuildBorderlessStyle();
        BuildGhostStyle();
        BuildDisabledStyle();
        BuildAddOnContentStyle();
    }

    private void BuildCommonStyle()
    {
        var commonStyle = new Style(selector => selector.Nesting());
        
        commonStyle.Add(Expander.MotionDurationProperty, SharedTokenKey.MotionDurationSlow);
        
        var frameStyle = new Style(selector => selector.Nesting().Template().Name(FramePart));
        frameStyle.Add(Border.BorderBrushProperty, SharedTokenKey.ColorBorder);
        frameStyle.Add(Border.CornerRadiusProperty, ExpanderTokenKey.ExpanderBorderRadius);
        commonStyle.Add(frameStyle);

        {
            var headerDecoratorStyle = new Style(selector => selector.Nesting().Template().Name(HeaderDecoratorPart));
            headerDecoratorStyle.Add(Border.BackgroundProperty, ExpanderTokenKey.HeaderBg);
            headerDecoratorStyle.Add(Border.BorderBrushProperty, SharedTokenKey.ColorTransparent);
            commonStyle.Add(headerDecoratorStyle);
        }

        var headerPresenter = new Style(selector => selector.Nesting().Template().Name(HeaderPresenterPart));
        headerPresenter.Add(ContentPresenter.ForegroundProperty, SharedTokenKey.ColorTextHeading);
        commonStyle.Add(headerPresenter);

        // ExpandIcon 
        var expandButtonStyle = new Style(selector => selector.Nesting().Template().Name(ExpandButtonPart));
        expandButtonStyle.Add(IconButton.IconWidthProperty, SharedTokenKey.IconSizeSM);
        expandButtonStyle.Add(IconButton.IconHeightProperty, SharedTokenKey.IconSizeSM);
        expandButtonStyle.Add(IconButton.DisabledIconColorProperty, SharedTokenKey.ColorTextDisabled);
        commonStyle.Add(expandButtonStyle);

        {
            // 打开关闭指示按钮的动画
            var isMotionEnabledStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(Expander.IsMotionEnabledProperty, true));
            var expandIconButtonStyle = new Style(selector => selector.Nesting().Template().Name(ExpandButtonPart));
            expandIconButtonStyle.Add(IconButton.TransitionsProperty, new SetterValueFactory<Transitions>(() =>
                new Transitions
                {
                    AnimationUtils.CreateTransition<TransformOperationsTransition>(Visual.RenderTransformProperty)
                }));
            isMotionEnabledStyle.Add(expandIconButtonStyle);
            commonStyle.Add(isMotionEnabledStyle);
        }

        // 设置打开状态
        var expandedStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(Avalonia.Controls.Expander.IsExpandedProperty, true));
        {
            var headerDecoratorStyle = new Style(selector => selector.Nesting().Template().Name(HeaderDecoratorPart));
            headerDecoratorStyle.Add(Border.BorderBrushProperty, SharedTokenKey.ColorBorder);
            expandedStyle.Add(headerDecoratorStyle);
        }
        commonStyle.Add(expandedStyle);

        Add(commonStyle);
    }

    private void BuildExpandButtonPositionStyle()
    {
        // 设置展开按钮的样式
        var startPositionStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(Expander.ExpandIconPositionProperty, ExpanderIconPosition.Start));
        {
            var expandButtonStyle = new Style(selector => selector.Nesting().Template().Name(ExpandButtonPart));
            expandButtonStyle.Add(Grid.ColumnProperty, 0);
            expandButtonStyle.Add(Layoutable.MarginProperty, ExpanderTokenKey.LeftExpandButtonHMargin);
            startPositionStyle.Add(expandButtonStyle);
        }
        Add(startPositionStyle);
        var endPositionStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(Expander.ExpandIconPositionProperty, ExpanderIconPosition.End));
        {
            var expandButtonStyle = new Style(selector => selector.Nesting().Template().Name(ExpandButtonPart));
            expandButtonStyle.Add(Grid.ColumnProperty, 3);
            expandButtonStyle.Add(Layoutable.MarginProperty, ExpanderTokenKey.RightExpandButtonHMargin);
            endPositionStyle.Add(expandButtonStyle);
        }
        Add(endPositionStyle);
    }

    private void BuildExpandDirectionStyle()
    {
        var upStyle = new Style(selector => selector.Nesting().Class(Expander.ExpandUpPC));

        // 设置水平和垂直对齐
        upStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
        upStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Bottom);

        {
            var headerLayoutTransformStyle =
                new Style(selector => selector.Nesting().Template().Name(HeaderLayoutTransformPart));
            headerLayoutTransformStyle.Add(DockPanel.DockProperty, Dock.Bottom);
            upStyle.Add(headerLayoutTransformStyle);
        }
        Add(upStyle);

        var downStyle = new Style(selector => selector.Nesting().Class(Expander.ExpandDownPC));
        // 设置水平和垂直对齐
        downStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
        downStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Top);

        {
            var headerLayoutTransformStyle =
                new Style(selector => selector.Nesting().Template().Name(HeaderLayoutTransformPart));
            headerLayoutTransformStyle.Add(DockPanel.DockProperty, Dock.Top);
            downStyle.Add(headerLayoutTransformStyle);
        }
        Add(downStyle);

        var leftStyle = new Style(selector => selector.Nesting().Class(Expander.ExpandLeftPC));
        // 设置水平和垂直对齐
        leftStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Right);
        leftStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch);

        {
            var headerLayoutTransformStyle =
                new Style(selector => selector.Nesting().Template().Name(HeaderLayoutTransformPart));
            headerLayoutTransformStyle.Add(DockPanel.DockProperty, Dock.Right);
            headerLayoutTransformStyle.Add(LayoutTransformControl.LayoutTransformProperty,
                new SetterValueFactory<ITransform>(() => new RotateTransform(90)));
            leftStyle.Add(headerLayoutTransformStyle);
        }
        Add(leftStyle);

        var rightStyle = new Style(selector => selector.Nesting().Class(Expander.ExpandRightPC));
        // 设置水平和垂直对齐
        rightStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Left);

        rightStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch);
        {
            var headerLayoutTransformStyle =
                new Style(selector => selector.Nesting().Template().Name(HeaderLayoutTransformPart));
            headerLayoutTransformStyle.Add(LayoutTransformControl.LayoutTransformProperty,
                new SetterValueFactory<ITransform>(() => new RotateTransform(90)));
            headerLayoutTransformStyle.Add(DockPanel.DockProperty, Dock.Left);
            rightStyle.Add(headerLayoutTransformStyle);
        }
        Add(rightStyle);
    }

    private void BuildSelectedStyle()
    {
        var selectedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Expanded));
        var upStyle       = new Style(selector => selector.Nesting().Class(Expander.ExpandUpPC));

        {
            // Expand Button
            var expandButtonStyle = new Style(selector => selector.Nesting().Template().Name(ExpandButtonPart));

            expandButtonStyle.Add(Visual.RenderTransformProperty, new SetterValueFactory<ITransform>(() =>
            {
                var transformOptions = new TransformOperations.Builder(1);
                transformOptions.AppendRotate(MathUtils.Deg2Rad(-90));
                return transformOptions.Build();
            }));

            upStyle.Add(expandButtonStyle);
        }

        selectedStyle.Add(upStyle);

        var downStyle = new Style(selector => selector.Nesting().Class(Expander.ExpandDownPC));
        {
            // Expand Button
            var expandButtonStyle = new Style(selector => selector.Nesting().Template().Name(ExpandButtonPart));

            expandButtonStyle.Add(Visual.RenderTransformProperty, new SetterValueFactory<ITransform>(() =>
            {
                var transformOptions = new TransformOperations.Builder(1);
                transformOptions.AppendRotate(MathUtils.Deg2Rad(90));
                return transformOptions.Build();
            }));

            downStyle.Add(expandButtonStyle);
        }

        selectedStyle.Add(downStyle);

        var leftStyle = new Style(selector => selector.Nesting().Class(Expander.ExpandLeftPC));
        {
            // Expand Button
            var expandButtonStyle = new Style(selector => selector.Nesting().Template().Name(ExpandButtonPart));

            expandButtonStyle.Add(Visual.RenderTransformProperty, new SetterValueFactory<ITransform>(() =>
            {
                var transformOptions = new TransformOperations.Builder(1);
                transformOptions.AppendRotate(MathUtils.Deg2Rad(90));
                return transformOptions.Build();
            }));

            leftStyle.Add(expandButtonStyle);
        }
        selectedStyle.Add(leftStyle);

        var rightStyle = new Style(selector => selector.Nesting().Class(Expander.ExpandRightPC));
        {
            // Expand Button
            var expandButtonStyle = new Style(selector => selector.Nesting().Template().Name(ExpandButtonPart));

            expandButtonStyle.Add(Visual.RenderTransformProperty, new SetterValueFactory<ITransform>(() =>
            {
                var transformOptions = new TransformOperations.Builder(1);
                transformOptions.AppendRotate(MathUtils.Deg2Rad(-90));
                return transformOptions.Build();
            }));

            rightStyle.Add(expandButtonStyle);
        }
        selectedStyle.Add(rightStyle);

        Add(selectedStyle);
    }

    private void BuildSizeTypeStyle()
    {
        {
            var contentPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ContentPresenterPart));
            contentPresenterStyle.Add(ContentPresenter.BorderBrushProperty, SharedTokenKey.ColorBorder);
            Add(contentPresenterStyle);
        }
        var largeSizeStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(Expander.SizeTypeProperty, SizeType.Large));
        {
            var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(HeaderDecoratorPart));
            decoratorStyle.Add(Decorator.PaddingProperty, ExpanderTokenKey.HeaderPaddingLG);
            decoratorStyle.Add(TextElement.FontSizeProperty, SharedTokenKey.FontSizeLG);
            decoratorStyle.Add(TextBlock.LineHeightProperty, SharedTokenKey.FontHeightLG);
            largeSizeStyle.Add(decoratorStyle);
        }

        {
            var contentPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ContentPresenterPart));
            contentPresenterStyle.Add(ContentPresenter.PaddingProperty, ExpanderTokenKey.ContentPaddingLG);
            largeSizeStyle.Add(contentPresenterStyle);
        }

        Add(largeSizeStyle);

        var middleSizeStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(Expander.SizeTypeProperty, SizeType.Middle));
        {
            var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(HeaderDecoratorPart));
            decoratorStyle.Add(Decorator.PaddingProperty, ExpanderTokenKey.HeaderPadding);
            decoratorStyle.Add(TextElement.FontSizeProperty, SharedTokenKey.FontSize);
            decoratorStyle.Add(TextBlock.LineHeightProperty, SharedTokenKey.FontHeight);
            middleSizeStyle.Add(decoratorStyle);
        }

        {
            var contentPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ContentPresenterPart));
            contentPresenterStyle.Add(ContentPresenter.PaddingProperty, ExpanderTokenKey.ContentPadding);
            middleSizeStyle.Add(contentPresenterStyle);
        }
        Add(middleSizeStyle);

        var smallSizeStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(Expander.SizeTypeProperty, SizeType.Small));
        {
            var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(HeaderDecoratorPart));
            decoratorStyle.Add(Decorator.PaddingProperty, ExpanderTokenKey.HeaderPaddingSM);
            decoratorStyle.Add(TextElement.FontSizeProperty, SharedTokenKey.FontSize);
            decoratorStyle.Add(TextBlock.LineHeightProperty, SharedTokenKey.FontHeight);
            smallSizeStyle.Add(decoratorStyle);
        }

        {
            var contentPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ContentPresenterPart));
            contentPresenterStyle.Add(ContentPresenter.PaddingProperty, ExpanderTokenKey.ContentPaddingSM);
            smallSizeStyle.Add(contentPresenterStyle);
        }

        Add(smallSizeStyle);
    }

    private void BuildTriggerStyle()
    {
        var headerTriggerHandleStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(Expander.TriggerTypeProperty, ExpanderTriggerType.Header));
        var headerDecoratorStyle = new Style(selector => selector.Nesting().Template().Name(HeaderDecoratorPart));
        headerDecoratorStyle.Add(InputElement.CursorProperty,
            new SetterValueFactory<Cursor>(() => new Cursor(StandardCursorType.Hand)));
        headerTriggerHandleStyle.Add(headerDecoratorStyle);
        Add(headerTriggerHandleStyle);

        var iconTriggerHandleStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(Expander.TriggerTypeProperty, ExpanderTriggerType.Icon));
        var expandIconStyle = new Style(selector => selector.Nesting().Template().Name(ExpandButtonPart));
        expandIconStyle.Add(InputElement.CursorProperty,
            new SetterValueFactory<Cursor>(() => new Cursor(StandardCursorType.Hand)));
        iconTriggerHandleStyle.Add(expandIconStyle);
        Add(iconTriggerHandleStyle);
    }

    private void BuildBorderlessStyle()
    {
        var borderlessStyle =
            new Style(selector => selector.Nesting().PropertyEquals(Expander.IsBorderlessProperty, true));
        var contentPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ContentPresenterPart));
        contentPresenterStyle.Add(ContentPresenter.BackgroundProperty, CollapseTokenKey.HeaderBg);
        borderlessStyle.Add(contentPresenterStyle);
        Add(borderlessStyle);
    }

    private void BuildGhostStyle()
    {
        var ghostStyle =
            new Style(selector => selector.Nesting().PropertyEquals(Expander.IsGhostStyleProperty, true));
        var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(HeaderDecoratorPart));
        decoratorStyle.Add(Border.BackgroundProperty, ExpanderTokenKey.ContentBg);
        ghostStyle.Add(decoratorStyle);
        Add(ghostStyle);
    }

    private void BuildDisabledStyle()
    {
        var disabledStyle =
            new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
        disabledStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorTextDisabled);
        var headerContentPresenterStyle = new Style(selector =>
            selector.Nesting().Template().Name(HeaderDecoratorPart).Descendant().OfType<ContentPresenter>());
        headerContentPresenterStyle.Add(ContentPresenter.ForegroundProperty, SharedTokenKey.ColorTextDisabled);
        disabledStyle.Add(headerContentPresenterStyle);

        Add(disabledStyle);
    }

    private void BuildAddOnContentStyle()
    {
        var addOnContentStyle = new Style(selector =>
            selector.Nesting().Template().Name(AddOnContentPresenterPart).Descendant().OfType<Icon>());
        addOnContentStyle.Add(Layoutable.WidthProperty, SharedTokenKey.IconSize);
        addOnContentStyle.Add(Layoutable.HeightProperty, SharedTokenKey.IconSize);
        Add(addOnContentStyle);
    }
}