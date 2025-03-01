using AtomUI.IconPkg;
using AtomUI.IconPkg.AntDesign;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class DrawerInfoContainerTheme : BaseControlTheme
{
    public const string RootLayoutPart = "PART_RootLayout";
    public const string FramePart = "PART_Frame";
    public const string InfoLayoutPart = "PART_InfoLayout";
    public const string InfoHeaderPart = "PART_InfoHeader";
    public const string InfoFooterPart = "PART_InfoFooter";
    public const string InfoContainerPart = "PART_InfoContainer";
    public const string CloseButtonPart = "PART_CloseButton";
    public const string HeaderTextPart = "PART_HeaderText";
    public const string ExtraContentPresenterPart = "PART_ExtraContentPresenter";

    public DrawerInfoContainerTheme() : base(typeof(DrawerInfoContainer))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<DrawerInfoContainer>((drawerInfoContainer, scope) =>
        {
            var rootLayout = new Panel()
            {
                Name = RootLayoutPart
            };
            var frame = new Border()
            {
                Name = FramePart
            };
            rootLayout.Children.Add(frame);
            // TODO 是否在这里加上滚动条支持，还没内容控件自己进行处理
            var infoLayout = new Grid
            {
                Name = InfoLayoutPart,
                RowDefinitions = new RowDefinitions()
                {
                    new RowDefinition(GridLength.Auto),
                    new RowDefinition(GridLength.Auto),
                    new RowDefinition(GridLength.Star),
                    new RowDefinition(GridLength.Auto),
                    new RowDefinition(GridLength.Auto),
                }
            };
            rootLayout.Children.Add(infoLayout);

            BuildHeader(drawerInfoContainer, infoLayout, scope);
            var separator = new Separator();
            Grid.SetRow(separator, 1);
            infoLayout.Children.Add(separator);

            var contentPresenter = new ContentPresenter
            {
                Name = InfoContainerPart,
            };
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty,
                DrawerInfoContainer.ContentProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentTemplateProperty,
                DrawerInfoContainer.ContentTemplateProperty);
            Grid.SetRow(contentPresenter, 2);
            infoLayout.Children.Add(contentPresenter);

            var footerSeparator = new Separator();
            Grid.SetRow(footerSeparator, 3);
            infoLayout.Children.Add(footerSeparator);
            CreateTemplateParentBinding(footerSeparator, Separator.IsVisibleProperty,
                DrawerInfoContainer.HasFooterProperty);
            var footerPresenter = new ContentPresenter
            {
                Name = InfoFooterPart
            };
            CreateTemplateParentBinding(footerPresenter, ContentPresenter.IsVisibleProperty,
                DrawerInfoContainer.HasFooterProperty);
            CreateTemplateParentBinding(footerPresenter, ContentPresenter.ContentProperty,
                DrawerInfoContainer.FooterProperty);
            CreateTemplateParentBinding(footerPresenter, ContentPresenter.ContentTemplateProperty,
                DrawerInfoContainer.FooterTemplateProperty);
            Grid.SetRow(footerPresenter, 4);
            infoLayout.Children.Add(footerPresenter);

            return rootLayout;
        });
    }

    private void BuildHeader(DrawerInfoContainer drawerInfoContainer, Grid rootLayout, INameScope scope)
    {
        var headerLayout = new Grid
        {
            Name = InfoHeaderPart,
            ColumnDefinitions = new ColumnDefinitions()
            {
                new ColumnDefinition(GridLength.Auto),
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto)
            }
        };
        Grid.SetRow(headerLayout, 0);
        rootLayout.Children.Add(headerLayout);

        var closeIcon = AntDesignIconPackage.CloseOutlined();

        drawerInfoContainer.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(closeIcon,
            Icon.NormalFilledBrushProperty, SharedTokenKey.ColorIcon));
        drawerInfoContainer.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(closeIcon,
            Icon.ActiveFilledBrushProperty, SharedTokenKey.ColorIconHover));
        drawerInfoContainer.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(closeIcon,
            Icon.SelectedFilledBrushProperty, SharedTokenKey.ColorIconHover));

        var closeButton = new IconButton()
        {
            Name                = CloseButtonPart,
            Icon                = closeIcon,
            IsEnableHoverEffect = true,
            VerticalAlignment   = VerticalAlignment.Center,
        };
        closeButton.RegisterInNameScope(scope);
        Grid.SetColumn(closeButton, 0);
        CreateTemplateParentBinding(closeButton, IconButton.IsMotionEnabledProperty,
            DrawerInfoContainer.IsMotionEnabledProperty,
            BindingPriority.LocalValue);
        CreateTemplateParentBinding(closeButton, IconButton.IsVisibleProperty,
            DrawerInfoContainer.IsShowCloseButtonProperty);
        headerLayout.Children.Add(closeButton);

        var headerText = new TextBlock
        {
            Name                = HeaderTextPart,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment   = VerticalAlignment.Center,
        };
        CreateTemplateParentBinding(headerText, TextBlock.TextProperty, DrawerInfoContainer.TitleProperty);
        Grid.SetColumn(headerText, 1);
        headerLayout.Children.Add(headerText);

        var extraContentPresenter = new ContentPresenter
        {
            Name                       = ExtraContentPresenterPart,
            HorizontalContentAlignment = HorizontalAlignment.Left,
            VerticalContentAlignment   = VerticalAlignment.Top,
        };
        CreateTemplateParentBinding(extraContentPresenter, ContentPresenter.IsVisibleProperty,
            DrawerInfoContainer.HasExtraProperty);
        CreateTemplateParentBinding(extraContentPresenter, ContentPresenter.ContentProperty,
            DrawerInfoContainer.ExtraProperty);
        CreateTemplateParentBinding(extraContentPresenter, ContentPresenter.ContentTemplateProperty,
            DrawerInfoContainer.ExtraTemplateProperty);
        Grid.SetColumn(extraContentPresenter, 2);
        headerLayout.Children.Add(extraContentPresenter);
    }

    protected override void BuildStyles()
    {
        base.BuildStyles();
        this.Add(DrawerInfoContainer.ClipToBoundsProperty, false);
        var frameStyle = new Style(selector => selector.Nesting().Template().Name(FramePart));
        frameStyle.Add(Border.BackgroundProperty, SharedTokenKey.ColorBgContainer);
        Add(frameStyle);
        BuildShadowsStyle();
        BuildDialogSizeStyle();
        BuildHeaderStyle();
        BuildContentStyle();
        BuildFooterStyle();
    }

    private void BuildShadowsStyle()
    {
        var topPlacementStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(DrawerInfoContainer.PlacementProperty, DrawerPlacement.Top));
        {
            var frameStyle = new Style(selector => selector.Nesting().Template().Name(FramePart));
            frameStyle.Add(Border.BoxShadowProperty, DrawerTokenKey.BoxShadowDrawerUp);
            topPlacementStyle.Add(frameStyle);
        }

        Add(topPlacementStyle);
        var rightPlacementStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(DrawerInfoContainer.PlacementProperty, DrawerPlacement.Right));
        {
            var frameStyle = new Style(selector => selector.Nesting().Template().Name(FramePart));
            frameStyle.Add(Border.BoxShadowProperty, DrawerTokenKey.BoxShadowDrawerRight);
            rightPlacementStyle.Add(frameStyle);
        }
        Add(rightPlacementStyle);
        var bottomPlacementStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(DrawerInfoContainer.PlacementProperty, DrawerPlacement.Bottom));
        {
            var frameStyle = new Style(selector => selector.Nesting().Template().Name(FramePart));
            frameStyle.Add(Border.BoxShadowProperty, DrawerTokenKey.BoxShadowDrawerDown);
            bottomPlacementStyle.Add(frameStyle);
        }
        Add(bottomPlacementStyle);
        var leftPlacementStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(DrawerInfoContainer.PlacementProperty, DrawerPlacement.Left));
        {
            var frameStyle = new Style(selector => selector.Nesting().Template().Name(FramePart));
            frameStyle.Add(Border.BoxShadowProperty, DrawerTokenKey.BoxShadowDrawerLeft);
            leftPlacementStyle.Add(frameStyle);
        }
        Add(leftPlacementStyle);
    }

    private void BuildDialogSizeStyle()
    {
        // 摆放位置样式
        // TODO Error 这样直接设置 Binding 所有的该对样公用一个会不会内存泄漏
        // TODO review memory leak
        var topPlacementStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(DrawerInfoContainer.PlacementProperty, DrawerPlacement.Top));
        topPlacementStyle.Add(DrawerInfoContainer.VerticalAlignmentProperty, VerticalAlignment.Top);
        topPlacementStyle.Add(DrawerInfoContainer.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
        topPlacementStyle.Add(DrawerInfoContainer.HeightProperty, new Binding()
        {
            Path           = DrawerInfoContainer.DialogSizeProperty.Name,
            Mode           = BindingMode.Default,
            RelativeSource = new RelativeSource(RelativeSourceMode.Self)
        });
        Add(topPlacementStyle);
        var rightPlacementStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(DrawerInfoContainer.PlacementProperty, DrawerPlacement.Right));
        rightPlacementStyle.Add(DrawerInfoContainer.VerticalAlignmentProperty, VerticalAlignment.Stretch);
        rightPlacementStyle.Add(DrawerInfoContainer.HorizontalAlignmentProperty, HorizontalAlignment.Right);
        rightPlacementStyle.Add(DrawerInfoContainer.WidthProperty, new Binding()
        {
            Path           = DrawerInfoContainer.DialogSizeProperty.Name,
            Mode           = BindingMode.Default,
            RelativeSource = new RelativeSource(RelativeSourceMode.Self)
        });
        Add(rightPlacementStyle);
        var bottomPlacementStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(DrawerInfoContainer.PlacementProperty, DrawerPlacement.Bottom));
        bottomPlacementStyle.Add(DrawerInfoContainer.VerticalAlignmentProperty, VerticalAlignment.Bottom);
        bottomPlacementStyle.Add(DrawerInfoContainer.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
        bottomPlacementStyle.Add(DrawerInfoContainer.HeightProperty, new Binding()
        {
            Path           = DrawerInfoContainer.DialogSizeProperty.Name,
            Mode           = BindingMode.Default,
            RelativeSource = new RelativeSource(RelativeSourceMode.Self)
        });
        Add(bottomPlacementStyle);
        var leftPlacementStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(DrawerInfoContainer.PlacementProperty, DrawerPlacement.Left));
        leftPlacementStyle.Add(DrawerInfoContainer.VerticalAlignmentProperty, VerticalAlignment.Stretch);
        leftPlacementStyle.Add(DrawerInfoContainer.HorizontalAlignmentProperty, HorizontalAlignment.Left);
        leftPlacementStyle.Add(DrawerInfoContainer.WidthProperty, new Binding()
        {
            Path           = DrawerInfoContainer.DialogSizeProperty.Name,
            Mode           = BindingMode.Default,
            RelativeSource = new RelativeSource(RelativeSourceMode.Self)
        });
        Add(leftPlacementStyle);
    }

    private void BuildHeaderStyle()
    {
        var headerStyle = new Style(selector => selector.Nesting().Template().Name(InfoHeaderPart));
        headerStyle.Add(Grid.MarginProperty, DrawerTokenKey.HeaderMargin);
        Add(headerStyle);
        var headerTextStyle = new Style(selector => selector.Nesting().Template().Name(HeaderTextPart));
        headerTextStyle.Add(TextBlock.FontSizeProperty, SharedTokenKey.FontSizeLG);
        headerTextStyle.Add(TextBlock.LineHeightProperty, SharedTokenKey.FontHeightLG);
        headerTextStyle.Add(TextBlock.FontWeightProperty, SharedTokenKey.FontWeightStrong);
        Add(headerTextStyle);

        var closeButtonStyle = new Style(selector => selector.Nesting().Template().Name(CloseButtonPart));
        closeButtonStyle.Add(IconButton.CornerRadiusProperty, SharedTokenKey.BorderRadiusSM);
        closeButtonStyle.Add(IconButton.PaddingProperty, DrawerTokenKey.CloseIconPadding);
        closeButtonStyle.Add(IconButton.MarginProperty, DrawerTokenKey.CloseIconMargin);
        closeButtonStyle.Add(IconButton.IconWidthProperty, SharedTokenKey.FontSize);
        closeButtonStyle.Add(IconButton.IconHeightProperty, SharedTokenKey.FontSize);
        Add(closeButtonStyle);
    }

    private void BuildContentStyle()
    {
        var contentPresenterStyle = new Style(selector => selector.Nesting().Template().Name(InfoContainerPart));
        contentPresenterStyle.Add(ContentPresenter.PaddingProperty, DrawerTokenKey.ContentPadding);
        Add(contentPresenterStyle);
    }

    private void BuildFooterStyle()
    {
        var footerStyle = new Style(selector => selector.Nesting().Template().Name(InfoFooterPart));
        footerStyle.Add(ContentPresenter.PaddingProperty, DrawerTokenKey.FooterPadding);
        Add(footerStyle);
    }
}