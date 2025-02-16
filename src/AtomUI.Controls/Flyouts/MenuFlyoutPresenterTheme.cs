using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;

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

            TokenResourceBinder.CreateTokenBinding(arrowDecorator, TemplatedControl.BackgroundProperty,
                MenuTokenKey.MenuBgColor);
            TokenResourceBinder.CreateTokenBinding(arrowDecorator, Layoutable.MinWidthProperty,
                MenuTokenKey.MenuPopupMinWidth);
            TokenResourceBinder.CreateTokenBinding(arrowDecorator, Layoutable.MaxWidthProperty,
                MenuTokenKey.MenuPopupMaxWidth);
            TokenResourceBinder.CreateTokenBinding(arrowDecorator, Layoutable.MinHeightProperty,
                MenuTokenKey.MenuPopupMinHeight);
            TokenResourceBinder.CreateTokenBinding(arrowDecorator, Layoutable.MaxHeightProperty,
                MenuTokenKey.MenuPopupMaxHeight);
            TokenResourceBinder.CreateTokenBinding(arrowDecorator, TemplatedControl.PaddingProperty,
                MenuTokenKey.MenuPopupContentPadding);

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
    }
}