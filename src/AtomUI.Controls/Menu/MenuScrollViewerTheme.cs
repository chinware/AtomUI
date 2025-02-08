using AtomUI.Data;
using AtomUI.IconPkg.AntDesign;
using AtomUI.Media;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input.GestureRecognizers;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class MenuScrollViewerTheme : BaseControlTheme
{
    public const string ScrollUpButtonPart = "PART_ScrollUpButton";
    public const string ScrollDownButtonPart = "PART_ScrollDownButton";
    public const string ScrollViewContentPart = "PART_ContentPresenter";

    public MenuScrollViewerTheme()
        : base(typeof(MenuScrollViewer))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<MenuScrollViewer>((viewer, scope) =>
        {
            var transitions = new Transitions();
            transitions.Add(
                AnimationUtils.CreateTransition<SolidColorBrushTransition>(TemplatedControl.BackgroundProperty));
            var dockPanel = new DockPanel();
            var scrollUpButton = new IconButton
            {
                Name = ScrollUpButtonPart,
                Icon = AntDesignIconPackage.UpOutlined(),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment   = VerticalAlignment.Center,
                Transitions         = transitions,
                RenderTransform     = null
            };
            CreateTemplateParentBinding(scrollUpButton, Button.CommandProperty,
                nameof(MenuScrollViewer.LineUp));
            TokenResourceBinder.CreateTokenBinding(scrollUpButton, IconButton.IconWidthProperty,
                MenuTokenKey.ScrollButtonIconSize);
            TokenResourceBinder.CreateTokenBinding(scrollUpButton, IconButton.IconHeightProperty,
                MenuTokenKey.ScrollButtonIconSize);
            DockPanel.SetDock(scrollUpButton, Dock.Top);
            var scrollDownButton = new IconButton
            {
                Name = ScrollDownButtonPart,
                Icon = AntDesignIconPackage.DownOutlined(),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment   = VerticalAlignment.Center,
                Transitions         = transitions,
                RenderTransform     = null
            };
            CreateTemplateParentBinding(scrollDownButton, Avalonia.Controls.Button.CommandProperty,
                nameof(MenuScrollViewer.LineDown));
            TokenResourceBinder.CreateTokenBinding(scrollDownButton, IconButton.IconWidthProperty,
                MenuTokenKey.ScrollButtonIconSize);
            TokenResourceBinder.CreateTokenBinding(scrollDownButton, IconButton.IconHeightProperty,
                MenuTokenKey.ScrollButtonIconSize);
            DockPanel.SetDock(scrollDownButton, Dock.Bottom);

            var scrollViewContent = CreateScrollContentPresenter(viewer);

            dockPanel.Children.Add(scrollUpButton);
            dockPanel.Children.Add(scrollDownButton);
            dockPanel.Children.Add(scrollViewContent);
            scrollUpButton.RegisterInNameScope(scope);
            scrollDownButton.RegisterInNameScope(scope);
            scrollViewContent.RegisterInNameScope(scope);
            return dockPanel;
        });
    }

    private ScrollContentPresenter CreateScrollContentPresenter(MenuScrollViewer viewer)
    {
        var scrollViewContent = new ScrollContentPresenter
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
        {
            var iconButtonStyle = new Style(selector => selector.Nesting().Template().OfType<IconButton>());
            iconButtonStyle.Add(TemplatedControl.PaddingProperty, MenuTokenKey.ScrollButtonPadding);
            iconButtonStyle.Add(Layoutable.MarginProperty, MenuTokenKey.ScrollButtonMargin);
            iconButtonStyle.Add(TemplatedControl.BackgroundProperty, SharedTokenKey.ColorTransparent);
            Add(iconButtonStyle);
        }

        {
            var iconButtonStyle = new Style(selector =>
                selector.Nesting().Template().OfType<IconButton>().Class(StdPseudoClass.PointerOver));
            iconButtonStyle.Add(TemplatedControl.BackgroundProperty, MenuTokenKey.ItemHoverBg);
            Add(iconButtonStyle);
        }
    }
}