using AtomUI.IconPkg;
using AtomUI.MotionScene;
using AtomUI.Theme;
using AtomUI.Theme.Data;
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
internal class CollapseItemTheme : BaseControlTheme
{
    public const string MainLayoutPart = "PART_MainLayout";
    public const string ExpandButtonPart = "PART_ExpandButton";
    public const string HeaderPresenterPart = "PART_HeaderPresenter";
    public const string HeaderDecoratorPart = "PART_HeaderDecorator";
    public const string ContentPresenterPart = "PART_ContentPresenter";
    public const string AddOnContentPresenterPart = "PART_AddOnContentPresenter";
    public const string ContentMotionActorPart = "PART_ContentMotionActor";

    public CollapseItemTheme() : base(typeof(CollapseItem))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<CollapseItem>((collapseItem, scope) =>
        {
            var mainLayout = new DockPanel
            {
                Name          = MainLayoutPart,
                LastChildFill = true
            };

            BuildHeader(mainLayout, scope);
            var motionActor = new MotionActorControl()
            {
                Name = ContentMotionActorPart
            };
            motionActor.SetCurrentValue(Visual.IsVisibleProperty, false);
            var contentPresenter = new ContentPresenter
            {
                Name = ContentPresenterPart
            };
            motionActor.Child = contentPresenter;
            TokenResourceBinder.CreateTokenBinding(contentPresenter, ContentPresenter.BorderBrushProperty,
                SharedTokenKey.ColorBorder);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty,
                ContentControl.ContentProperty,
                BindingMode.Default, new FuncValueConverter<object?, object?>(
                    o =>
                    {
                        if (o is string str)
                        {
                            return new SingleLineText()
                            {
                                Text              = str,
                                VerticalAlignment = VerticalAlignment.Center
                            };
                        }

                        return o;
                    }));
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentTemplateProperty,
                ContentControl.ContentTemplateProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.BorderThicknessProperty,
                CollapseItem.ContentBorderThicknessProperty);
            mainLayout.Children.Add(motionActor);
            motionActor.RegisterInNameScope(scope);
            contentPresenter.RegisterInNameScope(scope);
            return mainLayout;
        });
    }

    private void BuildHeader(DockPanel layout, INameScope scope)
    {
        var headerDecorator = new Border
        {
            Name = HeaderDecoratorPart
        };
        headerDecorator.RegisterInNameScope(scope);
        DockPanel.SetDock(headerDecorator, Dock.Top);

        TokenResourceBinder.CreateTokenBinding(headerDecorator, Border.BorderBrushProperty,
            SharedTokenKey.ColorBorder);
        CreateTemplateParentBinding(headerDecorator, Border.BorderThicknessProperty,
            CollapseItem.HeaderBorderThicknessProperty);

        var headerLayout = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions
            {
                new(GridLength.Auto),
                new(GridLength.Star),
                new(GridLength.Auto),
                new(GridLength.Auto)
            }
        };

        var expandButton = new IconButton
        {
            Name              = ExpandButtonPart,
            VerticalAlignment = VerticalAlignment.Center
        };
        TokenResourceBinder.CreateTokenBinding(expandButton, IconButton.IconWidthProperty,
            SharedTokenKey.IconSize);
        TokenResourceBinder.CreateTokenBinding(expandButton, IconButton.IconHeightProperty,
            SharedTokenKey.IconSize);
        expandButton.RegisterInNameScope(scope);
        CreateTemplateParentBinding(expandButton, IconButton.IconProperty, CollapseItem.ExpandIconProperty);
        CreateTemplateParentBinding(expandButton, Visual.IsVisibleProperty, CollapseItem.IsShowExpandIconProperty);
        CreateTemplateParentBinding(expandButton, InputElement.IsEnabledProperty, InputElement.IsEnabledProperty);

        headerLayout.Children.Add(expandButton);

        var headerPresenter = new ContentPresenter
        {
            Name                       = HeaderPresenterPart,
            HorizontalAlignment        = HorizontalAlignment.Stretch,
            HorizontalContentAlignment = HorizontalAlignment.Left,
            VerticalContentAlignment = VerticalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(0, 1, 0, 0)
        };
        CreateTemplateParentBinding(headerPresenter, ContentPresenter.ContentProperty,
            HeaderedContentControl.HeaderProperty);
        CreateTemplateParentBinding(headerPresenter, ContentPresenter.ContentTemplateProperty,
            HeaderedContentControl.HeaderTemplateProperty);
        Grid.SetColumn(headerPresenter, 1);
        headerPresenter.RegisterInNameScope(scope);
        headerLayout.Children.Add(headerPresenter);

        var addContentPresenter = new ContentPresenter
        {
            Name                = AddOnContentPresenterPart,
            VerticalAlignment   = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Left
        };
        CreateTemplateParentBinding(addContentPresenter, ContentPresenter.ContentProperty,
            CollapseItem.AddOnContentProperty);
        CreateTemplateParentBinding(addContentPresenter, ContentPresenter.ContentTemplateProperty,
            CollapseItem.AddOnContentTemplateProperty);
        Grid.SetColumn(addContentPresenter, 2);
        headerLayout.Children.Add(addContentPresenter);

        headerDecorator.Child = headerLayout;
        layout.Children.Add(headerDecorator);
    }

    protected override void BuildStyles()
    {
        BuildCommonStyle();
        BuildTriggerStyle();
        BuildTriggerPositionStyle();
        BuildSizeTypeStyle();
        BuildSelectedStyle();
        BuildBorderlessStyle();
        BuildGhostStyle();
        BuildDisabledStyle();
        BuildAddOnContentStyle();
    }

    private void BuildCommonStyle()
    {
        var commonStyle    = new Style(selector => selector.Nesting());
        {
            var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(HeaderDecoratorPart));
            decoratorStyle.Add(Border.BackgroundProperty, CollapseTokenKey.HeaderBg);
            commonStyle.Add(decoratorStyle);
        }

        var headerPresenter = new Style(selector => selector.Nesting().Template().Name(HeaderPresenterPart));
        headerPresenter.Add(ContentPresenter.ForegroundProperty, SharedTokenKey.ColorTextHeading);
        commonStyle.Add(headerPresenter);

        {
            // ExpandIcon 
            var expandIconStyle =
                new Style(selector => selector.Nesting().Template().Name(ExpandButtonPart).Descendant().OfType<Icon>());
            expandIconStyle.Add(Layoutable.WidthProperty, SharedTokenKey.IconSizeSM);
            expandIconStyle.Add(Layoutable.HeightProperty, SharedTokenKey.IconSizeSM);
            commonStyle.Add(expandIconStyle);
        }
        
        // 动画相关
        var isMotionEnabledStyle = new Style(selector => selector.Nesting().PropertyEquals(CollapseItem.IsMotionEnabledProperty, true));
        {
            var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(HeaderDecoratorPart));
            decoratorStyle.Add(Border.TransitionsProperty, new SetterValueFactory<Transitions>(() => new Transitions
            {
                AnimationUtils.CreateTransition<ThicknessTransition>(Border.BorderThicknessProperty)
            }));
            isMotionEnabledStyle.Add(decoratorStyle);
            
            var expandIconStyle =
                new Style(selector => selector.Nesting().Template().Name(ExpandButtonPart));
            expandIconStyle.Add(IconButton.TransitionsProperty, new SetterValueFactory<Transitions>(() => new Transitions()
            {
                AnimationUtils.CreateTransition<TransformOperationsTransition>(Visual.RenderTransformProperty)
            }));
            isMotionEnabledStyle.Add(expandIconStyle);
        }
        commonStyle.Add(isMotionEnabledStyle);

        Add(commonStyle);
    }

    private void BuildSelectedStyle()
    {
        var selectedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Selected));
        // Expand Button
        var expandButtonStyle = new Style(selector => selector.Nesting().Template().Name(ExpandButtonPart));
        
        expandButtonStyle.Add(Visual.RenderTransformProperty, new SetterValueFactory<ITransform>(() =>
        {
            var transformOptions  = new TransformOperations.Builder(1);
            transformOptions.AppendRotate(MathUtils.Deg2Rad(90));
            return transformOptions.Build();
        }));

        selectedStyle.Add(expandButtonStyle);
        Add(selectedStyle);
    }

    private void BuildSizeTypeStyle()
    {
        var largeSizeStyle =
            new Style(selector => selector.Nesting().PropertyEquals(CollapseItem.SizeTypeProperty, SizeType.Large));
        {
            var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(HeaderDecoratorPart));
            decoratorStyle.Add(Decorator.PaddingProperty, CollapseTokenKey.CollapseHeaderPaddingLG);
            decoratorStyle.Add(TextElement.FontSizeProperty, SharedTokenKey.FontSizeLG);
            decoratorStyle.Add(SingleLineText.LineHeightProperty, SharedTokenKey.FontHeightLG);
            largeSizeStyle.Add(decoratorStyle);
        }

        {
            var contentPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ContentPresenterPart));
            contentPresenterStyle.Add(ContentPresenter.PaddingProperty,
                CollapseTokenKey.CollapseContentPaddingLG);
            largeSizeStyle.Add(contentPresenterStyle);
        }

        Add(largeSizeStyle);

        var middleSizeStyle =
            new Style(selector => selector.Nesting().PropertyEquals(CollapseItem.SizeTypeProperty, SizeType.Middle));
        {
            var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(HeaderDecoratorPart));
            decoratorStyle.Add(Decorator.PaddingProperty, CollapseTokenKey.HeaderPadding);
            decoratorStyle.Add(TextElement.FontSizeProperty, SharedTokenKey.FontSize);
            decoratorStyle.Add(SingleLineText.LineHeightProperty, SharedTokenKey.FontHeight);
            middleSizeStyle.Add(decoratorStyle);
        }

        {
            var contentPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ContentPresenterPart));
            contentPresenterStyle.Add(ContentPresenter.PaddingProperty, CollapseTokenKey.ContentPadding);
            middleSizeStyle.Add(contentPresenterStyle);
        }
        Add(middleSizeStyle);

        var smallSizeStyle =
            new Style(selector => selector.Nesting().PropertyEquals(CollapseItem.SizeTypeProperty, SizeType.Small));
        {
            var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(HeaderDecoratorPart));
            decoratorStyle.Add(Decorator.PaddingProperty, CollapseTokenKey.CollapseHeaderPaddingSM);
            decoratorStyle.Add(TextElement.FontSizeProperty, SharedTokenKey.FontSize);
            decoratorStyle.Add(TextBlock.LineHeightProperty, SharedTokenKey.FontHeight);
            smallSizeStyle.Add(decoratorStyle);
        }

        {
            var contentPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ContentPresenterPart));
            contentPresenterStyle.Add(ContentPresenter.PaddingProperty,
                CollapseTokenKey.CollapseContentPaddingSM);
            smallSizeStyle.Add(contentPresenterStyle);
        }

        Add(smallSizeStyle);
    }

    private void BuildTriggerStyle()
    {
        var headerTriggerHandleStyle = new Style(selector => selector.Nesting()
                                                                     .PropertyEquals(
                                                                         CollapseItem.TriggerTypeProperty,
                                                                         CollapseTriggerType.Header));
        var headerDecoratorStyle = new Style(selector => selector.Nesting().Template().Name(HeaderDecoratorPart));
        headerDecoratorStyle.Add(InputElement.CursorProperty, new SetterValueFactory<Cursor>(() => new Cursor(StandardCursorType.Hand)));
        headerTriggerHandleStyle.Add(headerDecoratorStyle);
        Add(headerTriggerHandleStyle);

        var iconTriggerHandleStyle =
            new Style(selector => selector.Nesting()
                                          .PropertyEquals(CollapseItem.TriggerTypeProperty, CollapseTriggerType.Icon));
        var expandIconStyle = new Style(selector => selector.Nesting().Template().Name(ExpandButtonPart));
        expandIconStyle.Add(InputElement.CursorProperty, new SetterValueFactory<Cursor>(() => new Cursor(StandardCursorType.Hand)));
        iconTriggerHandleStyle.Add(expandIconStyle);
        Add(iconTriggerHandleStyle);
    }

    private void BuildTriggerPositionStyle()
    {
        var startPositionStyle = new Style(selector => selector.Nesting()
                                                               .PropertyEquals(
                                                                   CollapseItem.ExpandIconPositionProperty,
                                                                   CollapseExpandIconPosition.Start));
        {
            var expandButtonStyle = new Style(selector => selector.Nesting().Template().Name(ExpandButtonPart));
            expandButtonStyle.Add(Grid.ColumnProperty, 0);
            expandButtonStyle.Add(Layoutable.MarginProperty, CollapseTokenKey.LeftExpandButtonMargin);
            startPositionStyle.Add(expandButtonStyle);
        }
        Add(startPositionStyle);
        var endPositionStyle = new Style(selector => selector.Nesting()
                                                             .PropertyEquals(
                                                                 CollapseItem.ExpandIconPositionProperty,
                                                                 CollapseExpandIconPosition.End));
        {
            var expandButtonStyle = new Style(selector => selector.Nesting().Template().Name(ExpandButtonPart));
            expandButtonStyle.Add(Grid.ColumnProperty, 3);
            expandButtonStyle.Add(Layoutable.MarginProperty, CollapseTokenKey.RightExpandButtonMargin);
            endPositionStyle.Add(expandButtonStyle);
        }
        Add(endPositionStyle);
    }

    private void BuildBorderlessStyle()
    {
        var borderlessStyle =
            new Style(selector => selector.Nesting().PropertyEquals(CollapseItem.IsBorderlessProperty, true));
        var contentPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ContentPresenterPart));
        contentPresenterStyle.Add(ContentPresenter.BackgroundProperty, CollapseTokenKey.HeaderBg);
        borderlessStyle.Add(contentPresenterStyle);
        Add(borderlessStyle);
    }

    private void BuildGhostStyle()
    {
        var ghostStyle =
            new Style(selector => selector.Nesting().PropertyEquals(CollapseItem.IsGhostStyleProperty, true));
        var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(HeaderDecoratorPart));
        decoratorStyle.Add(Border.BackgroundProperty, CollapseTokenKey.ContentBg);
        ghostStyle.Add(decoratorStyle);
        Add(ghostStyle);
    }

    private void BuildDisabledStyle()
    {
        var disabledStyle =
            new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
        disabledStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorTextDisabled);
        var headerContentPresenterStyle = new Style(selector => selector.Nesting().Template().Name(HeaderDecoratorPart)
                                                                        .Descendant().OfType<ContentPresenter>());
        headerContentPresenterStyle.Add(ContentPresenter.ForegroundProperty, SharedTokenKey.ColorTextDisabled);
        disabledStyle.Add(headerContentPresenterStyle);

        Add(disabledStyle);
    }

    private void BuildAddOnContentStyle()
    {
        var addOnContentStyle = new Style(selector => selector.Nesting().Template().Name(AddOnContentPresenterPart)
                                                              .Descendant().OfType<Icon>());
        addOnContentStyle.Add(Layoutable.WidthProperty, SharedTokenKey.IconSize);
        addOnContentStyle.Add(Layoutable.HeightProperty, SharedTokenKey.IconSize);
        Add(addOnContentStyle);
    }
}