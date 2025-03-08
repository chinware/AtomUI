using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class MenuFlyoutPresenterTheme : BaseControlTheme
{
    public const string ItemsPresenterPart = "PART_ItemsPresenter";
    public const string RootContainerPart = "PART_RootContainer";

    public MenuFlyoutPresenterTheme() : base(typeof(MenuFlyoutPresenter))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<MenuFlyoutPresenter>((menuFlyoutPresenter, scope) =>
        {
            var arrowDecorator = new ArrowDecoratedBox
            {
                Name = RootContainerPart
            };
            arrowDecorator.RegisterInNameScope(scope);

            CreateTemplateParentBinding(arrowDecorator, ArrowDecoratedBox.IsShowArrowProperty,
                MenuFlyoutPresenter.IsShowArrowProperty);
            CreateTemplateParentBinding(arrowDecorator, ArrowDecoratedBox.ArrowPositionProperty,
                MenuFlyoutPresenter.ArrowPositionProperty);

            var scrollViewer = new MenuScrollViewer
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment   = VerticalAlignment.Stretch
            };
            var itemsPresenter = new ItemsPresenter
            {
                Name = ItemsPresenterPart
            };

            CreateTemplateParentBinding(itemsPresenter, ItemsPresenter.ItemsPanelProperty,
                ItemsControl.ItemsPanelProperty);
            KeyboardNavigation.SetTabNavigation(itemsPresenter, KeyboardNavigationMode.Continue);
            Grid.SetIsSharedSizeScope(itemsPresenter, true);
            scrollViewer.Content = itemsPresenter;

            arrowDecorator.Content = scrollViewer;

            return arrowDecorator;
        });
    }

    protected override void BuildStyles()
    {
        this.Add(TemplatedControl.BackgroundProperty, Brushes.Transparent);
        this.Add(TemplatedControl.CornerRadiusProperty, MenuTokenKey.MenuPopupBorderRadius);

        var arrowDecoratorStyle = new Style(selector => selector.Nesting().Template().Name(RootContainerPart));
        arrowDecoratorStyle.Add(TemplatedControl.BackgroundProperty, MenuTokenKey.MenuBgColor);
        arrowDecoratorStyle.Add(Layoutable.MinWidthProperty, MenuTokenKey.MenuPopupMinWidth);
        arrowDecoratorStyle.Add(Layoutable.MaxWidthProperty, MenuTokenKey.MenuPopupMaxWidth);
        arrowDecoratorStyle.Add(Layoutable.MinHeightProperty, MenuTokenKey.MenuPopupMinHeight);
        arrowDecoratorStyle.Add(Layoutable.MaxHeightProperty, MenuTokenKey.MenuPopupMaxHeight);
        arrowDecoratorStyle.Add(TemplatedControl.PaddingProperty, MenuTokenKey.MenuPopupContentPadding);
        Add(arrowDecoratorStyle);
    }
}