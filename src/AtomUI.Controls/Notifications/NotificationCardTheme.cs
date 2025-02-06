using AtomUI.IconPkg;
using AtomUI.IconPkg.AntDesign;
using AtomUI.MotionScene;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class NotificationCardTheme : BaseControlTheme
{
    public const string FrameDecoratorPart = "PART_FrameDecorator";
    public const string IconContentPart = "PART_IconContent";
    public const string HeaderContainerPart = "PART_HeaderContainer";
    public const string HeaderTitlePart = "PART_HeaderTitle";
    public const string ContentPart = "PART_Content";
    public const string CloseButtonPart = "PART_CloseButton";
    public const string MotionActorPart = "PART_MotionActor";
    public const string ProgressBarPart = "PART_ProgressBar";
    public const string MarginGhostDecoratorPart = "PART_MarginGhostDecorator";
    

    public NotificationCardTheme()
        : base(typeof(NotificationCard))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<NotificationCard>((card, scope) =>
        {
            BuildInstanceStyles(card);
            var motionActor = new MotionActorControl()
            {
                Name         = MotionActorPart,
                ClipToBounds = false
            };
            motionActor.RegisterInNameScope(scope);

            // 防止关闭的时候抖动，如果直接设置到 NotificationCard，layoutTransformControl没有办法平滑处理
            var marginGhostDecorator = new Border
            {
                Name = MarginGhostDecoratorPart
            };

            var frameDecorator = new Border
            {
                Name = FrameDecoratorPart
            };

            marginGhostDecorator.Child = frameDecorator;

            var mainLayout = new Grid
            {
                ColumnDefinitions = new ColumnDefinitions
                {
                    new(GridLength.Auto),
                    new(GridLength.Star)
                },
                RowDefinitions = new RowDefinitions
                {
                    new(GridLength.Auto),
                    new(GridLength.Star),
                    new(GridLength.Auto)
                }
            };

            frameDecorator.Child = mainLayout;
            BuildHeader(mainLayout, scope);
            BuildContent(mainLayout, scope);
            BuildProgressBar(mainLayout, scope);
            frameDecorator.RegisterInNameScope(scope);

            motionActor.Child = marginGhostDecorator;
            return motionActor;
        });
    }

    private void BuildHeader(Grid layout, INameScope scope)
    {
        var iconContent = new ContentPresenter
        {
            Name = IconContentPart
        };
        CreateTemplateParentBinding(iconContent, Visual.IsVisibleProperty, NotificationCard.IconProperty,
            BindingMode.Default,
            ObjectConverters.IsNotNull);
        CreateTemplateParentBinding(iconContent, ContentPresenter.ContentProperty, NotificationCard.IconProperty);
        TokenResourceBinder.CreateTokenBinding(iconContent, Layoutable.MarginProperty,
            NotificationTokenResourceKey.NotificationIconMargin);
        Grid.SetRow(iconContent, 0);
        Grid.SetColumn(iconContent, 0);

        layout.Children.Add(iconContent);

        var headerContainer = new DockPanel
        {
            Name          = HeaderContainerPart,
            LastChildFill = true
        };

        var headerTitle = new SelectableTextBlock
        {
            Name = HeaderTitlePart
        };
        TokenResourceBinder.CreateGlobalTokenBinding(headerTitle, SelectableTextBlock.SelectionBrushProperty,
            DesignTokenKey.SelectionBackground);
        TokenResourceBinder.CreateGlobalTokenBinding(headerTitle, SelectableTextBlock.SelectionForegroundBrushProperty,
            DesignTokenKey.SelectionForeground);

        CreateTemplateParentBinding(headerTitle, TextBlock.TextProperty, NotificationCard.TitleProperty);
   
        var closeIcon = AntDesignIconPackage.CloseOutlined();
        TokenResourceBinder.CreateTokenBinding(closeIcon, Icon.NormalFilledBrushProperty,
            DesignTokenKey.ColorIcon);
        TokenResourceBinder.CreateTokenBinding(closeIcon, Icon.ActiveFilledBrushProperty,
            DesignTokenKey.ColorIconHover);
        var closeIconButton = new IconButton
        {
            Name                = CloseButtonPart,
            Icon                = closeIcon,
            IsEnableHoverEffect = true,
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment   = VerticalAlignment.Center
        };
        closeIconButton.RegisterInNameScope(scope);
        TokenResourceBinder.CreateTokenBinding(closeIconButton, TemplatedControl.PaddingProperty,
            DesignTokenKey.PaddingXXS, BindingPriority.Template,
            o =>
            {
                if (o is double dval)
                {
                    return new Thickness(dval);
                }

                return new Thickness(0);
            });
        TokenResourceBinder.CreateTokenBinding(closeIconButton, TemplatedControl.CornerRadiusProperty,
            DesignTokenKey.BorderRadiusSM);
        TokenResourceBinder.CreateTokenBinding(closeIconButton, IconButton.IconHeightProperty,
            DesignTokenKey.IconSizeSM);
        TokenResourceBinder.CreateTokenBinding(closeIconButton, IconButton.IconWidthProperty,
            DesignTokenKey.IconSizeSM);
        DockPanel.SetDock(closeIconButton, Dock.Right);
        headerContainer.Children.Add(closeIconButton);
        headerContainer.Children.Add(headerTitle);

        Grid.SetRow(headerContainer, 0);
        Grid.SetColumn(headerContainer, 1);
        layout.Children.Add(headerContainer);
    }

    private void BuildContent(Grid layout, INameScope scope)
    {
        var contentPresenter = new ContentPresenter
        {
            Name         = ContentPart,
            TextWrapping = TextWrapping.Wrap
        };
        TokenResourceBinder.CreateTokenBinding(contentPresenter, Layoutable.MarginProperty,
            NotificationTokenResourceKey.NotificationContentMargin);
        CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty,
            ContentControl.ContentProperty);
        CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentTemplateProperty,
            ContentControl.ContentTemplateProperty);
        Grid.SetColumn(contentPresenter, 1);
        Grid.SetRow(contentPresenter, 1);
        layout.Children.Add(contentPresenter);
    }

    private void BuildProgressBar(Grid layout, INameScope scope)
    {
        var progressBar = new NotificationProgressBar
        {
            Name = ProgressBarPart
        };
        progressBar.RegisterInNameScope(scope);
        CreateTemplateParentBinding(progressBar, Visual.IsVisibleProperty,
            NotificationCard.EffectiveShowProgressProperty);
        TokenResourceBinder.CreateTokenBinding(progressBar, Layoutable.MarginProperty,
            NotificationTokenResourceKey.NotificationProgressMargin);
        Grid.SetColumn(progressBar, 0);
        Grid.SetRow(progressBar, 1);
        Grid.SetColumnSpan(progressBar, 2);
        layout.Children.Add(progressBar);
    }

    protected override void BuildStyles()
    {
        BuildCommonStyle();
        BuildHeaderStyle();
        BuildContentStyle();
    }

    private void BuildCommonStyle()
    {
        var commonStyle = new Style(selector => selector.Nesting());

        var topLeftStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(NotificationCard.PositionProperty, NotificationPosition.TopLeft));

        {
            var marginGhostDecoratorStyle =
                new Style(selector => selector.Nesting().Template().Name(MarginGhostDecoratorPart));
            marginGhostDecoratorStyle.Add(Layoutable.MarginProperty,
                NotificationTokenResourceKey.NotificationTopMargin);
            topLeftStyle.Add(marginGhostDecoratorStyle);
        }
        commonStyle.Add(topLeftStyle);

        var topCenterStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(NotificationCard.PositionProperty, NotificationPosition.TopCenter));
        {
            var marginGhostDecoratorStyle =
                new Style(selector => selector.Nesting().Template().Name(MarginGhostDecoratorPart));
            marginGhostDecoratorStyle.Add(Layoutable.MarginProperty,
                NotificationTokenResourceKey.NotificationTopMargin);
            topCenterStyle.Add(marginGhostDecoratorStyle);
        }
        commonStyle.Add(topCenterStyle);

        var topRightStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(NotificationCard.PositionProperty, NotificationPosition.TopRight));
        {
            var marginGhostDecoratorStyle =
                new Style(selector => selector.Nesting().Template().Name(MarginGhostDecoratorPart));
            marginGhostDecoratorStyle.Add(Layoutable.MarginProperty,
                NotificationTokenResourceKey.NotificationTopMargin);
            topRightStyle.Add(marginGhostDecoratorStyle);
        }
        commonStyle.Add(topRightStyle);

        var bottomLeftStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(NotificationCard.PositionProperty, NotificationPosition.BottomLeft));
        {
            var marginGhostDecoratorStyle =
                new Style(selector => selector.Nesting().Template().Name(MarginGhostDecoratorPart));
            marginGhostDecoratorStyle.Add(Layoutable.MarginProperty,
                NotificationTokenResourceKey.NotificationBottomMargin);
            bottomLeftStyle.Add(marginGhostDecoratorStyle);
        }
        commonStyle.Add(bottomLeftStyle);

        var bottomCenterStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(NotificationCard.PositionProperty, NotificationPosition.BottomCenter));
        {
            var marginGhostDecoratorStyle =
                new Style(selector => selector.Nesting().Template().Name(MarginGhostDecoratorPart));
            marginGhostDecoratorStyle.Add(Layoutable.MarginProperty,
                NotificationTokenResourceKey.NotificationBottomMargin);
            bottomCenterStyle.Add(marginGhostDecoratorStyle);
        }
        commonStyle.Add(bottomCenterStyle);

        var bottomRightStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(NotificationCard.PositionProperty, NotificationPosition.BottomRight));
        {
            var marginGhostDecoratorStyle =
                new Style(selector => selector.Nesting().Template().Name(MarginGhostDecoratorPart));
            marginGhostDecoratorStyle.Add(Layoutable.MarginProperty,
                NotificationTokenResourceKey.NotificationBottomMargin);
            bottomRightStyle.Add(marginGhostDecoratorStyle);
        }
        commonStyle.Add(bottomRightStyle);

        commonStyle.Add(Layoutable.WidthProperty, NotificationTokenResourceKey.NotificationWidth);

        var frameDecoratorStyle = new Style(selector => selector.Nesting().Template().Name(FrameDecoratorPart));
        frameDecoratorStyle.Add(Decorator.PaddingProperty, NotificationTokenResourceKey.NotificationPadding);
        frameDecoratorStyle.Add(Border.BoxShadowProperty, DesignTokenKey.BoxShadows);
        frameDecoratorStyle.Add(Border.BackgroundProperty, NotificationTokenResourceKey.NotificationBg);
        frameDecoratorStyle.Add(Border.CornerRadiusProperty, DesignTokenKey.BorderRadiusLG);
        commonStyle.Add(frameDecoratorStyle);

        var closedStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(NotificationCard.IsClosedProperty, true));
        closedStyle.Add(Layoutable.MarginProperty, new Thickness(0));
        commonStyle.Add(closedStyle);

        Add(commonStyle);
    }

    private void BuildHeaderStyle()
    {
        var titleStyle = new Style(selector => selector.Nesting().Template().Name(HeaderTitlePart));
        titleStyle.Add(ContentPresenter.LineHeightProperty, DesignTokenKey.FontHeightLG);
        titleStyle.Add(ContentPresenter.FontSizeProperty, DesignTokenKey.FontSizeLG);
        titleStyle.Add(ContentPresenter.ForegroundProperty, DesignTokenKey.ColorTextHeading);
        Add(titleStyle);

        var headerContainer = new Style(selector => selector.Nesting().Template().Name(HeaderContainerPart));
        headerContainer.Add(Layoutable.MarginProperty, NotificationTokenResourceKey.HeaderMargin);
        Add(headerContainer);
    }

    private void BuildContentStyle()
    {
        var contentStyle = new Style(selector => selector.Nesting().Template().Name(ContentPart));
        contentStyle.Add(ContentPresenter.ForegroundProperty, DesignTokenKey.ColorText);
        contentStyle.Add(ContentPresenter.FontSizeProperty, DesignTokenKey.FontSize);
        contentStyle.Add(ContentPresenter.LineHeightProperty, DesignTokenKey.FontHeight);
        Add(contentStyle);
    }

    protected override void BuildInstanceStyles(Control control)
    {
        var iconStyle = new Style(selector => selector.Name(IconContentPart).Child().OfType<Icon>());
        iconStyle.Add(Layoutable.WidthProperty, NotificationTokenResourceKey.NotificationIconSize);
        iconStyle.Add(Layoutable.HeightProperty, NotificationTokenResourceKey.NotificationIconSize);
        control.Styles.Add(iconStyle);
    }
}