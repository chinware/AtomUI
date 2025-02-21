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

    public DrawerInfoContainerTheme() : base(typeof(DrawerInfoContainerX))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<DrawerInfoContainerX>((drawerInfoContainer, scope) =>
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

            BuildHeader(infoLayout, scope);
            var separator = new Separator();
            Grid.SetRow(separator, 1);
            infoLayout.Children.Add(separator);

            var contentPresenter = new ContentPresenter
            {
                Name = InfoContainerPart,
            };
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty, DrawerInfoContainerX.ContentProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentTemplateProperty, DrawerInfoContainerX.ContentTemplateProperty);
            Grid.SetRow(contentPresenter, 2);
            infoLayout.Children.Add(contentPresenter);
            
            var footerSeparator = new Separator();
            Grid.SetRow(footerSeparator, 3);
            infoLayout.Children.Add(footerSeparator);
            CreateTemplateParentBinding(footerSeparator, Separator.IsVisibleProperty, DrawerInfoContainerX.HasFooterProperty);
            var footerPresenter = new ContentPresenter
            {
                Name = InfoFooterPart
            };
            CreateTemplateParentBinding(footerPresenter, ContentPresenter.IsVisibleProperty, DrawerInfoContainerX.HasFooterProperty);
            CreateTemplateParentBinding(footerPresenter, ContentPresenter.ContentProperty, DrawerInfoContainerX.FooterProperty);
            CreateTemplateParentBinding(footerPresenter, ContentPresenter.ContentTemplateProperty, DrawerInfoContainerX.FooterTemplateProperty);
            Grid.SetRow(footerPresenter, 4);
            infoLayout.Children.Add(footerPresenter);
            
            return rootLayout;
        });
    }

    private void BuildHeader(Grid rootLayout, INameScope scope)
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
        
        TokenResourceBinder.CreateTokenBinding(closeIcon, Icon.NormalFilledBrushProperty, SharedTokenKey.ColorIcon);
        TokenResourceBinder.CreateTokenBinding(closeIcon, Icon.ActiveFilledBrushProperty, SharedTokenKey.ColorIconHover);
        TokenResourceBinder.CreateTokenBinding(closeIcon, Icon.SelectedFilledBrushProperty, SharedTokenKey.ColorIconHover);
        
        var closeButton = new IconButton()
        {
            Name                = CloseButtonPart,
            Icon                = closeIcon,
            IsEnableHoverEffect = true,
            VerticalAlignment   = VerticalAlignment.Center,
        };
        closeButton.RegisterInNameScope(scope);
        Grid.SetColumn(closeButton, 0);
        CreateTemplateParentBinding(closeButton, IconButton.IsMotionEnabledProperty, DrawerInfoContainerX.IsMotionEnabledProperty, 
            BindingPriority.LocalValue);
        CreateTemplateParentBinding(closeButton, IconButton.IsVisibleProperty, DrawerInfoContainerX.IsShowCloseButtonProperty);
        headerLayout.Children.Add(closeButton);

        var headerText = new TextBlock
        {
            Name = HeaderTextPart,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center,
        };
        CreateTemplateParentBinding(headerText, TextBlock.TextProperty, DrawerInfoContainerX.TitleProperty);
        Grid.SetColumn(headerText, 1);
        headerLayout.Children.Add(headerText);

        var extraContentPresenter = new ContentPresenter
        {
            Name = ExtraContentPresenterPart,
            HorizontalContentAlignment = HorizontalAlignment.Left,
            VerticalContentAlignment = VerticalAlignment.Top,
        };
        CreateTemplateParentBinding(extraContentPresenter, ContentPresenter.IsVisibleProperty, DrawerInfoContainerX.HasExtraProperty);
        CreateTemplateParentBinding(extraContentPresenter, ContentPresenter.ContentProperty, DrawerInfoContainerX.ExtraProperty);
        CreateTemplateParentBinding(extraContentPresenter, ContentPresenter.ContentTemplateProperty, DrawerInfoContainerX.ExtraTemplateProperty);
        Grid.SetColumn(extraContentPresenter, 2);
        headerLayout.Children.Add(extraContentPresenter);
    }

    protected override void BuildStyles()
    {
        base.BuildStyles();
        this.Add(DrawerInfoContainerX.ClipToBoundsProperty, false);
        var frameStyle = new Style(selector => selector.Nesting().Template().Name(FramePart));
        frameStyle.Add(Border.BackgroundProperty, SharedTokenKey.ColorBgContainer);
        Add(frameStyle);
        BuildShadowsStyle();
        BuildSizeTypeStyle();
        BuildHeaderStyle();
        BuildContentStyle();
        BuildFooterStyle();
    }

    private void BuildShadowsStyle()
    {
        var topPlacementStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(DrawerInfoContainerX.PlacementProperty, DrawerPlacement.Top));
        {
            var frameStyle = new Style(selector => selector.Nesting().Template().Name(FramePart));
            frameStyle.Add(Border.BoxShadowProperty, DrawerTokenKey.BoxShadowDrawerUp);
            topPlacementStyle.Add(frameStyle);
        }
      
        Add(topPlacementStyle);
        var rightPlacementStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(DrawerInfoContainerX.PlacementProperty, DrawerPlacement.Right));
        {
            var frameStyle = new Style(selector => selector.Nesting().Template().Name(FramePart));
            frameStyle.Add(Border.BoxShadowProperty, DrawerTokenKey.BoxShadowDrawerRight);
            rightPlacementStyle.Add(frameStyle);
        }
        Add(rightPlacementStyle);
        var bottomPlacementStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(DrawerInfoContainerX.PlacementProperty, DrawerPlacement.Bottom));
        {
            var frameStyle = new Style(selector => selector.Nesting().Template().Name(FramePart));
            frameStyle.Add(Border.BoxShadowProperty, DrawerTokenKey.BoxShadowDrawerDown);
            bottomPlacementStyle.Add(frameStyle);
        }
        Add(bottomPlacementStyle);
        var leftPlacementStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(DrawerInfoContainerX.PlacementProperty, DrawerPlacement.Left));
        {
            var frameStyle = new Style(selector => selector.Nesting().Template().Name(FramePart));
            frameStyle.Add(Border.BoxShadowProperty, DrawerTokenKey.BoxShadowDrawerLeft);
            bottomPlacementStyle.Add(frameStyle);
        }
        Add(leftPlacementStyle);
    }

    private void BuildSizeTypeStyle()
    {
        var smallSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(DrawerInfoContainerX.SizeTypeProperty, SizeType.Small));

        {
            // 摆放位置样式
            var topPlacementStyle = new Style(selector => selector.Nesting().PropertyEquals(DrawerInfoContainerX.PlacementProperty, DrawerPlacement.Top));
            topPlacementStyle.Add(DrawerInfoContainerX.VerticalAlignmentProperty, VerticalAlignment.Top);
            topPlacementStyle.Add(DrawerInfoContainerX.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
            topPlacementStyle.Add(DrawerInfoContainerX.HeightProperty, DrawerTokenKey.SmallSize);
            smallSizeStyle.Add(topPlacementStyle);
            var rightPlacementStyle = new Style(selector => selector.Nesting().PropertyEquals(DrawerInfoContainerX.PlacementProperty, DrawerPlacement.Right));
            rightPlacementStyle.Add(DrawerInfoContainerX.VerticalAlignmentProperty, VerticalAlignment.Stretch);
            rightPlacementStyle.Add(DrawerInfoContainerX.HorizontalAlignmentProperty, HorizontalAlignment.Right);
            rightPlacementStyle.Add(DrawerInfoContainerX.WidthProperty, DrawerTokenKey.SmallSize);
            smallSizeStyle.Add(rightPlacementStyle);
            var bottomPlacementStyle = new Style(selector => selector.Nesting().PropertyEquals(DrawerInfoContainerX.PlacementProperty, DrawerPlacement.Bottom));
            bottomPlacementStyle.Add(DrawerInfoContainerX.VerticalAlignmentProperty, VerticalAlignment.Bottom);
            bottomPlacementStyle.Add(DrawerInfoContainerX.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
            bottomPlacementStyle.Add(DrawerInfoContainerX.HeightProperty, DrawerTokenKey.SmallSize);
            smallSizeStyle.Add(bottomPlacementStyle);
            var leftPlacementStyle = new Style(selector => selector.Nesting().PropertyEquals(DrawerInfoContainerX.PlacementProperty, DrawerPlacement.Left));
            leftPlacementStyle.Add(DrawerInfoContainerX.VerticalAlignmentProperty, VerticalAlignment.Stretch);
            leftPlacementStyle.Add(DrawerInfoContainerX.HorizontalAlignmentProperty, HorizontalAlignment.Left);
            leftPlacementStyle.Add(DrawerInfoContainerX.WidthProperty, DrawerTokenKey.SmallSize);
            smallSizeStyle.Add(leftPlacementStyle);
        }
        
        Add(smallSizeStyle);
        
        var middleSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(DrawerInfoContainerX.SizeTypeProperty, SizeType.Middle));
        {
            // 摆放位置样式
            var topPlacementStyle = new Style(selector => selector.Nesting().PropertyEquals(DrawerInfoContainerX.PlacementProperty, DrawerPlacement.Top));
            topPlacementStyle.Add(DrawerInfoContainerX.VerticalAlignmentProperty, VerticalAlignment.Top);
            topPlacementStyle.Add(DrawerInfoContainerX.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
            topPlacementStyle.Add(DrawerInfoContainerX.HeightProperty, DrawerTokenKey.MiddleSize);
            middleSizeStyle.Add(topPlacementStyle);
            var rightPlacementStyle = new Style(selector => selector.Nesting().PropertyEquals(DrawerInfoContainerX.PlacementProperty, DrawerPlacement.Right));
            rightPlacementStyle.Add(DrawerInfoContainerX.VerticalAlignmentProperty, VerticalAlignment.Stretch);
            rightPlacementStyle.Add(DrawerInfoContainerX.HorizontalAlignmentProperty, HorizontalAlignment.Right);
            rightPlacementStyle.Add(DrawerInfoContainerX.WidthProperty, DrawerTokenKey.MiddleSize);
            middleSizeStyle.Add(rightPlacementStyle);
            var bottomPlacementStyle = new Style(selector => selector.Nesting().PropertyEquals(DrawerInfoContainerX.PlacementProperty, DrawerPlacement.Bottom));
            bottomPlacementStyle.Add(DrawerInfoContainerX.VerticalAlignmentProperty, VerticalAlignment.Bottom);
            bottomPlacementStyle.Add(DrawerInfoContainerX.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
            bottomPlacementStyle.Add(DrawerInfoContainerX.HeightProperty, DrawerTokenKey.MiddleSize);
            middleSizeStyle.Add(bottomPlacementStyle);
            var leftPlacementStyle = new Style(selector => selector.Nesting().PropertyEquals(DrawerInfoContainerX.PlacementProperty, DrawerPlacement.Left));
            leftPlacementStyle.Add(DrawerInfoContainerX.VerticalAlignmentProperty, VerticalAlignment.Stretch);
            leftPlacementStyle.Add(DrawerInfoContainerX.HorizontalAlignmentProperty, HorizontalAlignment.Left);
            leftPlacementStyle.Add(DrawerInfoContainerX.WidthProperty, DrawerTokenKey.MiddleSize);
            middleSizeStyle.Add(leftPlacementStyle);
        }
        Add(middleSizeStyle);
        
        var largeSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(DrawerInfoContainerX.SizeTypeProperty, SizeType.Large));
        {
            // 摆放位置样式
            var topPlacementStyle = new Style(selector => selector.Nesting().PropertyEquals(DrawerInfoContainerX.PlacementProperty, DrawerPlacement.Top));
            topPlacementStyle.Add(DrawerInfoContainerX.VerticalAlignmentProperty, VerticalAlignment.Top);
            topPlacementStyle.Add(DrawerInfoContainerX.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
            topPlacementStyle.Add(DrawerInfoContainerX.HeightProperty, DrawerTokenKey.LargeSize);
            largeSizeStyle.Add(topPlacementStyle);
            var rightPlacementStyle = new Style(selector => selector.Nesting().PropertyEquals(DrawerInfoContainerX.PlacementProperty, DrawerPlacement.Right));
            rightPlacementStyle.Add(DrawerInfoContainerX.VerticalAlignmentProperty, VerticalAlignment.Stretch);
            rightPlacementStyle.Add(DrawerInfoContainerX.HorizontalAlignmentProperty, HorizontalAlignment.Right);
            rightPlacementStyle.Add(DrawerInfoContainerX.WidthProperty, DrawerTokenKey.LargeSize);
            largeSizeStyle.Add(rightPlacementStyle);
            var bottomPlacementStyle = new Style(selector => selector.Nesting().PropertyEquals(DrawerInfoContainerX.PlacementProperty, DrawerPlacement.Bottom));
            bottomPlacementStyle.Add(DrawerInfoContainerX.VerticalAlignmentProperty, VerticalAlignment.Bottom);
            bottomPlacementStyle.Add(DrawerInfoContainerX.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
            bottomPlacementStyle.Add(DrawerInfoContainerX.HeightProperty, DrawerTokenKey.LargeSize);
            largeSizeStyle.Add(bottomPlacementStyle);
            var leftPlacementStyle = new Style(selector => selector.Nesting().PropertyEquals(DrawerInfoContainerX.PlacementProperty, DrawerPlacement.Left));
            leftPlacementStyle.Add(DrawerInfoContainerX.VerticalAlignmentProperty, VerticalAlignment.Stretch);
            leftPlacementStyle.Add(DrawerInfoContainerX.HorizontalAlignmentProperty, HorizontalAlignment.Left);
            leftPlacementStyle.Add(DrawerInfoContainerX.WidthProperty, DrawerTokenKey.LargeSize);
            largeSizeStyle.Add(leftPlacementStyle);
        }
        Add(largeSizeStyle);
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