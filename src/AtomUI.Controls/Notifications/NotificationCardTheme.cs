using AtomUI.IconPkg;
using AtomUI.IconPkg.AntDesign;
using AtomUI.MotionScene;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
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
    public const string FramePart = "PART_Frame";
    public const string IconContentPart = "PART_IconContent";
    public const string HeaderContainerPart = "PART_HeaderContainer";
    public const string HeaderTitlePart = "PART_HeaderTitle";
    public const string ContentPart = "PART_Content";
    public const string CloseButtonPart = "PART_CloseButton";
    public const string MotionActorPart = "PART_MotionActor";
    public const string ProgressBarPart = "PART_ProgressBar";
    
    public NotificationCardTheme()
        : base(typeof(NotificationCard))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<NotificationCard>((notificationCard, scope) =>
        {
            BuildInstanceStyles(notificationCard);
            var motionActor = new MotionActorControl()
            {
                Name         = MotionActorPart,
                ClipToBounds = false,
                UseRenderTransform = false
            };
            motionActor.RegisterInNameScope(scope);
            
            var frame = new Border
            {
                Name = FramePart
            };

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

            frame.Child = mainLayout;
            BuildHeader(notificationCard, mainLayout, scope);
            BuildContent(notificationCard, mainLayout, scope);
            BuildProgressBar(notificationCard, mainLayout, scope);
            frame.RegisterInNameScope(scope);

            motionActor.Child = frame;
            return motionActor;
        });
    }

    private void BuildHeader(NotificationCard notificationCard, Grid layout, INameScope scope)
    {
        var iconContent = new ContentPresenter
        {
            Name = IconContentPart
        };
        CreateTemplateParentBinding(iconContent, Visual.IsVisibleProperty, NotificationCard.IconProperty,
            BindingMode.Default,
            ObjectConverters.IsNotNull);
        CreateTemplateParentBinding(iconContent, ContentPresenter.ContentProperty, NotificationCard.IconProperty);
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
        
        CreateTemplateParentBinding(headerTitle, TextBlock.TextProperty, NotificationCard.TitleProperty);
        
        var closeButton = new IconButton
        {
            Name                = CloseButtonPart,
            Icon                = AntDesignIconPackage.CloseOutlined(),
            IsEnableHoverEffect = true,
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment   = VerticalAlignment.Center
        };
        closeButton.RegisterInNameScope(scope);
       
        DockPanel.SetDock(closeButton, Dock.Right);
        headerContainer.Children.Add(closeButton);
        headerContainer.Children.Add(headerTitle);

        Grid.SetRow(headerContainer, 0);
        Grid.SetColumn(headerContainer, 1);
        layout.Children.Add(headerContainer);
    }

    private void BuildContent(NotificationCard notificationCard, Grid layout, INameScope scope)
    {
        var contentPresenter = new ContentPresenter
        {
            Name         = ContentPart,
            TextWrapping = TextWrapping.Wrap
        };

        CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty,
            ContentControl.ContentProperty);
        CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentTemplateProperty,
            ContentControl.ContentTemplateProperty);
        Grid.SetColumn(contentPresenter, 1);
        Grid.SetRow(contentPresenter, 1);
        layout.Children.Add(contentPresenter);
    }

    private void BuildProgressBar(NotificationCard notificationCard, Grid layout, INameScope scope)
    {
        var progressBar = new NotificationProgressBar
        {
            Name = ProgressBarPart
        };
        progressBar.RegisterInNameScope(scope);
        CreateTemplateParentBinding(progressBar, Visual.IsVisibleProperty,
            NotificationCard.EffectiveShowProgressProperty);
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
        
        var progressBarStyle = new Style(selector => selector.Nesting().Template().Name(ProgressBarPart));
        progressBarStyle.Add(Layoutable.MarginProperty, NotificationTokenKey.NotificationProgressMargin);
        commonStyle.Add(progressBarStyle);

        var topLeftStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(NotificationCard.PositionProperty, NotificationPosition.TopLeft));

        {
            var frameStyle =
                new Style(selector => selector.Nesting().Template().Name(FramePart));
            frameStyle.Add(Layoutable.MarginProperty,
                NotificationTokenKey.NotificationTopMargin);
            topLeftStyle.Add(frameStyle);
        }
        commonStyle.Add(topLeftStyle);

        var topCenterStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(NotificationCard.PositionProperty, NotificationPosition.TopCenter));
        {
            var frameStyle =
                new Style(selector => selector.Nesting().Template().Name(FramePart));
            frameStyle.Add(Layoutable.MarginProperty,
                NotificationTokenKey.NotificationTopMargin);
            topCenterStyle.Add(frameStyle);
        }
        commonStyle.Add(topCenterStyle);

        var topRightStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(NotificationCard.PositionProperty, NotificationPosition.TopRight));
        {
            var frameStyle =
                new Style(selector => selector.Nesting().Template().Name(FramePart));
            frameStyle.Add(Layoutable.MarginProperty,
                NotificationTokenKey.NotificationTopMargin);
            topRightStyle.Add(frameStyle);
        }
        commonStyle.Add(topRightStyle);

        var bottomLeftStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(NotificationCard.PositionProperty, NotificationPosition.BottomLeft));
        {
            var frameStyle =
                new Style(selector => selector.Nesting().Template().Name(FramePart));
            frameStyle.Add(Layoutable.MarginProperty,
                NotificationTokenKey.NotificationBottomMargin);
            bottomLeftStyle.Add(frameStyle);
        }
        commonStyle.Add(bottomLeftStyle);

        var bottomCenterStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(NotificationCard.PositionProperty, NotificationPosition.BottomCenter));
        {
            var frameStyle =
                new Style(selector => selector.Nesting().Template().Name(FramePart));
            frameStyle.Add(Layoutable.MarginProperty,
                NotificationTokenKey.NotificationBottomMargin);
            bottomCenterStyle.Add(frameStyle);
        }
        commonStyle.Add(bottomCenterStyle);

        var bottomRightStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(NotificationCard.PositionProperty, NotificationPosition.BottomRight));
        {
            var frameStyle =
                new Style(selector => selector.Nesting().Template().Name(FramePart));
            frameStyle.Add(Layoutable.MarginProperty,
                NotificationTokenKey.NotificationBottomMargin);
            bottomRightStyle.Add(frameStyle);
        }
        commonStyle.Add(bottomRightStyle);

        commonStyle.Add(Layoutable.WidthProperty, NotificationTokenKey.NotificationWidth);

        {
            var frameStyle = new Style(selector => selector.Nesting().Template().Name(FramePart));
            frameStyle.Add(Decorator.PaddingProperty, NotificationTokenKey.NotificationPadding);
            frameStyle.Add(Border.BoxShadowProperty, SharedTokenKey.BoxShadows);
            frameStyle.Add(Border.BackgroundProperty, NotificationTokenKey.NotificationBg);
            frameStyle.Add(Border.CornerRadiusProperty, SharedTokenKey.BorderRadiusLG);
            commonStyle.Add(frameStyle);
        }

        var closedStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(NotificationCard.IsClosedProperty, true));
        closedStyle.Add(Layoutable.MarginProperty, new Thickness(0));
        commonStyle.Add(closedStyle);

        Add(commonStyle);
    }

    private void BuildHeaderStyle()
    {
        var titleStyle = new Style(selector => selector.Nesting().Template().Name(HeaderTitlePart));
        titleStyle.Add(ContentPresenter.LineHeightProperty, SharedTokenKey.FontHeightLG);
        titleStyle.Add(ContentPresenter.FontSizeProperty, SharedTokenKey.FontSizeLG);
        titleStyle.Add(ContentPresenter.ForegroundProperty, SharedTokenKey.ColorTextHeading);
        titleStyle.Add(SelectableTextBlock.SelectionBrushProperty, SharedTokenKey.SelectionBackground);
        titleStyle.Add(SelectableTextBlock.SelectionForegroundBrushProperty, SharedTokenKey.SelectionForeground);
        Add(titleStyle);
        
        var iconContentStyle = new Style(selector => selector.Nesting().Template().Name(IconContentPart));
        iconContentStyle.Add(Layoutable.MarginProperty, NotificationTokenKey.NotificationIconMargin);
        Add(iconContentStyle);

        var headerContainer = new Style(selector => selector.Nesting().Template().Name(HeaderContainerPart));
        headerContainer.Add(Layoutable.MarginProperty, NotificationTokenKey.HeaderMargin);
        Add(headerContainer);
        
        var closeButtonStyle = new Style(selector => selector.Nesting().Template().Name(CloseButtonPart));
        closeButtonStyle.Add(IconButton.NormalIconColorProperty, SharedTokenKey.ColorIcon);
        closeButtonStyle.Add(IconButton.ActiveIconColorProperty, SharedTokenKey.ColorIconHover);
        closeButtonStyle.Add(IconButton.PaddingProperty, NotificationTokenKey.NotificationCloseButtonPadding);
        closeButtonStyle.Add(IconButton.CornerRadiusProperty, SharedTokenKey.BorderRadiusSM);
        closeButtonStyle.Add(IconButton.IconHeightProperty, SharedTokenKey.IconSizeSM);
        closeButtonStyle.Add(IconButton.IconWidthProperty, SharedTokenKey.IconSizeSM);
        Add(closeButtonStyle);
    }

    private void BuildContentStyle()
    {
        var contentStyle = new Style(selector => selector.Nesting().Template().Name(ContentPart));
        contentStyle.Add(ContentPresenter.ForegroundProperty, SharedTokenKey.ColorText);
        contentStyle.Add(ContentPresenter.FontSizeProperty, SharedTokenKey.FontSize);
        contentStyle.Add(ContentPresenter.LineHeightProperty, SharedTokenKey.FontHeight);
        contentStyle.Add(Layoutable.MarginProperty, NotificationTokenKey.NotificationContentMargin);
        
        Add(contentStyle);
    }

    protected override void BuildInstanceStyles(Control control)
    {
        var iconStyle = new Style(selector => selector.Name(IconContentPart).Child().OfType<Icon>());
        iconStyle.Add(Layoutable.WidthProperty, NotificationTokenKey.NotificationIconSize);
        iconStyle.Add(Layoutable.HeightProperty, NotificationTokenKey.NotificationIconSize);
        control.Styles.Add(iconStyle);
    }
}