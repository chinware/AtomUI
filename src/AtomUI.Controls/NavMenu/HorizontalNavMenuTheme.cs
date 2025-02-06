using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class HorizontalNavMenuTheme : BaseNavMenuTheme
{
    public const string ID = "HorizontalNavMenu";

    public HorizontalNavMenuTheme()
        : base(typeof(NavMenu))
    {
    }

    public override string ThemeResourceKey()
    {
        return ID;
    }

    protected override Control BuildMenuContent(NavMenu navMenu, INameScope scope)
    {
        var layout = new DockPanel()
        {
            LastChildFill = true
        };
        var horizontalLine = new Rectangle();
        layout.Children.Add(horizontalLine);
        DockPanel.SetDock(horizontalLine, Dock.Bottom);
        CreateTemplateParentBinding(horizontalLine, Rectangle.HeightProperty,
            NavMenu.HorizontalBorderThicknessProperty);
        TokenResourceBinder.CreateSharedTokenBinding(horizontalLine, Rectangle.FillProperty,
            DesignTokenKey.ColorBorderSecondary);

        layout.Children.Add(BuildItemPresenter(true, scope));
        return layout;
    }

    protected override void BuildStyles()
    {
        base.BuildStyles();
        var commonStyle = new Style(selector => selector.Nesting());

        var horizontalStyle = new Style(selector => selector.Nesting().Class(NavMenu.HorizontalModePC));
        horizontalStyle.Add(NavMenu.BackgroundProperty, DesignTokenKey.ColorBgContainer);
        horizontalStyle.Add(NavMenu.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
        horizontalStyle.Add(NavMenu.VerticalAlignmentProperty, VerticalAlignment.Top);
        horizontalStyle.Add(NavMenu.HeightProperty, NavMenuTokenResourceKey.MenuHorizontalHeight);
        {
            var itemPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ItemsPresenterPart));
            itemPresenterStyle.Add(ItemsPresenter.ItemsPanelProperty, new FuncTemplate<Panel?>(() => new StackPanel
            {
                Orientation = Orientation.Horizontal
            }));
            horizontalStyle.Add(itemPresenterStyle);
        }

        commonStyle.Add(horizontalStyle);
    }
}