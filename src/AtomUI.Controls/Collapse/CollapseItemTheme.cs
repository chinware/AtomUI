using AtomUI.MotionScene;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using AtomUI.Theme.Utils;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Layout;
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
    public const string ContentAnimationTargetPart = "PART_ContentAnimationTarget";

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
            var animationPanel = new AnimationTargetPanel
            {
                Name = ContentAnimationTargetPart
            };
            animationPanel.SetCurrentValue(Visual.IsVisibleProperty, false);
            var contentPresenter = new ContentPresenter
            {
                Name = ContentPresenterPart
            };
            animationPanel.Children.Add(contentPresenter);
            TokenResourceBinder.CreateGlobalTokenBinding(contentPresenter, ContentPresenter.BorderBrushProperty,
                GlobalTokenResourceKey.ColorBorder);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty,
                ContentControl.ContentProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentTemplateProperty,
                ContentControl.ContentTemplateProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.BorderThicknessProperty,
                CollapseItem.ContentBorderThicknessProperty);
            mainLayout.Children.Add(animationPanel);
            animationPanel.RegisterInNameScope(scope);
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
        headerDecorator.Transitions = new Transitions
        {
            AnimationUtils.CreateTransition<ThicknessTransition>(Border.BorderThicknessProperty)
        };
        DockPanel.SetDock(headerDecorator, Dock.Top);

        TokenResourceBinder.CreateGlobalTokenBinding(headerDecorator, Border.BorderBrushProperty,
            GlobalTokenResourceKey.ColorBorder);
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
        TokenResourceBinder.CreateGlobalTokenBinding(expandButton, IconButton.IconWidthProperty,
            GlobalTokenResourceKey.IconSize);
        TokenResourceBinder.CreateGlobalTokenBinding(expandButton, IconButton.IconHeightProperty,
            GlobalTokenResourceKey.IconSize);
        expandButton.RegisterInNameScope(scope);
        expandButton.Transitions = new Transitions();
        expandButton.Transitions.Add(
            AnimationUtils.CreateTransition<TransformOperationsTransition>(Visual.RenderTransformProperty));
        CreateTemplateParentBinding(expandButton, IconButton.IconProperty, CollapseItem.ExpandIconProperty);
        CreateTemplateParentBinding(expandButton, Visual.IsVisibleProperty, CollapseItem.IsShowExpandIconProperty);
        CreateTemplateParentBinding(expandButton, InputElement.IsEnabledProperty, InputElement.IsEnabledProperty);

        headerLayout.Children.Add(expandButton);

        var headerPresenter = new ContentPresenter
        {
            Name                       = HeaderPresenterPart,
            HorizontalAlignment        = HorizontalAlignment.Stretch,
            HorizontalContentAlignment = HorizontalAlignment.Left
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
        var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(HeaderDecoratorPart));
        decoratorStyle.Add(Border.BackgroundProperty, CollapseTokenResourceKey.HeaderBg);
        commonStyle.Add(decoratorStyle);

        var headerPresenter = new Style(selector => selector.Nesting().Template().Name(HeaderPresenterPart));
        headerPresenter.Add(ContentPresenter.ForegroundProperty, GlobalTokenResourceKey.ColorTextHeading);
        commonStyle.Add(headerPresenter);

        // ExpandIcon 
        var expandIconStyle =
            new Style(selector => selector.Nesting().Template().Name(ExpandButtonPart).Descendant().OfType<PathIcon>());
        expandIconStyle.Add(Layoutable.WidthProperty, GlobalTokenResourceKey.IconSizeSM);
        expandIconStyle.Add(Layoutable.HeightProperty, GlobalTokenResourceKey.IconSizeSM);
        commonStyle.Add(expandIconStyle);

        Add(commonStyle);
    }

    private void BuildSelectedStyle()
    {
        var selectedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Selected));
        // Expand Button
        var expandButtonStyle = new Style(selector => selector.Nesting().Template().Name(ExpandButtonPart));
        var transformOptions  = new TransformOperations.Builder(1);
        transformOptions.AppendRotate(MathUtils.Deg2Rad(90));
        expandButtonStyle.Add(Visual.RenderTransformProperty, transformOptions.Build());

        selectedStyle.Add(expandButtonStyle);
        Add(selectedStyle);
    }

    private void BuildSizeTypeStyle()
    {
        var largeSizeStyle =
            new Style(selector => selector.Nesting().PropertyEquals(CollapseItem.SizeTypeProperty, SizeType.Large));
        {
            var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(HeaderDecoratorPart));
            decoratorStyle.Add(Decorator.PaddingProperty, CollapseTokenResourceKey.CollapseHeaderPaddingLG);
            decoratorStyle.Add(TextElement.FontSizeProperty, GlobalTokenResourceKey.FontSizeLG);
            decoratorStyle.Add(TextBlock.LineHeightProperty, GlobalTokenResourceKey.FontHeightLG);
            largeSizeStyle.Add(decoratorStyle);
        }

        {
            var contentPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ContentPresenterPart));
            contentPresenterStyle.Add(ContentPresenter.PaddingProperty,
                CollapseTokenResourceKey.CollapseContentPaddingLG);
            largeSizeStyle.Add(contentPresenterStyle);
        }

        Add(largeSizeStyle);

        var middleSizeStyle =
            new Style(selector => selector.Nesting().PropertyEquals(CollapseItem.SizeTypeProperty, SizeType.Middle));
        {
            var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(HeaderDecoratorPart));
            decoratorStyle.Add(Decorator.PaddingProperty, CollapseTokenResourceKey.HeaderPadding);
            decoratorStyle.Add(TextElement.FontSizeProperty, GlobalTokenResourceKey.FontSize);
            decoratorStyle.Add(TextBlock.LineHeightProperty, GlobalTokenResourceKey.FontHeight);
            middleSizeStyle.Add(decoratorStyle);
        }

        {
            var contentPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ContentPresenterPart));
            contentPresenterStyle.Add(ContentPresenter.PaddingProperty, CollapseTokenResourceKey.ContentPadding);
            middleSizeStyle.Add(contentPresenterStyle);
        }
        Add(middleSizeStyle);

        var smallSizeStyle =
            new Style(selector => selector.Nesting().PropertyEquals(CollapseItem.SizeTypeProperty, SizeType.Small));
        {
            var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(HeaderDecoratorPart));
            decoratorStyle.Add(Decorator.PaddingProperty, CollapseTokenResourceKey.CollapseHeaderPaddingSM);
            decoratorStyle.Add(TextElement.FontSizeProperty, GlobalTokenResourceKey.FontSize);
            decoratorStyle.Add(TextBlock.LineHeightProperty, GlobalTokenResourceKey.FontHeight);
            smallSizeStyle.Add(decoratorStyle);
        }

        {
            var contentPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ContentPresenterPart));
            contentPresenterStyle.Add(ContentPresenter.PaddingProperty,
                CollapseTokenResourceKey.CollapseContentPaddingSM);
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
        headerDecoratorStyle.Add(InputElement.CursorProperty, new Cursor(StandardCursorType.Hand));
        headerTriggerHandleStyle.Add(headerDecoratorStyle);
        Add(headerTriggerHandleStyle);

        var iconTriggerHandleStyle =
            new Style(selector => selector.Nesting()
                                          .PropertyEquals(CollapseItem.TriggerTypeProperty, CollapseTriggerType.Icon));
        var expandIconStyle = new Style(selector => selector.Nesting().Template().Name(ExpandButtonPart));
        expandIconStyle.Add(InputElement.CursorProperty, new Cursor(StandardCursorType.Hand));
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
            expandButtonStyle.Add(Layoutable.MarginProperty, CollapseTokenResourceKey.LeftExpandButtonMargin);
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
            expandButtonStyle.Add(Layoutable.MarginProperty, CollapseTokenResourceKey.RightExpandButtonMargin);
            endPositionStyle.Add(expandButtonStyle);
        }
        Add(endPositionStyle);
    }

    private void BuildBorderlessStyle()
    {
        var borderlessStyle =
            new Style(selector => selector.Nesting().PropertyEquals(CollapseItem.IsBorderlessProperty, true));
        var contentPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ContentPresenterPart));
        contentPresenterStyle.Add(ContentPresenter.BackgroundProperty, CollapseTokenResourceKey.HeaderBg);
        borderlessStyle.Add(contentPresenterStyle);
        Add(borderlessStyle);
    }

    private void BuildGhostStyle()
    {
        var ghostStyle =
            new Style(selector => selector.Nesting().PropertyEquals(CollapseItem.IsGhostStyleProperty, true));
        var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(HeaderDecoratorPart));
        decoratorStyle.Add(Border.BackgroundProperty, CollapseTokenResourceKey.ContentBg);
        ghostStyle.Add(decoratorStyle);
        Add(ghostStyle);
    }

    private void BuildDisabledStyle()
    {
        var disabledStyle =
            new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
        disabledStyle.Add(TemplatedControl.ForegroundProperty, GlobalTokenResourceKey.ColorTextDisabled);
        var headerContentPresenterStyle = new Style(selector => selector.Nesting().Template().Name(HeaderDecoratorPart)
                                                                        .Descendant().OfType<ContentPresenter>());
        headerContentPresenterStyle.Add(ContentPresenter.ForegroundProperty, GlobalTokenResourceKey.ColorTextDisabled);
        disabledStyle.Add(headerContentPresenterStyle);

        Add(disabledStyle);
    }

    private void BuildAddOnContentStyle()
    {
        var addOnContentStyle = new Style(selector => selector.Nesting().Template().Name(AddOnContentPresenterPart)
                                                              .Descendant().OfType<PathIcon>());
        addOnContentStyle.Add(Layoutable.WidthProperty, GlobalTokenResourceKey.IconSize);
        addOnContentStyle.Add(Layoutable.HeightProperty, GlobalTokenResourceKey.IconSize);
        Add(addOnContentStyle);
    }
}