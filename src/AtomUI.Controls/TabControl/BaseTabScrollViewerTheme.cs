using AtomUI.Data;
using AtomUI.IconPkg;
using AtomUI.IconPkg.AntDesign;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input.GestureRecognizers;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class BaseTabScrollViewerTheme : BaseControlTheme
{
    public const string ScrollStartEdgeIndicatorPart = "PART_ScrollStartEdgeIndicator";
    public const string ScrollEndEdgeIndicatorPart = "PART_ScrollEndEdgeIndicator";
    public const string ScrollMenuIndicatorPart = "PART_ScrollMenuIndicator";
    public const string ScrollViewContentPart = "PART_ContentPresenter";
    public const string ScrollViewLayoutPart = "PART_ScrollViewLayout";
    public const string ScrollViewWrapperLayoutPart = "PART_ScrollViewWrapperLayout";

    public BaseTabScrollViewerTheme()
        : base(typeof(BaseTabScrollViewer))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<BaseTabScrollViewer>((scrollViewer, scope) =>
        {
            scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            var containerLayout = new Panel
            {
                Name = ScrollViewWrapperLayoutPart
            };

            var scrollViewLayout = new DockPanel
            {
                Name = ScrollViewLayoutPart
            };
            
            var menuIndicatorIcon = AntDesignIconPackage.EllipsisOutlined();
            menuIndicatorIcon.HorizontalAlignment = HorizontalAlignment.Center;
            menuIndicatorIcon.VerticalAlignment   = VerticalAlignment.Center;

            TokenResourceBinder.CreateTokenBinding(menuIndicatorIcon, Icon.NormalFilledBrushProperty,
                GlobalTokenResourceKey.ColorTextSecondary);

            var menuIndicator = new IconButton
            {
                Name = ScrollMenuIndicatorPart,
                Icon = menuIndicatorIcon
            };
            TokenResourceBinder.CreateTokenBinding(menuIndicator, IconButton.IconWidthProperty,
                GlobalTokenResourceKey.IconSize);
            TokenResourceBinder.CreateTokenBinding(menuIndicator, IconButton.IconHeightProperty,
                GlobalTokenResourceKey.IconSize);
            menuIndicator.RegisterInNameScope(scope);

            var scrollViewContent = CreateScrollContentPresenter();

            scrollViewLayout.Children.Add(menuIndicator);
            scrollViewLayout.Children.Add(scrollViewContent);

            scrollViewContent.RegisterInNameScope(scope);

            var startEdgeIndicator = new Border
            {
                Name             = ScrollStartEdgeIndicatorPart,
                IsHitTestVisible = false
            };
            startEdgeIndicator.RegisterInNameScope(scope);

            containerLayout.Children.Add(startEdgeIndicator);

            var endEdgeIndicator = new Border
            {
                Name             = ScrollEndEdgeIndicatorPart,
                IsHitTestVisible = false
            };
            endEdgeIndicator.RegisterInNameScope(scope);

            containerLayout.Children.Add(endEdgeIndicator);
            containerLayout.Children.Add(scrollViewLayout);

            return containerLayout;
        });
    }

    private ScrollContentPresenter CreateScrollContentPresenter()
    {
        var scrollViewContent = new TabScrollContentPresenter
        {
            Name = ScrollViewContentPart
        };

        CreateTemplateParentBinding(scrollViewContent, Layoutable.MarginProperty,
            TemplatedControl.PaddingProperty);
        CreateTemplateParentBinding(scrollViewContent, ScrollContentPresenter.HorizontalSnapPointsAlignmentProperty,
            ScrollViewer.HorizontalSnapPointsAlignmentProperty);
        CreateTemplateParentBinding(scrollViewContent, ScrollContentPresenter.HorizontalSnapPointsTypeProperty,
            ScrollViewer.HorizontalSnapPointsTypeProperty);
        CreateTemplateParentBinding(scrollViewContent, ScrollContentPresenter.VerticalSnapPointsAlignmentProperty,
            ScrollViewer.VerticalSnapPointsAlignmentProperty);
        CreateTemplateParentBinding(scrollViewContent, ScrollContentPresenter.VerticalSnapPointsTypeProperty,
            ScrollViewer.VerticalSnapPointsTypeProperty);
        var scrollGestureRecognizer = new ScrollGestureRecognizer();
        BindUtils.RelayBind(scrollViewContent, ScrollContentPresenter.CanHorizontallyScrollProperty,
            scrollGestureRecognizer,
            ScrollGestureRecognizer.CanHorizontallyScrollProperty);
        BindUtils.RelayBind(scrollViewContent, ScrollContentPresenter.CanVerticallyScrollProperty,
            scrollGestureRecognizer,
            ScrollGestureRecognizer.CanVerticallyScrollProperty);

        CreateTemplateParentBinding(scrollGestureRecognizer, ScrollGestureRecognizer.IsScrollInertiaEnabledProperty,
            ScrollViewer.IsScrollInertiaEnabledProperty);
        scrollViewContent.GestureRecognizers.Add(scrollGestureRecognizer);

        return scrollViewContent;
    }

    protected override void BuildStyles()
    {
        var topPlacementStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(BaseTabScrollViewer.TabStripPlacementProperty, Dock.Top));
        {
            var contentPresenterStyle =
                new Style(selector => selector.Nesting().Template().Name(ScrollViewContentPart));
            contentPresenterStyle.Add(DockPanel.DockProperty, Dock.Left);
            var menuIndicatorStyle =
                new Style(selector => selector.Nesting().Template().Name(ScrollMenuIndicatorPart));
            menuIndicatorStyle.Add(DockPanel.DockProperty, Dock.Right);
            menuIndicatorStyle.Add(TemplatedControl.PaddingProperty,
                TabControlTokenResourceKey.MenuIndicatorPaddingHorizontal);

            var startEdgeIndicatorStyle =
                new Style(selector => selector.Nesting().Template().Name(ScrollStartEdgeIndicatorPart));
            startEdgeIndicatorStyle.Add(Layoutable.WidthProperty, TabControlTokenResourceKey.MenuEdgeThickness);
            startEdgeIndicatorStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Left);
            startEdgeIndicatorStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch);
            topPlacementStyle.Add(startEdgeIndicatorStyle);

            var endEdgeIndicatorStyle =
                new Style(selector => selector.Nesting().Template().Name(ScrollEndEdgeIndicatorPart));
            endEdgeIndicatorStyle.Add(Layoutable.WidthProperty, TabControlTokenResourceKey.MenuEdgeThickness);
            endEdgeIndicatorStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Right);
            endEdgeIndicatorStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch);

            topPlacementStyle.Add(endEdgeIndicatorStyle);

            topPlacementStyle.Add(menuIndicatorStyle);
            topPlacementStyle.Add(contentPresenterStyle);
        }

        Add(topPlacementStyle);

        var rightPlacementStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(BaseTabScrollViewer.TabStripPlacementProperty, Dock.Right));

        {
            var contentPresenterStyle =
                new Style(selector => selector.Nesting().Template().Name(ScrollViewContentPart));
            contentPresenterStyle.Add(DockPanel.DockProperty, Dock.Top);
            var menuIndicatorStyle =
                new Style(selector => selector.Nesting().Template().Name(ScrollMenuIndicatorPart));
            menuIndicatorStyle.Add(DockPanel.DockProperty, Dock.Bottom);
            menuIndicatorStyle.Add(TemplatedControl.PaddingProperty,
                TabControlTokenResourceKey.MenuIndicatorPaddingVertical);

            var startEdgeIndicatorStyle =
                new Style(selector => selector.Nesting().Template().Name(ScrollStartEdgeIndicatorPart));
            startEdgeIndicatorStyle.Add(Layoutable.HeightProperty, TabControlTokenResourceKey.MenuEdgeThickness);
            startEdgeIndicatorStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
            startEdgeIndicatorStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Top);
            rightPlacementStyle.Add(startEdgeIndicatorStyle);

            var endEdgeIndicatorStyle =
                new Style(selector => selector.Nesting().Template().Name(ScrollEndEdgeIndicatorPart));
            endEdgeIndicatorStyle.Add(Layoutable.HeightProperty, TabControlTokenResourceKey.MenuEdgeThickness);
            endEdgeIndicatorStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
            endEdgeIndicatorStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Bottom);
            rightPlacementStyle.Add(endEdgeIndicatorStyle);

            rightPlacementStyle.Add(menuIndicatorStyle);
            rightPlacementStyle.Add(contentPresenterStyle);
        }

        Add(rightPlacementStyle);

        var bottomPlacementStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(BaseTabScrollViewer.TabStripPlacementProperty, Dock.Bottom));

        {
            var contentPresenterStyle =
                new Style(selector => selector.Nesting().Template().Name(ScrollViewContentPart));
            contentPresenterStyle.Add(DockPanel.DockProperty, Dock.Left);
            var menuIndicatorStyle =
                new Style(selector => selector.Nesting().Template().Name(ScrollMenuIndicatorPart));
            menuIndicatorStyle.Add(DockPanel.DockProperty, Dock.Right);
            menuIndicatorStyle.Add(TemplatedControl.PaddingProperty,
                TabControlTokenResourceKey.MenuIndicatorPaddingHorizontal);

            var startEdgeIndicatorStyle =
                new Style(selector => selector.Nesting().Template().Name(ScrollStartEdgeIndicatorPart));
            startEdgeIndicatorStyle.Add(Layoutable.WidthProperty, TabControlTokenResourceKey.MenuEdgeThickness);
            startEdgeIndicatorStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Left);
            startEdgeIndicatorStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch);
            bottomPlacementStyle.Add(startEdgeIndicatorStyle);

            var endEdgeIndicatorStyle =
                new Style(selector => selector.Nesting().Template().Name(ScrollEndEdgeIndicatorPart));
            endEdgeIndicatorStyle.Add(Layoutable.WidthProperty, TabControlTokenResourceKey.MenuEdgeThickness);
            endEdgeIndicatorStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Right);
            endEdgeIndicatorStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch);
            bottomPlacementStyle.Add(endEdgeIndicatorStyle);

            bottomPlacementStyle.Add(menuIndicatorStyle);
            bottomPlacementStyle.Add(contentPresenterStyle);
        }

        Add(bottomPlacementStyle);

        var leftPlacementStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(BaseTabScrollViewer.TabStripPlacementProperty, Dock.Left));

        {
            var contentPresenterStyle =
                new Style(selector => selector.Nesting().Template().Name(ScrollViewContentPart));
            contentPresenterStyle.Add(DockPanel.DockProperty, Dock.Top);
            var menuIndicatorStyle =
                new Style(selector => selector.Nesting().Template().Name(ScrollMenuIndicatorPart));
            menuIndicatorStyle.Add(DockPanel.DockProperty, Dock.Bottom);
            menuIndicatorStyle.Add(TemplatedControl.PaddingProperty,
                TabControlTokenResourceKey.MenuIndicatorPaddingVertical);

            var startEdgeIndicatorStyle =
                new Style(selector => selector.Nesting().Template().Name(ScrollStartEdgeIndicatorPart));
            startEdgeIndicatorStyle.Add(Layoutable.HeightProperty, TabControlTokenResourceKey.MenuEdgeThickness);
            startEdgeIndicatorStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
            startEdgeIndicatorStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Top);
            leftPlacementStyle.Add(startEdgeIndicatorStyle);

            var endEdgeIndicatorStyle =
                new Style(selector => selector.Nesting().Template().Name(ScrollEndEdgeIndicatorPart));
            endEdgeIndicatorStyle.Add(Layoutable.HeightProperty, TabControlTokenResourceKey.MenuEdgeThickness);
            endEdgeIndicatorStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
            endEdgeIndicatorStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Bottom);
            leftPlacementStyle.Add(endEdgeIndicatorStyle);

            leftPlacementStyle.Add(menuIndicatorStyle);
            leftPlacementStyle.Add(contentPresenterStyle);
        }

        Add(leftPlacementStyle);
    }
}