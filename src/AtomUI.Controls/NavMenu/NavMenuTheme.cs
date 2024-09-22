using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class NavMenuTheme : BaseControlTheme
{
    public const string ItemsPresenterPart = "PART_ItemsPresenter";
    
    public NavMenuTheme()
        : base(typeof(NavMenu))
    {
    }
    
    protected override IControlTemplate? BuildControlTemplate()
    {
        return new FuncControlTemplate<NavMenu>((menu, scope) =>
        {
            var itemPresenter = new ItemsPresenter
            {
                Name                = ItemsPresenterPart,
            };

            KeyboardNavigation.SetTabNavigation(itemPresenter, KeyboardNavigationMode.Continue);
            var rootLayout = new DockPanel()
            {
                LastChildFill = true
            };

            var horizontalLine = new Rectangle();
            rootLayout.Children.Add(horizontalLine);
            DockPanel.SetDock(horizontalLine, Dock.Bottom);
            CreateTemplateParentBinding(horizontalLine, Rectangle.HeightProperty, NavMenu.HorizontalBorderThicknessProperty);
            TokenResourceBinder.CreateGlobalTokenBinding(horizontalLine, Rectangle.FillProperty, GlobalTokenResourceKey.ColorBorderSecondary);
            
            var border = new Border
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Child               = itemPresenter
            };
            CreateTemplateParentBinding(border, Decorator.PaddingProperty, TemplatedControl.PaddingProperty);
            CreateTemplateParentBinding(border, Border.BackgroundProperty, TemplatedControl.BackgroundProperty);
            CreateTemplateParentBinding(border, Border.BackgroundSizingProperty,
                TemplatedControl.BackgroundSizingProperty);
            CreateTemplateParentBinding(border, Border.BorderThicknessProperty,
                TemplatedControl.BorderThicknessProperty);
            CreateTemplateParentBinding(border, Border.BorderBrushProperty, TemplatedControl.BorderBrushProperty);
            CreateTemplateParentBinding(border, Border.CornerRadiusProperty, TemplatedControl.CornerRadiusProperty);
            
            rootLayout.Children.Add(border);
            return rootLayout;
        });
    }
    
    protected override void BuildStyles()
    {
        var commonStyle = new Style(selector => selector.Nesting());
        
        commonStyle.Add(TemplatedControl.BorderBrushProperty, GlobalTokenResourceKey.ColorBorderSecondary);
        commonStyle.Add(TemplatedControl.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadius);
        
        var horizontalStyle = new Style(selector => selector.Nesting().Class(NavMenu.HorizontalModePC));
        horizontalStyle.Add(NavMenu.BackgroundProperty, GlobalTokenResourceKey.ColorBgContainer);
        horizontalStyle.Add(NavMenu.HeightProperty, NavMenuTokenResourceKey.MenuHorizontalHeight);
        {
            var itemPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ItemsPresenterPart));
            itemPresenterStyle.Add(ItemsPresenter.ItemsPanelProperty, new FuncTemplate<Panel?>(() => new StackPanel
            {
                Orientation = Orientation.Horizontal
            }));
            itemPresenterStyle.Add(ItemsPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
            itemPresenterStyle.Add(ItemsPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
            horizontalStyle.Add(itemPresenterStyle);
        }
        
        commonStyle.Add(horizontalStyle);
        
        var verticalOrInlineStyle = new Style(selector => Selectors.Or(selector.Nesting().Class(NavMenu.VerticalModePC),
            selector.Nesting().Class(NavMenu.InlineModePC)));
        verticalOrInlineStyle.Add(NavMenu.BackgroundProperty, GlobalTokenResourceKey.ColorBgContainer);
        var darkStyle = new Style(selector => selector.Nesting().Class(NavMenu.DarkStylePC));
        darkStyle.Add(NavMenu.BackgroundProperty, NavMenuTokenResourceKey.DarkItemBg);
        verticalOrInlineStyle.Add(darkStyle);
        
        {
            var itemPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ItemsPresenterPart));
            itemPresenterStyle.Add(ItemsPresenter.ItemsPanelProperty, new FuncTemplate<Panel?>(() => new StackPanel
            {
                Orientation = Orientation.Vertical
            }));
            itemPresenterStyle.Add(ItemsPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Left);
            itemPresenterStyle.Add(ItemsPresenter.VerticalAlignmentProperty, VerticalAlignment.Stretch);
            verticalOrInlineStyle.Add(itemPresenterStyle);
        }
        
        commonStyle.Add(verticalOrInlineStyle);
        
        Add(commonStyle);
    }
    
}