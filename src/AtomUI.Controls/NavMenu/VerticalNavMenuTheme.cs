using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class VerticalNavMenuTheme : BaseNavMenuTheme
{
    public const string ID = "VerticalNavMenu";

    public VerticalNavMenuTheme()
        : base(typeof(NavMenu))
    {
    }

    public override string ThemeResourceKey()
    {
        return ID;
    }

    protected override Control BuildMenuContent(NavMenu navMenu, INameScope scope)
    {
        var scrollViewer = new ScrollViewer();

        CreateTemplateParentBinding(scrollViewer, ScrollViewer.AllowAutoHideProperty,
            ScrollViewer.AllowAutoHideProperty);
        CreateTemplateParentBinding(scrollViewer, ScrollViewer.HorizontalScrollBarVisibilityProperty,
            ScrollViewer.HorizontalScrollBarVisibilityProperty);
        CreateTemplateParentBinding(scrollViewer, ScrollViewer.VerticalScrollBarVisibilityProperty,
            ScrollViewer.VerticalScrollBarVisibilityProperty);
        CreateTemplateParentBinding(scrollViewer, ScrollViewer.IsScrollChainingEnabledProperty,
            ScrollViewer.IsScrollChainingEnabledProperty);
        CreateTemplateParentBinding(scrollViewer, ScrollViewer.IsDeferredScrollingEnabledProperty,
            ScrollViewer.IsDeferredScrollingEnabledProperty);
        
        var presenter    = BuildItemPresenter(false, scope);
        scrollViewer.Content = presenter;
        return scrollViewer;
    }

    protected override void BuildStyles()
    {
        base.BuildStyles();
        var commonStyle = new Style(selector => selector.Nesting());

        var verticalOrInlineStyle = new Style(selector => Selectors.Or(
            selector.Nesting().Class(NavMenu.VerticalModePC),
            selector.Nesting().Class(NavMenu.InlineModePC)));
        verticalOrInlineStyle.Add(NavMenu.PaddingProperty, NavMenuTokenKey.VerticalMenuContentPadding);
        verticalOrInlineStyle.Add(NavMenu.HorizontalAlignmentProperty, HorizontalAlignment.Left);
        verticalOrInlineStyle.Add(NavMenu.VerticalAlignmentProperty, VerticalAlignment.Stretch);
        verticalOrInlineStyle.Add(NavMenu.BackgroundProperty, SharedTokenKey.ColorBgContainer);
        var darkStyle = new Style(selector => selector.Nesting().Class(NavMenu.DarkStylePC));
        darkStyle.Add(NavMenu.BackgroundProperty, NavMenuTokenKey.DarkItemBg);
        verticalOrInlineStyle.Add(darkStyle);

        {
            var itemPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ItemsPresenterPart));
            itemPresenterStyle.Add(ItemsPresenter.ItemsPanelProperty, new FuncTemplate<Panel?>(() => new StackPanel
            {
                Orientation = Orientation.Vertical
            }));
            verticalOrInlineStyle.Add(itemPresenterStyle);

            var itemsPanelStyle = new Style(selector =>
                selector.Nesting().Template().Name(ItemsPresenterPart).Child().OfType<StackPanel>());
            itemsPanelStyle.Add(StackPanel.SpacingProperty, NavMenuTokenKey.VerticalItemsPanelSpacing);
            verticalOrInlineStyle.Add(itemsPanelStyle);
        }

        commonStyle.Add(verticalOrInlineStyle);
        Add(commonStyle);
    }
}